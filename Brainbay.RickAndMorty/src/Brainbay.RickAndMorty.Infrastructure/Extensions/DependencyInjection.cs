using Brainbay.RickAndMorty.Application.Interfaces;
using Brainbay.RickAndMorty.Infrastructure.Repositories;
using Brainbay.RickAndMorty.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Brainbay.RickAndMorty.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Redis configuration
        var redisConfiguration = configuration.GetSection("Redis")["ConnectionString"];
        var redis = ConnectionMultiplexer.Connect(redisConfiguration);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        services.AddScoped<ICacheService, RedisCacheService>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Add context
        services.AddDbContext<CharacterDbContext>(options =>
            options.UseMySQL(connectionString));
        
        // Add services and repositories
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<IEpisodeRepository, EpisodeRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ICharacterEpisodeRepository, CharacterEpisodeRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        
        return services;
    }
}