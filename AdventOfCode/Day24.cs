namespace AdventOfCode;

/// <summary>
/// --- Day 24: Crossed Wires ---
/// </summary>
public sealed class Day24 : BaseDay
{
    private readonly Dictionary<string, int> _values;
    private readonly List<(string, string, string, string)> _gates;
    private readonly Queue<string> _waiting;

    public Day24()
    {
        // var input =
            // "x00: 1\nx01: 0\nx02: 1\nx03: 1\nx04: 0\ny00: 1\ny01: 1\ny02: 1\ny03: 1\ny04: 1\n\nntg XOR fgs -> mjb\ny02 OR x01 -> tnw\nkwq OR kpj -> z05\nx00 OR x03 -> fst\ntgd XOR rvg -> z01\nvdt OR tnw -> bfw\nbfw AND frj -> z10\nffh OR nrd -> bqk\ny00 AND y03 -> djm\ny03 OR y00 -> psh\nbqk OR frj -> z08\ntnw OR fst -> frj\ngnj AND tgd -> z11\nbfw XOR mjb -> z00\nx03 OR x00 -> vdt\ngnj AND wpb -> z02\nx04 AND y00 -> kjc\ndjm OR pbm -> qhw\nnrd AND vdt -> hwm\nkjc AND fst -> rvg\ny04 OR y02 -> fgs\ny01 AND x02 -> pbm\nntg OR kjc -> kwq\npsh XOR fgs -> tgd\nqhw XOR tgd -> z09\npbm OR djm -> kpj\nx03 XOR y03 -> ffh\nx00 XOR y04 -> ntg\nbfw OR bqk -> z06\nnrd XOR fgs -> wpb\nfrj XOR qhw -> z04\nbqk OR frj -> z07\ny03 OR x01 -> nrd\nhwm AND bqk -> z03\ntgd XOR rvg -> z12\ntnw OR pbm -> gnj"
                // .Split("\n\n");
        var input = File.ReadAllText(InputFilePath).Split("\n\n");
        _values = input[0].Split('\n')
            .Select(s => s.Split(": "))
            .ToDictionary(arr => arr[0], arr => int.Parse(arr[1]));
        _gates = input[1].Split('\n')
            .Select(s => s.Split(' '))
            .Select(arr => (arr[1], arr[0], arr[2], arr[4]))
            .ToList();
        var unknown = _gates
            .SelectMany(g => new List<string> { g.Item2, g.Item3, g.Item4 })
            .Where(w => !_values.ContainsKey(w));
        _waiting = new Queue<string>(unknown);
    }

    public override ValueTask<string> Solve_1() => new($"{DoStuff()}"); // Test: 2024

    public override ValueTask<string> Solve_2() => new($"{DoStuff(true)}"); // Test: 

    private long DoStuff(bool part2 = false)
    {
        if (part2) return -1;
        while (_waiting.Count > 0)
        {
            var wire = _waiting.Dequeue();
            var gates = _gates.Where(g => g.Item4 == wire);
            var output = -1;
            foreach (var gate in gates)
            {
                if (!_values.TryGetValue(gate.Item2, out var i1) ||
                    !_values.TryGetValue(gate.Item3, out var i2)) continue;
                var o = gate.Item1 switch
                {
                    "AND" => i1 + i2 == 2 ? 1 : 0,
                    "XOR" => i1 + i2 == 1 ? 1 : 0,
                    "OR" => i1 + i2 == 1 || i1 + i2 == 2 ? 1 : 0,
                    _ => throw new ArgumentOutOfRangeException($"Unknown gate: {gate.Item1}.")
                };
                output = o;
                break;
            }

            if (output > -1) _values[wire] = output;
            else _waiting.Enqueue(wire);
        }

        var outputs = _values.Keys.Where(v => v.StartsWith('z')).ToArray();
        Array.Sort(outputs);
        Array.Reverse(outputs);
        var result = string.Join("", outputs.Select(o => _values[o]));
        return Convert.ToInt64(result, 2);
    }
}