using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode;

/// <summary>
/// --- Day 14: Restroom Redoubt ---
/// </summary>
public sealed partial class Day14 : BaseDay
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<Robot> _robots;

    public Day14()
    {
        // var input =
        // "p=0,4 v=3,-3\np=6,3 v=-1,-3\np=10,3 v=-1,2\np=2,0 v=2,-1\np=0,0 v=1,3\np=3,0 v=-2,-2\np=7,6 v=-1,-3\np=3,0 v=-1,-2\np=9,3 v=2,3\np=7,3 v=-1,2\np=2,4 v=2,-3\np=9,5 v=-3,-3"
        // .Split("\n").ToList();
        var input = File.ReadLines(InputFilePath).ToList();
        _width = input.Count > 12 ? 101 : 11;
        _height = input.Count > 12 ? 103 : 7;
        _robots = [];
        foreach (var m in input.Select(line => RobotRegex().Match(line)))
        {
            _robots.Add(new Robot((int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
                (int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value))));
        }
    }

    public override ValueTask<string> Solve_1() =>
        new($"{GetTotalSafetyFactor(_robots.Select(r => Move(r, 100)))}"); // Test: 12

    public override ValueTask<string> Solve_2() => new($"{FindEasterEgg()}");

    private Position Move(Robot robot, int seconds) =>
        new(((robot.P.Item1 + robot.V.Item1 * seconds) % _width + _width) % _width,
            ((robot.P.Item2 + robot.V.Item2 * seconds) % _height + _height) % _height);

    private long GetTotalSafetyFactor(IEnumerable<Position> positions) =>
        CountQuadrants(positions).Aggregate(1L, (agg, cur) => agg * cur);

    private long[] CountQuadrants(IEnumerable<Position> positions)
    {
        var middleX = _width / 2;
        var middleY = _height / 2;
        var quadrants = new long[4];
        foreach (var pos in positions.Where(p => p.X != middleX && p.Y != middleY))
        {
            if (pos.X > middleX)
            {
                if (pos.Y < middleY) quadrants[0] += 1;
                else quadrants[3] += 1;
            }
            else
            {
                if (pos.Y < middleY) quadrants[1] += 1;
                else quadrants[2] += 1;
            }
        }

        return quadrants;
    }

    private int FindEasterEgg()
    {
        var result = -1;
        var x = _robots.Count * _width * _height;
        for (var seconds = 1; seconds < x; seconds++)
        {
            var newPositions = _robots.Select(r => Move(r, seconds)).ToList();
            if (!FormsChristmasTree(newPositions)) continue;
            result = seconds;
            // Print(newPositions, seconds);
            break;
        }

        return result;
    }

    private static bool FormsChristmasTree(List<Position> positions) => positions.Distinct().Count() == positions.Count;

    private void Print(List<Position> positions, int t)
    {
        var grouped = (
            from r in positions
            group r by new
            {
                r.X,
                r.Y
            }
            into g
            orderby g.Key.Y, g.Key.X
            select new { g.Key.X, g.Key.Y, Count = g.Count() }).ToList();
        var currentLine = 0;
        while (currentLine < _height)
        {
            var line = new StringBuilder(new string('.', _width), _width);
            var group = grouped.Where(g => g.Y == currentLine).ToList();
            if (group.Count != 0)
                foreach (var g in group)
                    line[g.X] = (char)(g.Count + '0');

            Console.WriteLine(line);
            currentLine++;
        }

        Console.WriteLine($"Time {t}");
    }

    [GeneratedRegex(@"p=(-?[0-9]+),(-?[0-9]+) v=(-?[0-9]+),(-?[0-9]+)")]
    private static partial Regex RobotRegex();

    public record Position(int X, int Y);

    public record Robot((int, int) P, (int, int) V);
}