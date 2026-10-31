// Next Step C# — Video 5: async and await, explained like ordering food
// Cooking 3 dishes: one at a time, then all at once. The last part needs .NET 9 or later.

using System.Diagnostics;

// ---------- 1. One at a time: wait for each dish before ordering the next ----------
var sw = Stopwatch.StartNew();
await Cook("Pizza", 3000);
await Cook("Fries", 1000);
await Cook("Juice", 2000);
Console.WriteLine($"One at a time: {sw.ElapsedMilliseconds:N0} ms");
Console.WriteLine();

// ---------- 2. All at once: order everything, then wait ----------
sw.Restart();
Task<string> pizza = Cook("Pizza", 3000);   // order... don't wait yet
Task<string> fries = Cook("Fries", 1000);
Task<string> juice = Cook("Juice", 2000);
await Task.WhenAll(pizza, fries, juice);     // now wait for all of them
Console.WriteLine($"All at once:   {sw.ElapsedMilliseconds:N0} ms");
Console.WriteLine();

// ---------- 3. New in .NET 9: Task.WhenEach, eat each dish as soon as it's ready ----------
sw.Restart();
var orders = new[] { Cook("Pizza", 3000, quiet: true), Cook("Fries", 1000, quiet: true), Cook("Juice", 2000, quiet: true) };
await foreach (Task<string> ready in Task.WhenEach(orders))
    Console.WriteLine($"Eating {await ready} at {sw.ElapsedMilliseconds:N0} ms");

// Task.Delay stands in for the kitchen: waiting, without blocking anyone
static async Task<string> Cook(string dish, int ms, bool quiet = false)
{
    if (!quiet) Console.WriteLine($"  Ordered {dish}");
    await Task.Delay(ms);
    if (!quiet) Console.WriteLine($"  {dish} ready ({ms:N0} ms)");
    return dish;
}
