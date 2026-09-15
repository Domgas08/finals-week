namespace FinalsWeek.Core;

public record Question(string Prompt, IReadOnlyList<string> Options, int CorrectIndex, string? Topic = null, int TimeLimitSeconds = 20)
{
    public IReadOnlyList<string> Options { get; init; } = ValidateOptions(Options);

    private static IReadOnlyList<string> ValidateOptions(IReadOnlyList<string> options)
    {
        if (options.Count < 2 || options.Count > 6)
        {
            throw new ArgumentException("Options must contain between 2 and 6 items");
        }

        return options;
    }
}