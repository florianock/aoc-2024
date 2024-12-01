namespace AdventOfCode;

/// <summary>
/// --- Day 1: Historian Hysteria ---
/// </summary>
public class Day01 : BaseDay
{
    private readonly IEnumerable<string> _input;
    private (List<int>, List<int>)? _processed;
    
    public Day01()
    {
        _input = File.ReadLines(InputFilePath);
        // _input = "3   4\n4   3\n2   5\n1   3\n3   9\n3   3".Split("\n");
    }

    public override ValueTask<string> Solve_1() => new($"{SumLocationDistances()}");

    public override ValueTask<string> Solve_2() => new($"{MultiplyLocationCounts()}");

    private int SumLocationDistances()
    {
        // Test: 11
        var (list1, list2) = ProcessInput();

        var result = 0;
        for (var i = 0; i < list1.Count; i++)
        {
            result += Math.Abs(list1[i] - list2[i]);
        }
        return result;
    }

    private int MultiplyLocationCounts()
    {
        // Test: 31
        var (list1, list2) =  ProcessInput();
        var locationCounts = list2
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());

        return list1.Aggregate(0, (agg, next) => agg + next * locationCounts.GetValueOrDefault(next));
    }

    private (List<int>, List<int>) ProcessInput()
    {
        if (_processed is not null) return _processed.Value;
        
        var initial = (new List<int>(), new List<int>());

        var lists = _input
            .Select(x => x.Split("   "))
            .Aggregate(initial, (agg, next) =>
            {
                agg.Item1.Add(int.Parse(next[0]));
                agg.Item2.Add(int.Parse(next[1]));
                return agg;
            });

        lists.Item1.Sort();
        lists.Item2.Sort();

        _processed = lists;

        return lists;
    }
}
