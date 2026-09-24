namespace FinalsWeek.Core;

using System.Collections;

public class StudySessionEnumerator : IEnumerator<Question>
{
    private readonly IEnumerator<Question> cursor;

    public Guid CurrentQuestionId => Current.Id;

    object IEnumerator.Current => Current;

    public Question Current => cursor.Current ?? throw new InvalidOperationException("MoveNext must be called before CurrentQuestion can be read.");

    public StudySessionEnumerator(Deck deck)
    {
        ArgumentNullException.ThrowIfNull(deck, nameof(deck));
        this.cursor = deck.GetEnumerator();
    }

    public void Reset()
    {
        cursor.Reset();
    }

    public void Dispose()
    {
        cursor.Dispose();
    }
	
    public bool MoveNext()
    {
        return cursor.MoveNext();
    }

    public static StudySessionEnumerator Resume(Deck deck, Guid questionId)
    {
        var session = new StudySessionEnumerator(deck);
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
