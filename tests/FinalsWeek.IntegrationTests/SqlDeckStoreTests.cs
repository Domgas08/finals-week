namespace FinalsWeek.IntegrationTests;

using FinalsWeek.Application;
using FinalsWeek.Core;

public class SqlDeckStoreTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Deck_with_questions_is_saved_and_read_back_in_order()
    {
        var deck = new Deck("Integration deck");
        var question1 = new Question("First", new List<string> {"1", "2", "3"}, 0);
        var question2 = new Question("Second", new List<string> {"1", "2", "3"}, 1);
        var question3 = new Question("Third", new List<string> {"1", "2", "3"}, 2);
        deck.AddQuestion(question1);
        deck.AddQuestion(question2);
        deck.AddQuestion(question3);
        {
            await using var writeContext = fixture.CreateContext();
            var store = new SqlDeckStore(writeContext);
            await store.AddAsync(deck);
        }

        Deck? loaded;
        {
            await using var readContext = fixture.CreateContext();
            var store = new SqlDeckStore(readContext);
            loaded = await store.GetByIdAsync(deck.Id);
    }

        Assert.NotNull(loaded);
        Assert.Equal(deck.Title, loaded.Title);
        Assert.Equal(
            new[] { "First", "Second", "Third" },
            loaded.Questions.Select(q => q.Prompt));
        Assert.Equal(deck.Questions, loaded.Questions);
    }
}