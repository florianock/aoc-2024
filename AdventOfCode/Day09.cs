namespace AdventOfCode;

/// <summary>
/// --- Day 9: Disk Fragmenter ---
/// </summary>
public class Day09 : BaseDay
{
    private readonly string _input;

    public Day09()
    {
        // _input = File.ReadAllText(InputFilePath).Trim();
        _input = "2333133121414131402".Trim();
    }

    public override ValueTask<string> Solve_1() =>
        new($"{Compress(_input, false).Select((n, idx) => (long)n * idx).Sum()}"); // Test: 1928

    public override ValueTask<string> Solve_2() =>
        new($"{Compress(_input).Select((n, idx) => n < 0 ? 0 : (long)n * idx).Sum()}"); // Test: 2858

    private static int[] Compress(string input, bool preventFragmentation = true)
    {
        var diskMap = input.Select(c => int.Parse(c.ToString())).ToArray();
        var expanded = Expand(diskMap);
        var lastItemIndex = expanded.Length - 1;
        var data = diskMap.Index().Where(c => c.Item1 % 2 == 0).ToArray();

        if (!preventFragmentation)
        {
            for (var i = 0; i < expanded.Length; i++)
            {
                // Print(expanded);
                if (expanded[i] != -1) continue;
                while (expanded[lastItemIndex] == -1)
                {
                    lastItemIndex--;
                }

                if (lastItemIndex <= i) break;
                (expanded[i], expanded[lastItemIndex]) = (expanded[lastItemIndex], expanded[i]);
            }

            return expanded.Where(i => i > -1).ToArray();
        }

        foreach (var (idx, fileSize) in data.Reverse())
        {
            // Print(expanded);
            var id = idx / 2;
            var freeSpaceIdx = -1;
            for (var i = 0; i < expanded.Length - fileSize - 1; i++)
            {
                List<int> chunk = [];
                for (var s = 0; s < fileSize; s++)
                {
                    chunk.Add(expanded[i + s]);
                }

                if (chunk.Any(c => c == id)) break;
                if (chunk.Any(c => c != -1)) continue;
                freeSpaceIdx = i;
                break;
            }

            if (freeSpaceIdx < 0) continue;
            for (var k = 0; k < fileSize; k++)
            {
                expanded[freeSpaceIdx + k] = id;
            }

            expanded = expanded.Select((n, index) => index >= freeSpaceIdx + fileSize && n == id ? -1 : n)
                .ToArray();
        }

        return expanded.ToArray();
    }

    private static int[] Expand(int[] diskMap)
    {
        var id = 0;
        var result = new List<int>();
        for (var i = 0; i < diskMap.Length; i++)
        {
            var times = diskMap[i];
            var n = i % 2 == 0 ? id++ : -1;
            for (var j = 0; j < times; j++)
            {
                result.Add(n);
            }
        }

        return result.ToArray();
    }

    private static void Print(int[] diskMap)
    {
        foreach (var i in diskMap) Console.Write(i > -1 ? i.ToString() : ".");
        Console.Write("\n");
    }
}