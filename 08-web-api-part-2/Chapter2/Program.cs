// Next Step C# — Web API, Part 2 — Chapter 2: two tables (Expenses + Categories)
// First version. It builds and runs... test every request before you trust it.

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=expenses.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Categories.Any())
    {
        var food = new Category { Name = "Food" };
        var transport = new Category { Name = "Transport" };
        db.Categories.AddRange(food, transport, new Category { Name = "Bills" }, new Category { Name = "Fun" });
        db.Expenses.AddRange(
            new Expense { Description = "Pizza with friends", Amount = 25, Category = food },
            new Expense { Description = "Uber to airport", Amount = 40, Category = transport },
            new Expense { Description = "Groceries", Amount = 290, Category = food });
        db.SaveChanges();
    }
}

var api = app.MapGroup("/expenses");

// Get all expenses, with their category
api.MapGet("/", (AppDbContext db) =>
    Results.Ok(db.Expenses.Include(e => e.Category).ToList()));

// Get one expense
api.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Expenses.FindAsync(id) is Expense expense
        ? Results.Ok(expense)
        : Results.NotFound());

// Add an expense
api.MapPost("/", (NewExpense input, AppDbContext db) =>
{
    var expense = new Expense { Description = input.Description, Amount = input.Amount, CategoryId = input.CategoryId };
    db.Expenses.Add(expense);
    db.SaveChangesAsync();
    return Results.Created($"/expenses/{expense.Id}", expense);
});

// Delete an expense
api.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var expense = await db.Expenses.FindAsync(id);
    db.Expenses.Remove(expense!);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run("http://localhost:5000");

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Expense> Expenses { get; set; } = [];
}

public class Expense
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}

public record NewExpense(string Description, decimal Amount, int CategoryId);

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
}
