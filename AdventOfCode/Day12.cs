using AdventOfCode.Utils;

namespace AdventOfCode;

/// <summary>
/// --- Day 12: Garden Groups ---
/// </summary>
public sealed class Day12 : BaseDay
{
    private readonly List<string> _input;
    private readonly List<HashSet<(int, int)>> _regions;

    public Day12()
    {
        // _input = "AAAA\nBBCD\nBBCC\nEEEC".Split('\n').ToList();
        // _input = "OOOOO\nOXOXO\nOOOOO\nOXOXO\nOOOOO".Split('\n').ToList();
        // _input = "RRRRIICCFF\nRRRRIICCCF\nVVRRRCCFFF\nVVRCCCJFFF\nVVVVCJJCFE\nVVIVCCJJEE\nVVIIICJJEE\nMIIIIIJJEE\nMIIISIJEEE\nMMMISSJEEE".Split('\n').ToList();
        // _input = "EEEEE\nEXXXX\nEEEEE\nEXXXX\nEEEEE".Split('\n').ToList();
        // _input = "AAAAAA\nAAABBA\nAAABBA\nABBAAA\nABBAAA\nAAAAAA".Split('\n').ToList();
        _input = File.ReadLines(InputFilePath).ToList();

        var visited = new HashSet<(int, int)>();
        _regions = [];
        for (var r = 0; r < _input.Count; r++)
        {
            for (var c = 0; c < _input[0].Length; c++)
            {
                if (visited.Contains((r, c))) continue;
                var region = FindRegion((r, c));
                visited.UnionWith(region);
                _regions.Add(region);
            }
        }
    }

    public override ValueTask<string> Solve_1() =>
        new($"{_regions.Sum(r => GetPrice(r))}"); // Test: 140, 772, 1930, 692, 1184

    public override ValueTask<string> Solve_2() =>
        new($"{_regions.Sum(r => GetPrice(r, true))}"); // Test: 80, 436, 1206, 236, 368

    private int GetPrice(HashSet<(int, int)> region, bool bulkDiscount = false)
    {
        var area = region.Count;
        var perimeter = bulkDiscount ? CountCorners(region) : CountBorders(region);
        // Console.WriteLine($"Plant {_input[region.First().Item1][region.First().Item2]} (p x a): {perimeter} * {area} = {area * perimeter}");
        return area * perimeter;
    }

    private int CountBorders(HashSet<(int, int)> region)
    {
        var borders = 0;
        foreach (var (r, c) in region)
        {
            var neighborsInGrid = Grid.GetNeighbors(r, c).Where(p =>
                0 <= p.Item1 && p.Item1 < _input.Count && 0 <= p.Item2 && p.Item2 < _input[0].Length).ToList();
            var gridEdges = 4 - neighborsInGrid.Count;
            var plotsOfOtherRegions = neighborsInGrid.Count(p => !region.Contains(p));
            borders += gridEdges + plotsOfOtherRegions;
        }

        return borders;
    }

    private static int CountCorners(HashSet<(int, int)> region)
    {
        var corners = 0;
        foreach (var (r, c) in region)
        {
            var neighbors = Grid.GetNeighbors(r, c); // North, East, South, West

            // NW
            if (region.Contains(neighbors[0]) && region.Contains(neighbors[3]) && !region.Contains((r - 1, c - 1)))
                corners++;
            else if (!region.Contains(neighbors[0]) && !region.Contains(neighbors[3]))
                corners++;

            // NE
            if (region.Contains(neighbors[0]) && region.Contains(neighbors[1]) && !region.Contains((r - 1, c + 1)))
                corners++;
            else if (!region.Contains(neighbors[0]) && !region.Contains(neighbors[1]))
                corners++;

            // SE
            if (region.Contains(neighbors[2]) && region.Contains(neighbors[1]) && !region.Contains((r + 1, c + 1)))
                corners++;
            else if (!region.Contains(neighbors[2]) && !region.Contains(neighbors[1]))
                corners++;

            // SW
            if (region.Contains(neighbors[2]) && region.Contains(neighbors[3]) && !region.Contains((r + 1, c - 1)))
                corners++;
            else if (!region.Contains(neighbors[2]) && !region.Contains(neighbors[3]))
                corners++;
        }

        return corners;
    }

    private HashSet<(int, int)> FindRegion((int, int) plot)
    {
        var plant = _input[plot.Item1][plot.Item2];
        var visited = new HashSet<(int, int)>();
        var todo = new Queue<(int, int)>();
        todo.Enqueue(plot);
        while (todo.Count > 0)
        {
            var current = todo.Dequeue();
            if (!visited.Add(current)) continue;
            var (r, c) = current;
            var neighbors = Grid.GetNeighbors(r, c).Where(p =>
                0 <= p.Item1 && p.Item1 < _input.Count && 0 <= p.Item2 && p.Item2 < _input[0].Length &&
                !visited.Contains(p) && _input[p.Item1][p.Item2] == plant).ToList();
            foreach (var n in neighbors) todo.Enqueue(n);
        }

        return visited;
    }
}