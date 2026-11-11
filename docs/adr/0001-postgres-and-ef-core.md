# 0001: Postgres and EF Core for persistence

## Status

Accepted

## Context

We need a database that runs locally on every team member's machine with a single command, and a way for the code to store data in it. Right now decks live only in memory (`InMemoryDeckStore`), so everything is lost when the app restarts, and the upcoming register and log in feature will need to store users and their credentials.

## Decision

- Use PostgreSQL 17, run in Docker with `docker compose up -d`, so every team member gets the same setup.
- Use Entity Framework Core with the Npgsql provider, with one `AppDbContext` for the whole app (Identity tables will be added to it later).
- Keep `AppDbContext` and the migrations in the `FinalsWeek.Application` project.
- Apply migrations automatically on startup with `Database.Migrate()`, so a fresh database gets its schema without manual steps.
- Read the connection string from configuration (`ConnectionStrings:DefaultConnection`), not from code.

## Alternatives considered

- **SQLite:** needs no Docker and is simple, but it behaves differently from the server databases used in production, and it supports fewer features.
- **SQL Server:** works well with .NET, but it is heavier to run locally and is not as lightweight in Docker as Postgres.
- **Dapper or plain ADO.NET instead of EF Core:** gives more control over SQL, but we would have to write the schema management and mapping ourselves. EF Core migrations and Identity integration save that work.
- **Keeping the in-memory store:** no setup, but data disappears on restart, and it cannot store users.

## Consequences

- Every team member needs Docker installed to run the app locally.
- Schema changes go through EF Core migrations, which are committed to the repo.
- `FinalsWeek.Application` now depends on EF Core and Npgsql.
- The local database password is currently committed in `docker-compose.yml` and `appsettings.Development.json`. That is acceptable for a throwaway local database, but real credentials must come from environment variables or user-secrets in any shared or production environment.