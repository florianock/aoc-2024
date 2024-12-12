using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 6: Guard Gallivant ---
/// </summary>
public class Day06 : BaseDay
{
    private readonly List<string> _input;
    private readonly (int, int, Grid.Direction) _guard;
    private List<(int, int)> _path;

    public Day06()
    {
        _input = File.ReadLines(InputFilePath).ToList();
        // _input =
            // "....#.....\n.........#\n..........\n..#.......\n.......#..\n..........\n.#..^.....\n........#.\n#.........\n......#..."
                // .Split("\n").ToList();
        for (var r = 0; r < _input.Count; r++)
        {
            for (var c = 0; c < _input[0].Length; c++)
            {
                _guard = _input[r][c] switch
                {
                    '^' => (r, c, Grid.Direction.North),
                    _ => _guard
                };
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{CountGuardSteps()}"); // Test: 41

    public override ValueTask<string> Solve_2() => new($"{CountPossibleGuardLoops()}"); // Test: 6

    private int CountPossibleGuardLoops() => _path.Count(pos => CountGuardSteps(pos) is null);

    private int? CountGuardSteps((int, int)? extraObstacle = null)
    {
        var guard = _guard;
        var result = new List<(int, int, Grid.Direction)>();

        while (0 <= guard.Item1 && guard.Item1 < _input.Count &&
               0 <= guard.Item2 && guard.Item2 < _input[0].Length)
        {
            result.Add(guard);
            (int, int, Grid.Direction) nextPos;
            switch (guard.Item3)
            {
                case Grid.Direction.North:
                    nextPos = (guard.Item1 - 1, guard.Item2, Grid.Direction.North);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Grid.Direction.East);
                    }

                    break;
                case Grid.Direction.East:
                    nextPos = (guard.Item1, guard.Item2 + 1, Grid.Direction.East);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Grid.Direction.South);
                    }

                    break;
                case Grid.Direction.South:
                    nextPos = (guard.Item1 + 1, guard.Item2, Grid.Direction.South);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Grid.Direction.West);
                    }

                    break;
                case Grid.Direction.West:
                    nextPos = (guard.Item1, guard.Item2 - 1, Grid.Direction.West);
                    if (0 <= nextPos.Item1 && nextPos.Item1 < _input.Count &&
                        0 <= nextPos.Item2 && nextPos.Item2 < _input[0].Length &&
                        IsObstacle(nextPos.Item1, nextPos.Item2, extraObstacle))
                    {
                        nextPos = (guard.Item1, guard.Item2, Grid.Direction.North);
                    }

                    break;
                case Grid.Direction.NorthEast:
                case Grid.Direction.SouthEast:
                case Grid.Direction.SouthWest:
                case Grid.Direction.NorthWest:
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