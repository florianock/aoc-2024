namespace AdventOfCode;

/// <summary>
/// --- Day 1: Historian Hysteria ---
/// </summary>
public class Day01 : BaseDay
{
    private readonly string _input;
    
    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
        // _input = "3   4\n4   3\n2   5\n1   3\n3   9\n3   3";
    }

    public override ValueTask<string> Solve_1() => new($"{Solve_1_Original()}");

    public override ValueTask<string> Solve_2() => new($"{Solve_2_Original()}");

    private int Solve_1_Original()
    {
        // Test: 11
        var locationLists = Process(_input);

        var result = 0;
        
        for (var i = 0; i < locationLists.Item1.Count; i++)
        {
            result += Math.Abs(locationLists.Item1[i] - locationLists.Item2[i]);
        }
        
        return result;
    }

    private int Solve_2_Original()
    {
        // Test: 31
        var locationLists = Process(_input);
        var locationCounts = locationLists.Item2
            .GroupBy(i => i)
            .ToDictionary(g => g.Key, g => g.Count());

        var result = 0;
        
        foreach (var loc in locationLists.Item1)
        {
            result += loc * locationCounts.GetValueOrDefault(loc);
        }
        
        return result;
    }

    private static (List<int>, List<int>) Process(string input)
    {
        var initial = (new List<int>(), new List<int>());

        var lists = input
            .Split("\n")
            .Select(x => x.Split("   "))
            .Aggregate(initial, (agg, next) =>
            {
                agg.Item1.Add(int.Parse(next[0]));
                agg.Item2.Add(int.Parse(next[1]));
                return agg;
            });

        lists.Item1.Sort();
        lists.Item2.Sort();
        
        return lists;
    }
}
