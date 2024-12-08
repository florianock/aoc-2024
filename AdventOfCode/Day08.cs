namespace AdventOfCode;

/// <summary>
/// --- Day 8: Resonant Collinearity ---
/// </summary>
public class Day08 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<char, List<(int, int)>> _antennas;

    public Day08()
    {
        _input = File.ReadLines(InputFilePath).ToList();
        // _input =
        // "............\n........0...\n.....0......\n.......0....\n....0.......\n......A.....\n............\n............\n........A...\n.........A..\n............\n............"
        // .Split("\n").ToList();
        // _input =
        // "T.........\n...T......\n.T........\n..........\n..........\n..........\n..........\n..........\n..........\n.........."
        // .Split("\n").ToList();

        _antennas = new Dictionary<char, List<(int, int)>>();
        for (var i = 0; i < _input.Count; i++)
        {
            for (var j = 0; j < _input[0].Length; j++)
            {
                var nextChar = _input[i][j];
                if (nextChar == '.') continue;
                if (_antennas.TryGetValue(nextChar, out var position)) position.Add((i, j));
                else _antennas.Add(nextChar, [(i, j)]);
            }
        }
    }

    public override ValueTask<string> Solve_1() => new($"{GetAntiNodes(false).Count}"); // Test: 14, 3

    public override ValueTask<string> Solve_2() => new($"{GetAntiNodes().Count}"); // Test: 34, 9

    private HashSet<(int, int)> GetAntiNodes(bool withResonantHarmonics = true)
    {
        var antiNodes = new HashSet<(int, int)>();
        foreach (var (ch, antennas) in _antennas)
        {
            for (var i = 0; i < antennas.Count - 1; i++)
            {
                for (var j = i + 1; j < antennas.Count; j++)
                {
                    if (withResonantHarmonics && _antennas[ch].Count > 2)
                    {
                        antiNodes.Add(antennas[i]);
                        antiNodes.Add(antennas[j]);
                    }

                    var rowDiff = antennas[i].Item1 - antennas[j].Item1;
                    var colDiff = antennas[i].Item2 - antennas[j].Item2;

                    antiNodes.UnionWith(
                        FindAntiNodesForAntenna(antennas[i], rowDiff, colDiff, withResonantHarmonics));

                    antiNodes.UnionWith(
                        FindAntiNodesForAntenna(antennas[j], -1 * rowDiff, -1 * colDiff, withResonantHarmonics));
                }
            }
        }

        return antiNodes;
    }

    private HashSet<(int, int)> FindAntiNodesForAntenna((int, int) antenna1, int rowDiff, int colDiff,
        bool withResonantHarmonics)
    {
        var maxRow = _input.Count - 1;
        var maxCol = _input[0].Length - 1;
        var antiNodes = new HashSet<(int, int)>();
        var multiplier = 1;
        do
        {
            var antiNode = (antenna1.Item1 + rowDiff * multiplier, antenna1.Item2 + colDiff * multiplier);
            if (0 <= antiNode.Item1 && antiNode.Item1 <= maxRow && 0 <= antiNode.Item2 && antiNode.Item2 <= maxCol)
            {
                antiNodes.Add(antiNode);
                if (!withResonantHarmonics) break;
                multiplier++;
            }
            else break;
        } while (true);

        return antiNodes;
    }
}