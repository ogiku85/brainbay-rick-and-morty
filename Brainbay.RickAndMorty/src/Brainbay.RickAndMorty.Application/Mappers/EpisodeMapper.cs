using Brainbay.RickAndMorty.Application.Dtos.Response;
using Brainbay.RickAndMorty.Domain.Entities;

namespace Brainbay.RickAndMorty.Application.Mappers;

public static class EpisodeMapper
{
    public static EpisodeResponse ToEpisodeResponse(this Episode episode)
    {
        return new EpisodeResponse
        {
            Id = episode.Id,
            Url = episode.Url,
        };
    }
    
    public static EpisodeResponse? ToEpisodeResponse(this CharacterEpisode? characterEpisode)
    {
        if (characterEpisode == null || characterEpisode?.Episode == null)
            return null;
        
        return new EpisodeResponse
        {
            Id = characterEpisode.Episode.Id,
            Url = characterEpisode.Episode.Url,
        };
    }
    
    public static IReadOnlyCollection<EpisodeResponse?> ToEpisodeResponseList(this ICollection<CharacterEpisode?> characterEpisodes)
    {
        return characterEpisodes.Select(c => c.ToEpisodeResponse()).ToList();
    }
}