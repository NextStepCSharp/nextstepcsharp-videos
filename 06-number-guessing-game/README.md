# Build a C# Number Guessing Game with an AI Game Master (6 Chapters)

Free code for the Next Step C# video **"Build a C# Number Guessing Game with an AI Game Master (6 Chapters, Beginner)"**.

Each chapter is a complete `Program.cs` that builds on the one before.

| Chapter | What it adds | Needs |
|---|---|---|
| [Chapter1](Chapter1/Program.cs) | The basic game: random number, loop, too high / too low. **Has problems left in on purpose** (`int.Parse`) | .NET SDK |
| [Chapter2](Chapter2/Program.cs) | Input that never crashes: `int.TryParse` + range check | .NET SDK |
| [Chapter3](Chapter3/Program.cs) | Difficulty modes (Easy 1–50/10, Medium 1–100/7, Hard 1–1000/10) + `ReadNumber` helper | .NET SDK |
| [Chapter4](Chapter4/Program.cs) | High scores saved to `scores.json`, Top 3 with LINQ, C# 14 `field` keyword | **.NET 10** |
| [Chapter5](Chapter5/Program.cs) | AI game master: a riddle hint after your 3rd guess (validated, with a safe fallback) | .NET 10 + OllamaSharp + Ollama |
| [Chapter6](Chapter6/Program.cs) | You pick the number, the computer guesses with binary search, plus a worst-case proof | .NET SDK |

## Before you start

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (free)
- An editor: Visual Studio 2026 Community or VS Code with the C# Dev Kit (both free)

## How to run a chapter

```
dotnet new console -o GuessGame
cd GuessGame
```

Replace `Program.cs` with the chapter you want, then:

```
dotnet run
```

## Extra setup for Chapter 5

```
dotnet add package OllamaSharp
ollama pull phi3:mini
```

Make sure [Ollama](https://ollama.com) is running. If it isn't, the game still works and shows a plain hint instead.

## Results on my laptop (.NET 10.0.14)

Chapter 6, the computer playing against every possible number:

| Range | Worst case |
|---|---|
| 1–50 | 6 guesses |
| 1–100 | 7 guesses |
| 1–1000 | 10 guesses |

That's why Medium gives you 7 guesses and Hard gives you 10.

## Your challenge

Add a Chapter 7 yourself: a timer, a two-player mode, or a hint you design. Share what you built in the video's comments.
