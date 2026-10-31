// Next Step C# — Video 4: LINQ for Beginners
// 4 beginner LINQ mistakes (and the fixes). Each one prints what really happens.

List<Expense> expenses =
[
    new("Pizza with friends", 25, "Food"),
    new("Uber to airport", 40, "Transport"),
    new("Groceries", 290, "Food"),
];

// Mistake 1: First() when nothing matches
try
{
    var gym = expenses.First(e => e.Category == "Gym");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Mistake 1: {ex.Message}");
}

// ...and LINQ on a list that is null
List<Expense>? notLoaded = null;
try
{
    var any = notLoaded!.First();
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"Mistake 1b: {ex.Message}");
}

// Mistake 2: FirstOrDefault() returns null, then the next line crashes
var gymExpense = expenses.FirstOrDefault(e => e.Category == "Gym");
try
{
    Console.WriteLine(gymExpense!.Amount);
}
catch (NullReferenceException)
{
    Console.WriteLine("Mistake 2: NullReferenceException (FirstOrDefault gave us null)");
}

// Fix for 1 and 2: check for null before using the result
if (gymExpense is null)
    Console.WriteLine("Fix: no Gym expense found. No crash.");

// Mistake 3: the query runs again every time you use it
int checks = 0;
var bigOnes = expenses.Where(e => { checks++; return e.Amount > 30; });

Console.WriteLine($"Count: {bigOnes.Count()}");
foreach (var e in bigOnes)
    Console.WriteLine($"  {e.Description}");
Console.WriteLine($"Mistake 3: the filter ran {checks} times for {expenses.Count} expenses");

// ...and much worse inside a for loop
checks = 0;
for (int i = 0; i < bigOnes.Count(); i++)
{
    var e = bigOnes.ElementAt(i);
}
Console.WriteLine($"Mistake 3b: inside a for loop it ran {checks} times");

// Same mistake with 1,000 expenses
var many = Enumerable.Range(1, 1000).Select(i => new Expense($"item{i}", i % 60, "Food")).ToList();
checks = 0;
var bigMany = many.Where(e => { checks++; return e.Amount > 30; });
for (int i = 0; i < bigMany.Count(); i++)
{
    var e = bigMany.ElementAt(i);
}
Console.WriteLine($"Mistake 3c: with 1,000 expenses it ran {checks:N0} times");

// Fix: run the query once with ToList(), then reuse the list
checks = 0;
var bigList = expenses.Where(e => { checks++; return e.Amount > 30; }).ToList();
for (int i = 0; i < bigList.Count; i++)
{
    var e = bigList[i];
}
Console.WriteLine($"Fix 3: with ToList() it ran {checks} times");

// Mistake 4: Count() > 0 checks EVERY item, Any() stops at the first match
checks = 0;
bool hasFood1 = expenses.Count(e => { checks++; return e.Category == "Food"; }) > 0;
Console.WriteLine($"Mistake 4: Count() > 0 checked {checks} items");

checks = 0;
bool hasFood2 = expenses.Any(e => { checks++; return e.Category == "Food"; });
Console.WriteLine($"Fix 4: Any() checked {checks} item");

record Expense(string Description, decimal Amount, string Category);
