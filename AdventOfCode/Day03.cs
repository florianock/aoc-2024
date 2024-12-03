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

    public override ValueTask<string> Solve_1() => new($"{RunProgram(true)}"); // Test: 161

    public override ValueTask<string> Solve_2() => new($"{RunProgram()}"); // Test: 48

    private int RunProgram(bool ignoreConditionals = false)
    {
        var result = 0;
        var program = InstructionRegex().Matches(_input).Select(m => m.ToString());
        var isEnabled = true;
        foreach (var instruction in program)
        {
            switch (instruction)
            {
                case "do()": isEnabled = true; break;
                case "don't()": isEnabled = false; break;
                default:
                    if (ignoreConditionals || isEnabled)
                    {
                        result += instruction
                            .Substring(4, instruction.Length - 5)
                            .Split(',')
                            .Aggregate(1, (x, y) => x * int.Parse(y));
                    }

                    break;
            }
        }

        return result;
    }

    [GeneratedRegex(@"don't\(\)|do\(\)|mul\(\d{1,3},\d{1,3}\)")]
    private static partial Regex InstructionRegex();
}