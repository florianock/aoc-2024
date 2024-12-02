namespace AdventOfCode;

/// <summary>
/// --- Day 2: Red-Nosed Reports ---
/// </summary>
public class Day02 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day02()
    {
        _input = File.ReadLines(InputFilePath);
        // _input = "7 6 4 2 1\n1 2 7 8 9\n9 7 6 2 1\n1 3 2 4 5\n8 6 4 4 1\n1 3 6 7 9".Split("\n");
    }

    public override ValueTask<string> Solve_1() => new($"{GetReportsSafetyScore()}"); // Test: 2
    
    public override ValueTask<string> Solve_2() => new($"{GetReportsSafetyScore(true)}"); // Test: 4
    
    private int GetReportsSafetyScore(bool useProblemDampener = false) =>
        _input.Select(report => IsSafe(Process(report), useProblemDampener))
        .Select(isReportSafe => isReportSafe ? 1 : 0)
        .Sum(); 
    
    private static bool IsSafe(int[] report, bool useProblemDampener = false)
    {
        var initialDirection = int.Sign(report[1] - report[0]);
        for (var i = 0; i < report.Length - 1; i++)
        {
            var step = report[i + 1] - report[i];
            var direction = int.Sign(step);
            
            if (direction != 0 && direction == initialDirection && 1 <= Math.Abs(step) && Math.Abs(step) <= 3)
                continue;
            
            return useProblemDampener &&
                   (IsSafe(report.Where((_, idx) => idx != i).ToArray())
                    || IsSafe(report.Where((_, idx) => idx != i + 1).ToArray()));
        }
        return true;
    }

    private static int[] Process(string report) => report.Split(' ').Select(int.Parse).ToArray();
}
