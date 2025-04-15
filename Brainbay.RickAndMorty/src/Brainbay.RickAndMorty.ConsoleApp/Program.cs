using Brainbay.RickAndMorty.ConsoleApp.Services;
using Brainbay.RickAndMorty.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CharacterDbContext>(options =>
            options.UseMySQL(connectionString));

       // services.AddScoped<RickAndMortyImportService>(); // Register your service
        services.AddHttpClient<RickAndMortyImportService>();
    })
    .Build();

// 🔥 Run the service
using var scope = host.Services.CreateScope();
var importService = scope.ServiceProvider.GetRequiredService<RickAndMortyImportService>();

var url = "https://rickandmortyapi.com/api/character/?page=2";
//await importService.ImportCharactersAsync(url); // Or whatever your entry method is

await importService.ImportAllCharactersAsync(); // Or whatever your entry method is