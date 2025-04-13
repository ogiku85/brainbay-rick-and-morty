using System.Text.Json;
using Brainbay.RickAndMorty.ConsoleApp.Models;
using Brainbay.RickAndMorty.Domain.Entities;
using Brainbay.RickAndMorty.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Brainbay.RickAndMorty.ConsoleApp.Services;

public class CharacterImportService
{
    private readonly CharacterDbContext _dbContext;
    private readonly HttpClient _httpClient;

    public CharacterImportService(CharacterDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task ImportCharactersAsync(string apiUrl)
    {
        var response = await _httpClient.GetStringAsync(apiUrl);
        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);

        foreach (var apiCharacter in apiResponse.Results)
        {
            if (_dbContext.Characters.Any(c => c.ExternalId == apiCharacter.Id))
                continue;

            var origin = await GetOrCreateLocationAsync(apiCharacter.Origin);
            var location = await GetOrCreateLocationAsync(apiCharacter.Location);

            var character = new Character
            {
                ExternalId = apiCharacter.Id,
                Name = apiCharacter.Name,
                Status = apiCharacter.Status,
                Species = apiCharacter.Species,
                Type = apiCharacter.Type,
                Gender = apiCharacter.Gender,
                Image = apiCharacter.Image,
                Url = apiCharacter.Url,
                Created = apiCharacter.Created,
                Origin = origin,
                Location = location,
            };

            foreach (var epUrl in apiCharacter.Episode.Distinct())
            {
                var episode = await GetOrCreateEpisodeAsync(epUrl);
                character.CharacterEpisodes.Add(new CharacterEpisode
                {
                    Character = character,
                    Episode = episode
                });
            }

            _dbContext.Characters.Add(character);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<Location?> GetOrCreateLocationAsync(ApiLocation apiLoc)
    {
        if (string.IsNullOrWhiteSpace(apiLoc?.Url)) return null;

        var existing = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Url == apiLoc.Url);
        if (existing != null) return existing;

        var newLoc = new Location { Name = apiLoc.Name, Url = apiLoc.Url };
        _dbContext.Locations.Add(newLoc);
        return newLoc;
    }

    private async Task<Episode> GetOrCreateEpisodeAsync(string url)
    {
        var existing = await _dbContext.Episodes.FirstOrDefaultAsync(e => e.Url == url);
        if (existing != null) return existing;

        var newEp = new Episode { Url = url };
        _dbContext.Episodes.Add(newEp);
        return newEp;
    }
}
