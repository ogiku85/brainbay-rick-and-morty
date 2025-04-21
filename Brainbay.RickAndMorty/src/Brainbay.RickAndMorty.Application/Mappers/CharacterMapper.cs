using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Application.Mappers;

public static class CharacterMapper
{
    public static CharacterResponse? ToCharacterResponse(this Character? character)
    {
        if (character == null)
            return null;
        
        return new CharacterResponse
        {
            Id = character.Id,
            Name = character.Name,
            Status = character.Status.ToDto(),
            Gender = character.Gender,
            ImageUrl = character.ImageUrl,
            Species = character.Species,
            Type = character.Type,
            Location = character.Location.ToLocationResponse(),
            Origin = character.Origin.ToLocationResponse(),
            Episodes = character.CharacterEpisodes.ToEpisodeResponseList()

        };
    }

    public static List<CharacterResponse> ToCharacterResponseList(this IEnumerable<Character> characters)
    {
        return characters.Select(c => c.ToCharacterResponse()).ToList();
    }

    public static Character ToCharacter(this CreateCharacterRequest createCharacterRequest)
    {
        var id = Guid.NewGuid();
        return new Character
        {
            Id = id,
            Name = createCharacterRequest.Name,
            Status = createCharacterRequest.Status.ToDomain(),
            Species = createCharacterRequest.Species,
            Type = createCharacterRequest.Type,
            LocationId = createCharacterRequest.LocationId,
            OriginId = createCharacterRequest.OriginLocationId,
            ImageUrl = createCharacterRequest.ImageUrl,
            Gender = createCharacterRequest.Gender,
            CharacterEpisodes = createCharacterRequest.Episodes.Select(e => new CharacterEpisode
            {
                CharacterId = id,
                EpisodeId = e
            }).ToList()
        };
    }
}