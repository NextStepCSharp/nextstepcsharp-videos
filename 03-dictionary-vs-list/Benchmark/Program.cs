// Next Step C# — Video 3: Dictionary vs List
// Speed test: List.Find vs Dictionary.TryGetValue at 10 to 100,000 items
// Run in Release mode: dotnet run -c Release

using System.Diagnostics;

Console.WriteLine($".NET {Environment.Version}");
foreach (int n in new[] { 10, 100, 1_000, 10_000, 100_000 })
{
    var list = new List<(string Name, decimal Price)>();
    var dict = new Dictionary<string, decimal>();
    for (int i = 0; i < n; i++) { list.Add(($"item{i}", i)); dict[$"item{i}"] = i; }
    var keys = Enumerable.Range(0, 1000).Select(i => $"item{(i * 7919) % n}").ToArray();
    // warm-up
    for (int r = 0; r < 200; r++) { foreach (var k in keys) { list.Find(x => x.Name == k); dict.TryGetValue(k, out _); } }
    int reps = n >= 10_000 ? 5 : n >= 1000 ? 200 : 5000;
    var sw = Stopwatch.StartNew();
    for (int r = 0; r < reps; r++) foreach (var k in keys) list.Find(x => x.Name == k);
    double listNs = sw.Elapsed.TotalMilliseconds * 1e6 / (reps * keys.Length);
    sw.Restart();
    for (int r = 0; r < reps * 20; r++) foreach (var k in keys) dict.TryGetValue(k, out _);
    double dictNs = sw.Elapsed.TotalMilliseconds * 1e6 / (reps * 20 * keys.Length);
    Console.WriteLine($"{n,8} items | List.Find {listNs,12:F0} ns | Dictionary {dictNs,6:F0} ns | {listNs / dictNs,8:F0}x");
}
