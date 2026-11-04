namespace FinalsWeek.Core;

public record TopicSummary (string Topic, IReadOnlyList<QuestionResult> Results)
{
    public int TotalCount => Results.Count;

    public int CorrectCount => Results.Count(r => r.Outcome == AnswerOutcome.Correct);
}