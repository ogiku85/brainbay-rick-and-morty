using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Brainbay.RickAndMorty.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add services
        services.AddScoped<ICharacterService, CharacterService>();
        
        return services;
    }
}