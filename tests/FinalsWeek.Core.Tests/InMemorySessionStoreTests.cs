namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using FinalsWeek.Core;
using Xunit;

public class InMemorySessionStoreTests
{
    [Fact]
    public void Create_ReturnsSessionThatCanBeFoundById()
    {
        var store = new InMemorySessionStore();
        var deck = CreateDeckWithQuestions(2);

        var created = store.Create(deck);
        var found = store.Find(created.Id);

        Assert.Same(created, found);
    }

    [Fact]
    public void Create_CopiesDeckQuestionsIntoSession()
    {
        var store = new InMemorySessionStore();
        var deck = CreateDeckWithQuestions(3);

        var session = store.Create(deck);

        Assert.Equal(deck.Id, session.DeckId);
        Assert.Equal(3, session.Questions.Count);
    }

    [Fact]
    public void Create_TwoSessionsForSameDeck_AreIndependent()
    {
        var store = new InMemorySessionStore();
        var deck = CreateDeckWithQuestions(2);

        var first = store.Create(deck);
        var second = store.Create(deck);

        first.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);

        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(1, first.CurrentIndex);
        Assert.Equal(0, second.CurrentIndex);
    }

    [Fact]
    public void Create_NullDeck_ThrowsArgumentNullException()
    {
        var store = new InMemorySessionStore();

        var ex = Assert.Throws<ArgumentNullException>(() => store.Create(null!));

        Assert.Equal("deck", ex.ParamName);
    }

    [Fact]
    public void Find_UnknownId_ReturnsNullWithoutThrowing()
    {
        var store = new InMemorySessionStore();

        var found = store.Find(Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public void Find_EmptyGuid_ReturnsNull()
    {
        var store = new InMemorySessionStore();
        store.Create(CreateDeckWithQuestions(1));

        var found = store.Find(Guid.Empty);

        Assert.Null(found);
    }

    [Fact]
    public void Save_PersistsSessionProgress()
    {
        var store = new InMemorySessionStore();
        var session = store.Create(CreateDeckWithQuestions(2));

        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);
        store.Save(session);

        var found = store.Find(session.Id);

        Assert.NotNull(found);
        Assert.Equal(1, found!.CurrentIndex);
    }

    [Fact]
    public void Save_NullSession_ThrowsArgumentNullException()
    {
        var store = new InMemorySessionStore();

        var ex = Assert.Throws<ArgumentNullException>(() => store.Save(null!));

        Assert.Equal("session", ex.ParamName);
    }

    [Fact]
    public void GetFinished_UnfinishedSession_IsNotListed()
    {
        var store = new InMemorySessionStore();
        var session = store.Create(CreateDeckWithQuestions(2));

        session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);
        store.Save(session);

        Assert.Empty(store.GetFinished());
    }

    [Fact]
    public void GetFinished_ReturnsMostRecentlyFinishedFirst()
    {
        var store = new InMemorySessionStore();

        var older = store.Create(CreateDeckWithQuestions(1));
        older.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);
        store.Save(older);

        Thread.Sleep(20);

        var newer = store.Create(CreateDeckWithQuestions(1));
        newer.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);
        store.Save(newer);

        var finished = store.GetFinished();

        Assert.Equal(2, finished.Count);
        Assert.Same(newer, finished[0]);
        Assert.Same(older, finished[1]);
    }

    [Fact]
    public void GetFinished_EmptyStore_ReturnsEmptyList()
    {
        var store = new InMemorySessionStore();

        var finished = store.GetFinished();

        Assert.NotNull(finished);
        Assert.Empty(finished);
    }

    private static Deck CreateDeckWithQuestions(int count)
    {
        var deck = new Deck("Test deck");

        for (int i = 0; i < count; i++)
        {
            deck.AddQuestion(new Question($"Question {i + 1}", new List<string> { "A", "B" }, 0));
        }

        return deck;
    }
}