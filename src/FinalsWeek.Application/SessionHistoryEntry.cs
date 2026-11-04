namespace FinalsWeek.Application;

public record SessionHistoryEntry(
    Guid SessionId,
    string DeckTitle,
    int Score,
    int CorrectCount,
    int TotalQuestions,
    DateTimeOffset FinishedAt);