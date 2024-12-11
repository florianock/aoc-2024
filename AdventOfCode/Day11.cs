namespace AdventOfCode;

/// <summary>
/// --- Day 11: Plutonian Pebbles ---
/// </summary>
public sealed class Day11 : BaseDay
{
    private readonly IDictionary<string, long> _starterStoneCounts;
    private readonly IDictionary<string, string[]> _cache;

    public Day11()
    {
        var line = File.ReadAllText(InputFilePath).Split(' ').ToList();
        // var line = "125 17".Split(' ').ToList();
        _starterStoneCounts = new Dictionary<string, long>();
        foreach (var stone in line)
        {
            if (_starterStoneCounts.TryGetValue(stone, out var count))
                _starterStoneCounts[stone] = count + 1;
            else
                _starterStoneCounts.Add(stone, 1);
        }

        _cache = new Dictionary<string, string[]>();
    }

    public override ValueTask<string> Solve_1() =>
        new($"{StoneCountAfterBlinking(_starterStoneCounts, 25)}"); // Test: 55312 

    public override ValueTask<string> Solve_2() =>
        new($"{StoneCountAfterBlinking(_starterStoneCounts, 75)}"); // Test: 65601038650482

    private long StoneCountAfterBlinking(IDictionary<string, long> stoneCounts, int times)
    {
        for (var i = 0; i < times; i++)
            stoneCounts = Blink(stoneCounts);
        return stoneCounts.Sum(count => count.Value);
    }

    private Dictionary<string, long> Blink(IDictionary<string, long> stoneCounts)
    {
        var newStoneCounts = new Dictionary<string, long>();

        foreach (var kv in stoneCounts)
        {
            foreach (var stone in Cycle(kv))
            {
                if (newStoneCounts.TryGetValue(stone, out var count))
                    newStoneCounts[stone] = count + kv.Value;
                else
                    newStoneCounts.Add(stone, kv.Value);
            }
        }

        return newStoneCounts;
    }

    private string[] Cycle(KeyValuePair<string, long> kv)
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