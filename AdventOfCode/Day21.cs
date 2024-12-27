using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 21: Keypad Conundrum ---
/// </summary>
public sealed class Day21 : BaseDay
{
    private readonly IEnumerable<string> _input;
    private readonly Dictionary<char[][], Dictionary<(char, char), string[]>> _moves;
    private readonly Dictionary<(string, int), long> _cache;
    private readonly Dictionary<(char, char), int> _directionalPresses;

    private readonly char[][] _numericKeypad =
    [
        ['7', '8', '9'],
        ['4', '5', '6'],
        ['1', '2', '3'],
        ['X', '0', 'A']
    ];

    private readonly char[][] _directionalKeypad =
    [
        ['X', '^', 'A'],
        ['<', 'v', '>']
    ];

    public Day21()
    {
        _input = File.ReadLines(InputFilePath);
        // _input = "029A\n980A\n179A\n456A\n379A".Split("\n");
        Dictionary<char[][], Dictionary<char, (int, int)>> positions = [];
        _moves = [];
        List<char[][]> keypads = [_numericKeypad, _directionalKeypad];
        foreach (var keypad in keypads)
        {
            positions[keypad] = [];
            for (var r = 0; r < keypad.Length; r++)
            {
                for (var c = 0; c < keypad[r].Length; c++)
                {
                    if (keypad[r][c] != 'X') positions[keypad].Add(keypad[r][c], (r, c));
                }
            }

            _moves.Add(keypad, PossibleMovesOnKeypad(keypad, positions[keypad]));
        }

        _cache = [];
        _directionalPresses = _moves[_directionalKeypad].ToDictionary(kvp => kvp.Key, kvp => kvp.Value[0].Length);
    }

    public override ValueTask<string> Solve_1() => new($"{CalculateMinimumButtonPresses(2)}"); // Test: 126384

    public override ValueTask<string> Solve_2() => new($"{CalculateMinimumButtonPresses(25)}"); // Test: 154115708116294

    private long CountPressesOnDirectionalKeypad(string sequence, int depth = 2)
    {
        if (_cache.TryGetValue((sequence, depth), out var stepsCount)) return stepsCount;

        if (depth == 1)
        {
            var sum = 0;
            for (var i = -1; i < sequence.Length - 1; i++)
            {
                var x = i > -1 ? sequence[i] : 'A';
                sum += _directionalPresses[(x, sequence[i + 1])];
            }

            return sum;
        }

        var length = 0L;
        for (var i = -1; i < sequence.Length - 1; i++)
        {
            var x = i > -1 ? sequence[i] : 'A';
            length += _moves[_directionalKeypad][(x, sequence[i + 1])]
                .Select(subSequence => CountPressesOnDirectionalKeypad(subSequence, depth - 1)).Min();
        }

        _cache[(sequence, depth)] = length;
        return length;
    }

    private long CalculateMinimumButtonPresses(int roboDepth) =>
    (
        from input in _input
        let length = GetMovesOnNumericKeypad(input)
            .Select(m => CountPressesOnDirectionalKeypad(m, roboDepth))
            .Min()
        select Complexity(input, length)
    ).Sum();

    private string[] GetMovesOnNumericKeypad(string s)
    {
        List<string[]> options = [];
        for (var i = -1; i < s.Length - 1; i++)
        {
            var x = i > -1 ? s[i] : 'A';
            options.Add(_moves[_numericKeypad][(x, s[i + 1])]);
        }

        return options
            .CartesianProduct()
            .Select(arr => string.Join("", arr))
            .ToArray();
    }

    private static Dictionary<(char, char), string[]> PossibleMovesOnKeypad(char[][] keypad,
        Dictionary<char, (int, int)> pos)
    {
        Dictionary<(char, char), string[]> sequences = [];
        foreach (var x in pos.Keys)
        {
            foreach (var y in pos.Keys)
            {
                if (x == y)
                {
                    sequences.Add((x, y), ["A"]);
                    continue;
                }

                List<string> possibilities = [];
                var q = new Queue<((int, int), string)>();
                q.Enqueue((pos[x], ""));
                var optimal = int.MaxValue;
                while (q.Count > 0)
                {
                    var ((r, c), moves) = q.Dequeue();
                    foreach (var (nr, nc, nm) in GetNeighbors(r, c,
                                 n =>
                                     0 <= n.Item1 && n.Item1 < keypad.Length &&
                                     0 <= n.Item2 && n.Item2 < keypad[0].Length &&
                                     keypad[n.Item1][n.Item2] != 'X'))
                    {
                        if (keypad[nr][nc] == y)
                        {
                            if (optimal < moves.Length + 1) goto Found;
                            optimal = moves.Length + 1;
                            possibilities.Add(moves + nm + "A");
                        }
                        else
                        {
                            q.Enqueue(((nr, nc), moves + nm));
                        }
                    }
                }

                Found:
                sequences[(x, y)] = possibilities.ToArray();
            }
        }

        return sequences;
    }

    private static long Complexity(string code, long buttonPresses) =>
        buttonPresses * long.Parse(string.Join("", code.Where(char.IsDigit)));

    private static HashSet<(int, int, string)> GetNeighbors(int r, int c,
        Func<(int, int, string), bool> selector = null)
    {
        HashSet<(int, int, string)> ns =
            [(r - 1, c, "^"), (r + 1, c, "v"), (r, c - 1, "<"), (r, c + 1, ">")];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }
}