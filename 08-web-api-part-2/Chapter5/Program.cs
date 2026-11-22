// Next Step C# — Web API, Part 2 — Chapter 5: AI picks the category, the database saves it
// Chapter 4 + POST /expenses/smart (needs Ollama running with phi3:mini)

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=expenses.db")
           // NEW: print every SQL command EF Core sends to the database
           .LogTo(Console.WriteLine, LogLevel.Information));

// The AI client from Part 1
builder.Services.AddSingleton<IChatClient>(
    new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini"));

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

// NEW: send only a description and an amount. The AI picks the category, we save it.
api.MapPost("/smart", async (SmartExpense input, AppDbContext db, IChatClient ai) =>
{
    if (string.IsNullOrWhiteSpace(input.Description))
        return Results.BadRequest("Description is required.");

    // The category list now comes from the database, not from the code
    var categories = await db.Categories.ToListAsync();
    string names = string.Join(", ", categories.Select(c => c.Name));

    string prompt = $"""
        Pick the best category for this expense: "{input.Description}".
        Choose exactly one of: {names}.
        Reply with the category word only.
        """;

    string answer;
    try
    {
        ChatResponse response = await ai.GetResponseAsync(prompt);
        answer = response.Text.Trim().Trim('.', '"');
    }
    catch (HttpRequestException)
    {
        return Results.Problem("The AI is not running. Start Ollama and try again.", statusCode: 503);
    }

    // Never trust AI output blindly: it must match a category in our database
    var category = categories.FirstOrDefault(c => c.Name.Equals(answer, StringComparison.OrdinalIgnoreCase));
    if (category is null)
        return Results.Problem($"The AI answered '{answer}', which is not one of our categories.", statusCode: 502);

    var expense = new Expense { Description = input.Description, Amount = input.Amount, Category = category };
    db.Expenses.Add(expense);
    await db.SaveChangesAsync();

    return Results.Created($"/expenses/{expense.Id}",
        new ExpenseDto(expense.Id, expense.Description, expense.Amount, category.Name));
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

public record SmartExpense(string Description, decimal Amount);

public record CategoryTotal(string Category, decimal Total);

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
}
