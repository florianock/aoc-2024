namespace AdventOfCode;

/// <summary>
/// --- Day 21: Keypad Conundrum ---
/// </summary>
public sealed class Day21 : BaseDay
{
    private readonly IEnumerable<string> _input;

    private readonly Dictionary<char, (int, int)> _numericKeypad = new()
    {
        { 'A', (3, 2) },
        { '0', (3, 1) },
        { '1', (2, 0) },
        { '2', (2, 1) },
        { '3', (2, 2) },
        { '4', (1, 0) },
        { '5', (1, 1) },
        { '6', (1, 2) },
        { '7', (0, 0) },
        { '8', (0, 1) },
        { '9', (0, 2) },
        { 'X', (3, 0) }
    };

    private readonly Dictionary<char, (int, int)> _directionalKeypad = new()
    {
        { 'A', (0, 2) },
        { '^', (0, 1) },
        { '<', (1, 0) },
        { 'v', (1, 1) },
        { '>', (1, 2) },
        { 'X', (0, 0) }
    };

    public Day21()
    {
        // _input = File.ReadLines(InputFilePath); // 227666 too high
        _input = "029A\n980A\n179A\n456A\n379A".Split("\n");
    }

    public override ValueTask<string> Solve_1() => new($"{DoStuff()}"); // Test: 126384

    public override ValueTask<string> Solve_2() => new($"{DoStuff(true)}"); // Test: 

    private int DoStuff(bool part2 = false)
    {
        if (part2) return 0;
        return _input.Aggregate(0, (acc, code) => acc + Complexity(code, GetButtonPresses(code)));
    }

    private int GetButtonPresses(string code)
    {
        var current = code;
        List<Dictionary<char, (int, int)>> keypads = [_numericKeypad, _directionalKeypad, _directionalKeypad];
        foreach (var result in keypads.Select(keypad => current
                     .Select((c, i) => Move(i > 0 ? current[i - 1] : 'A', c, keypad))
                     .Aggregate("", (current1, m) => current1 + m)))
        {
            current = result;
        }

        Console.WriteLine($"{code}: {current}");
        return current.Length;
    }

    private static string Move(char start, char end, Dictionary<char, (int, int)> keypad)
    {
        // Console.WriteLine($"Inc from {start} to {end}");
        var result = Inc(keypad[start], keypad[end]);

        result += 'A';
        return result;

        string Inc((int, int) a, (int, int) b)
        {
            var avoid = keypad['X'];
            if (a == avoid)
                throw new ArgumentException($"Panic! Cannot go through {a} -> {b} when going from {start} to {end}.");
            if (a == b) return "";
            var rDiff = b.Item1 - a.Item1;
            var cDiff = b.Item2 - a.Item2;
            // Console.WriteLine($"row diff: {rDiff}; col diff: {cDiff}");
            var movingLeft = cDiff < 0;
            var (a1, a2) = a;
            var nextChar = "";
            var nextA = avoid;
            if (!movingLeft || cDiff == -1)
            {
                if (rDiff > 0)
                {
                    nextChar = "v";
                    nextA = (a1 + 1, a2);
                }
                else if (rDiff < 0)
                {
                    nextChar = "^";
                    nextA = (a1 - 1, a2);
                }
                else if (cDiff > 0)
                {
                    nextChar = ">";
                    nextA = (a1, a2 + 1);
                }
                else if (cDiff < 0)
                {
                    nextChar = "<";
                    nextA = (a1, a2 - 1);
                }
            }
            else
            {
                if (cDiff > 0)
                {
                    nextChar = ">";
                    nextA = (a1, a2 + 1);
                }
                else if (cDiff < 0)
                {
                    nextChar = "<";
                    nextA = (a1, a2 - 1);
                }
                else if (rDiff > 0)
                {
                    nextChar = "v";
                    nextA = (a1 + 1, a2);
                }
                else if (rDiff < 0)
                {
                    nextChar = "^";
                    nextA = (a1 - 1, a2);
                }
            }

            if (nextA == avoid)
            {
                if (avoid == (0, 0))
                {
                    if (movingLeft)
                    {
                        nextChar = "v";
                        nextA = (a1 + 1, a2);
                    }
                    else
                    {
                        nextChar = ">";
                        nextA = (a1, a2 + 1);
                    }
                }
                else // avoid == (3,0)
                {
                    if (movingLeft)
                    {
                        nextChar = "^";
                        nextA = (a1 - 1, a2);
                    }
                    else
                    {
                        nextChar = ">";
                        nextA = (a1, a2 + 1);
                    }
                }
            }

            return nextChar + Inc(nextA, b);
            // rDiff != 0 && cDiff != 0
            // should we move first over rows or columns? Or interchanging?
            // if (cDiff > 0) return ">" + Inc((a1, a2 + 1), b);
            // if (cDiff < 0) return "<" + Inc((a1, a2 - 1), b);
            // if (cDiff < 0 && rDiff == 0) // danger; avoid early
            // {
            // return "<" + Inc((a1, a2 - 1), b);
            // }
            // if (cDiff < 0 && !(avoid.Item1 == a1 && a2 - avoid.Item2 == 1)) // danger; avoid late
            // {
            // return "<" + Inc((a1, a2 - 1), b);
            // }
            return "";
        }
    }

    private static int Complexity(string code, int buttonPresses)
    {
        var numericPart = int.Parse(string.Join("", code.Where(char.IsDigit)));
        Console.WriteLine($"{code}: {buttonPresses} * {numericPart}");
        return buttonPresses * numericPart;
    }
}