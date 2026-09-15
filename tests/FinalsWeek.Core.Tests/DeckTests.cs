namespace FinalsWeek.Core.Tests;

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
}