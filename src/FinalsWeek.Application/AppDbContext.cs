using Microsoft.EntityFrameworkCore;

namespace FinalsWeek.Application;

public class AppDbContext : DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}