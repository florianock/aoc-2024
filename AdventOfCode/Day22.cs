namespace AdventOfCode;

/// <summary>
/// --- Day 22: Monkey Market ---
/// </summary>
public sealed class Day22 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day22()
    {
        _input = File.ReadLines(InputFilePath); // 1909 too low
        // _input = "1\n10\n100\n2024".Split('\n'); // part 1
        // _input = "1\n2\n3\n2024".Split('\n'); // part 2
        // _input = "123".Split('\n'); // part 2
    }

    public override ValueTask<string> Solve_1() =>
        new($"{_input.AsParallel().Sum(n => GetFinalSecretNumber(int.Parse(n)))}"); // Test: 37327623

    public override ValueTask<string> Solve_2() => new($"{_input
        .AsParallel()
        .Aggregate(new Dictionary<(int, int, int, int), long>(), (agg, n) => Merge(agg, GetChangesAndBananas(int.Parse(n))))
        .Max(n => n.Value)}"); // Test: 23

    private static long GetFinalSecretNumber(int number) =>
        Enumerable.Range(0, 2000).Aggregate((long)number, (current, _) => GetNextSecretNumber(current));

    private static Dictionary<(int, int, int, int), long> GetChangesAndBananas(int number)
    {
        const int iterations = 2000;
        var changes = new Dictionary<(int, int, int, int), long>();
        var previousPrice = GetPrice(number);
        var previousChanges = (0, 0, 0, 0);
        var currentSecret = (long)number;
        for (var i = 0; i < iterations - 1; i++)
        {
            currentSecret = GetNextSecretNumber(currentSecret);
            var price = GetPrice(currentSecret);
            var (_, a, b, c) = previousChanges;
            var newChange = (a, b, c, price - previousPrice);
            previousPrice = price;
            if (i >= 3)
            {
                changes[newChange] = changes.GetValueOrDefault(newChange, price);
            }

            previousChanges = newChange;
        }

        // if (changes.TryGetValue((0, 5, 0, 0), out var value)) Console.WriteLine($"{number} => {value}.");
        // else Console.WriteLine($"Not found for {number}.");

        return changes;

        int GetPrice(long n) => int.Parse(n.ToString()[^1..]);
    }

    private static long GetNextSecretNumber(long secretNumber)
    {
        secretNumber = Prune(Mix(secretNumber * 64L, secretNumber));
        secretNumber = Prune(Mix(secretNumber / 32L, secretNumber));
        return Prune(Mix(secretNumber * 2048L, secretNumber));

        long Mix(long n, long secret) => n ^ secret;
        long Prune(long n) => n % 16777216L;
    }

    private static Dictionary<(int, int, int, int), long> Merge(
        Dictionary<(int, int, int, int), long> a,
        Dictionary<(int, int, int, int), long> b
    )
    {
        return b.Aggregate(a, (agg, cur) =>
        {
            if (agg.TryGetValue(cur.Key, out var value)) agg[cur.Key] += cur.Value;
            else agg[cur.Key] = cur.Value;
            return agg;
        });
    }
}