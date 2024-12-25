using System.Diagnostics;

namespace AdventOfCode;

/// <summary>
/// --- Day 25: Code Chronicle ---
/// </summary>
public sealed class Day25 : BaseDay
{
    private readonly List<List<int>> _locks;
    private readonly List<List<int>> _keys;

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
            var lines = block.Split('\n');
            var pinCount = CountColumns(lines);
            if (lines[0].All(c => c == '#') && lines[^1].All(c => c == '.'))
                _locks.Add(pinCount);
            else if (lines[0].All(c => c == '.') && lines[^1].All(c => c == '#'))
                _keys.Add(pinCount);
            else throw new ArgumentException($"Input {block} is not valid.");
        }

        return;

        List<int> CountColumns(string[] lines)
        {
            var width = lines[0].Length;
            var result = new List<int>(width);
            for (var c = 0; c < width; c++)
            {
                result.Add(-1);
                foreach (var line in lines)
                    if (line[c] == '#') result[c] += 1;
            }

            return result;
        }
    }

    public override ValueTask<string> Solve_1() => new($"{FitKeys()}"); // Test: 3

    public override ValueTask<string> Solve_2() => new($"{FitKeys(true)}"); // Test: 

    private int FitKeys(bool part2 = false)
    {
        if (part2) return -1;
        var counter = 0;
        foreach (var l in _locks)
        {
            foreach (var k in _keys)
            {
                Debug.Assert(l.Count == k.Count);
                var fits = !l.Where((pins, col) => pins + k[col] > 5).Any();
                if (fits) counter++;
            }
        }

        return counter;
    }
}