// Next Step C# — Web API, Part 1 — Chapter 1: a whole Web API in ONE file (.NET 10)
// Run it with:  dotnet run api.cs
// Then open:    http://localhost:5000/expenses

#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

List<Expense> expenses =
[
    new(1, "Pizza with friends", 25, "Food"),
    new(2, "Uber to airport", 40, "Transport"),
    new(3, "Groceries", 290, "Food"),
];

var app = WebApplication.Create(args);

app.MapGet("/", () => "Hello from a one-file Web API!");
app.MapGet("/expenses", () => expenses);

app.Run("http://localhost:5000");

record Expense(int Id, string Description, decimal Amount, string Category);
