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
        // var input = File.ReadLines(InputFilePath).ToList(); // 1422
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
        var cheatOptions = new HashSet<(int, int)>();
        var current = _start;
        while (current != _end)
        {
            var neighbors = GetNeighbors(current);
            var walls = neighbors.Where(n => _grid[n.Item1, n.Item2] == '#').ToHashSet();
            neighbors.ExceptWith(walls);
            current = neighbors.First(n => !path.Contains(n));
            path.Add(current);
            cheatOptions.UnionWith(walls.Where(n =>
                n is { Item1: > 0, Item2: > 0 } &&
                n.Item1 < _grid.GetLength(0) &&
                n.Item2 < _grid.GetLength(1)));
        }

        path.TrimExcess();
        cheatOptions.TrimExcess();

        return cheatOptions
            .Select(w => CountCheats(w, path, cheatTime))
            .GroupBy(v => v)
            .Where(g => g.Key >= minimumPicoSecondsSaved)
            .Sum(g => g.Count());
    }

    private static int CountCheats((int, int) wall, List<(int, int)> path, int cheatTime)
    {
        var neighbors = GetNeighbors(wall, path.Contains);
        if (neighbors.Count < 2) return 0;
        var indexes = neighbors.Select(n => path.IndexOf(n)).ToArray();
        return indexes.Max() - indexes.Min() - 2;
    }

    private static HashSet<(int, int)> GetNeighbors((int r, int c) point, Func<(int, int), bool> selector = null)
    {
        HashSet<(int, int)> ns =
            [(point.r - 1, point.c), (point.r, point.c - 1), (point.r, point.c + 1), (point.r + 1, point.c)];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }
}