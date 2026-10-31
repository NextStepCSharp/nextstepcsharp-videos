# async and await in C#, Explained Like You're Ordering Food

Free code for the Next Step C# video **"async and await in C#, Explained Like You're Ordering Food"**.

| Folder | What it shows | Needs |
|---|---|---|
| [OrderFood](OrderFood/Program.cs) | Cooking 3 dishes one at a time vs all at once with `Task.WhenAll`, then `Task.WhenEach` | .NET 9 or later (for `Task.WhenEach`) |
| [Mistakes](Mistakes/Program.cs) | 3 beginner async mistakes: forgetting `await`, `.Result`, `async void`. **It crashes at the end on purpose** (the `async void` demo). | .NET SDK only |
| [AiCategories](AiCategories/Program.cs) | `await` with a real local AI: suggest categories for expenses | OllamaSharp + Ollama |

## Before you start

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (free)
- An editor: Visual Studio 2026 Community or VS Code with the C# Dev Kit (both free)

## How to run

```
dotnet new console -o AsyncDemo
cd AsyncDemo
```

Replace `Program.cs` with the file you want, then:

```
dotnet run
```

## Extra setup for AiCategories

```
dotnet add package OllamaSharp
ollama pull phi3:mini
```

Make sure [Ollama](https://ollama.com) is running first. Same setup as the expense tracker video (Level 5).

## Results on my laptop (.NET 10.0.14)

| Test | Result |
|---|---|
| 3 dishes, one at a time | 6,044 ms |
| 3 dishes, `Task.WhenAll` | 3,015 ms |
| `Task.WhenEach` | Fries at 1,013 ms, Juice at 2,010 ms, Pizza at 3,009 ms |
| `.Result` | thread blocked for 1,012 ms |

AI timings are not listed on purpose: with a local model, results changed a lot between runs because Ollama reuses work for prompts it has seen recently. Measure carefully.
