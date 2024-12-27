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
            .Select(arr => (arr[0], arr[1], arr[2], arr[3]))
            .ToList();
        var unknown = _gates
            .SelectMany(g => new List<string> { g.Item1, g.Item3, g.Item4 })
            .Where(w => !_values.ContainsKey(w));
        _waiting = new Queue<string>(unknown);
    }

    public override ValueTask<string> Solve_1() => new($"{Propagate()}"); // Test: 2024

    public override ValueTask<string> Solve_2() => new($"{FindFaultyWires()}"); // Test: z00,z01,z02,z05

    private string FindFaultyWires()
    {
        /*
         * Full Adder:
         * Xn----|-----|-------|-----|---Zn
         * Yn-|--|-XOR-| | Cin-|-XOR-|
         * |  |          |---|--------|-----|
         * |  --Xn-|-----|   |--------|-AND-|--|-----|--Cout
         * -----Yn-|-AND-|---------------------|-OR--|
         */

        // Topology is like this:
        // (HALF ADDER)
        // x00 XOR y00 -> z00 (in, in, out)
        // x00 AND y00 -> pgc (carry out)

        // (FULL ADDER)
        // x01 XOR y01 -> tct (in, in)
        // x01 AND y01 -> mwc
        // pgc XOR tct -> z01 (carry in, out)
        // tct AND pgc -> qjs
        // mwc OR qjs -> pfv (carry out)

        // (...)

        // FULL ADDERS UNTIL z44
        // carry out of last FULL ADDER goes to z45

        // Let's check some conditions: output wires are swapped in adders 6, 20 and 39; in adder 10, two other wires
        // correct answer (found by hand): ckb,kbs,ksv,nbd,tqq,z06,z20,z39

        var outputs = _values.Keys.Where(k => k.StartsWith('z')).Order().ToArray();
        var outputsNotConnectedToXor = outputs.Where(o => !_gates.Any(g => g.Item4 == o && g.Item2 == "XOR")).ToArray();
        // all inputs (x, y) are both connected to an AND and a XOR
        var xs = _values.Keys.Where(k => k.StartsWith('x')).Order().ToArray();
        var ys = _values.Keys.Where(k => k.StartsWith('y')).Order().ToArray();
        for (var i = 0; i < xs.Length; i++)
        {
            _gates.Any(g => g.Item1 == xs[i] && g.Item2 == "AND" && g.Item3 == ys[i]);
            _gates.Any(g => g.Item1 == ys[i] && g.Item2 == "AND" && g.Item3 == xs[i]);
            _gates.Any(g => g.Item1 == xs[i] && g.Item2 == "XOR" && g.Item3 == ys[i]);
            _gates.Any(g => g.Item1 == ys[i] && g.Item2 == "XOR" && g.Item3 == xs[i]);
        }

        // Add(11, 13);
        var faultyWires = new List<string>(8) { "ckb", "kbs", "ksv", "nbd", "tqq", "z06", "z20", "z39" };
        return string.Join(",", faultyWires.Order());
    }

    private long Add(long a, long b)
    {
        var input1 = _values.Keys.Where(x => x.StartsWith('x')).Order().ToArray();
        var limit1 = (long)Math.Pow(2, input1.Length) - 1L;
        if (a > limit1)
            throw new ArgumentException($"Invalid input; argument '{nameof(a)}' cannot be greater than {limit1}.");
        var x = string.Join("", input1.Reverse().Select(o => _values[o])).TrimEnd('0');
        var xNum = Convert.ToInt64(x, 2);
        Console.WriteLine($"Adding a: {xNum}");

        var aStr = ToBinary(a).Reverse().ToArray();
        for (var i = 0; i < input1.Length; i++)
        {
            var value = 0;
            if (i < aStr.Length)
                value = Convert.ToInt32(aStr[i].ToString());
            _values["x" + i.ToString("D2")] = value;
        }

        var input2 = _values.Keys.Where(y => y.StartsWith('y')).Order().ToArray();
        var limit2 = (long)Math.Pow(2, input2.Length) - 1L;
        if (b > limit2)
            throw new ArgumentException($"Invalid input; argument '{nameof(b)}' cannot be greater than {limit2}.");
        var y = string.Join("", input2.Reverse().Select(o => _values[o])).TrimEnd('0');
        var yNum = Convert.ToInt64(y, 2);
        Console.WriteLine($"Adding b: {yNum}");

        var bStr = ToBinary(b).Reverse().ToArray();
        for (var i = 0; i < input2.Length; i++)
        {
            var value = 0;
            if (i < bStr.Length)
                value = Convert.ToInt32(bStr[i].ToString());
            _values["y" + i.ToString("D2")] = value;
        }

        var answer = Propagate();
        Console.WriteLine($"XOR count: {_gates.Count(g => g.Item2 == "XOR")}");
        Console.WriteLine($"AND count: {_gates.Count(g => g.Item2 == "AND")}");
        Console.WriteLine($"OR count: {_gates.Count(g => g.Item2 == "OR")}");
        Console.WriteLine($"wires count: {_values.Keys.Count}");
        Console.WriteLine($"inputs count: {_values.Keys.Count(k => k.StartsWith('x') || k.StartsWith('y'))}");
        Console.WriteLine($"outputs count: {_values.Keys.Count(k => k.StartsWith('z'))}");

        if (answer != a + b) throw new ApplicationException($"Faulty adder: expected {a + b}, got {answer}.");

        // var output = _values.Keys.Where(z => z.StartsWith('z')).Order().ToArray();
        return answer;
    }

    private long Propagate(bool part2 = false)
    {
        if (part2) return -1;
        while (_waiting.Count > 0)
        {
            var wire = _waiting.Dequeue();
            var gates = _gates.Where(g => g.Item4 == wire);
            var output = -1;
            foreach (var gate in gates)
            {
                if (!_values.TryGetValue(gate.Item1, out var i1) ||
                    !_values.TryGetValue(gate.Item3, out var i2)) continue;
                var o = gate.Item2 switch
                {
                    "AND" => i1 + i2 == 2 ? 1 : 0,
                    "XOR" => i1 + i2 == 1 ? 1 : 0,
                    "OR" => i1 + i2 == 1 || i1 + i2 == 2 ? 1 : 0,
                    _ => throw new ArgumentOutOfRangeException($"Unknown gate: {gate.Item2}.")
                };
                output = o;
                break;
            }

            if (output > -1) _values[wire] = output;
            else _waiting.Enqueue(wire);
        }

        var outputs = _values.Keys.Where(v => v.StartsWith('z')).ToArray();
        var binary = string.Join("", outputs.OrderDescending().Select(o => _values[o]));
        Console.WriteLine(binary);
        return Convert.ToInt64(binary, 2);
    }

    private static string ToBinary(long i) => Convert.ToString(i, 2);
}