using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.ConsoleApp.Interfaces;
using Brainbay.RickAndMorty.ConsoleApp.Models;
using Brainbay.RickAndMorty.ConsoleApp.Services;
using Brainbay.RickAndMorty.Domain.Entities;
using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using FluentAssertions;


namespace Brainbay.RickAndMorty.Tests.Unit.ConsoleApp.Services;


public class RickAndMortyImportServiceTests
{
    private readonly Mock<ICharacterRepository> _characterRepo = new();
    private readonly Mock<IEpisodeRepository> _episodeRepo = new();
    private readonly Mock<ILocationRepository> _locationRepo = new();
    private readonly Mock<ICharacterEpisodeRepository> _characterEpisodeRepo = new();
    private readonly Mock<IRickAndMortyApiClient> _apiClient = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly IOptions<RickAndMortyApiOptions> _apiOptions;

    private readonly RickAndMortyImportService _service;

    public RickAndMortyImportServiceTests()
    {
        _apiOptions = Options.Create(new RickAndMortyApiOptions
        {
            BaseUrl = "https://example.com/api/character"
        });

        _service = new RickAndMortyImportService(
            _characterRepo.Object,
            _episodeRepo.Object,
            _locationRepo.Object,
            _characterEpisodeRepo.Object,
            _apiClient.Object,
            _unitOfWork.Object,
            _apiOptions
        );
    }

    [Fact]
    public async Task ImportAllCharactersAsync_ShouldClearDatabaseFirst()
    {
        _apiClient.Setup(x => x.GetCharactersPageAsync(It.IsAny<string>()))
                  .ReturnsAsync((string url) => new ApiResponse
                  {
                      Info = new ApiInfo { Next = null },
                      Results = new List<ApiCharacter>()
                  });

        await _service.ImportAllCharactersAsync();

        _characterEpisodeRepo.Verify(x => x.ClearAsync(), Times.Once);
        _characterRepo.Verify(x => x.ClearAsync(), Times.Once);
        _locationRepo.Verify(x => x.ClearAsync(), Times.Once);
        _episodeRepo.Verify(x => x.ClearAsync(), Times.Once);
    }

    [Fact]
    public async Task ImportAllCharactersAsync_ShouldImportAliveCharactersOnly()
    {
        var characters = new List<ApiCharacter>
        {
            new() {
                Id = 1, Name = "Rick", Status = "Alive", Species = "Human", Type = "", Gender = "Male",
                Origin = new ApiLocation { Name = "Earth", Url = "https://example.com/api/location/1" },
                Location = new ApiLocation { Name = "Citadel", Url = "https://example.com/api/location/2" },
                Episode = new List<string> { "https://example.com/api/episode/1" },
                Image = "https://example.com/image.png", Created = DateTime.UtcNow
            },
            new() {
                Id = 2, Name = "Morty", Status = "Dead", Species = "Human", Type = "", Gender = "Male",
                Origin = new ApiLocation(), Location = new ApiLocation(), Episode = new List<string>(),
                Image = "", Created = DateTime.UtcNow
            }
        };

        _apiClient.Setup(x => x.GetCharactersPageAsync(It.IsAny<string>()))
                  .ReturnsAsync(new ApiResponse
                  {
                      Info = new ApiInfo { Next = null },
                      Results = characters
                  });

        await _service.ImportAllCharactersAsync();

        _characterRepo.Verify(x => x.AddAsync(It.Is<Character>(c => c.Name == "Rick")), Times.Once);
        _characterRepo.Verify(x => x.AddAsync(It.Is<Character>(c => c.Name == "Morty")), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Exactly(2)); // once for clear, once for import
    }

    [Fact]
    public async Task ImportAllCharactersAsync_ShouldHandleMultiplePages()
    {
        _apiClient.SetupSequence(x => x.GetCharactersPageAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApiResponse
            {
                Info = new ApiInfo { Next = "https://example.com/api/character?page=2" },
                Results = new List<ApiCharacter>()
            })
            .ReturnsAsync(new ApiResponse
            {
                Info = new ApiInfo { Next = null },
                Results = new List<ApiCharacter>()
            });

        await _service.ImportAllCharactersAsync();

        _apiClient.Verify(x => x.GetCharactersPageAsync(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ImportAllCharactersAsync_ShouldNotThrowIfApiReturnsNull()
    {
        _apiClient.Setup(x => x.GetCharactersPageAsync(It.IsAny<string>()))
            .ReturnsAsync((ApiResponse?)null);

        var action = async () => await _service.ImportAllCharactersAsync();

        await action.Should().NotThrowAsync();
    }
}
