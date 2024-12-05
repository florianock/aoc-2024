namespace AdventOfCode;

/// <summary>
/// --- Day 5: Print Queue ---
/// </summary>
public sealed class Day05 : BaseDay
{
    private readonly Dictionary<List<string>, List<(string, string)>> _pageUpdatesWithRules;

    public Day05()
    {
        var input = File.ReadLines(InputFilePath).ToList();
        // var input =
            // "47|53\n97|13\n97|61\n97|47\n75|29\n61|13\n75|53\n29|13\n97|29\n53|29\n61|53\n97|53\n61|29\n47|13\n75|47\n97|75\n47|61\n75|61\n47|29\n75|13\n53|13\n\n75,47,61,53,29\n97,61,53,29,13\n75,29,13\n75,97,47,61,53\n61,13,29\n97,13,75,29,47"
                // .Split('\n').ToList();
        // Test1: 143
        // Test2: 123
        var cutOff = input.IndexOf(string.Empty);
        var pageOrderingRules = input[..cutOff]
            .Select(r => r.Split('|').ToArray())
            .Select(arr => (arr[0], arr[1]))
            .ToList();
        _pageUpdatesWithRules = input[(cutOff + 1)..]
            .Select(update => update.Split(',').ToList())
            .ToDictionary(update => update, update => pageOrderingRules
                .Where(r => update.Contains(r.Item1) && update.Contains(r.Item2))
                .ToList()
            );
    }

    public override ValueTask<string> Solve_1() => new($"{_pageUpdatesWithRules.Keys.Where(IsValid).Sum(MiddlePage)}");

    public override ValueTask<string> Solve_2() =>
        new($"{_pageUpdatesWithRules.Keys.Where(IsInvalid).Select(FixFaults).Sum(MiddlePage)}");

    private bool IsValid(List<string> update) => FindFault(update) is null;

    private bool IsInvalid(List<string> update) => !IsValid(update);

    private static int MiddlePage(List<string> update) => int.Parse(update[update.Count / 2]);

    private (int, int)? FindFault(List<string> update)
    {
        foreach (var rule in _pageUpdatesWithRules[update])
        {
            var a = update.IndexOf(rule.Item1);
            var b = update.IndexOf(rule.Item2);
            if (a > b) return (a, b);
        }

        return null;
    }

    private List<string> FixFaults(List<string> update)
    {
        while (true)
        {
            var fault = FindFault(update);
            if (fault is null) break;
            var (idx1, idx2) = fault.Value;
            (update[idx1], update[idx2]) = (update[idx2], update[idx1]);
        }

        return update;
    }
}