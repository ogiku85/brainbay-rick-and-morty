using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Brainbay.RickAndMorty.Infrastructure;


public class CharacterDbContextFactory : IDesignTimeDbContextFactory<CharacterDbContext>
{
    public CharacterDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Brainbay.RickAndMorty.WebApp"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<CharacterDbContext>();
        optionsBuilder.UseMySQL(connectionString);

        return new CharacterDbContext(optionsBuilder.Options);
    }
}