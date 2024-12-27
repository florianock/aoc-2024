namespace AdventOfCode;

/// <summary>
/// --- Day 25: Code Chronicle ---
/// </summary>
public sealed class Day25 : BaseDay
{
    private readonly List<int[]> _locks;
    private readonly List<int[]> _keys;

    public Day25()
    {
        var input = File.ReadAllText(InputFilePath).Split("\n\n");
        // var input =
        // "#####\n.####\n.####\n.####\n.#.#.\n.#...\n.....\n\n#####\n##.##\n.#.##\n...##\n...#.\n...#.\n.....\n\n.....\n#....\n#....\n#...#\n#.#.#\n#.###\n#####\n\n.....\n.....\n#.#..\n###..\n###.#\n###.#\n#####\n\n.....\n.....\n.....\n#....\n#.#..\n#.#.#\n#####"
        // .Split("\n\n");
        _locks = [];
        _keys = [];
        foreach (var block in input)
        {
            var width = block.IndexOf('\n');
            var isLock = false;
            var pins = Enumerable.Repeat(-1, width).ToArray();
            for (var i = 0; i < block.Length; i++)
                if (block[i] == '#')
                {
                    pins[i % (width + 1)] += 1;
                    if (i == 0) isLock = true;
                }

            if (isLock) _locks.Add(pins);
            else _keys.Add(pins);
        }
    }

    public override ValueTask<string> Solve_1() => new($"{FitKeys()}"); // Test: 3

    public override ValueTask<string> Solve_2() => new("Deliver Chronicle!");

    private int FitKeys() => _locks
        .Sum(l => _keys
            .Select(k => !l.Where((pins, col) => pins + k[col] > 5).Any())
            .Count(fits => fits)
        );
}