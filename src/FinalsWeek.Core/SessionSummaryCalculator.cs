namespace FinalsWeek.Core;

public static class SessionSummaryCalculator
{
    private const string UntaggedTopic = "Untagged";

    public static SessionSummary Summarise(StudySession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        var results = session.Questions
            .Select((question, index) => CreateResult(question, index, session))
            .ToList();

        var topics = results
            .GroupBy(r => r.Topic ?? UntaggedTopic)
            .OrderBy(g => g.Key == UntaggedTopic)
            .ThenBy(g => g.Key)
            .Select(g => new TopicSummary(g.Key, g.ToList()))
            .ToList();

        var elapsed = (session.FinishedAt ?? DateTimeOffset.UtcNow) - session.StartedAt;

        return new SessionSummary(
            session.Id,
            session.DeckId,
            results.Sum(r => r.Points),
            results.Count(r => r.Outcome == AnswerOutcome.Correct),
            session.Questions.Count,
            elapsed,
            topics);
    }

    private static QuestionResult CreateResult(Question question, int index, StudySession session)
    {
        session.Answers.TryGetValue(index, out var answer);

        var selected = answer?.SelectedOptionIndex;

        var studentAnswer = selected is int i && i >= 0 && i < question.Options.Count
            ? question.Options[i]
            : null;

        return new QuestionResult(
            question.Prompt,
            question.Options[question.CorrectIndex],
            answer?.Outcome ?? AnswerOutcome.Skipped,
            answer?.PointsAwarded ?? 0,
            question.Topic,
            studentAnswer);
    }
}