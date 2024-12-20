namespace AdventOfCode;

/// <summary>
/// --- Day 20: Race Condition ---
/// </summary>
public sealed class Day20 : BaseDay
{
    private readonly char[,] _grid;
    private readonly (int, int) _start;
    private readonly (int, int) _end;
    private readonly int _goalSaved;

    public Day20()
    {
        // var input = File.ReadLines(InputFilePath).ToList();
        var input =
            "###############\n#...#...#.....#\n#.#.#.#.#.###.#\n#S#...#.#.#...#\n#######.#.#.###\n#######.#.#...#\n#######.#.###.#\n###..E#...#...#\n###.#######.###\n#...###...#...#\n#.#####.#.###.#\n#.#...#.#.#...#\n#.#.#.#.#.#.###\n#...#...#...###\n###############"
                .Split("\n").ToList();
        _goalSaved = input.Count < 20 ? 12 : 100;
        _grid = new char[input.Count, input[0].Length];
        for (var r = 0; r < input.Count; r++)
        {
            for (var c = 0; c < input[0].Length; c++)
            {
                switch (input[r][c])
                {
                    case 'S': _start = (r, c); break;
                    case 'E': _end = (r, c); break;
                }

                _grid[r, c] = input[r][c];
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{Cheat(2, _goalSaved)}"); // Test: 8

    public override ValueTask<string> Solve_2() => new($"{Cheat(20, _goalSaved)}"); // Test: 

    private int Cheat(int cheatTime, int minimumPicoSecondsSaved)
    {
        List<(int, int)> path = [_start];
        var current = _start;
        while (current != _end)
        {
            var neighbors = GetNeighbors(current);
            var walls = neighbors.Where(n => _grid[n.Item1, n.Item2] == '#').ToHashSet();
            neighbors.ExceptWith(walls);
            current = neighbors.First(n => !path.Contains(n));
            path.Add(current);
        }

        path.TrimExcess();

        var grouped = path
            .Select(p => CountCheats(p, path, cheatTime))
            .GroupBy(v => v.Value);

        return grouped
            .Where(g => g.Key >= minimumPicoSecondsSaved)
            .Sum(g => g.Count());
    }

    private static KeyValuePair<(int, int), int> CountCheats((int, int) point, List<(int, int)> path, int cheatTime)
    {
        var shortcuts = path.Index().Where(p => ManhattanDistance(point, p.Item) <= cheatTime);
        var furthest = shortcuts.MaxBy(p => p.Index);
        var result = furthest.Index - path.IndexOf(point) - ManhattanDistance(point, furthest.Item);
        Console.WriteLine($"{point};{result}");
        return new KeyValuePair<(int, int), int>(point, result);
    }

    private static int ManhattanDistance((int, int) a, (int, int) b) =>
        Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);

    private static HashSet<(int, int)> GetNeighbors((int r, int c) point, Func<(int, int), bool> selector = null)
    {
        HashSet<(int, int)> ns =
            [(point.r - 1, point.c), (point.r, point.c - 1), (point.r, point.c + 1), (point.r + 1, point.c)];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }
}