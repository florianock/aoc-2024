namespace AdventOfCode;

/// <summary>
/// --- Day 11: Plutonian Pebbles ---
/// </summary>
public sealed class Day11 : BaseDay
{
    private readonly Dictionary<string, long> _starterStoneCounts;

    public Day11()
    {
        var line = File.ReadAllText(InputFilePath).Split(' ').ToList();
        // var line = "125 17".Split(' ').ToList();
        _starterStoneCounts = [];
        foreach (var stone in line)
        {
            if (_starterStoneCounts.TryGetValue(stone, out var count))
                _starterStoneCounts[stone] = count + 1;
            else
                _starterStoneCounts.Add(stone, 1);
        }
    }

    public override ValueTask<string> Solve_1() =>
        new($"{StoneCountAfterBlinking(_starterStoneCounts, 25)}"); // Test: 55312 

    public override ValueTask<string> Solve_2() =>
        new($"{StoneCountAfterBlinking(_starterStoneCounts, 75)}"); // Test: 65601038650482

    private static long StoneCountAfterBlinking(Dictionary<string, long> stoneCounts, int times)
    {
        for (var i = 0; i < times; i++)
            stoneCounts = Blink(stoneCounts);
        return stoneCounts.Sum(count => count.Value);
    }

    private static Dictionary<string, long> Blink(Dictionary<string, long> stoneCounts)
    {
        var newStoneCounts = new Dictionary<string, long>();

        foreach (var kv in stoneCounts)
        {
            var result = Cycle(kv);
            foreach (var stone in result)
            {
                if (newStoneCounts.TryGetValue(stone, out var count)) newStoneCounts[stone] = count + kv.Value;
                else newStoneCounts.Add(stone, kv.Value);
            }
        }

        return newStoneCounts;
    }

    private static string[] Cycle(KeyValuePair<string, long> kv)
    {
        if (long.Parse(kv.Key) == 0) return ["1"];
        if (kv.Key.Length % 2 != 0) return [(long.Parse(kv.Key) * 2024).ToString()];

        string[] result = [kv.Key[..(kv.Key.Length / 2)], kv.Key[(kv.Key.Length / 2)..]];
        return result.Select(n =>
        {
            var trimmed = n.TrimStart('0');
            return trimmed == string.Empty ? "0" : trimmed;
        }).ToArray();
    }
}