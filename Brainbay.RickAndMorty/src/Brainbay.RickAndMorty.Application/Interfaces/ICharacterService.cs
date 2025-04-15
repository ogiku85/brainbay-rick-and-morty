using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Dtos.Response;

namespace Brainbay.RickAndMorty.Application.Interfaces;


public interface ICharacterService
{
    Task<GetCharactersResponse> GetAllAsync();
    Task<GetCharactersResponse> GetByPlanetAsync(string planet);
    Task<CharacterResponse>  AddCharacterAsync(CreateCharacterRequest createCharacterRequest);
}