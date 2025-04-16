using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Brainbay.RickAndMorty.WebApp.Controllers;


[Route("Characters")]
public class CharacterController : Controller
{
    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpGet("Planet/{planetName}")]
    public async Task<IActionResult> CharactersByPlanet(string planetName)
    {
        var characters = (await _characterService.GetByPlanetAsync(planetName)).Characters;

        var viewModel = new CharactersByPlanetViewModel
        {
            PlanetName = planetName,
            Characters = characters
        };
        
        return View("~/Views/Character/CharactersByPlanet.cshtml", viewModel);
    }
}
