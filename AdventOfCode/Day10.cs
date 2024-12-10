namespace AdventOfCode;

/// <summary>
/// --- Day 10: Hoof It ---
/// </summary>
public class Day10 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), List<(int, int)>> _topsReached;

    public Day10()
    {
        // _input = "89010123\n78121874\n87430965\n96549874\n45678903\n32019012\n01329801\n10456732".Split('\n').ToList();
        _input = File.ReadLines(InputFilePath).ToList();
        _topsReached = new Dictionary<(int, int), List<(int, int)>>();
        for (var i = 0; i < _input.Count; i++)
        {
            for (var j = 0; j < _input[0].Length; j++)
            {
                if (_input[i][j] == '0') _topsReached.Add((i, j), GetTops((i, j)));
            }
        }
    }

    public override ValueTask<string> Solve_1() =>
        new($"{_topsReached.Sum(kv => kv.Value.Distinct().Count())}"); // Test: 36

    public override ValueTask<string> Solve_2() => new($"{_topsReached.Sum(kv => kv.Value.Count)}"); // Test: 81

    private List<(int, int)> GetTops((int, int) trailHead)
    {
        List<(int, int)> topsReached = [];
        var todo = new Queue<(int, int)>();
        todo.Enqueue(trailHead);
        HashSet<(int, int)> visited = [];
        while (todo.Count > 0)
        {
            var (r, c) = todo.Dequeue();
            visited.Add((r, c));
            if (_input[r][c] == '9')
            {
                topsReached.Add((r, c));
                continue;
            }

            List<(int, int)> nearestTrail = [(r - 1, c), (r, c + 1), (r + 1, c), (r, c - 1)];
            nearestTrail = nearestTrail.Where(p =>
                0 <= p.Item1 && p.Item1 < _input.Count &&
                0 <= p.Item2 && p.Item2 < _input[0].Length &&
                _input[p.Item1][p.Item2] - _input[r][c] == 1).ToList();

            foreach (var nextStep in nearestTrail.Where(next => !visited.Contains(next)))
            {
                todo.Enqueue(nextStep);
            }
        }

        return topsReached;
    }
}