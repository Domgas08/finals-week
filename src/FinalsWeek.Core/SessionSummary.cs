namespace FinalsWeek.Core;

public record SessionSummary(
    Guid SessionId,
    Guid DeckId,
    int TotalScore,
    int CorrectCount,
    int TotalQuestions,
    TimeSpan Elapsed,
    IReadOnlyList<TopicSummary> Topics)
{
    public double PercentageCorrect => TotalQuestions == 0
        ? 0.0
        : Math.Round((double)CorrectCount / TotalQuestions * 100, 2);
}