# Finals week
A study app for students. Make question sets for your courses, revise them, and play them live with your coursemates.


## What it is
Revising is usually rereading slides and hoping it sticks. You end up going over things you already know, and the practice questions everyone writes get thrown away after the exam.

Finals Week does three things:
 
- **Shared question sets.** You and your coursemates build a set of questions for a course. It stays there for next year.
 
- **Smart revision.** The app decides what you should revise today. Questions you keep getting right show up less often. Questions you keep missing come back tomorrow.
 
- **Live rounds.** Start a room, share the code, and your group answers the same questions at the same time against a clock. Faster correct answers score more.


## How you use it
1. Sign up
2. Add questions, or import a set from a file
3. Study — the app shows you what's due today
4. Play a live round with your group when you want to make it less boring
5. Check which topics you're weakest in

## Tech
| Layer | Technology |
|---|---|
| Backend | ASP.NET Core |
| Frontend | Blazor |
| Database | PostgreSQL |


### Team
Austėja Likaitė
Ugnius Kavaliauskas
Domas Grimalauskas
Feliksas Oskaras Klimas

## Running the database

1. Install Docker Desktop.
2. From the repo root, run: docker compose up -d

## Test coverage

Coverage is measured with [coverlet](https://github.com/coverlet-coverage/coverlet). EF migrations are excluded from the measurement.

### Run locally

From the repository root:

```bash
dotnet test --configuration Release /p:CollectCoverage=true /p:ExcludeByFile="**/Migrations/*.cs" /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage/
```

A coverage table is printed in the terminal. The full report is saved as `coverage/coverage.cobertura.xml` inside the test project folder.

### In CI

Every pull request run shows line and branch coverage in the job summary (open the run on the **Actions** tab and click **Summary**). The minimum line coverage is set in one place: `COVERAGE_THRESHOLD` at the top of `.github/workflows/ci.yml`.