using System.Collections.Concurrent;

namespace FinalsWeek.Core;

public class InMemoryDeckStore: IDeckStore
{
    private readonly ConcurrentBag<Deck> _decks = new();

    public void Add(Deck deck)
    {
        if (deck == null)
        {
            throw new ArgumentException(nameof(deck));
        }

        _decks.Add(deck);
    }

    public IReadOnlyList<Deck> GetAll()
    {
        return _decks.ToList().AsReadOnly();
    }
}