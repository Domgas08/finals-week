namespace FinalsWeek.Application;

using FinalsWeek.Core;
using Microsoft.EntityFrameworkCore;

public class SqlSessionStore(AppDbContext context) : ISessionStore
{
    private readonly AppDbContext context = context;

    public async Task<StudySession?> FindActiveAsync(Guid deckId)
    {
        return await context.StudySessions
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.DeckId == deckId && s.FinishedAt == null);
    }

    public async Task<StudySession> CreateAsync(Deck deck)
    {
        ArgumentNullException.ThrowIfNull(deck);

        var session = new StudySession(deck.Id, deck.Questions);
        context.StudySessions.Add(session);
        await context.SaveChangesAsync();
        return session;
    }

    public async Task<StudySession?> FindAsync(Guid sessionId)
    {
        return await context.StudySessions
            .Include(s => s.Questions)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task SaveAsync(StudySession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<StudySession>> GetFinishedAsync()
    {
        var finishedSessions = await context.StudySessions
            .Include(s => s.Questions)
            .Where(s => s.FinishedAt != null)
            .OrderByDescending(s => s.FinishedAt)
            .ToListAsync();

        return finishedSessions;
    }
}