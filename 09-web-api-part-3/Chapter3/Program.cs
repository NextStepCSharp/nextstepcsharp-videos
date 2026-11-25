// Next Step C# — Web API, Part 3 — Chapter 3: .NET 10 built-in validation

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=expenses.db"));
builder.Services.AddValidation();   // NEW in .NET 10

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();   // fresh data every run, so every test starts the same
    db.Database.EnsureCreated();
    var food = new Category { Name = "Food" };
    var transport = new Category { Name = "Transport" };
    db.Categories.AddRange(food, transport, new Category { Name = "Bills" }, new Category { Name = "Fun" });
    db.Expenses.AddRange(
        new Expense { Description = "Pizza with friends", Amount = 25, Category = food },
        new Expense { Description = "Uber to airport", Amount = 40, Category = transport },
        new Expense { Description = "Groceries", Amount = 290, Category = food });
    db.SaveChanges();
}

var api = app.MapGroup("/expenses");

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

// NEW in .NET 10: the rules live on the record (see UpdateExpense below).
// AddValidation() checks them BEFORE this code runs and returns 400 with every error at once.
api.MapPut("/{id:int}", async (int id, UpdateExpense input, AppDbContext db) =>
{
    var expense = await db.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
    if (expense is null)
        return Results.NotFound();

    expense.Description = input.Description!;
    expense.Amount = input.Amount!.Value;
    await db.SaveChangesAsync();

    return Results.Ok(new ExpenseDto(expense.Id, expense.Description, expense.Amount, expense.Category!.Name));
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

public record UpdateExpense(
    [Required, StringLength(100)] string? Description,
    [Required, Range(0.01, 100_000)] decimal? Amount);

public record ExpenseDto(int Id, string Description, decimal Amount, string Category);

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
}
