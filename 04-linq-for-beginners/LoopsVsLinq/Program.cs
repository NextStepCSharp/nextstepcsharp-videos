// Next Step C# — Video 4: LINQ for Beginners
// The same 4 tasks on our expense tracker: first with loops, then with LINQ.
// The last part (CountBy, Index) needs .NET 9 or later.

List<Expense> expenses =
[
    new("Pizza with friends", 25, "Food"),
    new("Uber to airport", 40, "Transport"),
    new("Groceries", 290, "Food"),
    new("Electricity", 120, "Bills"),
    new("Movie night", 18, "Fun"),
    new("Bus pass", 30, "Transport"),
];

// ---------- 1. Filter: all Food expenses ----------
List<Expense> foodLoop = [];
foreach (var e in expenses)
    if (e.Category == "Food")
        foodLoop.Add(e);

var foodLinq = expenses.Where(e => e.Category == "Food").ToList();
Console.WriteLine($"1. Food: {string.Join(", ", foodLinq.Select(e => e.Description))}");

// ---------- 2. Transform: just the descriptions ----------
List<string> namesLoop = [];
foreach (var e in expenses)
    namesLoop.Add(e.Description);

var namesLinq = expenses.Select(e => e.Description).ToList();
Console.WriteLine($"2. Names: {namesLinq.Count} descriptions");

// ---------- 3. Find: the first expense over $100 ----------
Expense? bigLoop = null;
foreach (var e in expenses)
{
    if (e.Amount > 100)
    {
        bigLoop = e;
        break;
    }
}

var bigLinq = expenses.FirstOrDefault(e => e.Amount > 100);
Console.WriteLine($"3. First over $100: {bigLinq?.Description}");

// ---------- 4. Group: total per category ----------
var totalsLoop = new Dictionary<string, decimal>();
foreach (var e in expenses)
{
    totalsLoop.TryGetValue(e.Category, out decimal sum);
    totalsLoop[e.Category] = sum + e.Amount;
}

var totalsLinq = expenses
    .GroupBy(e => e.Category)
    .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) });

Console.WriteLine("4. Totals:");
foreach (var t in totalsLinq)
    Console.WriteLine($"   {t.Category,-10} ${t.Total}");

// ---------- New in .NET 9 ----------
// CountBy: how many expenses per category, in one line
foreach (var (category, count) in expenses.CountBy(e => e.Category))
    Console.WriteLine($"CountBy: {category} = {count}");

// Index: the position AND the item, no counter variable
foreach (var (index, e) in expenses.Index())
    Console.WriteLine($"Index: {index}. {e.Description}");

record Expense(string Description, decimal Amount, string Category);
