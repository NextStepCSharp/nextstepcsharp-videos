// Next Step C# — Web API, Part 1 — Chapter 2: the SAME endpoint, two ways
// A controller (the way many of us learned) vs a Minimal API (.NET 10)

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();          // only needed for the controller version

var app = builder.Build();

app.MapControllers();                        // controller version: /controller/expenses
app.MapGet("/minimal/expenses", () => ExpenseData.All);   // Minimal API version: ONE line

app.Run("http://localhost:5000");

// ---- The controller version ----
[ApiController]
[Route("controller/expenses")]
public class ExpensesController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Expense> GetAll() => ExpenseData.All;
}

// ---- Shared data ----
public record Expense(int Id, string Description, decimal Amount, string Category);

public static class ExpenseData
{
    public static List<Expense> All { get; } =
    [
        new(1, "Pizza with friends", 25, "Food"),
        new(2, "Uber to airport", 40, "Transport"),
        new(3, "Groceries", 290, "Food"),
    ];
}
