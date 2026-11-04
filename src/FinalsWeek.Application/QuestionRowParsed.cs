namespace FinalsWeek.Application;

public record QuestionRowParsed(
    string Prompt,
    IReadOnlyList<string> Options,
    int CorrectIndex,
    string? Topic,
    int TimeLimitSeconds);