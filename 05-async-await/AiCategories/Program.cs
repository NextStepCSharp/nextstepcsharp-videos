// Next Step C# — Video 5: async and await
// Ask a local AI for 3 categories: one at a time, then all at once.
// Setup (same as video 2, Level 5): dotnet add package OllamaSharp
//        Ollama running, with: ollama pull phi3:mini

using System.Diagnostics;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient ai = new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini");
string[] categories = ["Food", "Transport", "Bills", "Shopping", "Fun", "Other"];
// Different expenses for each test, so neither test gets answers the AI has already seen
string[] batch1 = ["Pizza with friends", "Uber to airport", "Electricity bill"];
string[] batch2 = ["Groceries", "Bus pass", "Movie night"];

await Suggest("warm-up");   // load the model once, so the timing below is fair

var sw = Stopwatch.StartNew();
foreach (var e in batch1)
    Console.WriteLine($"{e} -> {await Suggest(e)}");
Console.WriteLine($"One at a time: {sw.ElapsedMilliseconds:N0} ms");
Console.WriteLine();

sw.Restart();
string[] answers = await Task.WhenAll(batch2.Select(Suggest));
for (int i = 0; i < batch2.Length; i++)
    Console.WriteLine($"{batch2[i]} -> {answers[i]}");
Console.WriteLine($"All at once:   {sw.ElapsedMilliseconds:N0} ms");

async Task<string> Suggest(string description)
{
    string prompt = $"""
        Pick the best category for this expense: "{description}".
        Choose exactly one of: {string.Join(", ", categories)}.
        Reply with the category word only.
        """;
    ChatResponse response = await ai.GetResponseAsync(prompt);
    string answer = response.Text.Trim().Trim('.', '"', '\'');
    // Never trust AI output blindly: only accept a category from our list
    return categories.FirstOrDefault(c => answer.Contains(c, StringComparison.OrdinalIgnoreCase)) ?? "Other";
}
