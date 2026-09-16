namespace FinalsWeek.Core;

public interface IDeckStore
{
    void Add(Deck Deck);

    IReadOnlyList<Deck> GetAll();
    
}