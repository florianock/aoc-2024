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
        var input = File.ReadAllText(InputFilePath);
        // var input = "###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############";
        var width = input.IndexOf('\n');
        _timeGoal = width < 20 ? 12 : 100;
        var s = input.IndexOf('S');
        var start = (s / (width + 1), s % (width + 1));

        List<(int, int)> path = [start];
        var current = start;
        while (true)
        {
            var neighbors = GetNeighbors(current, n => input[n.Item1 * (width + 1) + n.Item2] != '#');
            if (current != start && neighbors.Count == 1) break;
            current = neighbors.First(n => !path.Contains(n));
            path.Add(current);
        }

        _path = path.Index().ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{GetCheats(2, _timeGoal)}"); // Test: 8

    public override ValueTask<string> Solve_2() => new($"{GetCheats(20, _timeGoal)}");

    private int GetCheats(int cheatTime, int timeGoal)
    {
        var q = new Queue<(int, (int, int))>(_path);
        var cheatsCounter = 0;
        while (q.Count > 0)
            cheatsCounter += CountCheats(q.Dequeue(), q, cheatTime, timeGoal);

        return cheatsCounter;
    }

    private static int CountCheats((int Steps, (int, int) Coords) point,
        Queue<(int Steps, (int, int) Coords)> path,
        int cheatTime,
        int timeGoal)
    {
        if (path.Count == 0) return 0;
        return path
            .Where(p =>
            {
                var d = Math.Abs(point.Coords.Item1 - p.Coords.Item1) + Math.Abs(point.Coords.Item2 - p.Coords.Item2);
                return d <= cheatTime && p.Steps - point.Steps - d >= timeGoal;
            })
            .Count();
    }

    private static HashSet<(int, int)> GetNeighbors((int r, int c) point, Func<(int, int), bool> selector = null)
    {
        HashSet<(int, int)> ns =
            [(point.r - 1, point.c), (point.r, point.c - 1), (point.r, point.c + 1), (point.r + 1, point.c)];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }
}