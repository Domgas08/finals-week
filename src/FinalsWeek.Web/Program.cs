using FinalsWeek.Application;
using FinalsWeek.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ISessionStore, InMemorySessionStore>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure AppDbContext with retry logic
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    }));

// Configure Services as Scoped
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDeckStore, SqlDeckStore>();
builder.Services.AddScoped<UploadGuard>(sp => new UploadGuard(10 * 1024 * 1024));

var app = builder.Build();

// Refactor database migrations to async
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 5 attempts to migrate with a 5 second delay
    for (int i = 0; i < 5; i++)
    {
        try
        {
            await context.Database.MigrateAsync();
            break;
        }
        catch
        {
            if (i == 4)
            {
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
