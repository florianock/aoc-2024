using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 11: Plutonian Pebbles ---
/// </summary>
public sealed class Day11 : BaseDay
{
    private readonly Counter<string> _starterStoneCounts;
    private readonly IDictionary<string, string[]> _cache;

    public Day11()
    {
        var line = File.ReadAllText(InputFilePath).Split(' ').ToList();
        // var line = "125 17".Split(' ').ToList();
        _starterStoneCounts = new Counter<string>();
        foreach (var stone in line)
            _starterStoneCounts.Update(stone);

        _cache = new Dictionary<string, string[]>();
    }

    public override ValueTask<string> Solve_1() =>
        new($"{StoneCountAfterBlinking(_starterStoneCounts, 25)}"); // Test: 55312 

    public override ValueTask<string> Solve_2() =>
        new($"{StoneCountAfterBlinking(_starterStoneCounts, 75)}"); // Test: 65601038650482

    private long StoneCountAfterBlinking(Counter<string> stoneCounts, int times)
    {
        for (var i = 0; i < times; i++)
            stoneCounts = Blink(stoneCounts);
        return stoneCounts.Sum(count => count.Value);
    }

    private Counter<string> Blink(Counter<string> stoneCounts)
    {
        var newStoneCounts = new Counter<string>();

        foreach (var kv in stoneCounts)
        {
            foreach (var stone in ChangeStone(kv)) newStoneCounts.Update(stone, kv.Value);
        }

        return newStoneCounts;
    }

    private string[] ChangeStone(KeyValuePair<string, long> kv)
    {
        if (_cache.TryGetValue(kv.Key, out var cachedResult)) return cachedResult;

        string[] result;
        if (long.Parse(kv.Key) == 0)
            result = ["1"];
        else if (kv.Key.Length % 2 != 0)
            result = [(long.Parse(kv.Key) * 2024).ToString()];
        else
        {
            result = [kv.Key[..(kv.Key.Length / 2)], kv.Key[(kv.Key.Length / 2)..]];
            result = result.Select(n =>
            {
                var trimmed = n.TrimStart('0');
                return trimmed == string.Empty ? "0" : trimmed;
            }).ToArray();
        }

        _cache.Add(kv.Key, result);
        return result;
    }
}