// Next Step C# — Video 3: Dictionary vs List
// Budgets: the expense tracker gets a monthly budget per category.
// A Dictionary finds each category's budget instantly, no loop needed.
// Setup (same as video 2, Level 5): dotnet add package OllamaSharp
//        Ollama running, with: ollama pull phi3:mini

using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient ai = new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini");

// Category -> monthly budget.
// OrdinalIgnoreCase: "food", "Food" and "FOOD" all find the same key.
var budgets = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
{
    ["Food"] = 300,
    ["Transport"] = 100,
    ["Bills"] = 250,
    ["Shopping"] = 150,
    ["Fun"] = 80,
};

// Category -> spent so far this month
var spent = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

string[] categories = [.. budgets.Keys, "Other"];

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) Add expense   2) Budget report   3) Quit");
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

        Console.Write("Category (press Enter to let AI suggest): ");
        string category = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(category))
        {
            category = await SuggestCategory(description);
            Console.WriteLine($"AI suggested: {category}");
        }

        spent[category] = spent.GetValueOrDefault(category) + amount;

        // One lookup, no loop: is there a budget for this category?
        if (budgets.TryGetValue(category, out decimal budget))
        {
            decimal left = budget - spent[category];
            if (left < 0)
                Console.WriteLine($"Over budget! {category}: ${spent[category]:N2} spent of ${budget:N2}");
            else
                Console.WriteLine($"{category}: ${left:N2} left of ${budget:N2}");
        }
        else
        {
            Console.WriteLine($"No budget set for '{category}'.");
        }
    }
    else if (choice == "2")
    {
        foreach (var (category, budget) in budgets)
        {
            decimal s = spent.GetValueOrDefault(category);
            string flag = s > budget ? "  OVER" : "";
            Console.WriteLine($"{category,-10} ${s,9:N2} of ${budget,8:N2}{flag}");
        }
    }
    else if (choice == "3")
    {
        break;
    }
}

async Task<string> SuggestCategory(string description)
{
    string prompt = $"""
        Pick the best category for this expense: "{description}".
        Choose exactly one of: {string.Join(", ", categories)}.
        Reply with the category word only.
        """;

    try
    {
        ChatResponse response = await ai.GetResponseAsync(prompt);
        string answer = response.Text.Trim().Trim('.', '"', '\'');

        // Never trust AI output blindly: only accept a category from our list
        return categories.FirstOrDefault(c => answer.Contains(c, StringComparison.OrdinalIgnoreCase)) ?? "Other";
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Couldn't reach the AI. Is Ollama running?");
        return "Other";
    }
}
