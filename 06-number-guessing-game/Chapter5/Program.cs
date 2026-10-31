// Next Step C# — Video 6: Number Guessing Game
// Chapter 5: an AI game master gives a riddle hint after your 3rd guess
// Setup (same as video 2, Level 5): dotnet add package OllamaSharp
//        Ollama running, with: ollama pull phi3:mini

using System.Text.Json;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient ai = new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini");

const string ScoreFile = "scores.json";
List<Score> scores = LoadScores();

Mode[] modes = [new("Easy", 50, 10), new("Medium", 100, 7), new("Hard", 1000, 10)];

Console.WriteLine("1) Easy (1-50, 10 guesses)   2) Medium (1-100, 7 guesses)   3) Hard (1-1000, 10 guesses)");
Mode mode = modes[ReadNumber("Choose a mode: ", 1, 3) - 1];

int secret = Random.Shared.Next(1, mode.Max + 1);
Console.WriteLine($"{mode.Name}: guess my number from 1 to {mode.Max}. You have {mode.MaxGuesses} guesses.");

for (int guesses = 1; guesses <= mode.MaxGuesses; guesses++)
{
    int guess = ReadNumber($"Guess {guesses}/{mode.MaxGuesses}: ", 1, mode.Max);

    if (guess < secret)
        Console.WriteLine("Too low!");
    else if (guess > secret)
        Console.WriteLine("Too high!");
    else
    {
        Console.WriteLine($"Correct! You got it in {guesses} {(guesses == 1 ? "guess" : "guesses")}.");
        Console.Write("Your name for the high scores: ");
        scores.Add(new Score { Name = Console.ReadLine() ?? "", Mode = mode.Name, Guesses = guesses });
        SaveScores(scores);
        ShowTop3(mode.Name);
        return;
    }

    if (guesses == 3)
        Console.WriteLine($"AI game master: {await RiddleHint(secret)}");
}

Console.WriteLine($"Out of guesses! The number was {secret}.");

// One helper for every number we read: never crashes, always in range
static int ReadNumber(string prompt, int min, int max)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int n) && n >= min && n <= max)
            return n;
        Console.WriteLine($"Please enter a number from {min} to {max}.");
    }
}

async Task<string> RiddleHint(int number)
{
    // WE pick a true fact. The AI only turns it into a riddle,
    // so it can't invent a wrong hint.
    string fact = number % 5 == 0 ? "the number is a multiple of five"
                : number % 2 == 0 ? "the number is even"
                : "the number is odd";

    string prompt = $"Turn this fact into one short, fun riddle for a guessing game: \"{fact}\". Do not use any digits.";

    try
    {
        ChatResponse response = await ai.GetResponseAsync(prompt);
        string hint = response.Text.Trim();

        // Never trust AI output: no digits (it must not leak the number) and keep it short
        if (hint.Length == 0 || hint.Length > 200 || hint.Any(char.IsDigit))
            return $"Hint: {fact}.";
        return hint;
    }
    catch (HttpRequestException)
    {
        return $"Hint: {fact}. (AI not running)";
    }
}

List<Score> LoadScores() =>
    File.Exists(ScoreFile)
        ? JsonSerializer.Deserialize<List<Score>>(File.ReadAllText(ScoreFile)) ?? []
        : [];

void SaveScores(List<Score> all) =>
    File.WriteAllText(ScoreFile, JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true }));

void ShowTop3(string modeName)
{
    Console.WriteLine($"Top 3 ({modeName}):");
    foreach (var s in scores.Where(s => s.Mode == modeName).OrderBy(s => s.Guesses).Take(3))
        Console.WriteLine($"  {s.Name,-10} {s.Guesses} {(s.Guesses == 1 ? "guess" : "guesses")}");
}

record Mode(string Name, int Max, int MaxGuesses);

class Score
{
    // C# 14 (.NET 10): 'field' is the hidden variable behind the property.
    // Empty names become "Player", and extra spaces are trimmed.
    public string Name
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? "Player" : value.Trim();
    } = "Player";

    public string Mode { get; set; } = "";
    public int Guesses { get; set; }
}
