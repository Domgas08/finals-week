namespace FinalsWeek.Application;

public record QuestionTransfer
{
    public string? Prompt { get; init; }

    public List<string>? Options { get; init; }

    public int? CorrectIndex { get; init; }

    public string? Topic { get; init; }

    public int? TimeLimitSeconds { get; init; }
}