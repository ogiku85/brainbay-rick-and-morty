using System.Text.Json;
using Brainbay.RickAndMorty.ConsoleApp.Models;
using Brainbay.RickAndMorty.Domain.Entities;
using Brainbay.RickAndMorty.Infrastructure;

namespace Brainbay.RickAndMorty.ConsoleApp.Services;


public class RickAndMortyImportService
{
    private readonly CharacterDbContext _dbContext;
    private readonly HttpClient _httpClient;
    
    private Dictionary<string, Location> _locationCache = new Dictionary<string, Location>();
    private Dictionary<string, Episode>  _episodeCache = new Dictionary<string, Episode>();

    public RickAndMortyImportService(CharacterDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task ImportAllCharactersAsync()
    {
        await ClearDatabaseAsync();
        
        string nextPage = "https://rickandmortyapi.com/api/character";

        while (!string.IsNullOrEmpty(nextPage))
        {
            var json = await _httpClient.GetStringAsync(nextPage);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            await ImportCharactersAsync(apiResponse);
            nextPage = apiResponse.Info?.Next;
        }
    }

    public async Task ClearDatabaseAsync()
    { 
        _dbContext.RemoveRange(_dbContext.Characters);
        _dbContext.RemoveRange(_dbContext.Episodes);
        _dbContext.RemoveRange(_dbContext.Locations);
        _dbContext.RemoveRange(_dbContext.CharacterEpisodes);
        
        await _dbContext.SaveChangesAsync();
    }
    public async Task ImportCharactersAsync(ApiResponse apiResponse)
    {
        var locationsToSave = new Dictionary<string, Location>();
        var episodesToSave = new Dictionary<string, Episode>();

        foreach (var dto in apiResponse.Results)
        {
            if (dto.Status != "Alive")
            {
                continue;
            }
            
            // Handle origin location
            Location origin = null;
            if (!string.IsNullOrWhiteSpace(dto.Origin?.Url))
            {
                if (!_locationCache.TryGetValue(dto.Origin.Url, out origin))
                {
                    origin = new Location
                    {
                        Name = dto.Origin.Name,
                        Url = dto.Origin.Url,
                        ExternalId = ExtractExternalId(dto.Origin.Url)
                    };
                    _locationCache[dto.Origin.Url] = origin;
                    locationsToSave[dto.Origin.Url] = origin;
                }
            }

            // Handle current location
            Location location = null;
            if (!string.IsNullOrWhiteSpace(dto.Location?.Url))
            {
                if (!_locationCache.TryGetValue(dto.Location.Url, out location))
                {
                    location = new Location
                    {
                        Name = dto.Location.Name,
                        Url = dto.Location.Url,
                        ExternalId = ExtractExternalId(dto.Location.Url)
                    };
                    _locationCache[dto.Location.Url] = location;
                    locationsToSave[dto.Location.Url] = location;
                }
            }

            // Create character
            var character = new Character
            {
                ExternalId = dto.Id,
                Name = dto.Name,
                Status = dto.Status,
                Species = dto.Species,
                Type = dto.Type,
                Gender = dto.Gender,
                ImageUrl = dto.Image,
                CreatedAt = dto.Created,
                Origin = origin,
                Location = location
            };

            foreach (var episodeUrl in dto.Episode.Distinct())
            {
                if (!_episodeCache.TryGetValue(episodeUrl, out var episode))
                {
                    episode = new Episode
                    {
                        Url = episodeUrl,
                        ExternalId = ExtractExternalId(episodeUrl)
                    };
                    _episodeCache[episodeUrl] = episode;
                    episodesToSave[episodeUrl] = episode;
                }

                character.CharacterEpisodes.Add(new CharacterEpisode
                {
                    Character = character,
                    Episode = episode
                });
            }

            _dbContext.Characters.Add(character);
        }
        
        _dbContext.Locations.AddRange(locationsToSave.Values);
        _dbContext.Episodes.AddRange(episodesToSave.Values);

        await _dbContext.SaveChangesAsync();
    }

    private int? ExtractExternalId(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        var parts = url.Split('/');
        if (int.TryParse(parts.LastOrDefault(), out var id))
            return id;

        return null;
    }
}
