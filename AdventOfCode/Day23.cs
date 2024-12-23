namespace AdventOfCode;

/// <summary>
/// --- Day 23:  ---
/// </summary>
public sealed class Day23 : BaseDay
{
    private readonly HashSet<string[]> _couples;
    private readonly HashSet<string[]> _triples;

    public Day23()
    {
        var input =
            "kh-tc\nqp-kh\nde-cg\nka-co\nyn-aq\nqp-ub\ncg-tb\nvc-aq\ntb-ka\nwh-tc\nyn-cg\nkh-ub\nta-co\nde-co\ntc-td\ntb-wq\nwh-td\nta-ka\ntd-qp\naq-cg\nwq-ub\nub-vc\nde-ta\nwq-aq\nwq-vc\nwh-yn\nka-de\nkh-ta\nco-tc\nwh-qp\ntb-vc\ntd-yn"
                .Split('\n').ToList();
        // var input = File.ReadLines(InputFilePath).ToList();
        _couples = [];
        foreach (var parts in input.Select(line => line.Split('-')))
        {
            _couples.Add(parts);
            // _couples.Add((parts[1], parts[0]));
        }

        // Console.WriteLine($"{input.Count} -> {_couples.Count}");
        _triples = [];
    }

    public override ValueTask<string> Solve_1() => new($"{FindTriplesAndCountTs()}"); // Test: 7

    public override ValueTask<string> Solve_2() => new($"{FindLanParty()}"); // Test: co,de,ka,ta

    private string FindLanParty()
    {
        if (_triples == null || _triples.Count == 0)
            FindTriplesAndCountTs();
        // Lan Party is largest subset where every computer is connected to every other computer
        throw new NotImplementedException();
    }

    private int FindTriplesAndCountTs()
    {
        // var ts = _couples.Where(c => c.Item1.StartsWith('t') || c.Item2.StartsWith('t')).ToList();
        foreach (var couple in _couples)
        {
            var results = _couples.Where(c => c[0] != couple[0] && c[1] != couple[1] &&
                                              (c[0] == couple[0] || c[0] == couple[1])).Select(c => c[1]).ToList();
            var otherResults = _couples.Where(c => c[0] != couple[0] && c[1] != couple[1] &&
                                                   (c[1] == couple[0] || c[1] == couple[1])).Select(c => c[0]).ToList();
            results.AddRange(otherResults);
            var connects = results
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key);
            foreach (var c in connects)
            {
                string[] t = [couple[0], couple[1], c];
                Array.Sort(t);
                _triples.Add(t);
            }
        }

        var startWithT = _triples
            .Where(t => t.Any(x => x.StartsWith('t')))
            // .OrderBy()
            .ToList();
        return startWithT.Count;
    }
}