namespace AdventOfCode;

/// <summary>
/// --- Day 18: RAM Run ---
/// </summary>
public class Day18 : BaseDay
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<Point> _walls;
    private readonly char[][] _grid;
    private readonly int _time;

    public Day18()
    {
        var input = File.ReadLines(InputFilePath).ToList();
        // var input =
            // "5,4\n4,2\n4,5\n3,0\n2,1\n6,3\n2,4\n1,5\n0,6\n3,3\n2,6\n5,1\n1,2\n5,5\n2,5\n6,5\n1,4\n0,4\n6,4\n1,1\n6,1\n1,0\n0,5\n1,6\n2,0"
                // .Split("\n").ToList();
        _width = input.Count < 30 ? 7 : 71;
        _height = input.Count < 30 ? 7 : 71;
        _time = input.Count < 30 ? 12 : 1024;
        _walls = input.Select(line => line.Split(',')).Select(s => new Point(int.Parse(s[0]), int.Parse(s[1])))
            .ToList();
        _grid = new char[_height][];
        var wallsAtTime = _walls[.._time];
        for (var y = 0; y < _height; y++)
        {
            var row = new char[_width];
            for (var x = 0; x < _width; x++)
            {
                row[x] = wallsAtTime.Contains(new Point(x, y)) ? '#' : '.';
            }

            _grid[y] = row;
        }
    }

    public override ValueTask<string> Solve_1() => new($"{PathToEnd().Count}"); // Test: 22

    public override ValueTask<string> Solve_2() => new($"{GetCutOffPoint()}"); // Test: 6,1

    private string GetCutOffPoint()
    {
        // Console.WriteLine("Start");
        // Draw(_grid, path);
        var path = PathToEnd();
        for (var t = _time; t < _walls.Count; t++)
        {
            var wall = _walls[t];
            // Console.WriteLine($"{wall} has fallen down.");
            _grid[wall.Y][wall.X] = '#';
            if (path.Contains(wall)) path = PathToEnd();
            // Draw(_grid, path);
            if (path != null && path.Count != 0) continue;
            var correctWall = _walls[t - 1];
            return correctWall.X + "," + correctWall.Y;
        }

        return "-1,-1";
    }

    private HashSet<Point> PathToEnd()
    {
        var start = new Point(0, 0);
        var end = new Point(_width - 1, _height - 1);
        return AStar(start, end, ManhattanDistance);
    }
    
    private (int, Dictionary<Point, Point>) PathToEndBAK()
    {
        var start = new Point(0, 0);
        var end = new Point(_width - 1, _height - 1);
        var (distanceMap, path) = GetDistanceMap(start, _grid);
        var current = end;
        while (true)
        {
            if (current == start) break;
            var neighbors = GetNeighbors(current).Where(n =>
                !path.Keys.Contains(n) &&
                0 <= n.X && n.X < distanceMap.Length &&
                0 <= n.Y && n.Y < distanceMap[0].Length &&
                _grid[n.X][n.Y] != '#').ToList();
            if (neighbors.Count == 0) continue;
            current = neighbors.OrderBy(n => distanceMap[n.X][n.Y]).First();
        }

        // Draw(_grid, path);
        return (distanceMap[end.X][end.Y], path);
    }

    private HashSet<Point> AStar(Point start, Point goal, Func<Point, Point, int> h)
    {
        var openSet = new HashSet<Point>();
        var cameFrom = new Dictionary<Point, (Point, int)>();
        var gScore = new Dictionary<Point, int> { { start, 0 } };
        var fScore = new Dictionary<Point, int> { { start, h(start, goal) } };
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            var current = openSet.Select(p => (p, fScore[p])).OrderBy(p => p.Item2).First().Item1;
            openSet.Remove(current);
            if (current.Equals(goal)) return ReconstructPath(goal);
            foreach (var neighbor in GetNeighbors(current).Where(n =>
                         0 <= n.X && n.X < _grid.Length &&
                         0 <= n.Y && n.Y < _grid[0].Length &&
                         _grid[n.X][n.Y] != '#').ToList())
            {
                var tentativeGScore = gScore[current] + 1; // d(current, neighbor)
                if (gScore.TryGetValue(neighbor, out var value) && tentativeGScore >= value) continue;
                gScore[neighbor] = tentativeGScore;
                if (cameFrom.TryGetValue(neighbor, out _))
                    cameFrom[neighbor] = (current, tentativeGScore);
                else cameFrom.Add(neighbor, (current, tentativeGScore));
                if (fScore.ContainsKey(neighbor))
                    fScore[neighbor] = tentativeGScore + h(neighbor, goal);
                else fScore.Add(neighbor, tentativeGScore + h(neighbor, goal));
                openSet.Add(neighbor);
            }
        }

        return [];

        HashSet<Point> ReconstructPath(Point current)
        {
            var totalPath = new HashSet<Point>();
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current].Item1;
                totalPath.Add(current);
            }

            return totalPath;
        }
    }

    private static int ManhattanDistance(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private static (int[][], Dictionary<Point, Point>) GetDistanceMap(Point start, char[][] maze)
    {
        var distances = InitializeDistanceMap(maze.Length, maze[0].Length);
        distances[start.X][start.Y] = 0;
        var cameFrom = new Dictionary<Point, Point>();
        var todo = new Queue<(int, Point)>();
        todo.Enqueue((0, start));
        while (todo.Count > 0)
        {
            var (currentDistance, (x, y)) = todo.Dequeue();
            var neighbors = GetNeighbors(x, y);
            foreach (var n in neighbors.Where(n =>
                         0 <= n.X && n.X < maze.Length &&
                         0 <= n.Y && n.Y < maze[0].Length &&
                         maze[n.X][n.Y] != '#'))
            {
                var (neighborX, neighborY) = n;

                var neighborDistance = currentDistance + 1;

                if (neighborDistance >= distances[neighborX][neighborY]) continue;

                distances[neighborX][neighborY] = neighborDistance;
                cameFrom[n] = new Point(x, y);
                todo.Enqueue((neighborDistance, n));
            }
        }

        return (distances, cameFrom);

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

    private static void Draw(char[][] maze, HashSet<Point> path = null)
    {
        for (var x = 0; x < maze.Length; x++)
        {
            for (var y = 0; y < maze[0].Length; y++)
            {
                var s = path != null && path.Contains(new Point(x, y)) ? 'O' : maze[x][y];
                Console.Write(s);
            }

            Console.WriteLine();
        }
    }

    private static Point[] GetNeighbors(Point p) => GetNeighbors(p.X, p.Y);

    private static Point[] GetNeighbors(int x, int y) =>
    [
        new(x + 1, y), // R
        new(x, y + 1), // D
        new(x - 1, y), // L
        new(x, y - 1), // U
    ];

    private record Point(int X, int Y);
}