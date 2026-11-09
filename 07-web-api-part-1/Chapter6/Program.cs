// Next Step C# — Web API, Part 1 — Chapter 6: an AI endpoint
// POST /expenses/categorize asks a local AI (Ollama, phi3:mini) for a category.
// Setup:  dotnet add package OllamaSharp
//         ollama pull phi3:mini   (and make sure Ollama is running)

using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// NEW: allow our web page (http://localhost:5500) to call this API
builder.Services.AddCors(options =>
    options.AddPolicy("ExpensePage", policy =>
        policy.WithOrigins("http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// NEW: the AI client, registered once and shared by every request
builder.Services.AddSingleton<IChatClient>(
    new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini"));

var app = builder.Build();
app.UseCors("ExpensePage");   // NEW: switch the policy on

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

// NEW: ask the AI for a category -> 200 OK, 400 Bad Request, or 503 if the AI is not running
string[] categories = ["Food", "Transport", "Bills", "Shopping", "Fun", "Other"];

api.MapPost("/categorize", async (CategorizeRequest request, IChatClient ai) =>
{
    if (string.IsNullOrWhiteSpace(request.Description))
        return Results.BadRequest("Description is required.");

    string prompt = $"""
        Pick the best category for this expense: "{request.Description}".
        Choose exactly one of: {string.Join(", ", categories)}.
        Reply with the category word only.
        """;

    try
    {
        ChatResponse response = await ai.GetResponseAsync(prompt);
        string answer = response.Text.Trim();

        // Never trust AI output blindly: only accept a category from our list
        string category = categories.FirstOrDefault(
            c => answer.Contains(c, StringComparison.OrdinalIgnoreCase)) ?? "Other";

        return Results.Ok(new { request.Description, Category = category });
    }
    catch (HttpRequestException)
    {
        return Results.Problem("The AI is not running. Start Ollama and try again.", statusCode: 503);
    }
});

app.Run("http://localhost:5000");

record Expense(int Id, string Description, decimal Amount, string Category);
record NewExpense(string Description, decimal Amount, string Category);
record CategorizeRequest(string Description);
