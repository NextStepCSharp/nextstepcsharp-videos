// Next Step C# — Web API, Part 1 — Chapter 3: the expense tracker API, first version

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Expense> expenses =
[
    new(1, "Pizza with friends", 25, "Food"),
    new(2, "Uber to airport", 40, "Transport"),
    new(3, "Groceries", 290, "Food"),
];
int nextId = 4;

// Get all expenses
app.MapGet("/expenses", () => expenses);

// Get one expense
app.MapGet("/expenses/{id:int}", (int id) => expenses.FirstOrDefault(e => e.Id == id));

// Add an expense
app.MapPost("/expenses", (NewExpense input) =>
{
    expenses.Add(new Expense(nextId++, input.Description, input.Amount, input.Category));
    return Results.Ok();
});

// Total of all expenses
app.MapPost("/expenses/total", () => expenses.Sum(e => e.Amount));

// Delete an expense
app.MapDelete("/expenses/{id:int}", (int id) =>
{
    expenses.RemoveAll(e => e.Id == id);
    return Results.Ok();
});

app.Run("http://localhost:5000");

record Expense(int Id, string Description, decimal Amount, string Category);
record NewExpense(string Description, decimal Amount, string Category);
