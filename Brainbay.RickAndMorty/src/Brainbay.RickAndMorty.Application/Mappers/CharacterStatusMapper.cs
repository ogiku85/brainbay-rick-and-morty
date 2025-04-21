using Brainbay.RickAndMorty.Application.Dtos.Enums;
using Brainbay.RickAndMorty.Domain.Enums;

namespace Brainbay.RickAndMorty.Application.Mappers;

public static class CharacterStatusMapper
{
    public static CharacterStatus ToDomain(this CharacterStatusDto dtoStatus)
    {
        return dtoStatus switch
        {
            CharacterStatusDto.Alive => CharacterStatus.Alive,
            CharacterStatusDto.Dead => CharacterStatus.Dead,
            _ => CharacterStatus.Unknown
        };
    }
    
    public static CharacterStatusDto ToDto(this CharacterStatus domainStatus)
    {
        return domainStatus switch
        {
            CharacterStatus.Alive => CharacterStatusDto.Alive,
            CharacterStatus.Dead => CharacterStatusDto.Dead,
            _ => CharacterStatusDto.Unknown
        };
    }

}