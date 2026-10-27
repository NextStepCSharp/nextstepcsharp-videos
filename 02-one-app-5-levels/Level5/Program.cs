// Next Step C# — Video 2: One App, 5 Levels
// Level 5: let a local AI model suggest a category
// Setup: dotnet add package Microsoft.Data.Sqlite
//        dotnet add package OllamaSharp
//        Install Ollama (ollama.com), then run: ollama pull phi3:mini

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient ai = new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini");

using var db = new SqliteConnection("Data Source=expenses.db");
db.Open();

var create = db.CreateCommand();
create.CommandText = """
    CREATE TABLE IF NOT EXISTS Expenses (
        Id          INTEGER PRIMARY KEY AUTOINCREMENT,
        Description TEXT NOT NULL,
        Amount      REAL NOT NULL,
        Category    TEXT NOT NULL
    )
    """;
create.ExecuteNonQuery();

string[] categories = ["Food", "Transport", "Bills", "Shopping", "Fun", "Other"];

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

        Console.Write("Category (press Enter to let AI suggest): ");
        string category = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(category))
        {
            category = await SuggestCategory(description);
            Console.WriteLine($"AI suggested: {category}");
        }

        var insert = db.CreateCommand();
        insert.CommandText = "INSERT INTO Expenses (Description, Amount, Category) VALUES ($d, $a, $c)";
        insert.Parameters.AddWithValue("$d", description);
        insert.Parameters.AddWithValue("$a", amount);
        insert.Parameters.AddWithValue("$c", category);
        insert.ExecuteNonQuery();
        Console.WriteLine("Saved to database!");
    }
    else if (choice == "2")
    {
        var select = db.CreateCommand();
        select.CommandText = "SELECT Description, Category, Amount FROM Expenses ORDER BY Id";
        using var reader = select.ExecuteReader();
        while (reader.Read())
            Console.WriteLine($"{reader.GetString(0),-20} {reader.GetString(1),-12} {reader.GetDouble(2),10:C}");
    }
    else if (choice == "3")
    {
        var totals = db.CreateCommand();
        totals.CommandText = """
            SELECT Category, SUM(Amount) AS Total
            FROM Expenses
            GROUP BY Category
            ORDER BY Total DESC
            """;
        using var reader = totals.ExecuteReader();
        while (reader.Read())
            Console.WriteLine($"{reader.GetString(0),-12} {reader.GetDouble(1),10:C}");
    }
    else if (choice == "4")
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
