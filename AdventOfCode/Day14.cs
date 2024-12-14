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
            _robots.Add(new Robot(
                new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
                new Velocity(int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value)))
            );
        }
    }

    public override ValueTask<string> Solve_1() => new($"{GetTotalSafetyFactor(100)}"); // Test: 12

    public override ValueTask<string> Solve_2() => new($"{FindEasterEgg()}");

    private int FindEasterEgg()
    {
        var result = -1;
        var x = _robots.Count * _width * _height;
        for (var seconds = 1; seconds < x; seconds++)
        {
            var robots = _robots.Select(r => Move(r, seconds)).ToList();
            if (!FormsChristmasTree(robots)) continue;
            result = seconds;
            // Print(robots, seconds);
            break;
        }

        return result;
    }

    private static bool FormsChristmasTree(List<Robot> robots)
    {
        var unique = robots.Select(r => (r.P.X, r.P.Y)).ToHashSet();
        return unique.Count == robots.Count;
    }

    private long GetTotalSafetyFactor(int seconds)
    {
        var currentRobots = _robots.Select(r => Move(r, seconds)).ToList();
        // Print(currentRobots, seconds);
        return CountQuadrants(currentRobots).Aggregate(1L, (agg, cur) => agg * cur);
    }

    private Robot Move(Robot robot, int seconds)
    {
        var newPosition = new Point(
            ((robot.P.X + robot.V.X * seconds) % _width + _width) % _width,
            ((robot.P.Y + robot.V.Y * seconds) % _height + _height) % _height);
        return robot with { P = newPosition };
    }

    private long[] CountQuadrants(List<Robot> robots)
    {
        var middleWidth = _width / 2;
        var middleHeight = _height / 2;
        var quadrants = new long[4];
        foreach (var robot in robots.Where(robot => robot.P.X != middleWidth && robot.P.Y != middleHeight))
        {
            if (robot.P.X > middleWidth)
            {
                if (robot.P.Y < middleHeight) quadrants[0] += 1;
                else quadrants[3] += 1;
            }
            else
            {
                if (robot.P.Y < middleHeight) quadrants[1] += 1;
                else quadrants[2] += 1;
            }
        }

        return quadrants;
    }

    private void Print(List<Robot> robots, int t)
    {
        var grouped = (
            from r in robots
            group r by new
            {
                r.P.X,
                r.P.Y
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

    private record Robot(Point P, Velocity V);

    private record Point(int X, int Y);

    private record Velocity(int X, int Y);
}