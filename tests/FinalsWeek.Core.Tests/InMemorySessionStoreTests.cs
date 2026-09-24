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

        first.AnswerCurrentQuestion(AnswerOutcome.Correct);

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

        session.AnswerCurrentQuestion(AnswerOutcome.Correct);
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