namespace FinalsWeek.Core;

public class StudySession
{
    private readonly Dictionary<int, SessionAnswer> answers = new();

    public StudySession(Guid deckId, IReadOnlyList<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        Id = Guid.NewGuid();
        DeckId = deckId;
        Questions = questions.ToArray();
        CurrentIndex = 0;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public Guid DeckId { get; }

    public Guid Id { get; }

    public IReadOnlyList<Question> Questions { get; }

    public int CurrentIndex { get; private set; }

    public DateTimeOffset StartedAt { get; }

    public DateTimeOffset? FinishedAt { get; private set; }

    public IReadOnlyDictionary<int, SessionAnswer> Answers => answers;

    public SessionProgress Progress => new(answers.Count, Questions.Count);

    public bool IsCompleted => CurrentIndex >= Questions.Count;

    public Question? CurrentQuestion =>
        IsCompleted ? null : Questions[CurrentIndex];

    public void AnswerCurrentQuestion(int selectedOptionIndex, AnswerOutcome outcome, int pointsAwarded)
    {
        if (!Enum.IsDefined(outcome))
        {
            throw new ArgumentOutOfRangeException(nameof(outcome), "Unrecognized answer outcome.");
        }

        if (IsCompleted)
        {
            throw new InvalidOperationException("Session is already completed.");
        }

        answers[CurrentIndex] = new SessionAnswer(selectedOptionIndex, outcome, pointsAwarded);
        CurrentIndex++;

        if (IsCompleted)
        {
            FinishedAt = DateTimeOffset.UtcNow;
        }
    }

    public void SkipCurrentQuestion()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Session is already completed.");
        }

        answers[CurrentIndex] = new SessionAnswer(null, AnswerOutcome.Skipped, 0);
        CurrentIndex++;

        if (IsCompleted)
        {
            FinishedAt = DateTimeOffset.UtcNow;
        }
    }
}