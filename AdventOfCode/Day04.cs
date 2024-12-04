namespace AdventOfCode;

/// <summary>
/// --- Day 4: Ceres Search ---
/// </summary>
public sealed class Day04 : BaseDay
{
    private readonly Dictionary<(int, int), char> _input;

    public Day04()
    {
        var lines = File.ReadLines(InputFilePath).ToArray();
        // _lines = "MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX".Split('\n');
        // Test1: 18
        // Test2: 9
        _input = new Dictionary<(int, int), char>();
        for (var r = 0; r < lines.Length; r++)
        {
            for (var c = 0; c < lines[0].Length; c++)
            {
                _input.Add((r, c), lines[r][c]);
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{SearchForXmas()}");

    public override ValueTask<string> Solve_2() => new($"{SearchForRealXmas()}");

    private int SearchForXmas()
    {
        const string pattern = "XMAS";
        var result = 0;
        foreach (var point in _input.Where(kvp => kvp.Value == pattern[0]))
        {
            foreach (var direction in Enum.GetValues<Direction>())
            {
                var matchedCharacters = 1;
                foreach (var m in Enumerable.Range(1, 3))
                {
                    if (_input.GetValueOrDefault(Step(point.Key, direction, m), '.') != pattern[m]) break;
                    matchedCharacters++;
                }

                if (matchedCharacters == 4) result++;
            }
        }

        return result;
    }

    private int SearchForRealXmas()
    {
        var result = 0;
        foreach (var point in _input)
        {
            if (point.Value != 'A') continue;
            var (r, c) = point.Key;
            (int, int)[] diagonalCoordinates =
            [
                Step((r, c), Direction.NorthWest), Step((r, c), Direction.NorthEast),
                Step((r, c), Direction.SouthWest), Step((r, c), Direction.SouthEast)
            ];
            var diagonalChars = diagonalCoordinates.Select(p => _input.GetValueOrDefault(p, '.')).ToArray();
            if (diagonalChars.Count(i => i == 'M') == 2 && diagonalChars.Count(i => i == 'S') == 2 &&
                diagonalChars[1] != diagonalChars[2])
            {
                result += 1;
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