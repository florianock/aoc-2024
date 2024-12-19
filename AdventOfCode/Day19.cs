namespace AdventOfCode;

/// <summary>
/// --- Day 19: Linen Layout ---
/// </summary>
public sealed class Day19 : BaseDay
{
    private readonly List<string> _towelPatterns;
    private readonly IEnumerable<string> _designs;
    private readonly int _maxLen;
    private readonly Dictionary<string, long> _cache;

    public Day19()
    {
        var input = File.ReadAllText(InputFilePath).Split("\n\n").ToList();
        // var input = "r, wr, b, g, bwu, rb, gb, br\n\nbrwrr\nbggr\ngbbr\nrrbgbr\nubwu\nbwurrg\nbrgr\nbbrgwb"
        // .Split("\n\n")
        // .ToList();
        _towelPatterns = input[0].Split(", ").ToList();
        _designs = input[1].Split('\n').ToHashSet();
        _maxLen = _towelPatterns.Max(p => p.Length);
        _cache = new Dictionary<string, long>();
    }

    public override ValueTask<string> Solve_1() => new($"{_designs.Count(d => NumCombinations(d) > 0)}"); // Test: 6

    public override ValueTask<string> Solve_2() => new($"{_designs.Sum(NumCombinations)}"); // Test: 16

    private long NumCombinations(string d)
    {
        if (d == string.Empty) return 1;
        if (_cache.TryGetValue(d, out var match)) return match;
        var count = Enumerable.Range(0, Math.Min(d.Length, _maxLen) + 1).Where(i => _towelPatterns.Contains(d[..i]))
            .Sum(i => NumCombinations(d[i..]));

        _cache.Add(d, count);
        return count;
    }
}