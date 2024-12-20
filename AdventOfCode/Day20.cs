namespace AdventOfCode;

/// <summary>
/// --- Day 20: Race Condition ---
/// </summary>
public sealed class Day20 : BaseDay
{
    private readonly (int Steps, (int, int) Point)[] _path;
    private readonly int _timeGoal;

    public Day20()
    {
        var input = File.ReadLines(InputFilePath).ToList();
        // var input =
        // "###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############"
        // .Split("\n").ToList();
        _timeGoal = input.Count < 20 ? 12 : 100;
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
            }
        }

        List<(int, int)> path = [start];
        var current = start;
        while (current != end)
        {
            current = GetNeighbors(current, n => input[n.Item1][n.Item2] != '#').First(n => !path.Contains(n));
            path.Add(current);
        }

        _path = path.Index().ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{GetCheats(2, _timeGoal)}"); // Test: 8

    public override ValueTask<string> Solve_2() => new($"{GetCheats(20, _timeGoal)}");

    private int GetCheats(int cheatTime, int minimumTimeSaved)
    {
        var q = new Queue<(int, (int, int))>(_path);
        var cheatsCounter = new int[q.Count];
        while (q.Count > 0)
        {
            var next = q.Dequeue();
            var cheats = CountCheats(next, q, cheatTime);
            foreach (var i in cheats)
                cheatsCounter[i]++;
        }

        return cheatsCounter[minimumTimeSaved..].Sum();
    }

    private static IEnumerable<int> CountCheats((int Steps, (int, int) Coords) point,
        Queue<(int Steps, (int, int) Coords)> path,
        int cheatTime)
    {
        if (path.Count == 0) return [];
        return path
            .Where(p => Distance(point.Coords, p.Coords) <= cheatTime)
            .Select(shortcut => shortcut.Steps - point.Steps - Distance(point.Coords, shortcut.Coords));
    }

    private static int Distance((int, int) a, (int, int) b) =>
        Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);

    private static HashSet<(int, int)> GetNeighbors((int r, int c) point, Func<(int, int), bool> selector = null)
    {
        HashSet<(int, int)> ns =
            [(point.r - 1, point.c), (point.r, point.c - 1), (point.r, point.c + 1), (point.r + 1, point.c)];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }
}