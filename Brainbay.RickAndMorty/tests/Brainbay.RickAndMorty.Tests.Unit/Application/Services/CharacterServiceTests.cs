using Brainbay.RickAndMorty.Application.Dtos.Enums;
using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Application.Services;
using Brainbay.RickAndMorty.Domain.Entities;
using Brainbay.RickAndMorty.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Brainbay.RickAndMorty.Tests.Unit.Application.Services;


public class CharacterServiceTests
{
    private readonly Mock<ICharacterRepository> _characterRepoMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly Mock<ILogger<CharacterService>> _loggerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly CharacterService _service;

    public CharacterServiceTests()
    {
        _service = new CharacterService(
            _characterRepoMock.Object,
            _loggerMock.Object,
            _cacheMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCharactersFromCache()
    {
        // Arrange
        var cachedCharacters = new List<CharacterResponse> { new CharacterResponse { Name = "Rick" } };
        _cacheMock.Setup(c => c.GetAsync<List<CharacterResponse>>(It.IsAny<string>()))
                  .ReturnsAsync(cachedCharacters);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.FromDatabase.Should().BeFalse();
        result.Characters.Should().ContainSingle(c => c.Name == "Rick");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCharactersFromDatabaseAndSetsCache()
    {
        // Arrange
        _cacheMock.Setup(c => c.GetAsync<List<CharacterResponse>>(It.IsAny<string>()))
                  .ReturnsAsync((List<CharacterResponse>)null);

        var dbCharacters = new List<Character>
        {
            new Character { Name = "Morty", Status = CharacterStatus.Alive, Origin = new Location { Name = "Earth" } }
        };

        _characterRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(dbCharacters);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.FromDatabase.Should().BeTrue();
        result.Characters.Should().HaveCount(1).And.Contain(c => c.Name == "Morty");

        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<List<CharacterResponse>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetByPlanetAsync_ReturnsFilteredCharacters()
    {
        // Arrange
        var cachedCharacters = new List<CharacterResponse>
        {
            new CharacterResponse { Name = "Rick", Origin = new LocationResponse { Name = "Earth" } },
            new CharacterResponse { Name = "Birdperson", Origin = new LocationResponse { Name = "Bird World" } }
        };

        _cacheMock.Setup(c => c.GetAsync<List<CharacterResponse>>(It.IsAny<string>()))
                  .ReturnsAsync(cachedCharacters);

        // Act
        var result = await _service.GetByPlanetAsync("earth");

        // Assert
        result.Characters.Should().ContainSingle(c => c.Name == "Rick");
        result.FromDatabase.Should().BeFalse();
    }

    [Fact]
    public async Task AddCharacterAsync_ThrowsIfCharacterIsNotAlive()
    {
        // Arrange
        var request = new CreateCharacterRequest { Name = "Rick", Status = CharacterStatusDto.Dead};

        // Act
        Func<Task> act = async () => await _service.AddCharacterAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Only alive characters can be added.");
    }

    [Fact]
    public async Task AddCharacterAsync_ThrowsIfDuplicateCharacter()
    {
        // Arrange
        var request = new CreateCharacterRequest { Name = "Rick", Status = CharacterStatusDto.Alive };
        _characterRepoMock.Setup(r => r.CharacterExists("Rick")).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.AddCharacterAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Duplicate character.");
    }

    [Fact]
    public async Task AddCharacterAsync_SuccessfullyAddsCharacterAndInvalidatesCache()
    {
        // Arrange
        var request = new CreateCharacterRequest
        {
            Name = "Rick",
            Status = CharacterStatusDto.Alive,
            OriginLocationId = Guid.NewGuid(),
        };

        _characterRepoMock.Setup(r => r.CharacterExists("Rick")).ReturnsAsync(false);
        _characterRepoMock.Setup(r => r.AddAsync(It.IsAny<Character>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddCharacterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Rick");

        _characterRepoMock.Verify(r => r.AddAsync(It.IsAny<Character>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync("all_characters"), Times.Once);
    }
}
