# C# Dictionary vs List (Measured, for Beginners)

Free code for the Next Step C# video **"C# Dictionary vs List: Which Is Faster? (Measured, for Beginners)"**.

| Folder | What it shows | Needs |
|---|---|---|
| [Benchmark](Benchmark/Program.cs) | `List.Find` vs `Dictionary.TryGetValue` at 10 to 100,000 items | .NET SDK only |
| [Tips](Tips/Program.cs) | 3 beginner Dictionary mistakes + fixes, and the new `OrderedDictionary` | .NET 9 or later |
| [Budgets](Budgets/Program.cs) | Expense tracker budgets: category → monthly budget, AI suggests the category | OllamaSharp + Ollama (optional) |

## Before you start

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (free)
- An editor: Visual Studio 2026 Community or VS Code with the C# Dev Kit (both free)

## How to run

```
dotnet new console -o DictDemo
cd DictDemo
```

Replace `Program.cs` with the file you want, then:

```
dotnet run
```

For the **Benchmark**, use Release mode so the timing is fair:

```
dotnet run -c Release
```

## Extra setup for Budgets

```
dotnet add package OllamaSharp
```

It also uses [Ollama](https://ollama.com) with the `phi3:mini` model, the same setup as video 2, Level 5:

```
ollama pull phi3:mini
```

No AI? Just type the category yourself when the app asks.

## Results on my laptop

.NET 10.0.14, Release build, average time per lookup over 1,000 random lookups:

| Items | List.Find | Dictionary | Dictionary is |
|---|---|---|---|
| 10 | 45 ns | 13 ns | 3× faster |
| 100 | 297 ns | 10 ns | 30× faster |
| 1,000 | 2,376 ns | 11 ns | 207× faster |
| 10,000 | 23,834 ns | 17 ns | 1,441× faster |
| 100,000 | 634,913 ns | 35 ns | 18,335× faster |

Your numbers will be different. The pattern won't.

Tested on .NET 10.0.14 with the phi3:mini model.
