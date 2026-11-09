// Next Step C# — Web API, Part 1 — Chapter 4: the same API, with all 4 problems fixed
// Fixes: 404 when not found, 201 Created with the new expense, GET for total, 404 on bad delete
// Plus: MapGroup, so the "/expenses" route is written only once (no more route typos)

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Expense> expenses =
[
    new(1, "Pizza with friends", 25, "Food"),
    new(2, "Uber to airport", 40, "Transport"),
    new(3, "Groceries", 290, "Food"),
];
int nextId = 4;

var api = app.MapGroup("/expenses");

// Get all expenses -> 200 OK
api.MapGet("/", () => Results.Ok(expenses));

// Get one expense -> 200 OK, or 404 Not Found
api.MapGet("/{id:int}", (int id) =>
    expenses.FirstOrDefault(e => e.Id == id) is Expense expense
        ? Results.Ok(expense)
        : Results.NotFound());

// Add an expense -> 201 Created, with the new expense and where to find it
api.MapPost("/", (NewExpense input) =>
{
    var expense = new Expense(nextId++, input.Description, input.Amount, input.Category);
    expenses.Add(expense);
    return Results.Created($"/expenses/{expense.Id}", expense);
});

// Total of all expenses -> GET, because it only reads data
api.MapGet("/total", () => Results.Ok(new { Total = expenses.Sum(e => e.Amount) }));

// Delete an expense -> 204 No Content, or 404 Not Found
api.MapDelete("/{id:int}", (int id) =>
    expenses.RemoveAll(e => e.Id == id) > 0
        ? Results.NoContent()
        : Results.NotFound());

app.Run("http://localhost:5000");

record Expense(int Id, string Description, decimal Amount, string Category);
record NewExpense(string Description, decimal Amount, string Category);
