# Build a C# Web API, Step by Step: Part 2, a real database

Next Step C# · .NET 10 · EF Core 10 · SQLite

Part 1 kept expenses in a list, so they vanished on every restart. Part 2 saves them in a SQLite database file (expenses.db).

| Folder | What it shows |
|---|---|
| Chapter1 | EF Core + SQLite: one table, async CRUD, and data that survives a restart |
| Chapter2 | Two tables (Expenses + Categories). **First version with problems left in on purpose.** Test it before you trust it. |
| Chapter3 | The fixed version: DTOs, async everywhere, every save awaited, 404 instead of a crash |
| Chapter4 | New in .NET 10: `LeftJoin` for totals per category, plus the SQL that EF Core sends |
| Chapter5 | AI (Ollama, phi3:mini) picks the category and the database saves it |

## Run a chapter
1. Open the chapter folder in Visual Studio 2026 (or run `dotnet run` inside it).
2. Open `expenses.http` and click **Send Request** on each request, top to bottom.
3. Each chapter makes its own `expenses.db`. Delete it to start fresh.

Chapter 5 needs Ollama running: `ollama pull phi3:mini`.

Write it yourself first, then ask an AI to review it.
