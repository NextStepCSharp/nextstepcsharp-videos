// Next Step C# — Web API, Part 2 — Chapter 4: totals per category with LeftJoin (new in .NET 10)
// Chapter 3 + see the SQL EF Core sends + GET /expenses/by-category

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=expenses.db")
           // NEW: print every SQL command EF Core sends to the database
           .LogTo(Console.WriteLine, LogLevel.Information));

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

// FIX: return a DTO (only the fields the client needs), and use the async version
api.MapGet("/", async (AppDbContext db) =>
    Results.Ok(await db.Expenses
        .Select(e => new ExpenseDto(e.Id, e.Description, e.Amount, e.Category!.Name))
        .ToListAsync()));

api.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Expenses
        .Where(e => e.Id == id)
        .Select(e => new ExpenseDto(e.Id, e.Description, e.Amount, e.Category!.Name))
        .FirstOrDefaultAsync() is ExpenseDto expense
        ? Results.Ok(expense)
        : Results.NotFound());

// FIX: async lambda + await the save, and check the category exists first
api.MapPost("/", async (NewExpense input, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(input.CategoryId);
    if (category is null)
        return Results.BadRequest($"Category {input.CategoryId} does not exist.");

    var expense = new Expense { Description = input.Description, Amount = input.Amount, Category = category };
    db.Expenses.Add(expense);
    await db.SaveChangesAsync();

    var dto = new ExpenseDto(expense.Id, expense.Description, expense.Amount, category.Name);
    return Results.Created($"/expenses/{expense.Id}", dto);
});

// FIX: 404 when it is not there, instead of Remove(null) crashing
api.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var expense = await db.Expenses.FindAsync(id);
    if (expense is null)
        return Results.NotFound();

    db.Expenses.Remove(expense);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// NEW in .NET 10: LeftJoin. Every category, even one with no expenses (it gets 0).
// Before .NET 10 this needed GroupJoin + SelectMany + DefaultIfEmpty.
api.MapGet("/by-category", async (AppDbContext db) =>
{
    var rows = await db.Categories
        .LeftJoin(db.Expenses,
            category => category.Id,
            expense => expense.CategoryId,
            (category, expense) => new { category.Name, Amount = expense == null ? 0m : expense.Amount })
        .ToListAsync();

    // SQLite cannot add up decimals itself, so we group and add in C#
    var totals = rows
        .GroupBy(r => r.Name)
        .Select(g => new CategoryTotal(g.Key, g.Sum(r => r.Amount)))
        .OrderByDescending(t => t.Total)
        .ToList();

    return Results.Ok(totals);
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

// NEW: what the API sends back. No navigation properties, so no cycles.
public record ExpenseDto(int Id, string Description, decimal Amount, string Category);

public record CategoryTotal(string Category, decimal Total);

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
}
