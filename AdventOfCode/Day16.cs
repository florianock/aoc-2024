using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 16: Reindeer Maze ---
/// </summary>
public sealed class Day16 : BaseDay
{
    private readonly List<string> _input;
    private readonly DirPoint _start;
    private readonly (int, int) _end;
    private int[][] _distanceMap;

    public Day16()
    {
        // _input =
            // "###############\n#.......#....E#\n#.#.###.#.###.#\n#.....#.#...#.#\n#.###.#####.#.#\n#.#.#.......#.#\n#.#.#####.###.#\n#...........#.#\n###.#.#####.#.#\n#...#.....#.#.#\n#.#.#.###.#.#.#\n#.....#...#.#.#\n#.###.#.#.#.#.#\n#S..#.....#...#\n###############"
                // .Split("\n").ToList();
        // _input =
        // "#################\n#...#...#...#..E#\n#.#.#.#.#.#.#.#.#\n#.#.#.#...#...#.#\n#.#.#.#.###.#.#.#\n#...#.#.#.....#.#\n#.#.#.#.#.#####.#\n#.#...#.#.#.....#\n#.#.#####.#.###.#\n#.#.#.......#...#\n#.#.###.#####.###\n#.#.#...#.....#.#\n#.#.#.#####.###.#\n#.#.#.........#.#\n#.#.#.#########.#\n#S#.............#\n#################"
        // .Split("\n").ToList();
        _input = File.ReadLines(InputFilePath).ToList();

        for (var i = 0; i < _input.Count; i++)
        {
            for (var j = 0; j < _input[0].Length; j++)
            {
                switch (_input[i][j])
                {
                    case 'S': _start = new DirPoint(i, j, Grid.Direction.East); break;
                    case 'E': _end = (i, j); break;
                }
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{GetBestPathScore()}"); // Test: 7036, 11048

    public override ValueTask<string> Solve_2() => new($"{GetTilesOnBestPaths()}"); // Test: 45, 64

    private int GetBestPathScore()
    {
        _distanceMap ??= GetMinimalDistanceMap(_start, _input);
        return _distanceMap[_end.Item1][_end.Item2];
    }

    private int GetTilesOnBestPaths()
    {
        var bestScore = GetBestPathScore();
        HashSet<(int, int)> path = [];
        var backwardDistanceMap =
            GetMinimalDistanceMap(new DirPoint(_end.Item1, _end.Item2, Grid.Direction.South), _input);
        var bestBackwardScore = backwardDistanceMap[_start.Row][_start.Column];

        for (var i = 0; i < _input.Count; i++)
        {
            for (var j = 0; j < _input[0].Length; j++)
            {
                if (_input[i][j] == '#') continue;
                if (_distanceMap[i][j] + backwardDistanceMap[i][j] == bestScore ||
                    _distanceMap[i][j] + backwardDistanceMap[i][j] <= bestBackwardScore)
                    path.Add((i, j));
            }
        }

        // Draw(_input, path);

        return path.Count;
    }

    private static int[][] GetMinimalDistanceMap(DirPoint start, List<string> maze)
    {
        var distances = InitializeDistanceMap(maze.Count, maze[0].Length);
        distances[start.Row][start.Column] = 0;
        var todo = new Queue<(int, DirPoint)>();
        todo.Enqueue((0, start));
        while (todo.Count > 0)
        {
            var (currentDistance, (r, c, direction)) = todo.Dequeue();
            var neighbors = GetNeighbors(r, c);
            foreach (var n in neighbors.Where(n => maze[n.Row][n.Column] != '#'))
            {
                var (neighborRow, neighborCol, neighborDirection) = n;

                var neighborDistance = currentDistance;
                if (direction == neighborDirection) neighborDistance += 1;
                else if (direction == Grid.Direction.East && neighborDirection == Grid.Direction.West ||
                         direction == Grid.Direction.West && neighborDirection == Grid.Direction.East ||
                         direction == Grid.Direction.North && neighborDirection == Grid.Direction.South ||
                         direction == Grid.Direction.South && neighborDirection == Grid.Direction.North)
                    neighborDistance += 2001;
                else neighborDistance += 1001;

                if (neighborDistance >= distances[neighborRow][neighborCol]) continue;

                distances[neighborRow][neighborCol] = neighborDistance;
                todo.Enqueue((neighborDistance, n));
            }
        }

        return distances;

        int[][] InitializeDistanceMap(int numRows, int numColumns)
        {
            var map = new int[numRows][];
            for (var row = 0; row < numRows; row++) map[row] = new int[numColumns];
            foreach (var row in map)
                for (var col = 0; col < row.Length; col++)
                    row[col] = int.MaxValue;
            return map;
        }
    }

    private static DirPoint[] GetNeighbors(int r, int c) =>
    [
        new(r - 1, c, Grid.Direction.North),
        new(r, c - 1, Grid.Direction.West), new(r, c + 1, Grid.Direction.East),
        new(r + 1, c, Grid.Direction.South)
    ];

    private static void Draw(List<string> maze, HashSet<(int, int)> path = null)
    {
        for (var r = 0; r < maze.Count; r++)
        {
            for (var c = 0; c < maze[0].Length; c++)
            {
                var s = path != null && path.Contains((r, c)) ? 'O' : maze[r][c];
                Console.Write(s);
            }

            Console.WriteLine();
        }
    }

    private record DirPoint(int Row, int Column, Grid.Direction Direction);
}