namespace FinalsWeek.Core.Tests;

using FinalsWeek.Application;
using FinalsWeek.Core;
using Microsoft.EntityFrameworkCore;

using Xunit;

// Uses the EF Core in-memory database. It does not check foreign keys or
// column types, so these tests cover the store's filtering, not the schema.
public class SqlDeckStoreTests
{
    private const string Alice = "user-alice";
    private const string Bob = "user-bob";

    private readonly DbContextOptions<AppDbContext> options =
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    [Fact]
    public async Task AddAsync_RecordsWhoCreatedTheDeck()
    {
        var deck = new Deck("Matematika");
        await this.NewStore().AddAsync(deck, Alice);

        using var context = new AppDbContext(this.options);
        var saved = await context.Decks.SingleAsync(d => d.Id == deck.Id);

        Assert.Equal(Alice, saved.OwnerId);
    }

    [Fact]
    public async Task GetAllOwnedAsync_ReturnsOnlyThatUsersDecks()
    {
        var store = this.NewStore();
        await store.AddAsync(new Deck("Alice private"), Alice);
        await store.AddAsync(new Deck("Alice public", visibility: Visibility.Public), Alice);
        await store.AddAsync(new Deck("Bob public", visibility: Visibility.Public), Bob);
        await this.AddDeckWithoutOwnerAsync(new Deck("Old deck"));

        var decks = await this.NewStore().GetAllOwnedAsync(Alice);

        Assert.Equal(
            new[] { "Alice private", "Alice public" },
            decks.Select(d => d.Title).OrderBy(t => t));
    }

    [Fact]
    public async Task GetByIdAsync_SomeoneElsesPrivateDeck_ReturnsNull()
    {
        var deck = new Deck("Alice private", visibility: Visibility.Private);
        await this.NewStore().AddAsync(deck, Alice);

        Assert.Null(await this.NewStore().GetByIdAsync(deck.Id, Bob));
        Assert.Null(await this.NewStore().GetByIdAsync(deck.Id, null));
    }

    [Fact]
    public async Task GetByIdAsync_OwnPrivateDeck_ReturnsDeck()
    {
        var deck = new Deck("Alice private", visibility: Visibility.Private);
        await this.NewStore().AddAsync(deck, Alice);

        var found = await this.NewStore().GetByIdAsync(deck.Id, Alice);

        Assert.NotNull(found);
        Assert.Equal(deck.Id, found.Id);
    }

    [Fact]
    public async Task GetByIdAsync_SomeoneElsesPublicDeck_ReturnsDeck()
    {
        var deck = new Deck("Alice public", visibility: Visibility.Public);
        await this.NewStore().AddAsync(deck, Alice);

        Assert.NotNull(await this.NewStore().GetByIdAsync(deck.Id, Bob));
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        Assert.Null(await this.NewStore().GetByIdAsync(Guid.NewGuid(), Alice));
    }

    [Fact]
    public async Task GetByIdAsync_DeckCreatedBeforeOwnership_StillLoads()
    {
        var deck = new Deck("Old deck");
        deck.AddQuestion(new Question("2 + 2?", new List<string> { "3", "4" }, 1));
        await this.AddDeckWithoutOwnerAsync(deck);

        var found = await this.NewStore().GetByIdAsync(deck.Id, Bob);

        Assert.NotNull(found);
        Assert.Null(found.OwnerId);
        Assert.Single(found.Questions);
    }

    [Fact]
    public async Task AddQuestionToDeckAsync_SomeoneElsesDeck_IsRefused()
    {
        var deck = new Deck("Alice public", visibility: Visibility.Public);
        await this.NewStore().AddAsync(deck, Alice);
        var question = new Question("2 + 2?", new List<string> { "3", "4" }, 1);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => this.NewStore().AddQuestionToDeckAsync(deck.Id, question, Bob));

        var reloaded = await this.NewStore().GetByIdAsync(deck.Id, Alice);
        Assert.Empty(reloaded!.Questions);
    }

    [Fact]
    public async Task AddQuestionsToDeckAsync_OwnDeck_SavesQuestions()
    {
        var deck = new Deck("Alice private");
        await this.NewStore().AddAsync(deck, Alice);
        var questions = new List<Question>
        {
            new("2 + 2?", new List<string> { "3", "4" }, 1),
            new("3 + 3?", new List<string> { "6", "7" }, 0),
        };

        await this.NewStore().AddQuestionsToDeckAsync(deck.Id, questions, Alice);

        var reloaded = await this.NewStore().GetByIdAsync(deck.Id, Alice);
        Assert.Equal(2, reloaded!.Questions.Count);
    }

    // A new store and context for each call, like one per request in the app,
    // so a test cannot pass just because the context still remembers the deck.
    private SqlDeckStore NewStore()
    {
        return new SqlDeckStore(new AppDbContext(this.options));
    }

    // Saves a deck the way decks were saved before this change: with no owner.
    private async Task AddDeckWithoutOwnerAsync(Deck deck)
    {
        using var context = new AppDbContext(this.options);
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
    }
}
