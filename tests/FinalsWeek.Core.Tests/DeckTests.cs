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
}