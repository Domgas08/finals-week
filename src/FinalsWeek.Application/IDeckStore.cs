namespace FinalsWeek.Application;

using FinalsWeek.Core;

public interface IDeckStore
{
    Task AddAsync(Deck deck, string userId);

    Task<IReadOnlyList<Deck>> GetAllOwnedAsync(string userId);

    Task<Deck?> GetByIdAsync(Guid id, string? userId);

    Task UpdateAsync(Deck deck, string userId);

    Task AddQuestionsToDeckAsync(Guid deckId, IEnumerable<Question> questions, string userId);

    Task AddQuestionToDeckAsync(Guid deckId, Question question, string userId);
}