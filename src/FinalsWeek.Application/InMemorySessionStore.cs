namespace FinalsWeek.Application;

using System.Collections.Concurrent;
using FinalsWeek.Core;

public class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<Guid, StudySession> sessions = new();

    public StudySession Create(Deck deck)
    {
        var session = new StudySession(deck.Id, deck.Questions);
        sessions[session.Id] = session;
        return session;
    }

    public StudySession? Find(Guid sessionId)
    {
        if (sessions.TryGetValue(sessionId, out var session))
        {
            return session;
        }
        else
        {
            return null;
        }
    }

    public void Save(StudySession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        sessions[session.Id] = session;
    }

}