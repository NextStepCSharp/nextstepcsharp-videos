// Next Step C# — Video 6: Number Guessing Game
// Chapter 3: difficulty modes with a guess limit

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
        return;
    }
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

record Mode(string Name, int Max, int MaxGuesses);
