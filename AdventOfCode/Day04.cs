namespace AdventOfCode;

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

public sealed class Day04 : BaseDay
{
    private readonly Dictionary<(int, int), char> _input;
    private readonly Dictionary<char, List<(int, int)>> _chars;

    public Day04()
    {
        var lines = File.ReadLines(InputFilePath).ToArray();
        // var lines =
            // "MMMSXXMASM\nMSAMXMSMSA\nAMXSXMAAMM\nMSAMASMSMX\nXMASAMXAMM\nXXAMMXXAMA\nSMSMSASXSS\nSAXAMASAAA\nMAMMMXMMMM\nMXMXAXMASX"
                // .Split('\n');
        // Test1: 18
        // Test2: 9
        _chars = new Dictionary<char, List<(int, int)>>
        {
            { 'X', [] },
            { 'M', [] },
            { 'A', [] },
            { 'S', [] }
        };
        _input = new Dictionary<(int, int), char>();
        for (var r = 0; r < lines.Length; r++)
        {
            for (var c = 0; c < lines[0].Length; c++)
            {
                _input.Add((r, c), lines[r][c]);
                _chars[lines[r][c]].Add((r, c));
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{SearchForXmas()}");

    public override ValueTask<string> Solve_2() => new($"{SearchForRealXmas()}");

    private int SearchForXmas() =>
        _chars['X'].Aggregate(0, (agg, point) => agg + Count(point, "XMAS".ToCharArray(), Enum.GetValues<Direction>()));

    private int Count((int, int) point, char[] chars, Direction[] directions)
    {
        // Console.WriteLine($"{point} -> {string.Join("", chars)}");
        if (_input.GetValueOrDefault(point, '.') != chars[0]) return 0;
        if (chars[0] == 'S') return 1;
        return directions.Aggregate(0,
            (agg, direction) => agg + Count(Step(point, direction), chars[1..], [direction]));
    }

    private static (int, int) Step((int, int) point, Direction direction) => direction switch
    {
        Direction.North => (point.Item1 - 1, point.Item2),
        Direction.NorthEast => (point.Item1 - 1, point.Item2 + 1),
        Direction.East => (point.Item1, point.Item2 + 1),
        Direction.SouthEast => (point.Item1 + 1, point.Item2 + 1),
        Direction.South => (point.Item1 + 1, point.Item2),
        Direction.SouthWest => (point.Item1 + 1, point.Item2 - 1),
        Direction.West => (point.Item1, point.Item2 - 1),
        Direction.NorthWest => (point.Item1 - 1, point.Item2 - 1),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    private int SearchForRealXmas()
    {
        var result = 0;
        foreach (var (r, c) in _chars['A'])
        {
            (int, int)[] neighbors = [(r - 1, c - 1), (r - 1, c + 1), (r + 1, c - 1), (r + 1, c + 1)];
            var ps = neighbors.Select(p => _input.GetValueOrDefault(p, '.')).ToArray();
            if (ps.Count(i => i == 'M') == 2 && ps.Count(i => i == 'S') == 2 && ps[1] != ps[2])
            {
                result += 1;
            }
        }

        return result;
    }
}