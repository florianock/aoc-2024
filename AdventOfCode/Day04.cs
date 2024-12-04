namespace AdventOfCode;

/// <summary>
/// --- Day 4: Ceres Search ---
/// </summary>
public sealed class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadLines(InputFilePath).ToArray();
        // _input = "MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX".Split('\n');
        // Test1: 18
        // Test2: 9
    }

    public override ValueTask<string> Solve_1() => new($"{SearchForXmas()}");

    public override ValueTask<string> Solve_2() => new($"{SearchForCrossMas()}");

    private int SearchForXmas()
    {
        const string pattern = "XMAS";
        var result = 0;
        var maxRow = _input.Length - 1;
        var maxCol = _input[0].Length - 1;
        for (var r = 0; r <= maxRow; r++)
        {
            for (var c = 0; c <= maxCol; c++)
            {
                if (_input[r][c] != pattern[0]) continue;
                foreach (var direction in Enum.GetValues<Direction>())
                {
                    var matchedCharacters = 1;
                    foreach (var m in Enumerable.Range(1, 3))
                    {
                        var (nextRow, nextCol) = Step((r, c), direction, m);
                        if (nextRow < 0 || maxRow < nextRow ||
                            nextCol < 0 || maxCol < nextCol ||
                            _input[nextRow][nextCol] != pattern[m])
                        {
                            break;
                        }

                        matchedCharacters++;
                    }

                    if (matchedCharacters == 4) result++;
                }
            }
        }

        return result;
    }

    private int SearchForCrossMas()
    {
        var result = 0;
        var maxRow = _input.Length - 1;
        var maxCol = _input[0].Length - 1;
        for (var r = 1; r <= maxRow - 1; r++)
        {
            for (var c = 1; c <= maxCol - 1; c++)
            {
                if (_input[r][c] != 'A') continue;
                (int, int)[] diagonalCoordinates =
                [
                    Step((r, c), Direction.NorthWest), Step((r, c), Direction.NorthEast),
                    Step((r, c), Direction.SouthWest), Step((r, c), Direction.SouthEast)
                ];
                var diagonalChars = diagonalCoordinates
                    .Select(point => _input[point.Item1][point.Item2])
                    .ToArray();
                if (diagonalChars.Count(i => i == 'M') == 2
                    && diagonalChars.Count(i => i == 'S') == 2
                    && diagonalChars[1] != diagonalChars[2])
                {
                    result++;
                }
            }
        }

        return result;
    }

    private static (int, int) Step((int, int) point, Direction direction, int multiplier = 1) => direction switch
    {
        Direction.North => (point.Item1 - 1 * multiplier, point.Item2),
        Direction.NorthEast => (point.Item1 - 1 * multiplier, point.Item2 + 1 * multiplier),
        Direction.East => (point.Item1, point.Item2 + 1 * multiplier),
        Direction.SouthEast => (point.Item1 + 1 * multiplier, point.Item2 + 1 * multiplier),
        Direction.South => (point.Item1 + 1 * multiplier, point.Item2),
        Direction.SouthWest => (point.Item1 + 1 * multiplier, point.Item2 - 1 * multiplier),
        Direction.West => (point.Item1, point.Item2 - 1 * multiplier),
        Direction.NorthWest => (point.Item1 - 1 * multiplier, point.Item2 - 1 * multiplier),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}

internal enum Direction
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest
}