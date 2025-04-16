using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.WebApp.Controllers;
using Brainbay.RickAndMorty.WebApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Brainbay.RickAndMorty.Tests.Unit.WebApp.Controllers;

public class CharacterControllerTests
{   
    [Fact]
    public async Task CharactersByPlanet_ShouldReturnViewWithExpectedModel_WhenCharactersExist()
    {
        // Arrange
        var mockService = new Mock<ICharacterService>();
        var planetName = "Earth";
        var characters = new List<CharacterResponse>
        {
            new CharacterResponse { Id = Guid.NewGuid(), Name = "Rick" },
            new CharacterResponse { Id = Guid.NewGuid(), Name = "Morty" }
        };

        mockService
            .Setup(service => service.GetByPlanetAsync(planetName))
            .ReturnsAsync(new GetCharactersResponse { Characters = characters });

        var controller = new CharacterController(mockService.Object);

        // Act
        var result = await controller.CharactersByPlanet(planetName);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.ViewName.Should().Be("~/Views/Character/CharactersByPlanet.cshtml");

        var model = (result as ViewResult)?.Model as CharactersByPlanetViewModel;
        model.Should().NotBeNull();
        model!.PlanetName.Should().Be(planetName);
        model.Characters.Should().BeEquivalentTo(characters);
    }
    
    [Fact]
    public async Task CharactersByPlanet_ShouldReturnViewWithEmptyList_WhenNoCharactersFound()
    {
        // Arrange
        var mockService = new Mock<ICharacterService>();
        var planetName = "Unknown";

        mockService
            .Setup(service => service.GetByPlanetAsync(planetName))
            .ReturnsAsync(new GetCharactersResponse { Characters = new List<CharacterResponse>() });

        var controller = new CharacterController(mockService.Object);

        // Act
        var result = await controller.CharactersByPlanet(planetName);

        // Assert
        result.Should().BeOfType<ViewResult>();

        var model = (result as ViewResult)?.Model as CharactersByPlanetViewModel;
        model.Should().NotBeNull();
        model!.PlanetName.Should().Be(planetName);
        model.Characters.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CharactersByPlanet_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var mockService = new Mock<ICharacterService>();
        var planetName = "Mars";

        mockService
            .Setup(service => service.GetByPlanetAsync(planetName))
            .ThrowsAsync(new InvalidOperationException("Something went wrong"));

        var controller = new CharacterController(mockService.Object);

        // Act
        Func<Task> act = async () => await controller.CharactersByPlanet(planetName);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Something went wrong");
    }
    
}
