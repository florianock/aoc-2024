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
    }

    public override ValueTask<string> Solve_1() => new($"{SearchForXmas()}"); // Test: 18

    public override ValueTask<string> Solve_2() => new($"{SearchForX_Mas()}"); // Test: 9

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
                if (_input[r][c] != 'X') continue;
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

    private int SearchForX_Mas()
    {
        var result = 0;
        var maxRow = _input.Length - 1;
        var maxCol = _input[0].Length - 1;
        for (var r = 1; r <= maxRow - 1; r++)
        {
            for (var c = 1; c <= maxCol - 1; c++)
            {
                if (_input[r][c] != 'A') continue;
                var diagonalChars = (((int, int)[]) [(r - 1, c - 1), (r - 1, c + 1), (r + 1, c - 1), (r + 1, c + 1)])
                    .Select(point => _input[point.Item1][point.Item2]).ToArray();
                if (diagonalChars.Count(ch => ch == 'M') == 2 &&
                    diagonalChars.Count(ch => ch == 'S') == 2 &&
                    diagonalChars[1] != diagonalChars[2])
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