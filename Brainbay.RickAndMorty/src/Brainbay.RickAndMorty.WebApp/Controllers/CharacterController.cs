using Microsoft.AspNetCore.Mvc;

namespace Brainbay.RickAndMorty.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] { "Rick", "Morty" });
    }
}