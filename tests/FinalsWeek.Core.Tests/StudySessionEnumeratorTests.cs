namespace FinalsWeek.Core.Tests;

using FinalsWeek.Core;
using Xunit;

public class StudySessionEnumeratorTests
{
    [Fact]
    public void MoveNext_WalksThroughAllQuestionsInOrder()
    {
        var deck = new Deck("Sample deck");
        var first = new Question("What is 4 + 5?", new List<string> { "5", "6", "7", "8", "9" }, 4);
        var second = new Question("What is 2 + 2?", new List<string> { "3", "4", "5" }, 1);
        var third = new Question("What is 2*9+8?", new List<string> { "26", "18", "92", "44" }, 0);
        deck.AddQuestion(first);
        deck.AddQuestion(second);
        deck.AddQuestion(third);
        var session = new StudySessionEnumerator(deck);
        session.MoveNext();
        Assert.Same(first, session.CurrentQuestion);
        session.MoveNext();
        Assert.Same(second, session.CurrentQuestion);
        session.MoveNext();
        Assert.Same(third, session.CurrentQuestion);
    }

    [Fact]
    public void MoveNext_ReturnsFalseWhenDeckIsFinished()
    {
        var deck = new Deck("Sample deck");
        var first = new Question("What is 4 + 5?", new List<string> { "5", "6", "7", "8", "9" }, 4);
        var second = new Question("What is 2 + 2?", new List<string> { "3", "4", "5" }, 1);
        var third = new Question("What is 2*9+8?", new List<string> { "26", "18", "92", "44" }, 0);
        deck.AddQuestion(first);
        deck.AddQuestion(second);
        deck.AddQuestion(third);
        var session = new StudySessionEnumerator(deck);
        session.MoveNext();
        session.MoveNext();
        session.MoveNext();
        Assert.False(session.MoveNext());
    }

    [Fact]
    public void Resume_WithSavedQuestionId_ReturnsToThatQuestion()
    {
        var deck = new Deck("Sample deck");
        var first = new Question("What is 4 + 5?", new List<string> { "5", "6", "7", "8", "9" }, 4);
        var second = new Question("What is 2 + 2?", new List<string> { "3", "4", "5" }, 1);
        var third = new Question("What is 2*9+8?", new List<string> { "26", "18", "92", "44" }, 0);
        deck.AddQuestion(first);
        deck.AddQuestion(second);
        deck.AddQuestion(third);
        var session = new StudySessionEnumerator(deck);
        session.MoveNext();
        session.MoveNext();
        var savedId = session.CurrentQuestionId;
        var resumedSession = StudySessionEnumerator.Resume(deck, savedId);
        Assert.Equal(savedId, resumedSession.CurrentQuestionId);
    }
}
