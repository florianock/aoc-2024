using System.Text.RegularExpressions;

namespace AdventOfCode;

/// <summary>
/// --- Day 3: Mull It Over ---
/// </summary>
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
        var program = regex.Matches(_input);
        var isEnabled = true;
        foreach (Match line in program)
        {
            var instruction = line.ToString();
            switch (instruction)
            {
                case "do()": isEnabled = true; break;
                case "don't()": isEnabled = false; break;
                default:
                    if (isEnabled)
                    {
                        result += instruction.Substring(4, instruction.Length - 5)
                            .Split(',')
                            .Aggregate(1, (x, y) => x * int.Parse(y));
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
