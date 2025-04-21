using Brainbay.RickAndMorty.Domain.Entities;
using Brainbay.RickAndMorty.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brainbay.RickAndMorty.Infrastructure;

public class CharacterDbContext : DbContext
{
    public DbSet<Character> Characters { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<CharacterEpisode> CharacterEpisodes { get; set; }

    public CharacterDbContext(DbContextOptions<CharacterDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var converter = new EnumToStringConverter<CharacterStatus>();
        
        modelBuilder.Entity<CharacterEpisode>()
            .HasKey(ce => new { ce.CharacterId, ce.EpisodeId });

        modelBuilder.Entity<Character>()
            .HasIndex(c => c.ExternalId)
            .IsUnique();
        
        modelBuilder.Entity<Character>()
            .Property(e => e.Status)
            .HasConversion(converter);

        modelBuilder.Entity<Location>()
            .HasIndex(l => l.Url)
            .IsUnique();
    }
}