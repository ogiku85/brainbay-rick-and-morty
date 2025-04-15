using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Application.Interfaces;

public interface ICharacterRepository
{
    Task<IEnumerable<Character>> GetAllAsync();
    Task<IEnumerable<Character>> GetByOriginPlanetAsync(string originPlanet);
    Task AddCharacterAsync(Character character);
    Task<Character?> GetByNameAsync(string characterName);

    Task<bool> CharacterExists(string characterName);

    Task<int> SaveAsync();
}