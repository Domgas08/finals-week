namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using FinalsWeek.Core;

using Xunit;

public class InMemoryDeckStoreTests
{
    [Fact]
    public void GetById_WithExistingId_ReturnsDeck()
    {
        var store = new InMemoryDeckStore();
        var deck = new Deck("Matematika");
        store.Add(deck);

        var found = store.GetById(deck.Id);

        Assert.Same(deck, found);
    }

    [Fact]
    public void GetById_WithUnknownId_ReturnsNull()
    {
        var store = new InMemoryDeckStore();
        store.Add(new Deck("Matematika"));

        var found = store.GetById(Guid.NewGuid());

        Assert.Null(found);
    }
}
