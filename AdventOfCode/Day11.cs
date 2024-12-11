using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 11: Plutonian Pebbles ---
/// </summary>
public sealed class Day11 : BaseDay
{
    private readonly Counter<string> _starterStones;
    private readonly Dictionary<string, string[]> _cache;

    public Day11()
    {
        var line = File.ReadAllText(InputFilePath).Split(' ').ToList();
        // var line = "125 17".Split(' ').ToList();
        _starterStones = new Counter<string>();
        foreach (var stone in line)
            _starterStones.Update(stone);

        _cache = new Dictionary<string, string[]>();
    }

    public override ValueTask<string> Solve_1() =>
        new($"{StoneCountAfterBlinking(_starterStones, 25)}"); // Test: 55312 

    public override ValueTask<string> Solve_2() =>
        new($"{StoneCountAfterBlinking(_starterStones, 75)}"); // Test: 65601038650482

    private long StoneCountAfterBlinking(Counter<string> stones, int times)
    {
        for (var i = 0; i < times; i++)
            stones = Blink(stones);
        return stones.Sum(count => count.Value);
    }

    private Counter<string> Blink(Counter<string> stones)
    {
        var changedStones = new Counter<string>();

        foreach (var (stone, count) in stones)
        {
            foreach (var changedStone in ChangeStone(stone))
                changedStones.Update(changedStone, count);
        }

        return changedStones;
    }

    private string[] ChangeStone(string stone)
    {
        if (_cache.TryGetValue(stone, out var cachedResult)) return cachedResult;

        string[] result;
        if (long.Parse(stone) == 0)
            result = ["1"];
        else if (stone.Length % 2 != 0)
            result = [(long.Parse(stone) * 2024).ToString()];
        else
        {
            result = [stone[..(stone.Length / 2)], stone[(stone.Length / 2)..]];
            result = result.Select(n =>
            {
                var trimmed = n.TrimStart('0');
                return trimmed == string.Empty ? "0" : trimmed;
            }).ToArray();
        }

        _cache.Add(stone, result);
        return result;
    }
}