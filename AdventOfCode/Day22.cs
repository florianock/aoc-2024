namespace AdventOfCode;

using BananaChanges = Dictionary<(sbyte, sbyte, sbyte, sbyte), int>;

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
        .Sum(n => GetFinalSecretNumber(int.Parse(n)))}"); // Test: 37327623

    public override ValueTask<string> Solve_2() => new($"{_input
        .AsParallel()
        .Aggregate(new BananaChanges(), (agg, n) => Merge(agg, GetBananaChangesForSecret(int.Parse(n))))
        .Max(n => n.Value)}"); // Test: 23

    private static long GetFinalSecretNumber(int number) =>
        Enumerable.Range(0, 2000).Aggregate((long)number, (current, _) => GetNextSecretNumber(current));

    private static BananaChanges GetBananaChangesForSecret(int number)
    {
        var changesLookup = new BananaChanges();
        var previousPrice = GetPrice(number);
        (sbyte, sbyte, sbyte, sbyte) changes = (0, 0, 0, 0);
        var currentSecret = (long)number;
        for (var i = 0; i < 2000 - 1; i++)
        {
            currentSecret = GetNextSecretNumber(currentSecret);
            var price = GetPrice(currentSecret);
            var (_, a, b, c) = changes;
            changes = (a, b, c, (sbyte)(price - previousPrice));
            previousPrice = price;
            if (i >= 3)
            {
                changesLookup.TryAdd(changes, price);
            }
        }

        return changesLookup;

        sbyte GetPrice(long secret) => (sbyte)(secret % 10);
    }

    private static long GetNextSecretNumber(long secretNumber)
    {
        secretNumber = Prune(Mix(secretNumber * 64L, secretNumber));
        secretNumber = Prune(Mix(secretNumber / 32L, secretNumber));
        return Prune(Mix(secretNumber * 2048L, secretNumber));

        long Mix(long n, long secret) => n ^ secret;
        long Prune(long n) => n % 16777216L;
    }

    private static BananaChanges Merge(BananaChanges a, BananaChanges b)
    {
        return b.Aggregate(a, (agg, cur) =>
        {
            if (agg.TryGetValue(cur.Key, out _)) agg[cur.Key] += cur.Value;
            else agg[cur.Key] = cur.Value;
            return agg;
        });
    }
}