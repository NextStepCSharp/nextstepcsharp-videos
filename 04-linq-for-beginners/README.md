# LINQ for Beginners: Stop Writing a Loop for Everything

Free code for the Next Step C# video **"LINQ for Beginners: Stop Writing a Loop for Everything"**.

| Folder | What it shows | Needs |
|---|---|---|
| [LoopsVsLinq](LoopsVsLinq/Program.cs) | The same 4 tasks with loops, then LINQ: `Where`, `Select`, `FirstOrDefault`, `GroupBy` + the new `CountBy` and `Index` | .NET 9 or later |
| [Mistakes](Mistakes/Program.cs) | 4 beginner LINQ mistakes and their fixes, with counters that show what really runs | .NET SDK only |
| [Benchmark](Benchmark/Program.cs) | A `for` loop vs LINQ on 1,000,000 expenses | .NET SDK only |

## Before you start

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (free)
- An editor: Visual Studio 2026 Community or VS Code with the C# Dev Kit (both free)
- No extra packages

## How to run

```
dotnet new console -o LinqDemo
cd LinqDemo
```

Replace `Program.cs` with the file you want, then:

```
dotnet run
```

For the **Benchmark**, use Release mode, or the timing isn't fair:

```
dotnet run -c Release
```

## Results on my laptop

.NET 10.0.14, adding up Food across 1,000,000 expenses (average of 20 runs):

| Build | for loop | LINQ |
|---|---|---|
| Release | 7.99 ms | 16.78 ms |
| Debug | 14.69 ms | 14.60 ms |

And the biggest mistake: `Count()` + `ElementAt()` inside a `for` loop made the filter run **714,635 times** for 1,000 expenses. One `.ToList()` fixes it.

Your numbers will be different. The pattern won't.
