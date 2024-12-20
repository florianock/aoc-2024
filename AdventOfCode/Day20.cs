namespace AdventOfCode;

/// <summary>
/// --- Day 20: Race Condition ---
/// </summary>
public sealed class Day20 : BaseDay
{
    private readonly List<(int, int)> _path;
    private readonly int _timeGoal;

    public Day20()
    {
        var input = File.ReadLines(InputFilePath).ToList();
        // var input =
            // "###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############"
                // .Split("\n").ToList();
        _timeGoal = input.Count < 20 ? 12 : 100;
        var grid = new char[input.Count, input[0].Length];
        var start = (0, 0);
        var end = (1, 1);
        for (var r = 0; r < input.Count; r++)
        {
            for (var c = 0; c < input[0].Length; c++)
            {
                switch (input[r][c])
                {
                    case 'S': start = (r, c); break;
                    case 'E': end = (r, c); break;
                }

                grid[r, c] = input[r][c];
            }
        }

        _path = [start];
        var current = start;
        while (current != end)
        {
            var neighbors = GetNeighbors(current);
            var walls = neighbors.Where(n => grid[n.Item1, n.Item2] == '#').ToHashSet();
            neighbors.ExceptWith(walls);
            current = neighbors.First(n => !_path.Contains(n));
            _path.Add(current);
        }

        _path.TrimExcess();
    }

    public override ValueTask<string> Solve_1() => new($"{GetCheats(2, _timeGoal)}"); // Test: 8

    public override ValueTask<string> Solve_2() => new($"{GetCheats(20, _timeGoal)}");

    private int GetCheats(int cheatTime, int minimumTimeSaved) =>
        _path
            .SelectMany(p => CountCheats(p, _path, cheatTime))
            .Where(v => v.Value > 0)
            .GroupBy(v => v.Value)
            .Where(g => g.Key >= minimumTimeSaved)
            .Sum(g => g.Count());

    private static HashSet<KeyValuePair<Cheat, int>> CountCheats((int, int) point, List<(int, int)> path, int cheatTime)
    {
        var pIdx = path.IndexOf(point);
        var shortcuts = path[(pIdx + 1)..].Index().Where(p => ManhattanDistance(point, p.Item) <= cheatTime);
        HashSet<KeyValuePair<Cheat, int>> cheats = [];
        foreach (var shortcut in shortcuts)
        {
            var result = shortcut.Index - ManhattanDistance(point, shortcut.Item) + 1;
            cheats.Add(new KeyValuePair<Cheat, int>(new Cheat(point, shortcut.Item), result));
        }

        return cheats;
    }

    private static int ManhattanDistance((int, int) a, (int, int) b) =>
        Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);

    private static HashSet<(int, int)> GetNeighbors((int r, int c) point, Func<(int, int), bool> selector = null)
    {
        HashSet<(int, int)> ns =
            [(point.r - 1, point.c), (point.r, point.c - 1), (point.r, point.c + 1), (point.r + 1, point.c)];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }

    private record Cheat((int, int) Start, (int, int) End);
}