namespace Brainbay.RickAndMorty.Application.Dtos.Response;

public class GetCharactersResponse
{
    public List<CharacterResponse> Characters { get; set; }
    public bool FromDatabase { get; set; }
}