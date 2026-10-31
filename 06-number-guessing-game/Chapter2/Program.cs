// Next Step C# — Video 6: Number Guessing Game
// Chapter 2: input that never crashes (TryParse + range check)

int secret = Random.Shared.Next(1, 101);
int guesses = 0;

Console.WriteLine("Guess my number (1 to 100)!");

while (true)
{
    Console.Write("Your guess: ");

    // TryParse never crashes. It returns false for "abc", "" or null.
    if (!int.TryParse(Console.ReadLine(), out int guess))
    {
        Console.WriteLine("That's not a number. Try again.");
        continue;
    }

    if (guess < 1 || guess > 100)
    {
        Console.WriteLine("Please pick a number from 1 to 100.");
        continue;
    }

    guesses++;   // only real guesses count

    if (guess < secret)
        Console.WriteLine("Too low!");
    else if (guess > secret)
        Console.WriteLine("Too high!");
    else
    {
        Console.WriteLine($"Correct! You got it in {guesses} {(guesses == 1 ? "guess" : "guesses")}.");
        break;
    }
}
