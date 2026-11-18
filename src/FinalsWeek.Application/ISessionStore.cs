namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface ISessionStore
{
    Task<StudySession?> FindActiveAsync(Guid deckId);

    Task<StudySession> CreateAsync(Deck deck);

    Task<StudySession?> FindAsync(Guid sessionId);

    Task SaveAsync(StudySession session);

    Task<IReadOnlyList<StudySession>> GetFinishedAsync();
}