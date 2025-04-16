using System.Text.Json;
using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Application.Mappers;
using Brainbay.RickAndMorty.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Brainbay.RickAndMorty.Application.Services;

    public class CharacterService : ICharacterService
    {

        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<CharacterService> _logger;
        private readonly ICacheService _cache;
        private const string CacheKey = "all_characters";

        public CharacterService(ICharacterRepository characterRepository, ILogger<CharacterService> logger, ICacheService cache)
        {
            _characterRepository = characterRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<GetCharactersResponse> GetAllAsync()
        {
            var fromDatabase = false;
            var characterResponses = await _cache.GetAsync<List<CharacterResponse>>(CacheKey);


            if (characterResponses == null)
            {
                characterResponses = (await _characterRepository.GetAllAsync()).ToCharacterResponseList();
                fromDatabase = true;

                await _cache.SetAsync(CacheKey, characterResponses, TimeSpan.FromMinutes(5));
            }


            return new GetCharactersResponse{ Characters = characterResponses, FromDatabase = fromDatabase };
        }

        public async Task<GetCharactersResponse> GetByPlanetAsync(string planet)
        {
            //return (await GetAllAsync()).Characters;
            var getCharactersResponse = await GetAllAsync();
            var charactersFromCurrentPlanet = getCharactersResponse?.Characters
                .Where(c => c.Origin?.Name.ToLower() == planet?.ToLower())
                .ToList();

            return new GetCharactersResponse{ Characters = charactersFromCurrentPlanet, FromDatabase = getCharactersResponse.FromDatabase };
        }

        public async Task<CharacterResponse> AddCharacterAsync(CreateCharacterRequest createCharacterRequest)
        {
            if (createCharacterRequest.Status?.ToLower() != "alive")
                throw new ArgumentException("Only alive characters can be added.");

            var characterExists = await _characterRepository.CharacterExists(createCharacterRequest.Name);
            
            if (!characterExists)
                throw new InvalidOperationException("Duplicate character.");

            var character = createCharacterRequest.ToCharacter();
           await _characterRepository.AddAsync(character);
           await _characterRepository.SaveAsync();

            await _cache.RemoveAsync(CacheKey); // Invalidate cache

            return character.ToCharacterResponse();
        }
    }