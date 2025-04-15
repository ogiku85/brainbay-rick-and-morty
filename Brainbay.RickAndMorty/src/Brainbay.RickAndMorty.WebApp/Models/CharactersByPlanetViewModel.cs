using Brainbay.RickAndMorty.Application.Dtos.Response;

namespace Brainbay.RickAndMorty.WebApp.Models;

public class CharactersByPlanetViewModel
{
    public string PlanetName { get; set; }
    public List<CharacterResponse> Characters { get; set; } = new();
}
