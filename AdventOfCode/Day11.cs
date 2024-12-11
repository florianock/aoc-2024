namespace AdventOfCode;

/// <summary>
/// --- Day 11: Plutonian Pebbles ---
/// </summary>
public sealed class Day11 : BaseDay
{
    private List<long> _input;

    public Day11()
    {
        _input = File.ReadAllText(InputFilePath).Split(' ').Select(long.Parse).ToList();
        // _input = "125 17".Split(' ').Select(long.Parse).ToList();
    }

    public override ValueTask<string> Solve_1() => new($"{StoneCountAfterBlinking(25)}"); // Test: 55312 

    public override ValueTask<string> Solve_2() => new($"{StoneCountAfterBlinking(75)}");

    private long StoneCountAfterBlinking(int times)
    {
        for (var i = 0; i < times; i++)
        {
            Blink();
        }

        return _input.Count;
    }

    private void Blink()
    {
        List<long> stones = [];
        foreach (var i in _input)
        {
            if (i == 0)
            {
                stones.Add(1);
            }
            else if (i.ToString().Length % 2 == 0)
            {
                var a = i.ToString()[..(i.ToString().Length / 2)];
                stones.Add(long.Parse(a));

                var b = i.ToString()[(i.ToString().Length / 2)..];
                stones.Add(long.Parse(b));
            }
            else
            {
                stones.Add(i * 2024);
            }
        }

        _input = stones;
    }
}