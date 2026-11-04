namespace FinalsWeek.Application;

using FinalsWeek.Core;

public static class SessionHistoryExtensions
{
    public static SessionHistoryEntry ToHistoryEntry(this StudySession session, string deckTitle)
    {
        ArgumentNullException.ThrowIfNull(session);

        return new SessionHistoryEntry(
            session.Id,
            deckTitle,
            session.Answers.Values.Sum(a => a.PointsAwarded),
            session.Answers.Values.Count(a => a.Outcome == AnswerOutcome.Correct),
            session.Questions.Count,
            session.FinishedAt ?? session.StartedAt);
    }
}