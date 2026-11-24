namespace FinalsWeek.IntegrationTests;

public class FixtureSmokeTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Database_is_reachable()
    {
        await using var context = fixture.CreateContext();
        Assert.True(await context.Database.CanConnectAsync());
    }
}
