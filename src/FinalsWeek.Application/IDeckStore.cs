namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface IDeckStore
{
    void Add(Deck deck);

    IReadOnlyList<Deck> GetAll();
}