namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface ISessionStore
{
    StudySession Create(Deck deck);

    StudySession? Find(Guid sessionId);

    void Save(StudySession session);
}