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


var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // configure api base url
        services.Configure<RickAndMortyApiOptions>(
            context.Configuration.GetSection("RickAndMortyApi"));

        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

        // db context
        services.AddDbContext<CharacterDbContext>(options =>
            options.UseMySQL(connectionString));

        services.AddInfrastructure(context.Configuration);
        services.AddHttpClient<IRickAndMortyApiClient, RickAndMortyApiClient>();
        services.AddScoped<RickAndMortyImportService>();
        
        // HttpClient with Polly
        services.AddHttpClient<IRickAndMortyApiClient, RickAndMortyApiClient>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetTimeoutPolicy());

        // Import Service
        services.AddScoped<RickAndMortyImportService>();
    })
    .Build();

// 🔥 Run the service
using var scope = host.Services.CreateScope();
var importService = scope.ServiceProvider.GetRequiredService<RickAndMortyImportService>();

await importService.ImportAllCharactersAsync(); 

// ---- Polly Policies ----

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Exponential backoff
}

static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
{
    return Policy.TimeoutAsync<HttpResponseMessage>(10); // 10 seconds timeout
}