#nullable enable
using System.Text.RegularExpressions;

namespace AdventOfCode;

/// <summary>
/// --- Day 13: Claw Contraption ---
/// </summary>
public sealed partial class Day13 : BaseDay
{
    private readonly List<Machine> _clawMachines;

    public Day13()
    {
        // var input =
        // "Button A: X+94, Y+34\nButton B: X+22, Y+67\nPrize: X=8400, Y=5400\n\nButton A: X+26, Y+66\nButton B: X+67, Y+21\nPrize: X=12748, Y=12176\n\nButton A: X+17, Y+86\nButton B: X+84, Y+37\nPrize: X=7870, Y=6450\n\nButton A: X+69, Y+23\nButton B: X+27, Y+71\nPrize: X=18641, Y=10279"
        // .Split("\n\n").ToList();
        var input = File.ReadAllText(InputFilePath).Split("\n\n").ToList();

        _clawMachines = [];
        foreach (var block in input)
        {
            List<long> numbers = [];
            foreach (Match match in ClawMachineRegex().Matches(block))
                numbers.Add(long.Parse(match.Value));
            _clawMachines.Add(new Machine(
                new Button(numbers[0], numbers[1]),
                new Button(numbers[2], numbers[3]),
                new Prize(numbers[4], numbers[5]))
            );
        }
    }

    public override ValueTask<string> Solve_1() => new($"{FewestTokensToWinAll()}"); // Test: 480

    public override ValueTask<string> Solve_2() => new($"{FewestTokensToWinAll(true)}"); // Test: 875318608908

    private long FewestTokensToWinAll(bool fixUnitConversionError = false) =>
        _clawMachines
            .Select(machine =>
            {
                if (!fixUnitConversionError) return GetMinimumButtonPressesToWin(machine);
                const long adjustment = 10_000_000_000_000;
                return GetMinimumButtonPressesToWin(machine with
                {
                    Prize = new Prize(X: machine.Prize.X + adjustment, Y: machine.Prize.Y + adjustment)
                });
            })
            .Sum(result => result is null ? 0 : 3 * result.A + result.B);

    private static ButtonPresses? GetMinimumButtonPressesToWin(Machine machine)
    {
        var (ax, ay) = machine.A;
        var (bx, by) = machine.B;
        var (px, py) = machine.Prize;

        var d = ax * by - ay * bx;
        var dx = px * by - py * bx;
        var dy = py * ax - px * ay;

        if (dx % d != 0 || dy % d != 0) return null;

        return new ButtonPresses(dx / d, dy / d);
    }

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex ClawMachineRegex();

    private record Button(long X, long Y);

    private record Prize(long X, long Y);

    private record Machine(Button A, Button B, Prize Prize);
    
    private record ButtonPresses(long A, long B);
}
