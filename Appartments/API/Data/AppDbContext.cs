using Microsoft.EntityFrameworkCore;
using Appartments.API.Models;

namespace Appartments.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Alternative> Alternatives { get; set; }
    public DbSet<Criterion> Criteria { get; set; }
    public DbSet<Vector> Vectors { get; set; }
    public DbSet<LPR> LPRs { get; set; }
    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Alternative>().ToTable("alternative");
        modelBuilder.Entity<Criterion>().ToTable("criterion");
        modelBuilder.Entity<Vector>().ToTable("vector");
        modelBuilder.Entity<LPR>().ToTable("lpr");
        modelBuilder.Entity<Result>().ToTable("result");

        modelBuilder.Entity<Alternative>(entity =>
        {
            entity.HasKey(e => e.Alternative_id);
            entity.Property(e => e.Alternative_id).HasColumnName("alternative_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasColumnName("name");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.Criterion_id);
            entity.Property(e => e.Criterion_id).HasColumnName("criterion_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasColumnName("name");
        });

        modelBuilder.Entity<Vector>(entity =>
        {
            entity.HasKey(e => e.Vector_id);
            entity.Property(e => e.Vector_id).HasColumnName("vector_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Alternative_id).HasColumnName("alternative_id");
            entity.Property(e => e.Criterion_id).HasColumnName("criterion_id");
            entity.Property(e => e.Value).HasColumnName("value");
            entity.HasOne<Alternative>()
                  .WithMany()
                  .HasForeignKey(e => e.Alternative_id);
                  
            entity.HasOne<Criterion>()
                  .WithMany()
                  .HasForeignKey(e => e.Criterion_id);
        });

        modelBuilder.Entity<LPR>(entity =>
        {
            entity.HasKey(e => e.LPR_id);
            entity.Property(e => e.LPR_id).HasColumnName("lpr_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasColumnName("name");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.Password).HasColumnName("password");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.Result_id);
            entity.Property(e => e.Result_id).HasColumnName("result_id").ValueGeneratedOnAdd();
            entity.Property(e => e.LPR_id).HasColumnName("lpr_id");
            entity.Property(e => e.Alternative_id).HasColumnName("alternative_id");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.HasOne<LPR>()
                  .WithMany()
                  .HasForeignKey(e => e.LPR_id);
            entity.HasOne<Alternative>()
                  .WithMany()
                  .HasForeignKey(e => e.Alternative_id);
        });

    }
}