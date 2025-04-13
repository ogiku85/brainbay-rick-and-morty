using Brainbay.RickAndMorty.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        modelBuilder.Entity<CharacterEpisode>()
            .HasKey(ce => new { ce.CharacterId, ce.EpisodeId });

        modelBuilder.Entity<Character>()
            .HasIndex(c => c.ExternalId)
            .IsUnique();

        modelBuilder.Entity<Location>()
            .HasIndex(l => l.Url)
            .IsUnique();
    }
}