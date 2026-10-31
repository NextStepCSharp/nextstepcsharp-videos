// Next Step C# — Video 4: LINQ for Beginners
// Speed test: a for loop vs LINQ on 1,000,000 expenses
// Run in Release mode: dotnet run -c Release

using System.Diagnostics;

Console.WriteLine($".NET {Environment.Version}");

string[] categories = ["Food", "Transport", "Bills", "Shopping", "Fun"];
var rng = new Random(42);
List<Expense> expenses = [];
for (int i = 0; i < 1_000_000; i++)
    expenses.Add(new Expense($"item{i}", rng.Next(1, 500), categories[rng.Next(categories.Length)]));

// Warm up both versions first, so we time the real work
for (int r = 0; r < 5; r++) { LoopTotal(); LinqTotal(); }

const int Runs = 20;
var sw = Stopwatch.StartNew();
for (int r = 0; r < Runs; r++) LoopTotal();
double loopMs = sw.Elapsed.TotalMilliseconds / Runs;

sw.Restart();
for (int r = 0; r < Runs; r++) LinqTotal();
double linqMs = sw.Elapsed.TotalMilliseconds / Runs;

Console.WriteLine($"Food total: {LoopTotal():N0} (loop) = {LinqTotal():N0} (LINQ)");
Console.WriteLine($"for loop : {loopMs,6:F2} ms for 1,000,000 expenses");
Console.WriteLine($"LINQ     : {linqMs,6:F2} ms for 1,000,000 expenses");
Console.WriteLine($"LINQ took {linqMs / loopMs:F1}x as long");

decimal LoopTotal()
{
    decimal total = 0;
    for (int i = 0; i < expenses.Count; i++)
        if (expenses[i].Category == "Food")
            total += expenses[i].Amount;
    return total;
}

decimal LinqTotal() =>
    expenses.Where(e => e.Category == "Food").Sum(e => e.Amount);

record Expense(string Description, decimal Amount, string Category);
