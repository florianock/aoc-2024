using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day03 : BaseDay
{
    private readonly string _input;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
        // _input = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
    }

    public override ValueTask<string> Solve_1() => new($"{RunProgram(InstructionRegex())}"); // Test: 161

    public override ValueTask<string> Solve_2() => new($"{RunProgram(InstructionRegexWithConditionals())}"); // Test: 48

    private int RunProgram(Regex regex)
    {
        var result = 0;
        var matches = regex.Matches(_input);
        var isEnabled = true;
        foreach (Match match in matches)
        {
            var statement = match.ToString();
            switch (statement)
            {
                case "do()": isEnabled = true; break;
                case "don't()": isEnabled = false; break;
                default:
                    if (isEnabled)
                    {
                        result += statement.Substring(4, statement.Length - 5)
                            .Split(',')
                            .Select(int.Parse)
                            .Aggregate(1, (x, y) => x * y);
                    }
                    break;
            }
        }
        return result;
    }
    
    [GeneratedRegex(@"mul\(-?[0-9]+,-?[0-9]+\)")]
    private static partial Regex InstructionRegex();

    [GeneratedRegex(@"don't\(\)|do\(\)|mul\(-?[0-9]+,-?[0-9]+\)")]
    private static partial Regex InstructionRegexWithConditionals();
}
