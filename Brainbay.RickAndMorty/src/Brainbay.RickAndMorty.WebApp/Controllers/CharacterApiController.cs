using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Brainbay.RickAndMorty.WebApp.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CharacterApiController : ControllerBase
{
    private readonly ICharacterService _characterService;

    public CharacterApiController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _characterService.GetAllAsync();

        if (result.Characters.Count > 0)
        {
            Response.Headers.Append("from-database", result.FromDatabase.ToString().ToLower());
            return Ok(result.Characters);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CreateCharacterRequest request)
    {
        var result = await _characterService.AddCharacterAsync(request);
        return Created(string.Empty, result);
    }
}