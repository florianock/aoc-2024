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

    public override ValueTask<string> Solve_1() => new($"{_pageUpdates.Where(IsValid).Sum(MiddlePage)}");

    public override ValueTask<string> Solve_2() =>
        new($"{_pageUpdates.Where(IsInvalid).Select(FixFaults).Sum(MiddlePage)}");

    private bool IsValid(int[] update) => FindFault(update) is null;

    private bool IsInvalid(int[] update) => !IsValid(update);

    private static int MiddlePage(int[] update) => update[update.Length / 2];

    private (int, int)? FindFault(int[] update)
    {
        var relevantRules = _pageOrderingRules.Where(r => update.Contains(r.Item1) && update.Contains(r.Item2));
        foreach (var rule in relevantRules)
        {
            var a = Array.IndexOf(update, rule.Item1);
            var b = Array.IndexOf(update, rule.Item2);
            if (a > b) return (a, b);
        }

        return null;
    }

    private int[] FixFaults(int[] update)
    {
        var corrected = update;
        while (true)
        {
            var conflicting = FindFault(corrected);
            if (conflicting is null) break;
            var (idx1, idx2) = conflicting.Value;
            (corrected[idx1], corrected[idx2]) = (corrected[idx2], corrected[idx1]);
        }

        return corrected;
    }
}