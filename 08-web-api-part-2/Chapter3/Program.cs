// Next Step C# — Web API, Part 2 — Chapter 3: the fixed version
// Fixes: DTOs instead of entities, async everywhere, await every save, 404 instead of a crash

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

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
}
