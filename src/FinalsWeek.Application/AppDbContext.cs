using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FinalsWeek.Application;

public class AppDbContext : IdentityDbContext<AppUser>{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}