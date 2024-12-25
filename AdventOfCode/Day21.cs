using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 21: Keypad Conundrum ---
/// </summary>
public sealed class Day21 : BaseDay
{
    private readonly IEnumerable<string> _input;
    private readonly Dictionary<char[][], Dictionary<(char, char), string[]>> _moves;
    private readonly Dictionary<string, (string[], int)> _cache;

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
        // _input = File.ReadLines(InputFilePath);
        _input = "029A\n980A\n179A\n456A\n379A".Split("\n");
        // _input = ["029A"];
        var positions = new Dictionary<char[][], Dictionary<char, (int, int)>>();
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

            _moves.Add(keypad, MovesForKeypad(keypad, positions[keypad]));
        }

        _cache = [];
    }

    public override ValueTask<string> Solve_1() => new($"{CalculateMinimumButtonPresses(2)}"); // Test: 126384

    public override ValueTask<string> Solve_2() => new($"{CalculateMinimumButtonPresses(3)}"); // Test: 

    private int CalculateMinimumButtonPresses(int roboDepth)
    {
        if (roboDepth > 2) return 1;
        return _input.Select(s =>
            {
                var (currentMoves, minLen) = GetMoves(s, _numericKeypad);
                for (var i = roboDepth; i > 0; i--)
                {
                    var a = currentMoves.Select(m => GetMoves(m, _directionalKeypad)).ToArray();
                    var moves = a.Select(b => b.Item1);
                    var lengths = a.Select(b => b.Item2);
                }

                return (s, minLen);
            })
            .Aggregate(0, (acc, cur) => acc + Complexity(cur.Item1, cur.Item2));
    }

    private (string[], int) GetMoves(string s, char[][] keypad)
    {
        if (_cache.TryGetValue(s, out var cachedValue)) return cachedValue;

        var seqs = _moves[keypad];
        s = "A" + s;
        List<string[]> options = [];
        for (var i = 0; i < s.Length - 1; i++)
            options.Add(seqs[(s[i], s[i + 1])]);

        var product = options.CartesianProduct().Select(x => x.ToList()).ToList();
        var result = product.Select(arr => string.Join("", arr)).ToArray();
        var minLen = result.Min(r => r.Length);
        result = result.Where(r => r.Length == minLen).ToArray();
        _cache[s] = (result, minLen);
        return (result, minLen);
    }

    private static Dictionary<(char, char), string[]> MovesForKeypad(char[][] keypad, Dictionary<char, (int, int)> pos)
    {
        Dictionary<(char, char), string[]> seqs = [];
        foreach (var x in pos.Keys)
        {
            foreach (var y in pos.Keys)
            {
                if (x == y)
                {
                    seqs.Add((x, y), ["A"]);
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
                seqs[(x, y)] = possibilities.ToArray();
            }
        }

        return seqs;
    }

    private static int Complexity(string code, int buttonPresses) =>
        buttonPresses * int.Parse(string.Join("", code.Where(char.IsDigit)));

    private static HashSet<(int, int, string)> GetNeighbors(int r, int c,
        Func<(int, int, string), bool> selector = null)
    {
        HashSet<(int, int, string)> ns =
            [(r - 1, c, "^"), (r + 1, c, "v"), (r, c - 1, "<"), (r, c + 1, ">")];
        return selector != null ? ns.Where(selector).ToHashSet() : ns.ToHashSet();
    }
}