using System.Diagnostics;

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
        _input.Select(puzzle => HasSolution(puzzle[1..], puzzle[0], ['+', '*']) ? puzzle[0] : 0).Sum();

    private long SecondPart() =>
        _input.Select(puzzle => HasSolution(puzzle[1..], puzzle[0], ['+', '*', '|']) ? puzzle[0] : 0).Sum();

    private static bool HasSolution(List<long> parts, long answer, char[] operators)
    {
        List<string> operatorCombinations = [];
        for (var i = Math.Pow(operators.Length, parts.Count - 1) - 1; i >= 0; i--)
        {
            var s = ConvertToBaseString((int)i, operators.Length)
                .Replace('2', '|')
                .Replace('1', '*')
                .Replace('0', '+')
                .PadLeft(parts.Count - 1, '+');
            operatorCombinations.Add(s);
        }

        return operatorCombinations.Any(combination => Solve(parts, answer, combination));
    }

    private static string ConvertToBaseString(int i, int newBase)
    {
        return newBase switch
        {
            2 => Convert.ToString(i, 2),
            3 => ConvertToTernary(i),
            _ => throw new ArgumentException("Only base 2 and 3 are supported.")
        };
    }

    private static bool Solve(List<long> parts, long answer, string s)
    {
        Debug.Assert(parts.Count == s.Length + 1);
        var result = parts[0];
        for (var i = 1; i < parts.Count; i++)
        {
            switch (s[i - 1])
            {
                case '+': result += parts[i]; break;
                case '*': result *= parts[i]; break;
                case '|': result = long.Parse($"{result}{parts[i]}"); break;
                default: throw new ArgumentException($"{s[i - 1]} is not a valid operator.");
            }
        }

        return result == answer;
    }

    // from: https://www.geeksforgeeks.org/ternary-number-system-or-base-3-numbers/
    private static string ConvertToTernary(int n)
    {
        if (n == 0) return "";

        var x = n % 3;
        n /= 3;
        if (x < 0)
            n += 1;

        var remainder = ConvertToTernary(n);

        return x >= 0 ? remainder + x : remainder + (x + 3 * -1);
    }
}