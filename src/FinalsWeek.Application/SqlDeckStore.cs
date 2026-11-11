namespace FinalsWeek.Application;

using FinalsWeek.Core;
using Microsoft.EntityFrameworkCore;

public class SqlDeckStore(AppDbContext context) : IDeckStore
{
    private readonly AppDbContext context = context;

    public async Task AddAsync(Deck deck)
    {
        ArgumentNullException.ThrowIfNull(deck);

        await this.context.Decks.AddAsync(deck);
        await this.context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Deck>> GetAllAsync()
    {
        return await this.context.Decks
            .Include(d => d.Questions)
            .ToListAsync();
    }

    public async Task<Deck?> GetByIdAsync(Guid id)
    {
        return await this.context.Decks
            .Include(d => d.Questions)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task UpdateAsync(Deck deck)
    {
    ArgumentNullException.ThrowIfNull(deck);
    this.context.Decks.Update(deck);
    await this.context.SaveChangesAsync();
    }

    public async Task AddQuestionToDeckAsync(Guid deckId, Question question)
    {
        var deck = await this.context.Decks
            .Include(d => d.Questions)
            .FirstOrDefaultAsync(d => d.Id == deckId) 
            ?? throw new KeyNotFoundException("Deck not found");

        deck.AddQuestion(question);
        await this.context.SaveChangesAsync();
    }
}