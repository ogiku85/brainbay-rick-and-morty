using Brainbay.RickAndMorty.ConsoleApp.Clients;
using Brainbay.RickAndMorty.ConsoleApp.Interfaces;
using Brainbay.RickAndMorty.ConsoleApp.Models;
using Brainbay.RickAndMorty.ConsoleApp.Services;
using Brainbay.RickAndMorty.Infrastructure;
using Brainbay.RickAndMorty.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using MySql.Data.MySqlClient;
using System.Net.Sockets;
using Serilog;



try
{
    // Load configuration first
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    // Configure Serilog

    var elasticUri = configuration["Serilog:ElasticUri"] ?? "http://elasticsearch:9200";

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console()
        .CreateLogger();
    

    Log.Information("🚀 Starting RickAndMorty.ConsoleApp...");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
            config.AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
            services.Configure<RickAndMortyApiOptions>(
                context.Configuration.GetSection("RickAndMortyApiOptions"));

            var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CharacterDbContext>(options =>
                options.UseMySQL(connectionString));

            services.AddInfrastructure(context.Configuration);
            services.AddHttpClient<IRickAndMortyApiClient, RickAndMortyApiClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetTimeoutPolicy());

            services.AddScoped<RickAndMortyImportService>();
        })
        .Build();

    // Retry DB migration using Polly
    var retryPolicy = Policy
        .Handle<MySqlException>()
        .Or<SocketException>()
        .WaitAndRetry(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, delay, attempt, _) =>
        {
            Log.Warning("⚠️ Retry {Attempt} after {Delay}s: {Message}", attempt, delay.TotalSeconds, ex.Message);
        });

    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CharacterDbContext>();

        retryPolicy.Execute(() =>
        {
            Log.Information("⏳ Attempting database migration...");
            dbContext.Database.Migrate();
            Log.Information("✅ Database migration complete.");
        });
    }

    // Run import
    using (var scope = host.Services.CreateScope())
    {
        Log.Information("🔥 Starting import service...");

        var importService = scope.ServiceProvider.GetRequiredService<RickAndMortyImportService>();
        await importService.ImportAllCharactersAsync();

        Log.Information("✅ Finished import.");
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Unhandled exception in RickAndMorty.ConsoleApp");
}
finally
{
    Log.Information("👋 Shutting down RickAndMorty.ConsoleApp");
    Log.CloseAndFlush();
}

// ---- Polly Policies ----

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
{
    return Policy.TimeoutAsync<HttpResponseMessage>(10);
}
