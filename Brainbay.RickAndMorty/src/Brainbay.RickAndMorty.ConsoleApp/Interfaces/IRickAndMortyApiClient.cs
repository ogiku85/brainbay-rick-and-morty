using Brainbay.RickAndMorty.ConsoleApp.Models;

namespace Brainbay.RickAndMorty.ConsoleApp.Interfaces;

public interface IRickAndMortyApiClient
{
    Task<ApiResponse?> GetCharactersPageAsync(string url);
}
