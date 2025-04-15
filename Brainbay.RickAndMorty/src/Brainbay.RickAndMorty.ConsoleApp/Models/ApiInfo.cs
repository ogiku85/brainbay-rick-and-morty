namespace Brainbay.RickAndMorty.ConsoleApp.Models;

public class ApiInfo
{
    public int Count { get; set; }
    public int Pages { get; set; }
    public string? Next { get; set; }
    public string? Prev { get; set; }
}