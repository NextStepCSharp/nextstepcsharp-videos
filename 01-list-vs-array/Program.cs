// Next Step C# — Video 1: List vs Array
// .NET 10 console app (top-level statements)
// Full video: https://www.youtube.com/@NextStepCSharp

// 1. Arrays: fixed size
int[] scores = [90, 85, 77];
Console.WriteLine(scores[0]);      // 90
Console.WriteLine(scores.Length);  // 3
scores[1] = 88;                    // changing a value works
// scores[3] = 70;                 // crash: IndexOutOfRangeException

// 2. Lists: grow and shrink
List<int> scoreList = [90, 85, 77];
scoreList.Add(95);
scoreList.Remove(85);
Console.WriteLine(scoreList.Count); // 3

// 3. Under the hood: Capacity
List<int> numbers = new List<int>();
Console.WriteLine($"Count: {numbers.Count}, Capacity: {numbers.Capacity}");
for (int i = 1; i <= 9; i++)
{
    numbers.Add(i);
    Console.WriteLine($"Count: {numbers.Count}, Capacity: {numbers.Capacity}");
}

// Tip: if you know the size, set the capacity up front
List<int> bigList = new List<int>(1000);
Console.WriteLine(bigList.Capacity); // 1000

// 4. Mistake 1: Reverse() does not sort
List<string> fruits = ["banana", "apple", "cherry"];
fruits.Reverse();
Console.WriteLine(string.Join(", ", fruits)); // cherry, apple, banana
fruits.Sort();
fruits.Reverse();
Console.WriteLine(string.Join(", ", fruits)); // cherry, banana, apple

// 5. Mistake 2: removing items inside foreach
List<int> nums = [1, 2, 3, 4, 5, 6];
// foreach (int n in nums)
// {
//     if (n % 2 == 0) nums.Remove(n); // crash: InvalidOperationException
// }
nums.RemoveAll(n => n % 2 == 0);
Console.WriteLine(string.Join(", ", nums)); // 1, 3, 5
