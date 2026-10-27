// Next Step C# — Video 2: One App, 5 Levels
// Level 4: store expenses in a real database (SQLite)
// Setup: dotnet add package Microsoft.Data.Sqlite

using Microsoft.Data.Sqlite;

using var db = new SqliteConnection("Data Source=expenses.db");
db.Open();

// Create the table the first time the app runs
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

        var insert = db.CreateCommand();
        insert.CommandText = "INSERT INTO Expenses (Description, Amount, Category) VALUES ($d, $a, $c)";
        insert.Parameters.AddWithValue("$d", description);   // parameters, never string concatenation
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
