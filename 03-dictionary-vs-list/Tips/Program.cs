// Next Step C# — Video 3: Dictionary vs List
// 3 beginner Dictionary mistakes + the new OrderedDictionary (needs .NET 9 or later)

var budgets = new Dictionary<string, decimal>
{
    ["Food"] = 300,
    ["Transport"] = 100,
};

// Mistake 1: reading a key that doesn't exist (a typo!) crashes
try
{
    Console.WriteLine(budgets["Fod"]);
}
catch (KeyNotFoundException e)
{
    Console.WriteLine($"Mistake 1: {e.Message}");
}

// Fix 1: TryGetValue never crashes
if (budgets.TryGetValue("Fod", out decimal fod))
    Console.WriteLine(fod);
else
    Console.WriteLine("Fix 1: TryGetValue says 'Fod' isn't there. No crash.");

// Mistake 2: Add with a key that already exists crashes
try
{
    budgets.Add("Food", 350);
}
catch (ArgumentException e)
{
    Console.WriteLine($"Mistake 2: {e.Message}");
}

// Fix 2: the indexer adds OR updates; TryAdd only adds if the key is new
budgets["Food"] = 350;
bool added = budgets.TryAdd("Food", 999);
Console.WriteLine($"Fix 2: Food is now {budgets["Food"]}, TryAdd returned {added}");

// Mistake 3: ContainsKey + indexer searches the dictionary twice
if (budgets.ContainsKey("Transport"))
    Console.WriteLine($"Mistake 3: Transport = {budgets["Transport"]} (two lookups)");

// Fix 3: TryGetValue checks and reads in one lookup
if (budgets.TryGetValue("Transport", out decimal transport))
    Console.WriteLine($"Fix 3: Transport = {transport} (one lookup)");

// Order: a normal Dictionary does NOT promise any order
var normal = new Dictionary<string, int> { ["Mon"] = 1, ["Tue"] = 2, ["Wed"] = 3 };
normal.Remove("Mon");
normal["Thu"] = 4;
Console.WriteLine($"Dictionary:        {string.Join(", ", normal.Keys)}");

// .NET 9+: OrderedDictionary keeps the order you added things in
var ordered = new OrderedDictionary<string, int> { ["Mon"] = 1, ["Tue"] = 2, ["Wed"] = 3 };
ordered.Remove("Mon");
ordered["Thu"] = 4;
Console.WriteLine($"OrderedDictionary: {string.Join(", ", ordered.Keys)}");

// ...and you can use positions, like a List
ordered.Insert(0, "Sun", 0);
Console.WriteLine($"First item: {ordered.GetAt(0).Key}");
