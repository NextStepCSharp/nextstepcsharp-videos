// Next Step C# — Web API, Part 2 — Chapter 1: a real database (EF Core + SQLite)
// Same endpoints as Part 1, but the expenses now live in a file: expenses.db
// Package: Microsoft.EntityFrameworkCore.Sqlite (already in ExpenseApi.csproj)

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// NEW: register the database. SQLite = one file, no server to install.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=expenses.db"));

var app = builder.Build();

// NEW: create the database file + tables the first time, and add 3 starter expenses once
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Expenses.Any())
    {
        db.Expenses.AddRange(
            new Expense { Description = "Pizza with friends", Amount = 25, Category = "Food" },
            new Expense { Description = "Uber to airport", Amount = 40, Category = "Transport" },
            new Expense { Description = "Groceries", Amount = 290, Category = "Food" });
        db.SaveChanges();
    }
}

var api = app.MapGroup("/expenses");

// Get all expenses -> 200
api.MapGet("/", async (AppDbContext db) =>
    Results.Ok(await db.Expenses.ToListAsync()));

// Get one expense -> 200 or 404
api.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Expenses.FindAsync(id) is Expense expense
        ? Results.Ok(expense)
        : Results.NotFound());

// Add an expense -> 201 (the database gives it its id)
api.MapPost("/", async (NewExpense input, AppDbContext db) =>
{
    var expense = new Expense { Description = input.Description, Amount = input.Amount, Category = input.Category };
    db.Expenses.Add(expense);
    await db.SaveChangesAsync();
    return Results.Created($"/expenses/{expense.Id}", expense);
});

// Total of all expenses -> 200
// SQLite cannot add up decimal values itself, so we load the amounts and add them in C#
api.MapGet("/total", async (AppDbContext db) =>
{
    var amounts = await db.Expenses.Select(e => e.Amount).ToListAsync();
    return Results.Ok(new { Total = amounts.Sum() });
});

// Delete an expense -> 204 or 404
api.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var expense = await db.Expenses.FindAsync(id);
    if (expense is null)
        return Results.NotFound();

    db.Expenses.Remove(expense);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run("http://localhost:5000");

// A class (not a record) so EF Core can fill in the Id after saving
public class Expense
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string Category { get; set; } = "";
}

public record NewExpense(string Description, decimal Amount, string Category);

// The database: one DbSet = one table
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
}
