namespace FinalsWeek.IntegrationTests;

using FinalsWeek.Application;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
    .WithImage("postgres:17")
    .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public AppDbContext CreateContext()
{

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(_container.GetConnectionString() + ";Include Error Detail=true")
        .Options;

    return new AppDbContext(options);
}
}