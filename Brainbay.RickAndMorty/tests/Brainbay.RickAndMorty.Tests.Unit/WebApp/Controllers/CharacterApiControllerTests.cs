using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.WebApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

namespace Brainbay.RickAndMorty.Tests.Unit.WebApp.Controllers;

public class CharacterApiControllerTests
{
    private readonly Mock<ICharacterService> _characterServiceMock;
    private readonly CharacterApiController _controller;

    public CharacterApiControllerTests()
    {
        _characterServiceMock = new Mock<ICharacterService>();
        _controller = new CharacterApiController(_characterServiceMock.Object);

        // Set up a fake HTTP context for header testing
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOk_WithCharactersAndHeader()
    {
        // Arrange
        var characters = new List<CharacterResponse>
        {
            new CharacterResponse { Id = Guid.NewGuid(), Name = "Rick Sanchez", Status = "Alive" },
            new CharacterResponse { Id = Guid.NewGuid(), Name = "Morty Smith", Status = "Alive" }
        };

        _characterServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(new GetCharactersResponse
            {
                Characters = characters,
                FromDatabase = true
            });

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(characters);

        _controller.Response.Headers.Should().ContainKey("from-database");
        _controller.Response.Headers["from-database"].ToString().Should().Be("true");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNotFound_WhenNoCharacters()
    {
        // Arrange
        _characterServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(new GetCharactersResponse
            {
                Characters = new List<CharacterResponse>(),
                FromDatabase = false
            });

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_ShouldReturnCreated_WithCharacter()
    {
        // Arrange
        var request = new CreateCharacterRequest
        {
            Name = "Summer Smith",
            Status = "Alive",
            Species = "Human"
        };

        var createdCharacter = new CharacterResponse
        {
            Id = Guid.NewGuid(),
            Name = "Summer Smith",
            Status = "Alive",
            Species = "Human"
        };

        _characterServiceMock
            .Setup(service => service.AddCharacterAsync(request))
            .ReturnsAsync(createdCharacter);

        // Act
        var result = await _controller.PostAsync(request);

        // Assert
        result.Should().BeOfType<CreatedResult>()
            .Which.Value.Should().BeEquivalentTo(createdCharacter);
    }
}
