namespace AdventOfCode;

/// <summary>
/// --- Day 23: LAN Party ---
/// </summary>
public sealed class Day23 : BaseDay
{
    private readonly HashSet<HashSet<string>> _couples;

    public Day23()
    {
        // var input =
            // "kh-tc\nqp-kh\nde-cg\nka-co\nyn-aq\nqp-ub\ncg-tb\nvc-aq\ntb-ka\nwh-tc\nyn-cg\nkh-ub\nta-co\nde-co\ntc-td\ntb-wq\nwh-td\nta-ka\ntd-qp\naq-cg\nwq-ub\nub-vc\nde-ta\nwq-aq\nwq-vc\nwh-yn\nka-de\nkh-ta\nco-tc\nwh-qp\ntb-vc\ntd-yn"
                // .Split('\n');
        var input = File.ReadLines(InputFilePath);
        _couples = [];
        foreach (var parts in input.Select(line => line.Split('-')))
            _couples.Add([parts[0], parts[1]]);
    }

    public override ValueTask<string> Solve_1() =>
        new($"{GroupLarger(_couples).Count(str => str.Split(',').Any(s => s.StartsWith('t')))}"); // Test: 7

    public override ValueTask<string> Solve_2() => new($"{FindLanParty(_couples)}"); // Test: co,de,ka,ta

    private static HashSet<string> GroupLarger(HashSet<HashSet<string>> collection)
    {
        var groupSize = collection.First().Count;
        return collection.AsParallel().Select(item =>
                collection
                    .Select(i => i.Except(item))
                    .Where(s => s.Count() == 1)
                    .SelectMany(s => s)
                    .GroupBy(x => x)
                    .OrderByDescending(g => g.Count())
                    .Where(g => g.Count() == groupSize)
                    .Select(g => new List<string>(groupSize + 1) { g.Key })
                    .Select(s =>
                    {
                        s.AddRange(item);
                        return string.Join(",", s.Order().ToArray());
                    })
            )
            .Where(s => s.Any())
            .SelectMany(s => s)
            .ToHashSet();
    }

    private static string FindLanParty(HashSet<HashSet<string>> collection)
    {
        var nextLarger = GroupLarger(collection).Select(str => str.Split(',').ToHashSet());
        Dictionary<string, int> popularity = [];
        foreach (var item in nextLarger)
        {
            foreach (var i in item)
            {
                if (!popularity.TryGetValue(i, out _)) popularity.Add(i, 0);
                popularity[i] += 1;
            }
        }

        var max = popularity.Max(x => x.Value);
        var answer = popularity.Where(kv => kv.Value == max).Select(kv => kv.Key).ToArray();

        return string.Join(",", answer.Order());
    }
}