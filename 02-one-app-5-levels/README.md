# 5 C# Projects for Beginners, Built as ONE App

Free code for the Next Step C# video **"5 C# Projects for Beginners, in Order (Built as ONE App)"**.

The 5 projects are 5 levels of the same app: an expense tracker that grows one skill at a time. Each level is a complete `Program.cs` for a .NET 10 console app.

| Level | What it adds | Skill |
|---|---|---|
| [Level1](Level1/Program.cs) | Console menu, add and list expenses | Variables, loops, input |
| [Level2](Level2/Program.cs) | `Expense` record + `List<Expense>` + LINQ totals | Classes, collections, LINQ |
| [Level3](Level3/Program.cs) | Saves to `expenses.json` | JSON with System.Text.Json |
| [Level4](Level4/Program.cs) | Saves to an SQLite database | SQL, parameters |
| [Level5](Level5/Program.cs) | AI suggests a category (local model) | Microsoft.Extensions.AI + Ollama |

## Before you start

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (free)
- An editor: Visual Studio 2026 Community or VS Code with the C# Dev Kit (both free)
- Levels 1-3 need nothing else

## How to run a level

```
dotnet new console -o ExpenseTracker
cd ExpenseTracker
```

Replace `Program.cs` with the level's file, then run:

```
dotnet run
```

## Extra setup for levels 4 and 5

**Level 4** needs one package:

```
dotnet add package Microsoft.Data.Sqlite
```

**Level 5** needs two packages:

```
dotnet add package Microsoft.Data.Sqlite
dotnet add package OllamaSharp
```

It also needs [Ollama](https://ollama.com), a free app that runs the AI model on your computer. Install it, then download the model (about 2.2 GB):

```
ollama pull phi3:mini
```

Tested with Microsoft.Data.Sqlite 10.0.14, OllamaSharp 5.5.0 and phi3:mini.

## Challenge

Build Level 1 yourself first, then add one level at a time. Compare with this code only when you get stuck.
