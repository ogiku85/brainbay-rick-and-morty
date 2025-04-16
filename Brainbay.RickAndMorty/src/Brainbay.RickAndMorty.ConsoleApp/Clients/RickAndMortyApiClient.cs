using System.Text.Json;
using Brainbay.RickAndMorty.ConsoleApp.Interfaces;
using Brainbay.RickAndMorty.ConsoleApp.Models;

namespace Brainbay.RickAndMorty.ConsoleApp.Clients;


public class RickAndMortyApiClient : IRickAndMortyApiClient
{
    private readonly HttpClient _httpClient;

    public RickAndMortyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse?> GetCharactersPageAsync(string url)
    {
        var response = await _httpClient.GetStringAsync(url);
        return JsonSerializer.Deserialize<ApiResponse>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
