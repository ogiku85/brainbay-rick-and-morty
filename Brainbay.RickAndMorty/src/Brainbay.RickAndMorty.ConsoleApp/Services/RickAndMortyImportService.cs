using System.Text.Json;
using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.ConsoleApp.Interfaces;
using Brainbay.RickAndMorty.ConsoleApp.Models;
using Brainbay.RickAndMorty.Domain.Entities;
using Brainbay.RickAndMorty.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brainbay.RickAndMorty.ConsoleApp.Services;

public class RickAndMortyImportService
{
    private readonly ICharacterRepository _characterRepo;
    private readonly IEpisodeRepository _episodeRepo;
    private readonly ILocationRepository _locationRepo;
    private readonly ICharacterEpisodeRepository _characterEpisodeRepo;
    private readonly IRickAndMortyApiClient _apiClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RickAndMortyImportService> _logger;

    private readonly Dictionary<string, Location> _locationCache = new();
    private readonly Dictionary<string, Episode> _episodeCache = new();
    private readonly string _baseUrl;
    public RickAndMortyImportService(
        ICharacterRepository characterRepo,
        IEpisodeRepository episodeRepo,
        ILocationRepository locationRepo,
        ICharacterEpisodeRepository characterEpisodeRepo,
        IRickAndMortyApiClient apiClient,
        IUnitOfWork unitOfWork,
        IOptions<RickAndMortyApiOptions> apiOptions,
        ILogger<RickAndMortyImportService> logger)
    {
        _characterRepo = characterRepo;
        _episodeRepo = episodeRepo;
        _locationRepo = locationRepo;
        _characterEpisodeRepo = characterEpisodeRepo;
        _apiClient = apiClient;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _baseUrl = apiOptions.Value.BaseUrl;
    }

    public async Task ImportAllCharactersAsync()
    {
        await ClearDatabaseAsync();
        
        string? nextPage = _baseUrl;

        while (!string.IsNullOrEmpty(nextPage))
        {
            var apiResponse = await _apiClient.GetCharactersPageAsync(nextPage);
            if (apiResponse == null) break;

            await ImportCharactersAsync(apiResponse);
            nextPage = apiResponse.Info?.Next;
        }
    }

    private async Task ClearDatabaseAsync()
    {
        _logger.LogInformation("Clearing database...");
        
        await _characterEpisodeRepo.ClearAsync();
        await _characterRepo.ClearAsync();
        await _locationRepo.ClearAsync();
        await _episodeRepo.ClearAsync();
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Database cleared.");
    }

    private async Task ImportCharactersAsync(ApiResponse apiResponse)
    {
        _logger.LogInformation("Processing and Importing characters...");
        
        var locationsToSave = new Dictionary<string, Location>();
        var episodesToSave = new Dictionary<string, Episode>();

        foreach (var dto in apiResponse.Results.Where(c => c.Status == "Alive"))
        {
            var origin = ResolveAndTrackLocation(dto.Origin, locationsToSave);
            var location = ResolveAndTrackLocation(dto.Location, locationsToSave);

            var character = CreateCharacter(dto, origin, location);

            foreach (var episodeUrl in dto.Episode.Distinct())
            {
                var episode = ResolveAndTrackEpisode(episodeUrl, episodesToSave);

                character.CharacterEpisodes.Add(new CharacterEpisode
                {
                    Character = character,
                    Episode = episode
                });
            }

            await _characterRepo.AddAsync(character);
        }

        await _locationRepo.AddRangeAsync(locationsToSave.Values);
        await _episodeRepo.AddRangeAsync(episodesToSave.Values);

        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("{apiResponse.results.Count} Characters processed from current response.}.");
    }

    private Episode ResolveAndTrackEpisode(string episodeUrl, Dictionary<string, Episode> episodesToSave)
    {
        if (_episodeCache.TryGetValue(episodeUrl, out var cachedEpisode))
            return cachedEpisode;

        var episode = CreateEpisode(episodeUrl);
        _episodeCache[episodeUrl] = episode;
        episodesToSave[episodeUrl] = episode;

        return episode;
    }

    private Episode CreateEpisode(string url)
    {
        return new Episode
        {
            Url = url,
            ExternalId = ExtractExternalId(url)
        };
    }

    private Location? ResolveAndTrackLocation(ApiLocation apiLocation, Dictionary<string, Location> locationsToSave)
    {
        if (string.IsNullOrWhiteSpace(apiLocation.Url))
            return null;

        if (_locationCache.TryGetValue(apiLocation.Url, out var cachedLocation))
            return cachedLocation;

        var location = CreateLocation(apiLocation.Url, apiLocation.Name);

        _locationCache[apiLocation.Url] = location;
        locationsToSave[apiLocation.Url] = location;

        return location;
    }

    private Location CreateLocation(string url, string? name)
    {
        return new Location
        {
            Url = url,
            Name = name,
            ExternalId = ExtractExternalId(url)
        };
    }

    private Character CreateCharacter(ApiCharacter dto, Location? origin, Location? location)
    {
        return new Character
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
            Location = location,
            CharacterEpisodes = new List<CharacterEpisode>()
        };
    }

    private int? ExtractExternalId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        var parts = url.Split('/');
        return int.TryParse(parts.LastOrDefault(), out var id) ? id : null;
    }
}
