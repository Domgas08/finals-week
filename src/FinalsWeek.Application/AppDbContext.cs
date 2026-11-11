namespace FinalsWeek.Application;

using FinalsWeek.Core;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Deck> Decks => Set<Deck>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Deck>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasColumnType("uuid");

            entity.OwnsMany(d => d.Questions, q =>
            {
                q.ToTable("Questions");
                q.WithOwner().HasForeignKey("DeckId");

                q.HasKey(quest => quest.Id);
                q.Property(quest => quest.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                q.Property(quest => quest.Prompt).IsRequired();
                q.Property(quest => quest.CorrectIndex).IsRequired();
                q.Property(quest => quest.Topic);
                q.Property(quest => quest.TimeLimitSeconds).IsRequired();

                q.Property(quest => quest.Options)
                    .HasColumnName("Options")
                    .HasColumnType("text[]");
            });

            entity.Navigation(d => d.Questions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}