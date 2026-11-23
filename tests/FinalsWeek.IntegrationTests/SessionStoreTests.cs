namespace FinalsWeek.IntegrationTests;

using FinalsWeek.Application;
using FinalsWeek.Core;

public class SqlSessionStoreTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    private static Deck CreateDeck()
    {
        var deck = new Deck("Session deck");
        deck.AddQuestion(new Question("Q1", new List<string> { "a", "b", "c" }, 0));
        deck.AddQuestion(new Question("Q2", new List<string> { "a", "b", "c" }, 1));
        deck.AddQuestion(new Question("Q3", new List<string> { "a", "b", "c" }, 2));
        return deck;
    }

    [Fact]
    public async Task Completed_session_with_answers_is_saved_and_read_back_correctly()
    {
        var deck = CreateDeck();
        Guid sessionId;

        await using (var writeContext = fixture.CreateContext())
        {
            await new SqlDeckStore(writeContext).AddAsync(deck);

            var sessionStore = new SqlSessionStore(writeContext);
            var session = await sessionStore.CreateAsync(deck);
            sessionId = session.Id;

            session.AnswerCurrentQuestion(0, AnswerOutcome.Correct, 10);
            session.SkipCurrentQuestion();
            session.AnswerCurrentQuestion(1, AnswerOutcome.Incorrect, 0);
            await sessionStore.SaveAsync(session);
        }

        StudySession? loaded;
        await using (var readContext = fixture.CreateContext())
        {
            loaded = await new SqlSessionStore(readContext).FindAsync(sessionId);
        }

        Assert.NotNull(loaded);
        Assert.Equal(deck.Id, loaded.DeckId);
        Assert.Equal(3, loaded.CurrentIndex);
        Assert.True(loaded.IsCompleted);
        Assert.NotNull(loaded.FinishedAt);

        Assert.Equal(new[] { "Q1", "Q2", "Q3" }, loaded.Questions.Select(q => q.Prompt));

        var expected = new[]
        {
            new SessionAnswer(0, 0, AnswerOutcome.Correct, 10),
            new SessionAnswer(1, null, AnswerOutcome.Skipped, 0),
            new SessionAnswer(2, 1, AnswerOutcome.Incorrect, 0),
        };
        Assert.Equal(expected, loaded.Answers.Values.OrderBy(a => a.QuestionIndex));
        Assert.Equal(3, loaded.Progress.Answered); // adjust to SessionProgress's real property name
    }

    [Fact]
    public async Task In_progress_session_is_found_as_active_and_not_as_finished()
    {
        var deck = CreateDeck();
        Guid sessionId;

        await using (var writeContext = fixture.CreateContext())
        {
            await new SqlDeckStore(writeContext).AddAsync(deck);
            var sessionStore = new SqlSessionStore(writeContext);
            var session = await sessionStore.CreateAsync(deck);
            sessionId = session.Id;

            session.AnswerCurrentQuestion(2, AnswerOutcome.Incorrect, 0);
            await sessionStore.SaveAsync(session);
        }

        await using var readContext = fixture.CreateContext();
        var store = new SqlSessionStore(readContext);

        var active = await store.FindActiveAsync(deck.Id);
        Assert.NotNull(active);
        Assert.Equal(sessionId, active.Id);
        Assert.Equal(1, active.CurrentIndex);
        Assert.Null(active.FinishedAt);
        Assert.Single(active.Answers);
        Assert.True(active.Answers.ContainsKey(0));

        var finished = await store.GetFinishedAsync();
        Assert.DoesNotContain(finished, s => s.Id == sessionId);
    }
}