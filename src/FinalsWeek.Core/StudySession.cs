namespace FinalsWeek.Core;

public class StudySession
{
    private readonly Dictionary<int, AnswerOutcome> answers = new();

    public StudySession(Deck deck, IReadOnlyList<Question> questions)
    {
        Deck = deck ?? throw new ArgumentNullException(nameof(deck));
        ArgumentNullException.ThrowIfNull(questions);

        Questions = questions.ToArray();
        CurrentIndex = 0;
    }

    public Deck Deck { get; }

    public IReadOnlyList<Question> Questions { get; }

    public int CurrentIndex { get; private set; }

    public IReadOnlyDictionary<int, AnswerOutcome> Answers => answers;

    public SessionProgress Progress => new(answers.Count, Questions.Count);

    public bool IsCompleted => CurrentIndex >= Questions.Count;

    public Question? CurrentQuestion =>
        IsCompleted ? null : Questions[CurrentIndex];

    public void AnswerCurrentQuestion(AnswerOutcome outcome)
    {
        if (!Enum.IsDefined(outcome))
        {
            throw new ArgumentOutOfRangeException(nameof(outcome), "Unrecognized answer outcome.");
        }

        if (IsCompleted)
        {
            throw new InvalidOperationException("Session is already completed.");
        }

        answers[CurrentIndex] = outcome;
        CurrentIndex++;
    }

    public void SkipCurrentQuestion()
    {
        AnswerCurrentQuestion(AnswerOutcome.Skipped);
    }
}