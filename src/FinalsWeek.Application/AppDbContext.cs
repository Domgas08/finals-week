namespace FinalsWeek.Application;

using FinalsWeek.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Deck> Decks => Set<Deck>();

    public DbSet<StudySession> StudySessions => Set<StudySession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Deck>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasColumnType("uuid");

            entity.OwnsMany(d => d.Questions, q =>
            {
                q.Property<int>("Position");
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

        modelBuilder.Entity<StudySession>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).HasColumnType("uuid");

            entity.Property(s => s.DeckId).HasColumnType("uuid").IsRequired();
            entity.Property(s => s.CurrentIndex).IsRequired();
            entity.Property(s => s.StartedAt).IsRequired();
            entity.Property(s => s.FinishedAt);

            entity.OwnsMany(s => s.Questions, q =>
            {
                q.Property<int>("Position");
                q.ToTable("SessionQuestions");
                q.WithOwner().HasForeignKey("SessionId");

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

            entity.Navigation(s => s.Questions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.OwnsMany<SessionAnswer>("answers", a =>
            {
                a.ToTable("SessionAnswers");
                a.Property(ans => ans.QuestionIndex).IsRequired().ValueGeneratedNever();
                a.WithOwner().HasForeignKey("SessionId");

                a.Property("SessionId").HasColumnType("uuid");
                a.Property(ans => ans.QuestionIndex).IsRequired();
                a.HasKey("SessionId", "QuestionIndex");

                a.Property(ans => ans.SelectedOptionIndex);
                a.Property(ans => ans.Outcome).IsRequired();
                a.Property(ans => ans.PointsAwarded).IsRequired();
            });
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.SetQuestionPositions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        this.SetQuestionPositions();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetQuestionPositions()
    {
        this.ChangeTracker.DetectChanges();

        foreach (var entry in this.ChangeTracker.Entries<Deck>()
            .Where(e => e.State != EntityState.Deleted)
            .ToList())
        {
            var position = 0;
            foreach (var question in entry.Entity.Questions)
            {
                this.Entry(question).Property("Position").CurrentValue = position++;
            }
        }

        foreach (var entry in this.ChangeTracker.Entries<StudySession>()
            .Where(e => e.State != EntityState.Deleted)
            .ToList())
        {
            var position = 0;
            foreach (var question in entry.Entity.Questions)
            {
                this.Entry(question).Property("Position").CurrentValue = position++;
            }
        }
    }

}