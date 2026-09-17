namespace FinalsWeek.Application;

using FinalsWeek.Core;

public class InMemoryDeckStore : IDeckStore
{
    private readonly List<Deck> decks = new();

    public void Add(Deck deck)
    {
        if (deck == null)
        {
            throw new ArgumentNullException(nameof(deck));
        }

        decks.Add(deck);
    }

    public IReadOnlyList<Deck> GetAll()
    {
        return decks.ToList().AsReadOnly();
    }

    public Deck? GetById(Guid id)
    {
        return decks.FirstOrDefault(d => d.Id == id);
    }
}