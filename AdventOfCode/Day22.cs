namespace AdventOfCode;

/// <summary>
/// --- Day 22: Monkey Market ---
/// </summary>
public sealed class Day22 : BaseDay
{
    private readonly IEnumerable<long> _input;

    public Day22()
    {
        _input = File.ReadLines(InputFilePath).Select(long.Parse).ToList();
        // _input = "1\n10\n100\n2024".Split('\n').Select(long.Parse).ToList(); // part 1, Test: 37327623
        // _input = "1\n2\n3\n2024".Split('\n').Select(long.Parse).ToList(); // part 2, Test: 23
    }

    public override ValueTask<string> Solve_1() => new($"{_input.AsParallel().Sum(GetFinalSecretNumber)}");

    public override ValueTask<string> Solve_2() => new($"{_input
        .Aggregate(new Dictionary<int, int>(), (agg, n) => GetChangesForSecret(n, agg))
        .Max(n => n.Value)}");

    private static long GetFinalSecretNumber(long number) =>
        Enumerable.Range(0, 2000).Aggregate(number, (current, _) => GetNextSecretNumber(current));

    private static Dictionary<int, int> GetChangesForSecret(long number, Dictionary<int, int> allChanges)
    {
        var seenKeys = new HashSet<int>();
        var previousPrice = GetPrice(number);
        var key = 0;
        var changes = (0, 0, 0, 0);
        for (var i = 0; i < 1999; i++)
        {
            number = GetNextSecretNumber(number);
            var price = GetPrice(number);
            var change = price - previousPrice;
            key = ((key & 0x7FFF) << 5) + change + 10;
            var (_, a, b, c) = changes;
            changes = (a, b, c, change);
            // Console.WriteLine($"{changes} -> {key}");
            previousPrice = price;
            if (i < 3 || seenKeys.Contains(key)) continue;
            var value = allChanges.GetValueOrDefault(key, 0);
            allChanges[key] = price + value;
            seenKeys.Add(key);
        }

        return allChanges;

        int GetPrice(long secret) => (int)(secret % 10);
    }

    private static long GetNextSecretNumber(long secretNumber)
    {
        secretNumber = Prune(Mix(secretNumber << 6, secretNumber));
        secretNumber = Prune(Mix(secretNumber >> 5, secretNumber));
        return Prune(Mix(secretNumber << 11, secretNumber));

        long Mix(long n, long secret) => n ^ secret;
        long Prune(long n) => n & 0xFFFFFF;
    }
}