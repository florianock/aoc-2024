namespace AdventOfCode;

/// <summary>
/// --- Day 6: Guard Gallivant ---
/// </summary>
public class Day06 : BaseDay
{
    private readonly List<string> _input;
    private readonly (int, int, Direction) _guard;
    private readonly List<(int, int)> _obstacles;
    private List<(int, int)> _path;

    public Day06()
    {
        _input = File.ReadLines(InputFilePath).ToList();
        // _input =
            // "....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#..."
                // .Split("\n").ToList();
        _obstacles = [];
        for (var r = 0; r < _input.Count; r++)
        {
            for (var c = 0; c < _input[0].Length; c++)
            {
                switch (_input[r][c])
                {
                    case '#':
                        _obstacles.Add((r, c));
                        break;
                    case '^':
                        _guard = (r, c, Direction.North);
                        break;
                }
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{CountGuardSteps()}"); // Test: 41

    public override ValueTask<string> Solve_2() => new($"{CountPossibleGuardLoops()}"); // Test: 6

    private int CountPossibleGuardLoops() => _path.Count(pos => CountGuardSteps(pos) is null);

    private int? CountGuardSteps((int, int)? extraObstacle = null)
    {
        var guard = _guard;
        var result = new List<(int, int, Direction)>();

        while (0 <= guard.Item1 && guard.Item1 < _input.Count &&
               0 <= guard.Item2 && guard.Item2 < _input[0].Length)
        {
            result.Add(guard);
            (int, int, Direction) nextPos;
            switch (guard.Item3)
            {
                case Direction.North:
                    nextPos = (guard.Item1 - 1, guard.Item2, Direction.North);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Direction.East);
                    }

                    break;
                case Direction.East:
                    nextPos = (guard.Item1, guard.Item2 + 1, Direction.East);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Direction.South);
                    }

                    break;
                case Direction.South:
                    nextPos = (guard.Item1 + 1, guard.Item2, Direction.South);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Direction.West);
                    }

                    break;
                case Direction.West:
                    nextPos = (guard.Item1, guard.Item2 - 1, Direction.West);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Direction.North);
                    }

                    break;
                case Direction.NorthEast:
                case Direction.SouthEast:
                case Direction.SouthWest:
                case Direction.NorthWest:
                default:
                    throw new InvalidOperationException(nameof(guard.Item3));
            }

            if (result.Contains(nextPos)) return null;
            guard = nextPos;
        }

        _path = result.Select(pos => (pos.Item1, pos.Item2)).Distinct().ToList();
        return _path.Count;
    }

    private bool IsObstacle(int r, int c, (int, int)? extra = null) =>
        extra is not null && (r, c) == extra || _input[r][c] == '#';
}