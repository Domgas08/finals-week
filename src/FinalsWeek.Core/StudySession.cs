namespace FinalsWeek.Core;

public class StudySession
{
    private readonly IEnumerator<Question> cursor;

    public Guid CurrentQuestionId => CurrentQuestion.Id;

    public Question CurrentQuestion => cursor.Current ?? throw new InvalidOperationException("MoveNext must be called before CurrentQuestion can be read.");

    public StudySession(Deck deck)
    {
        ArgumentNullException.ThrowIfNull(deck, nameof(deck));
        this.cursor = deck.GetEnumerator();
    }

    public bool MoveNext()
    {
        return cursor.MoveNext();
    }

    public static StudySession Resume(Deck deck, Guid questionId)
    {
        var session = new StudySession(deck);
        while (session.MoveNext())
        {
            if (session.CurrentQuestionId == questionId)
            {
                return session;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(questionId), $"Question with ID {questionId} not found in deck {deck.Title}.");
    }
}