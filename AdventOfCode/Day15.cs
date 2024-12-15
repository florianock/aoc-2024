namespace AdventOfCode;

/// <summary>
/// --- Day 15: Warehouse Woes ---
/// </summary>
public sealed class Day15 : BaseDay
{
    private readonly Warehouse _warehouse1;
    private readonly Warehouse _warehouse2;
    private readonly string _instructions;

    public Day15()
    {
        // var input = "########\n#..O.O.#\n##@.O..#\n#...O..#\n#.#.O..#\n#...O..#\n#......#\n########\n\n<^^>>>vv<v>>v<<"
        // .Split("\n\n");
        // var input = "#######\n#...#.#\n#.....#\n#..OO@#\n#..O..#\n#.....#\n#######\n\n<vv<<^^<<^^".Split("\n\n");
        // var input =
        // "##########\n#..O..O.O#\n#......O.#\n#.OO..O.O#\n#..O@..O.#\n#O#..O...#\n#O..O..O.#\n#.OO.O.OO#\n#....O...#\n##########\n\n<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^\nvvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v\n><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<\n<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^\n^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><\n^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^\n>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^\n<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>\n^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>\nv^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^"
        // .Split("\n\n");
        var input = File.ReadAllText(InputFilePath).Split("\n\n");

        _instructions = input[1].ReplaceLineEndings("");
        var grid = input[0].Split("\n");
        _warehouse1 = new Warehouse(grid.Length, grid[0].Length);
        _warehouse2 = new Warehouse(grid.Length, grid[0].Length * 2);
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                var cur = grid[r][c];
                _warehouse1.Grid[r, c] = cur;
                switch (cur)
                {
                    case 'O':
                        _warehouse1.GpsSum += Gps(r, c);
                        _warehouse2.GpsSum += Gps(r, c * 2);
                        _warehouse2.Grid[r, c * 2] = '[';
                        _warehouse2.Grid[r, c * 2 + 1] = ']';
                        break;
                    case '#':
                        _warehouse2.Grid[r, c * 2] = '#';
                        _warehouse2.Grid[r, c * 2 + 1] = '#';
                        break;
                    case '.':
                        _warehouse2.Grid[r, c * 2] = '.';
                        _warehouse2.Grid[r, c * 2 + 1] = '.';
                        break;
                    case '@':
                        _warehouse1.Robot = (r, c);
                        _warehouse2.Robot = (r, c * 2);
                        _warehouse2.Grid[r, c * 2] = '@';
                        _warehouse2.Grid[r, c * 2 + 1] = '.';
                        break;
                }
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{FollowInstructions(_warehouse1)}"); // Test: 2028, 10092

    public override ValueTask<string> Solve_2() => new($"{FollowInstructions(_warehouse2)}"); // Test: 1751, 9021

    private long FollowInstructions(Warehouse warehouse)
    {
        // Draw(warehouse);
        foreach (var d in _instructions.Where(d => CanMove(warehouse.Robot, d, warehouse)))
        {
            warehouse = MakeMove(warehouse.Robot, d, warehouse);
            // Draw(warehouse, d);
        }

        return warehouse.GpsSum;
    }

    private static bool CanMove((int, int) pos, char d, Warehouse warehouse)
    {
        var (r, c) = pos;
        if (r < 0 || warehouse.Grid.GetLength(0) <= r || c < 0 || warehouse.Grid.GetLength(1) <= c)
            throw new IndexOutOfRangeException($"({r},{c})");

        var nextPos = d switch
        {
            '^' => (r - 1, c),
            '>' => (r, c + 1),
            'v' => (r + 1, c),
            '<' => (r, c - 1),
            _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
        };

        return warehouse.Grid[nextPos.Item1, nextPos.Item2] switch
        {
            '#' => false,
            '.' => true,
            'O' => CanMove(nextPos, d, warehouse),
            '@' => CanMove(nextPos, d, warehouse),
            '[' => d is '<' or '>'
                ? CanMove(nextPos, d, warehouse)
                : CanMove(nextPos, d, warehouse) && CanMove((nextPos.Item1, nextPos.Item2 + 1), d, warehouse),
            ']' => d is '<' or '>'
                ? CanMove(nextPos, d, warehouse)
                : CanMove(nextPos, d, warehouse) && CanMove((nextPos.Item1, nextPos.Item2 - 1), d, warehouse),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static Warehouse MakeMove((int, int) pos, char d, Warehouse warehouse)
    {
        var (r, c) = pos;
        if (r < 0 || warehouse.Grid.GetLength(0) <= r || c < 0 || warehouse.Grid.GetLength(1) <= c)
            throw new IndexOutOfRangeException($"({r},{c})");

        var nextPos = d switch
        {
            '^' => (r - 1, c),
            '>' => (r, c + 1),
            'v' => (r + 1, c),
            '<' => (r, c - 1),
            _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
        };

        switch (warehouse.Grid[nextPos.Item1, nextPos.Item2])
        {
            case '.':
                break;
            case 'O':
                warehouse = MakeMove(nextPos, d, warehouse);
                break;
            case '[':
                if (d is 'v' or '^')
                {
                    warehouse = MakeMove((nextPos.Item1, nextPos.Item2 + 1), d, warehouse);
                }

                warehouse = MakeMove(nextPos, d, warehouse);

                break;
            case ']':
                if (d is 'v' or '^')
                {
                    warehouse = MakeMove((nextPos.Item1, nextPos.Item2 - 1), d, warehouse);
                }

                warehouse = MakeMove(nextPos, d, warehouse);

                break;
            default:
                throw new ArgumentOutOfRangeException($"{nameof(nextPos)}: {nextPos}");
        }

        warehouse = DoIt(r, c, nextPos.Item1, nextPos.Item2, warehouse);

        return warehouse;

        Warehouse DoIt(int a, int b, int a2, int b2, Warehouse wh)
        {
            (wh.Grid[a2, b2], wh.Grid[a, b]) = (wh.Grid[a, b], wh.Grid[a2, b2]);
            switch (wh.Grid[a2, b2])
            {
                case 'O' or '[':
                    switch (d)
                    {
                        case '^': wh.GpsSum -= 100; break;
                        case '>': wh.GpsSum += 1; break;
                        case 'v': wh.GpsSum += 100; break;
                        case '<': wh.GpsSum -= 1; break;
                    }

                    break;
                case '@':
                    wh.Robot = (a2, b2);
                    break;
            }

            return wh;
        }
    }

    private static long Gps(int r, int c) => 100 * r + c;

    private static void Draw(Warehouse warehouse, char d = 'i')
    {
        var title = d == 'i' ? "Initial State" : d.ToString();
        Console.WriteLine($"Move: {title}; GPS Sum: {warehouse.GpsSum}");
        for (var r = 0; r < warehouse.Grid.GetLength(0); r++)
        {
            for (var c = 0; c < warehouse.Grid.GetLength(1); c++)
            {
                Console.Write(warehouse.Grid[r, c]);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    private struct Warehouse(int width, int height)
    {
        public char[,] Grid { get; } = new char[width, height];
        public long GpsSum { get; set; } = 0;
        public (int, int) Robot { get; set; }
    }
}