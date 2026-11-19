namespace FinalsWeek.Application;

using FinalsWeek.Core;
using Microsoft.EntityFrameworkCore;

public class SqlDeckStore(AppDbContext context) : IDeckStore
{
    private readonly AppDbContext context = context;

    public async Task AddAsync(Deck deck, string userId)
    {
        ArgumentNullException.ThrowIfNull(deck);

        deck.AssignOwner(userId);

        await this.context.Decks.AddAsync(deck);
        await this.context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Deck>> GetAllOwnedAsync(string userId)
    {
        return await this.context.Decks
            .Include(d => d.Questions)
            .Where(d => d.OwnerId == userId)
            .ToListAsync();
    }

    public async Task<Deck?> GetByIdAsync(Guid id, string? userId)
    {
        var deck = await this.context.Decks
            .Include(d => d.Questions)
            .FirstOrDefaultAsync(d => d.Id == id);

        return deck is not null && deck.CanBeViewed(userId) ? deck : null;
    }

    public async Task UpdateAsync(Deck deck, string userId)
    {
        ArgumentNullException.ThrowIfNull(deck);
        if (!deck.CanBeEdited(userId))
        {
            throw new KeyNotFoundException("Deck not found");
        }

        this.context.Decks.Update(deck);
        await this.context.SaveChangesAsync();
    }

    public async Task AddQuestionToDeckAsync(Guid deckId, Question question, string userId)
    {
        var deck = await this.context.Decks
            .Include(d => d.Questions)
            .FirstOrDefaultAsync(d => d.Id == deckId)
            ?? throw new KeyNotFoundException("Deck not found");

        if (!deck.CanBeEdited(userId))
        {
            throw new KeyNotFoundException("Deck not found");
        }

        deck.AddQuestion(question);
        await this.context.SaveChangesAsync();
    }

    public async Task AddQuestionsToDeckAsync(Guid deckId, IEnumerable<Question> questions, string userId)
    {
        ArgumentNullException.ThrowIfNull(questions);

        var deck = await this.context.Decks
            .Include(d => d.Questions)
            .FirstOrDefaultAsync(d => d.Id == deckId)
            ?? throw new KeyNotFoundException("Deck not found");

        if (!deck.CanBeEdited(userId))
        {
            throw new KeyNotFoundException("Deck not found");
        }

        foreach (var question in questions)
        {
            deck.AddQuestion(question);
        }

        await this.context.SaveChangesAsync();
    }
}