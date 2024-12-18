namespace AdventOfCode;

/// <summary>
/// --- Day 18: RAM Run ---
/// </summary>
public sealed class Day18 : BaseDay
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<Point> _walls;
    private readonly char[][] _grid;

    public Day18()
    {
        var input = File.ReadLines(InputFilePath).ToList();
        // var input =
        // "5,4\n4,2\n4,5\n3,0\n2,1\n6,3\n2,4\n1,5\n0,6\n3,3\n2,6\n5,1\n1,2\n5,5\n2,5\n6,5\n1,4\n0,4\n6,4\n1,1\n6,1\n1,0\n0,5\n1,6\n2,0"
        // .Split("\n").ToList();
        _width = input.Count < 30 ? 7 : 71;
        _height = input.Count < 30 ? 7 : 71;
        var time = input.Count < 30 ? 12 : 1024;
        _walls = input
            .Select(line => line.Split(','))
            .Select(arr => new Point(int.Parse(arr[0]), int.Parse(arr[1])))
            .ToList();
        _grid = new char[_height][];
        var wallsAtTime = _walls[..time];
        for (var y = 0; y < _height; y++)
        {
            var row = new char[_width];
            for (var x = 0; x < _width; x++)
            {
                row[x] = '.';
            }

            _grid[y] = row;
        }

        foreach (var wall in wallsAtTime) _grid[wall.Y][wall.X] = '#';
    }

    public override ValueTask<string> Solve_1() =>
        new(
            $"{ShortestPath(new Point(0, 0), new Point(_width - 1, _height - 1), _grid, ChebyshevDistance).Count}"); // Test: 22

    public override ValueTask<string> Solve_2() => new($"{GetCutOffPoint()}"); // Test: 6,1

    private string GetCutOffPoint()
    {
        const int wall = 100;
        const int flooded = 10;
        const int empty = 0;

        var map = new int[_height][];
        for (var y = 0; y < _height; y++)
        {
            map[y] = new int[_width];
            for (var x = 0; x < _width; x++)
            {
                map[y][x] = empty;
            }
        }

        foreach (var w in _walls) map[w.Y][w.X] = wall;

        var start = new Point(0, 0);
        var end = new Point(_width - 1, _height - 1);

        FloodFill(start);

        foreach (var w in Enumerable.Reverse(_walls))
        {
            if (GetNeighbors(w).Any(n => map[n.Y][n.X] == flooded))
                FloodFill(w);
            else
                map[w.Y][w.X] = empty;

            if (map[end.Y][end.X] == flooded) return $"{w.X},{w.Y}";
        }

        return "-1,-1";

        void FloodFill(Point p)
        {
            map[p.Y][p.X] = flooded;
            var neighbors = GetNeighbors(p).Where(n => map[n.Y][n.X] < flooded).ToList();
            foreach (var n in neighbors) FloodFill(n);
        }
    }

    private HashSet<Point> ShortestPath(Point start, Point goal, char[][] grid, Func<Point, Point, int> h = null)
    {
        var frontier = new PriorityQueue<Point, int>();
        var cameFrom = new Dictionary<Point, (Point, int)>();
        var gScore = new Dictionary<Point, int> { { start, 0 } };
        var fScore = new Dictionary<Point, int> { { start, h?.Invoke(start, goal) ?? 0 } };
        frontier.Enqueue(start, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current.Equals(goal)) return ReconstructPath(goal);
            foreach (var neighbor in GetNeighbors(current).Where(n => grid[n.Y][n.X] != '#').ToList())
            {
                var tentativeGScore = gScore[current] + 1; // d(current, neighbor)
                if (gScore.TryGetValue(neighbor, out var value) && tentativeGScore >= value) continue;
                gScore[neighbor] = tentativeGScore;
                cameFrom[neighbor] = (current, tentativeGScore);
                if (fScore.ContainsKey(neighbor))
                    fScore[neighbor] = tentativeGScore + (h?.Invoke(neighbor, goal) ?? 0);
                else fScore.Add(neighbor, tentativeGScore + (h?.Invoke(neighbor, goal) ?? 0));
                // actually should skip if frontier already contains neighbor
                frontier.Enqueue(neighbor, fScore[neighbor]);
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

    private static int ChebyshevDistance(Point a, Point b) => Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));

    private static void Draw(char[][] maze, HashSet<Point> path = null)
    {
        Console.WriteLine();
        for (var y = 0; y < maze.Length; y++)
        {
            for (var x = 0; x < maze[0].Length; x++)
            {
                var s = path != null && path.Contains(new Point(x, y)) ? 'O' : maze[y][x];
                Console.Write(s);
            }

            Console.WriteLine();
        }
    }

    private Point[] GetNeighbors(Point p)
    {
        var (x, y) = p;
        Point[] neighbors = [new(x + 1, y), new(x, y + 1), new(x - 1, y), new(x, y - 1)];
        return neighbors.Where(n => 0 <= n.X && n.X < _width && 0 <= n.Y && n.Y < _height).ToArray();
    }

    private record Point(int X, int Y);
}