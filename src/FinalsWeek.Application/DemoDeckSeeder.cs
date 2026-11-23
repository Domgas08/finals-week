namespace FinalsWeek.Application;

using FinalsWeek.Core;
using Microsoft.EntityFrameworkCore;

public class DemoDeckSeeder(AppDbContext context)
{
    private readonly AppDbContext context = context;

    public async Task SeedAsync()
    {
        if (await this.context.Decks.AnyAsync(d => d.Id == DemoDeck.Id))
        {
            return;
        }

        this.context.Decks.Add(DemoDeck.Create());
        await this.context.SaveChangesAsync();
    }
}