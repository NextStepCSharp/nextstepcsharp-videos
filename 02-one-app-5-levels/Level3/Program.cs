// Next Step C# — Video 2: One App, 5 Levels
// Level 3: save expenses to a JSON file so they survive a restart

using System.Text.Json;

const string FilePath = "expenses.json";

List<Expense> expenses = Load();
Console.WriteLine($"Loaded {expenses.Count} expense(s) from {FilePath}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Add   2) List   3) Totals by category   4) Quit");
    Console.Write("Choose: ");
    string? choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Description: ");
        string description = Console.ReadLine() ?? "";

        Console.Write("Amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("That's not a number. Try again.");
            continue;
        }

        Console.Write("Category: ");
        string category = Console.ReadLine() ?? "Other";

        expenses.Add(new Expense(description, amount, category));
        Save(expenses);
        Console.WriteLine("Saved to file!");
    }
    else if (choice == "2")
    {
        foreach (Expense e in expenses)
            Console.WriteLine($"{e.Description,-20} {e.Category,-12} {e.Amount,10:C}");

        Console.WriteLine($"{"TOTAL",-33} {expenses.Sum(e => e.Amount),10:C}");
    }
    else if (choice == "3")
    {
        foreach (var g in expenses.GroupBy(e => e.Category).OrderByDescending(g => g.Sum(e => e.Amount)))
            Console.WriteLine($"{g.Key,-12} {g.Sum(e => e.Amount),10:C}");
    }
    else if (choice == "4")
    {
        break;
    }
}

static List<Expense> Load()
{
    if (!File.Exists(FilePath))
        return [];

    string json = File.ReadAllText(FilePath);
    return JsonSerializer.Deserialize<List<Expense>>(json) ?? [];
}

static void Save(List<Expense> expenses)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    File.WriteAllText(FilePath, JsonSerializer.Serialize(expenses, options));
}

record Expense(string Description, decimal Amount, string Category);
