﻿namespace AdventOfCode;

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
        _walls = input
            .Select(line => line.Split(','))
            .Select(arr => new Point(int.Parse(arr[0]), int.Parse(arr[1])))
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
        var path = PathToEnd();
        for (var t = _time; t < _walls.Count; t++)
        {
            var wall = _walls[t];
            _grid[wall.Y][wall.X] = '#';
            if (path.Contains(wall)) path = PathToEnd();
            // Draw(_grid, path);
            if (path.Count > 0) continue;
            return wall.X + "," + wall.Y;
        }

        return "-1,-1";
    }

    private HashSet<Point> PathToEnd()
    {
        var start = new Point(0, 0);
        var end = new Point(_width - 1, _height - 1);
        return AStar(start, end, ManhattanDistance);
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
                         0 <= n.Y && n.Y < _grid.Length &&
                         0 <= n.X && n.X < _grid[0].Length &&
                         _grid[n.Y][n.X] != '#').ToList())
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

    private static void Draw(char[][] maze, HashSet<Point> path = null)
    {
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

    private static Point[] GetNeighbors(Point p) => GetNeighbors(p.X, p.Y);

    private static Point[] GetNeighbors(int x, int y) => [new(x + 1, y), new(x, y + 1), new(x - 1, y), new(x, y - 1)];

    private record Point(int X, int Y);
}