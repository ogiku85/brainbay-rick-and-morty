using Brainbay.RickAndMorty.ConsoleApp.Clients;
using Brainbay.RickAndMorty.ConsoleApp.Models;
using Brainbay.RickAndMorty.ConsoleApp.Services;
using Brainbay.RickAndMorty.Infrastructure;
using Brainbay.RickAndMorty.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.MySql;
using Xunit;

namespace Brainbay.RickAndMorty.Tests.Integration.ConsoleApp.Services;

public class RickAndMortyImportServiceTests : IAsyncLifetime
{
    private readonly MySqlContainer _dbContainer;
    private CharacterDbContext _context = null!;
    private RickAndMortyImportService _importService = null!;

    public RickAndMortyImportServiceTests()
    {
        _dbContainer = new MySqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpw")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseMySQL(_dbContainer.GetConnectionString())
            .Options;

        _context = new CharacterDbContext(options);
        await _context.Database.EnsureCreatedAsync();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://rickandmortyapi.com")
        };

        var apiClient = new RickAndMortyApiClient(httpClient); // Assuming your implementation

        _importService = new RickAndMortyImportService(
            new CharacterRepository(_context),
            new EpisodeRepository(_context),
            new LocationRepository(_context),
            new CharacterEpisodeRepository(_context),
            apiClient,
            new EfUnitOfWork(_context),
            Options.Create(new RickAndMortyApiOptions
            {
                BaseUrl = "/api/character"
            }),
            new LoggerFactory().CreateLogger<RickAndMortyImportService>()
        );
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    [Fact]
    public async Task ImportAllCharactersAsync_SavesAliveCharactersFromRealApi()
    {
        // Act
        await _importService.ImportAllCharactersAsync();

        // Assert
        var characters = await _context.Characters.Include(c => c.CharacterEpisodes).ToListAsync();
        var locations = await _context.Locations.ToListAsync();
        var episodes = await _context.Episodes.ToListAsync();

        characters.Should().NotBeEmpty("because real API should return alive characters");
        episodes.Should().NotBeEmpty("because characters have appeared in episodes");

        characters.Should().OnlyContain(c => c.Status == "Alive");
        characters.Count().Should().Be(439);
    }
}
