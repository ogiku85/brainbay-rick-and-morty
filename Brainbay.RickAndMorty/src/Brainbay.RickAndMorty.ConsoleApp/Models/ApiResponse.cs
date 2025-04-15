namespace Brainbay.RickAndMorty.ConsoleApp.Models;

public class ApiResponse
{
    public ApiInfo Info { get; set; } = default!;
    public List<ApiCharacter> Results { get; set; } = default!;
}