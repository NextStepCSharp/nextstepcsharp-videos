// Next Step C# — Video 6: Number Guessing Game
// Chapter 1: the basic game (with a beginner mistake left in ON PURPOSE: int.Parse)

int secret = Random.Shared.Next(1, 101);
int guesses = 0;

Console.WriteLine("Guess my number (1 to 100)!");

while (true)
{
    Console.Write("Your guess: ");
    int guess = int.Parse(Console.ReadLine()!);
    guesses++;

    if (guess < secret)
        Console.WriteLine("Too low!");
    else if (guess > secret)
        Console.WriteLine("Too high!");
    else
    {
        Console.WriteLine($"Correct! You got it in {guesses} guesses.");
        break;
    }
}
