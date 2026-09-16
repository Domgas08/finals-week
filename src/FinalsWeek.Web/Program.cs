using FinalsWeek.Core

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDeckStore, InMemoryDeckStore>();

var app = builder.Build();

app.Run();