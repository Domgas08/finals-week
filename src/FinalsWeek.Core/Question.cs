namespace FinalsWeek.Core;

public record Question
{
    public Question(string prompt, IReadOnlyList<string> options, int correctIndex, string? topic = null, int timeLimitSeconds = 20)
    {
        Prompt = ValidatePrompt(prompt);
        Options = ValidateOptions(options);
        CorrectIndex = ValidateCorrectIndex(correctIndex, Options);
        Topic = topic;
        TimeLimitSeconds = ValidateTimeLimit(timeLimitSeconds);
        Id = Guid.NewGuid();
    }

    protected Question(Question original)
    {
        CorrectIndex = ValidateCorrectIndex(original.CorrectIndex, original.Options);
        Prompt = ValidatePrompt(original.Prompt);
        Options = ValidateOptions(original.Options);
        TimeLimitSeconds = ValidateTimeLimit(original.TimeLimitSeconds);
        Topic = original.Topic;
        Id = Guid.NewGuid();
    }

    public string Prompt { get; private init; }

    public IReadOnlyList<string> Options { get; private init; }

    public int CorrectIndex { get; private init; }

    public string? Topic { get; private init; }

    public Guid Id { get; }

    public int TimeLimitSeconds { get; private init; }

    public virtual bool Equals(Question? other)
    {
        if (other is null)
        {
            return false;
        }

        return Prompt == other.Prompt && Options.SequenceEqual(other.Options) && CorrectIndex == other.CorrectIndex && Topic == other.Topic && TimeLimitSeconds == other.TimeLimitSeconds;
    }

    public override int GetHashCode()
    {
        HashCode hash = default;
        foreach (var option in Options)
        {
            hash.Add(option);
        }

        hash.Add(Prompt);
        hash.Add(CorrectIndex);
        hash.Add(Topic);
        hash.Add(TimeLimitSeconds);
        return hash.ToHashCode();
    }

    private static string ValidatePrompt(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or whitespace");
        }

        return prompt;
    }

    private static int ValidateTimeLimit(int timeLimitSeconds)
    {
        if (timeLimitSeconds < 5)
        {
            throw new ArgumentOutOfRangeException(nameof(timeLimitSeconds), "Time limit must be at least 5 seconds");
        }

        return timeLimitSeconds;
    }

    private static IReadOnlyList<string> ValidateOptions(IReadOnlyList<string> options)
    {
        if (options is null)
        {
             throw new ArgumentNullException(nameof(options));
        }

        if (options.Count < 2 || options.Count > 6)
        {
            throw new ArgumentException("Options must contain between 2 and 6 items");
        }

        ValidateOptionsListContents(options);
        return options.ToList().AsReadOnly();
    }

    private static void ValidateOptionsListContents(IReadOnlyList<string> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(options[i]))
            {
                throw new ArgumentException($"Option number [{i + 1}] cannot be null or whitespace");
            }
        }
    }

    private static int ValidateCorrectIndex(int correctIndex, IReadOnlyList<string> options)
    {
        if (correctIndex < 0 || correctIndex >= options.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(correctIndex), "Correct index must be within the range of the options list");
        }

        return correctIndex;
    }
}