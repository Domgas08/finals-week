namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using FinalsWeek.Core;

using Xunit;

public class DeckTests
{
    [Fact]
    public void Constructor_WithValidTitle_CreatesDeck()
    {
        var deck = new Deck("Kompiuteriu Architektura");
        Assert.Equal("Kompiuteriu Architektura", deck.Title);
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Deck(string.Empty));
    }

    [Fact]
    public void Constructor_WithTitleOver120Chars_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Deck(new string('a', 121)));
    }

    [Fact]
    public void Rename_WithNewTitle_RenamesDeck()
    {
        var deck = new Deck("Old Title");
        deck.Rename("New Title");
        Assert.Equal("New Title", deck.Title);
    }

    [Fact]
    public void NewDeck_HasNoQuestions()
    {
        var deck = new Deck("Matematika");

        Assert.Empty(deck.Questions);
    }

    [Fact]
    public void AddQuestion_WithValidQuestion_AppearsInQuestions()
    {
        var deck = new Deck("Matematika");
        var question = new Question("2 + 2?", new List<string> { "3", "4" }, 1);

        deck.AddQuestion(question);

        Assert.Single(deck.Questions);
        Assert.Same(question, deck.Questions[0]);
    }

    [Fact]
    public void AddQuestion_WithNull_ThrowsArgumentNullException()
    {
        var deck = new Deck("Matematika");

        Assert.Throws<ArgumentNullException>(() => deck.AddQuestion(null!));
    }

    [Fact]
    public void RemoveQuestion_WithExistingId_RemovesAndReturnsTrue()
    {
        var deck = new Deck("Matematika");
        var question = CreateQuestion();
        deck.AddQuestion(question);

        var removed = deck.RemoveQuestion(question.Id);

        Assert.True(removed);
        Assert.Empty(deck.Questions);
    }

    [Fact]
    public void RemoveQuestion_WithUnknownId_ReturnsFalseAndKeepsQuestions()
    {
        var deck = new Deck("Matematika");
        deck.AddQuestion(CreateQuestion());

        var removed = deck.RemoveQuestion(Guid.NewGuid());

        Assert.False(removed);
        Assert.Single(deck.Questions);
    }

    [Fact]
    public void RemoveQuestion_WithDuplicateContent_RemovesOnlyMatchingId()
    {
        var deck = new Deck("Matematika");
        var first = CreateQuestion();
        var second = CreateQuestion();
        deck.AddQuestion(first);
        deck.AddQuestion(second);

        deck.RemoveQuestion(second.Id);

        Assert.Single(deck.Questions);
        Assert.Same(first, deck.Questions[0]);
    }

    private static Question CreateQuestion()
    {
        return new Question("Kiek bus 2 + 2?", new List<string> { "3", "4" }, 1);
    }
}