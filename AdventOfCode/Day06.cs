using System.Runtime.CompilerServices;

namespace AdventOfCode;

/// <summary>
/// --- Day 6: Guard Gallivant ---
/// </summary>
public class Day06 : BaseDay
{
    private record Obstacle((int, int) Position, bool[] VisitedFromDirection, (int, int)?[] Neighbors);

    private readonly List<string> _input;
    private List<Obstacle> _obstacles;
    private readonly (int, int, Direction) _guard;

    public Day06()
    {
        _input = File.ReadLines(InputFilePath).ToList();
        // _input =
            // "....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#..."
                // .Split("\n").ToList();
        List<(int, int)> obstacles = [];
        for (var r = 0; r < _input.Count; r++)
        {
            for (var c = 0; c < _input[0].Length; c++)
            {
                switch (_input[r][c])
                {
                    case '#':
                        obstacles.Add((r, c));
                        break;
                    case '^':
                        _guard = (r, c, Direction.North);
                        break;
                }
            }
        }

        _obstacles = [];
        foreach (var obstacle in obstacles)
        {
            var guardPos = (obstacle.Item1 + 1, obstacle.Item2, Direction.North);
            var northNeighbors = obstacles.Where(o => o.Item1 == guardPos.Item1 && o.Item2 > guardPos.Item2).ToList();
            (int, int)? northNeighbor = northNeighbors.Count > 0 ? northNeighbors.MinBy(o => o.Item2) : null;

            guardPos = (obstacle.Item1, obstacle.Item2 - 1, Direction.East);
            var eastNeighbors = obstacles.Where(o => o.Item1 > guardPos.Item1 && o.Item2 == guardPos.Item2).ToList();
            (int, int)? eastNeighbor = eastNeighbors.Count > 0 ? eastNeighbors.MinBy(o => o.Item1) : null;

            guardPos = (obstacle.Item1 - 1, obstacle.Item2, Direction.South);
            var southNeighbors = obstacles.Where(o => o.Item1 == guardPos.Item1 && o.Item2 < guardPos.Item2).ToList();
            (int, int)? southNeighbor = southNeighbors.Count > 0 ? southNeighbors.MinBy(o => o.Item2) : null;

            guardPos = (obstacle.Item1, obstacle.Item2 + 1, Direction.West);
            var westNeighbors = obstacles.Where(o => o.Item1 < guardPos.Item1 && o.Item2 == guardPos.Item2).ToList();
            (int, int)? westNeighbor = westNeighbors.Count > 0 ? westNeighbors.MinBy(o => o.Item1) : null;

            _obstacles.Add(new Obstacle(obstacle, [false, false, false, false],
                [northNeighbor, eastNeighbor, southNeighbor, westNeighbor]));
        }
    }

    public override ValueTask<string> Solve_1() => new($"{CountGuardSteps()}"); // Test: 41

    public override ValueTask<string> Solve_2() => new($"{SecondPart()}"); // Test: 

    private int CountGuardSteps()
    {
        var guard = _guard;
        var firstObstacle = _obstacles.Select(o => o.Position)
            .Where(ob => ob.Item2 == guard.Item2 && ob.Item1 < guard.Item1)
            .MinBy(ob => Math.Abs(ob.Item1 - guard.Item1));

        // start walking
        var current = _obstacles.First(o => o.Position == firstObstacle);
        var outsideGrid = false;
        List<(int, int, Direction)> path = [guard];
        guard = guard with { Item1 = firstObstacle.Item1 + 1 };
        while (!outsideGrid)
        {
            path.Add(guard);
            switch (guard.Item3)
            {
                case Direction.North:
                    // if (current.VisitedFromDirection[0]) return 0;
                    // else
                    // {
                    current.VisitedFromDirection[0] = true;
                    if (current.Neighbors[0] is not null)
                    {
                        current = _obstacles.First(o => o.Position == current.Neighbors[0]);
                        guard = (current.Position.Item1, current.Position.Item2 - 1, Direction.East);
                    }
                    else
                    {
                        path.Add((current.Position.Item1, _input[0].Length - 1, Direction.East));
                        outsideGrid = true;
                    }

                    // }
                    break;
                case Direction.East:
                    // if (!_obstacles[current].Item1[1]) _obstacles[current].Item1[1] = true;
                    current.VisitedFromDirection[1] = true;
                    if (current.Neighbors[1] is not null)
                    {
                        current = _obstacles.First(o => o.Position == current.Neighbors[1]);
                        guard = (current.Position.Item1 - 1, current.Position.Item2, Direction.South);
                    }
                    else
                    {
                        path.Add((_input.Count - 1, current.Position.Item2, Direction.South));
                        outsideGrid = true;
                    }

                    break;
                case Direction.South:
                    // if (!_obstacles[current].Item1[2]) _obstacles[current].Item1[2] = true;
                    current.VisitedFromDirection[2] = true;
                    if (current.Neighbors[2] is not null)
                    {
                        current = _obstacles.First(o => o.Position == current.Neighbors[2]);
                        guard = (current.Position.Item1, current.Position.Item2 + 1, Direction.West);
                    }
                    else
                    {
                        path.Add((current.Position.Item1, 0, Direction.West));
                        outsideGrid = true;
                    }

                    break;
                case Direction.West:
                    // if (!_obstacles[current].Item1[3]) _obstacles[current].Item1[1] = true;
                    current.VisitedFromDirection[3] = true;
                    if (current.Neighbors[3] is not null)
                    {
                        current = _obstacles.First(o => o.Position == current.Neighbors[3]);
                        guard = (current.Position.Item1 + 1, current.Position.Item2, Direction.North);
                    }
                    else
                    {
                        path.Add((0, current.Position.Item2, Direction.North));
                        outsideGrid = true;
                    }

                    break;
                default: throw new InvalidOperationException();
            }
        }

        // direction = Direction.East;
        // current = _obstacles[current].Item2[1];

        //////////////////

        return Expand(path).Count;
    }

    private int CountGuardSteps2()
    {
        var guard = (_guard.Item1, _guard.Item2);
        var direction = _guard.Item3;
        var result = new HashSet<(int, int)>();
        
        while (0 <= guard.Item1 && guard.Item1 < _input.Count &&
               0 <= guard.Item2 && guard.Item2 < _input[0].Length)
        {
            result.Add(guard);
            var nextPos = guard;
            switch (direction)
            {
                case Direction.North:
                    nextPos = (guard.Item1 - 1, guard.Item2);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        _input[nextPos.Item1][nextPos.Item2] == '#')
                    {
                        direction = Direction.East;
                        nextPos = (guard.Item1, guard.Item2 + 1);
                    }
        
                    break;
                case Direction.East:
                    nextPos = (guard.Item1, guard.Item2 + 1);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        _input[nextPos.Item1][nextPos.Item2] == '#')
                    {
                        direction = Direction.South;
                        nextPos = (guard.Item1 + 1, guard.Item2);
                    }
        
                    break;
                case Direction.South:
                    nextPos = (guard.Item1 + 1, guard.Item2);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        _input[nextPos.Item1][nextPos.Item2] == '#')
                    {
                        direction = Direction.West;
                        nextPos = (guard.Item1, guard.Item2 - 1);
                    }
        
                    break;
                case Direction.West:
                    nextPos = (guard.Item1, guard.Item2 - 1);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        _input[nextPos.Item1][nextPos.Item2] == '#')
                    {
                        direction = Direction.North;
                        nextPos = (guard.Item1 - 1, guard.Item2);
                    }
        
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        
            guard = nextPos;
        }
        
        return result.Count;
    }
    
    private HashSet<(int, int)> Expand(List<(int, int, Direction)> path)
    {
        HashSet<(int, int)> result = [];
        for (var i = 0; i < path.Count - 1; i++)
        {
            var currentPosition = path[i];
            result.Add((currentPosition.Item1, currentPosition.Item2));
            var nextPosition = path[i + 1];
            var r = nextPosition.Item1 - currentPosition.Item1;
            switch (r)
            {
                case < 0:
                {
                    for (var row = -1; row > r; row--)
                        result.Add((currentPosition.Item1 + row, currentPosition.Item2));
                    break;
                }
                case > 0:
                {
                    for (var row = 1; row < r; row++)
                        result.Add((currentPosition.Item1 + row, currentPosition.Item2));
                    break;
                }
            }

            var c = nextPosition.Item2 - currentPosition.Item2;
            switch (c)
            {
                case < 0:
                {
                    for (var col = -1; col > c; col--)
                        result.Add((currentPosition.Item1, currentPosition.Item2 + col));
                    break;
                }
                case > 0:
                {
                    for (var col = 1; col < c; col++)
                        result.Add((currentPosition.Item1, currentPosition.Item2 + col));
                    break;
                }
            }
        }

        result.Add((path[^1].Item1, path[^1].Item2));
        return result;
    }

    private int SecondPart()
    {
        return 0;
    }
}