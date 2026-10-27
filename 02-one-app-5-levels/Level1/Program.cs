// Next Step C# — Video 2: One App, 5 Levels
// Level 1: a console expense tracker (variables, loops, input)

List<string> descriptions = [];
List<decimal> amounts = [];

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Add expense   2) List expenses   3) Quit");
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

        descriptions.Add(description);
        amounts.Add(amount);
        Console.WriteLine("Saved!");
    }
    else if (choice == "2")
    {
        decimal total = 0;
        for (int i = 0; i < descriptions.Count; i++)
        {
            Console.WriteLine($"{descriptions[i],-20} {amounts[i],10:C}");
            total += amounts[i];
        }
        Console.WriteLine($"{"TOTAL",-20} {total,10:C}");
    }
    else if (choice == "3")
    {
        break;
    }
}
