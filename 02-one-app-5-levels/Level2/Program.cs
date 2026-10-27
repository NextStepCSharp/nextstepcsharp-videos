// Next Step C# — Video 2: One App, 5 Levels
// Level 2: an Expense type + List<Expense> + LINQ totals

List<Expense> expenses = [];

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
        Console.WriteLine("Saved!");
    }
    else if (choice == "2")
    {
        foreach (Expense e in expenses)
            Console.WriteLine($"{e.Description,-20} {e.Category,-12} {e.Amount,10:C}");

        Console.WriteLine($"{"TOTAL",-33} {expenses.Sum(e => e.Amount),10:C}");
    }
    else if (choice == "3")
    {
        var totals = expenses
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .OrderByDescending(t => t.Total);

        foreach (var t in totals)
            Console.WriteLine($"{t.Category,-12} {t.Total,10:C}");
    }
    else if (choice == "4")
    {
        break;
    }
}

// One expense = one object. No more keeping two lists in sync.
record Expense(string Description, decimal Amount, string Category);
