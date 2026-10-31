// Next Step C# — Video 6: Number Guessing Game
// Chapter 6: YOU pick the number, the computer guesses, and it never loses (binary search)

Console.WriteLine("Think of a number from 1 to 100. I'll guess it.");

int low = 1, high = 100, tries = 0;

while (true)
{
    int guess = (low + high) / 2;   // always guess the middle
    tries++;
    Console.Write($"Is it {guess}? (h = too high, l = too low, c = correct): ");
    string answer = (Console.ReadLine() ?? "").Trim().ToLower();

    if (answer == "c")
    {
        Console.WriteLine($"Got it in {tries} guesses!");
        break;
    }

    if (answer == "h") high = guess - 1;          // throw away the top half
    else if (answer == "l") low = guess + 1;      // throw away the bottom half
    else
    {
        tries--;
        Console.WriteLine("Please type h, l or c.");
        continue;
    }

    if (low > high)
    {
        Console.WriteLine("That's impossible. Did you change your number?");
        break;
    }
}

// Proof: play every possible secret number and keep the worst case
Console.WriteLine();
Console.WriteLine($"Worst case for 1-50:   {WorstCase(50)} guesses");
Console.WriteLine($"Worst case for 1-100:  {WorstCase(100)} guesses");
Console.WriteLine($"Worst case for 1-1000: {WorstCase(1000)} guesses");

static int WorstCase(int max)
{
    int worst = 0;
    for (int secret = 1; secret <= max; secret++)
    {
        int lo = 1, hi = max, t = 0;
        while (true)
        {
            int g = (lo + hi) / 2;
            t++;
            if (g == secret) break;
            if (g < secret) lo = g + 1; else hi = g - 1;
        }
        worst = Math.Max(worst, t);
    }
    return worst;
}
