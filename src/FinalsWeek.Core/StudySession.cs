namespace FinalsWeek.Core;

public class StudySession
{
    private readonly List<Question> questions = new();
    private readonly List<SessionAnswer> answers = new();

    public StudySession(Guid deckId, IReadOnlyList<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        Id = Guid.NewGuid();
        DeckId = deckId;
        CurrentIndex = 0;
        StartedAt = DateTimeOffset.UtcNow;

        foreach (var q in questions)
        {
            var clone = new Question(
                prompt: q.Prompt,
                options: q.Options,
                correctIndex: q.CorrectIndex,
                topic: q.Topic,
                timeLimitSeconds: q.TimeLimitSeconds);
            this.questions.Add(clone);
        }
    }

    private StudySession()
    {
    }

    public Guid DeckId { get; private set; }

    public Guid Id { get; private set; }

    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    public int CurrentIndex { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset? FinishedAt { get; private set; }

    public IReadOnlyDictionary<int, SessionAnswer> Answers =>
        answers.ToDictionary(a => a.QuestionIndex, a => a);

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

        answers.RemoveAll(a => a.QuestionIndex == CurrentIndex);
        answers.Add(new SessionAnswer(CurrentIndex, selectedOptionIndex, outcome, pointsAwarded));
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

        answers.RemoveAll(a => a.QuestionIndex == CurrentIndex);
        answers.Add(new SessionAnswer(CurrentIndex, null, AnswerOutcome.Skipped, 0));
        CurrentIndex++;

        if (IsCompleted)
        {
            FinishedAt = DateTimeOffset.UtcNow;
        }
    }
}