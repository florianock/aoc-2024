namespace AdventOfCode;

/// <summary>
/// --- Day 5: Print Queue ---
/// </summary>
public sealed class Day05 : BaseDay
{
    private readonly (int, int)[] _pageOrderingRules;
    private readonly IEnumerable<int[]> _pageUpdates;

    public Day05()
    {
        var input = File.ReadLines(InputFilePath).ToArray();
        // var input =
            // "47|53\n97|13\n97|61\n97|47\n75|29\n61|13\n75|53\n29|13\n97|29\n53|29\n61|53\n97|53\n61|29\n47|13\n75|47\n97|75\n47|61\n75|61\n47|29\n75|13\n53|13\n\n75,47,61,53,29\n97,61,53,29,13\n75,29,13\n75,97,47,61,53\n61,13,29\n97,13,75,29,47"
                // .Split('\n');
        // Test1: 143
        // Test2: 123
        var cutOff = Array.IndexOf(input, string.Empty);
        _pageOrderingRules = input[..cutOff]
            .Select(r => r.Split('|').Select(int.Parse).ToArray())
            .Select(arr => (arr[0], arr[1]))
            .ToArray();
        _pageUpdates = input[(cutOff + 1)..].Select(update => update.Split(',').Select(int.Parse).ToArray());
    }

    public override ValueTask<string> Solve_1() => new($"{SumValidUpdates()}");

    public override ValueTask<string> Solve_2() => new($"{SumCorrectedInvalidUpdates()}");

    private int SumValidUpdates() => _pageUpdates.Where(IsValid).Sum(Middle);

    private int SumCorrectedInvalidUpdates() => _pageUpdates.Where(IsInvalid).Select(FixFaults).Sum(Middle);

    private bool IsValid(int[] update) => !IsInvalid(update);

    private bool IsInvalid(int[] update) => FindFault(update).HasValue;

    private static int Middle(int[] update) => update[update.Length / 2];

    private (int, int)? FindFault(int[] update)
    {
        for (var i = 0; i < update.Length; i++)
        {
            for (var j = i + 1; j < update.Length; j++)
            {
                if (!_pageOrderingRules.Contains((update[j], update[i]))) continue;
                // Console.WriteLine($"{string.Join(",", update)} violates rule {update[j]}|{update[i]}");
                return (update[j], update[i]);
            }
        }

        return null;
    }

    private int[] FixFaults(int[] update)
    {
        var corrected = update;
        while (true)
        {
            var conflictingRule = FindFault(corrected);
            if (!conflictingRule.HasValue) break;
            var idx1 = Array.IndexOf(update, conflictingRule.Value.Item1);
            var idx2 = Array.IndexOf(update, conflictingRule.Value.Item2);
            corrected[idx1] = conflictingRule.Value.Item2;
            corrected[idx2] = conflictingRule.Value.Item1;
        }
        return corrected;
    }
}