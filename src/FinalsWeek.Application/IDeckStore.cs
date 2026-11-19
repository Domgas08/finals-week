namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface IDeckStore
{
    Task AddAsync(Deck deck);

    Task<IReadOnlyList<Deck>> GetAllAsync();

    Task<Deck?> GetByIdAsync(Guid id);

    Task UpdateAsync(Deck deck);

    Task AddQuestionsToDeckAsync(Guid deckId, IEnumerable<Question> questions);

    Task AddQuestionToDeckAsync(Guid deckId, Question question);
}