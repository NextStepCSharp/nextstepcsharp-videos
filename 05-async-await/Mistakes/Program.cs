// Next Step C# — Video 5: async and await
// 3 beginner async mistakes. Each one prints what really happens.

using System.Diagnostics;

// Mistake 1: forgetting await
Console.WriteLine("Mistake 1: no await");
SaveExpense("Pizza");                 // starts saving... and we don't wait
Console.WriteLine("  App says: Saved!");  // printed BEFORE the save finished
await Task.Delay(1500);               // (just so we can see the save finish)
Console.WriteLine();

Console.WriteLine("Fix 1: await it");
await SaveExpense("Pizza");
Console.WriteLine("  App says: Saved!");  // now it's true
Console.WriteLine();

// Mistake 2: .Result blocks the thread while it waits
Console.WriteLine("Mistake 2: .Result");
var sw = Stopwatch.StartNew();
string category = GetCategory().Result;   // this thread can do NOTHING for 1 second
Console.WriteLine($"  {category}: thread was blocked for {sw.ElapsedMilliseconds:N0} ms");
Console.WriteLine("  (in a desktop app, this freezes the window)");
Console.WriteLine();

// Mistake 3: async void, try/catch can't catch its exception
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.WriteLine($"  CRASH: {((Exception)e.ExceptionObject).Message} (try/catch did NOT catch it)");

Console.WriteLine("Fix 3 first: async Task, exception caught");
try
{
    await LoadBudgetsTask();
}
catch (Exception ex)
{
    Console.WriteLine($"  Caught: {ex.Message}");
}
Console.WriteLine();

Console.WriteLine("Mistake 3: async void");
try
{
    LoadBudgetsVoid();                // can't be awaited
}
catch (Exception ex)
{
    Console.WriteLine($"  Caught: {ex.Message}");   // never runs
}
await Task.Delay(1000);

static async Task SaveExpense(string name)
{
    await Task.Delay(1000);           // pretend to save to a database
    Console.WriteLine($"  ...{name} actually saved");
}

static async Task<string> GetCategory()
{
    await Task.Delay(1000);           // pretend to ask the AI
    return "Food";
}

static async Task LoadBudgetsTask()
{
    await Task.Delay(100);
    throw new InvalidOperationException("budgets.json not found");
}

static async void LoadBudgetsVoid()
{
    await Task.Delay(100);
    throw new InvalidOperationException("budgets.json not found");
}
