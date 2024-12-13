using System.Text.RegularExpressions;

namespace AdventOfCode;

/// <summary>
/// --- Day 13: Claw Contraption ---
/// </summary>
public sealed partial class Day13 : BaseDay
{
    private readonly List<List<(long, long)>> _clawMachines;

    public Day13()
    {
        // var input =
        // "Button A: X+94, Y+34\nButton B: X+22, Y+67\nPrize: X=8400, Y=5400\n\nButton A: X+26, Y+66\nButton B: X+67, Y+21\nPrize: X=12748, Y=12176\n\nButton A: X+17, Y+86\nButton B: X+84, Y+37\nPrize: X=7870, Y=6450\n\nButton A: X+69, Y+23\nButton B: X+27, Y+71\nPrize: X=18641, Y=10279"
        // .Split("\n\n").ToList();
        var input = File.ReadAllText(InputFilePath).Split("\n\n").ToList();

        _clawMachines = [];
        foreach (var block in input)
        {
            List<long> machine = [];
            foreach (Match match in ClawMachineRegex().Matches(block))
                machine.Add(long.Parse(match.Value));
            _clawMachines.Add(machine.Chunk(2).Select(arr => (arr[0], arr[1])).ToList());
        }
    }

    public override ValueTask<string> Solve_1() => new($"{FewestTokensToWinAll()}"); // Test: 480

    public override ValueTask<string> Solve_2() => new($"{FewestTokensToWinAll(true)}"); // Test: 875318608908

    private long FewestTokensToWinAll(bool fixUnitConversionError = false) =>
        _clawMachines
            .Select(machine => GetMinimumTokensToWin(machine[0], machine[1], machine[2], fixUnitConversionError))
            .Sum(result => result is null ? 0 : 3 * result.Value.Item1 + result.Value.Item2);

    private static (long, long)? GetMinimumTokensToWin((long, long) a, (long, long) b, (long, long) prize,
        bool fixUnitConversionError = false)
    {
        var (ax, ay) = a;
        var (bx, by) = b;
        var (px, py) = prize;
        if (fixUnitConversionError)
        {
            px += 10_000_000_000_000;
            py += 10_000_000_000_000;
        }

        var d = ax * by - ay * bx;
        var dx = px * by - py * bx;
        var dy = py * ax - px * ay;

        if (dx % d != 0 || dy % d != 0) return null;

        return (dx / d, dy / d);
    }

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex ClawMachineRegex();
}