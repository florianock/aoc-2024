namespace AdventOfCode;

/// <summary>
/// --- Day 7: Bridge Repair ---
/// </summary>
public class Day07 : BaseDay
{
    private readonly List<List<long>> _input;

    public Day07()
    {
        var lines = File.ReadLines(InputFilePath);
        // var lines =
        // "190: 10 19\n3267: 81 40 27\n83: 17 5\n156: 15 6\n7290: 6 8 6 15\n161011: 16 10 13\n192: 17 8 14\n21037: 9 7 18 13\n292: 11 6 16 20"
        // .Split("\n");

        _input = lines.Select(s => s.Split(' ').Select(i => long.Parse(i.Replace(":", ""))).ToList()).ToList();
    }

    public override ValueTask<string> Solve_1() => new($"{FirstPart()}"); // Test: 3749

    public override ValueTask<string> Solve_2() => new($"{SecondPart()}"); // Test: 11387

    private long FirstPart() =>
        _input.Select(puzzle => HasSolution(puzzle[0], puzzle[1..]) ? puzzle[0] : 0).Sum();

    private long SecondPart() =>
        _input.Select(puzzle => HasSolution(puzzle[0], puzzle[1..], true) ? puzzle[0] : 0).Sum();

    private static bool HasSolution(long answer, List<long> parts, bool useConcatenation = false)
    {
        if (parts.Count == 1) return answer == parts[0];
        if (answer % parts[^1] == 0 && HasSolution(answer / parts[^1], parts[..^1], useConcatenation)) return true;
        if (answer > parts[^1] && HasSolution(answer - parts[^1], parts[..^1], useConcatenation)) return true;
        if (!useConcatenation) return false;
        var strAnswer = answer.ToString();
        var strLast = parts[^1].ToString();
        return strAnswer.EndsWith(strLast) && strAnswer.Length > strLast.Length &&
               HasSolution(long.Parse(strAnswer[..^strLast.Length]), parts[..^1], true);
    }
}