namespace AdventOfCode;

using BananaChanges = Dictionary<(byte, byte, byte, byte), int>;

/// <summary>
/// --- Day 22: Monkey Market ---
/// </summary>
public sealed class Day22 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day22()
    {
        _input = File.ReadLines(InputFilePath);
        // _input = "1\n10\n100\n2024".Split('\n'); // part 1
        // _input = "1\n2\n3\n2024".Split('\n'); // part 2
    }

    public override ValueTask<string> Solve_1() => new($"{_input
        .AsParallel()
        .Sum(n => GetFinalSecretNumber(long.Parse(n)))}"); // Test: 37327623

    public override ValueTask<string> Solve_2() => new($"{_input
        .Aggregate(new BananaChanges(), (agg, n) => GetBananaChangesForSecret(long.Parse(n), agg))
        .Max(n => n.Value)}"); // Test: 23

    private static long GetFinalSecretNumber(long number) =>
        Enumerable.Range(0, 2000).Aggregate(number, (current, _) => GetNextSecretNumber(current));

    private static BananaChanges GetBananaChangesForSecret(long number, BananaChanges changesLookup)
    {
        var theseChanges = new HashSet<(byte, byte, byte, byte)>();
        var previousPrice = GetPrice(number);
        (byte, byte, byte, byte) key = (0, 0, 0, 0);
        var currentSecret = number;
        for (var i = 0; i < 2000 - 1; i++)
        {
            currentSecret = GetNextSecretNumber(currentSecret);
            var price = GetPrice(currentSecret);
            var (_, a, b, c) = key;
            key = (a, b, c, (byte)(price - previousPrice + 10));
            previousPrice = price;
            if (i < 3 || theseChanges.Contains(key)) continue;
            var value = changesLookup.GetValueOrDefault(key, 0);
            changesLookup[key] = price + value;
            theseChanges.Add(key);
        }

        return changesLookup;

        byte GetPrice(long secret) => (byte)(secret % 10);
    }

    private static long GetNextSecretNumber(long secretNumber)
    {
        secretNumber = Prune(Mix(secretNumber << 6, secretNumber));
        secretNumber = Prune(Mix(secretNumber >> 5, secretNumber));
        return Prune(Mix(secretNumber << 11, secretNumber));

        long Mix(long n, long secret) => n ^ secret;
        long Prune(long n) => n % 16777216L;
    }
}