using AirlineGame.Models;
using Microsoft.EntityFrameworkCore;

namespace AirlineGame.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<GameConfig> GameConfigs => Set<GameConfig>();
    public DbSet<PlayerStrategy> PlayerStrategies => Set<PlayerStrategy>();
    public DbSet<MatrixCell> MatrixCells => Set<MatrixCell>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<GameRound> GameRounds => Set<GameRound>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameConfig>(e =>
        {
            e.HasMany(g => g.Strategies)
                .WithOne(s => s.GameConfig)
                .HasForeignKey(s => s.GameConfigId);
            e.HasMany(g => g.MatrixCells)
                .WithOne(c => c.GameConfig)
                .HasForeignKey(c => c.GameConfigId);
            e.HasMany(g => g.Sessions)
                .WithOne(s => s.GameConfig)
                .HasForeignKey(s => s.GameConfigId);
        });

        modelBuilder.Entity<PlayerStrategy>(e =>
        {
            e.HasIndex(s => new { s.GameConfigId, s.PlayerNumber, s.Index }).IsUnique();
        });

        modelBuilder.Entity<MatrixCell>(e =>
        {
            e.HasIndex(c => new { c.GameConfigId, c.Row, c.Col }).IsUnique();
        });

        modelBuilder.Entity<GameSession>(e =>
        {
            e.HasMany(s => s.Rounds)
                .WithOne(r => r.GameSession)
                .HasForeignKey(r => r.GameSessionId);
        });
    }
}
