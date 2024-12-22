namespace AdventOfCode;

/// <summary>
/// --- Day 22: Monkey Market ---
/// </summary>
public sealed class Day22 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day22()
    {
        _input = File.ReadLines(InputFilePath);
        // _input = "1\n10\n100\n2024".Split('\n'); // part 1
        // _input = "1\n2\n3\n2024".Split('\n'); // part 2
    }

    public override ValueTask<string> Solve_1() => new($"{_input.AsParallel().Sum(n => IterateSecretNumbers(int.Parse(n), 2000))}"); // Test: 37327623

    public override ValueTask<string> Solve_2() => new($"{_input.Sum(n => IterateSecretNumbers(int.Parse(n), 2000, true))}"); // Test: 23

    private static long IterateSecretNumbers(int number, int iterations, bool part2 = false)
    {
        if (part2) return 0; // -2,1,-1,3
        var result = (long)number;
        for (var i = 0; i < iterations; i++)
        {
            result = GetNextSecretNumber(result);
        }

        // Console.WriteLine($"{number}: {result}");
        return result;

        long GetNextSecretNumber(long n)
        {
            n = ((n * 64L) ^ n) % 16777216L;
            n = ((n / 32L) ^ n) % 16777216L;
            n = ((n * 2048L) ^ n) % 16777216L;
            return n;
        }
    }
}