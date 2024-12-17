namespace AdventOfCode;

/// <summary>
/// --- Day 17: Chronospatial Computer ---
/// </summary>
public class Day17 : BaseDay
{
    private int _pointer;
    private int _registerA;
    private int _registerB;
    private int _registerC;
    private byte[] _program;
    private List<int> _out;

    private readonly string[] _input;

    public Day17()
    {
        _input = File.ReadAllText(InputFilePath).Split("\n\n");
        // _input = "Register A: 729\nRegister B: 0\nRegister C: 0\n\nProgram: 0,1,5,4,3,0".Split("\n\n").ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{Run()}"); // Test: "4,6,3,5,6,3,5,2,1,0"

    public override ValueTask<string> Solve_2() => new($"{SecondPart()}"); // Test: 

    private string Run()
    {
        Initialize();
        while (0 <= _pointer && _pointer < _program.Length)
        {
            var (instruction, operand) = Read();
            Execute(instruction, operand);
        }

        return string.Join(",", _out);
    }

    private (byte instruction, byte operand) Read() => (_program[_pointer], _program[_pointer + 1]);

    private void Execute(byte opcode, byte operand)
    {
        var increasePointer = true;
        switch (opcode)
        {
            case 0: // adv
                _registerA = (int)Math.Floor(_registerA / Math.Pow(2, Combo(operand)));
                Console.WriteLine("adv");
                break;
            case 1: // bxl*
                _registerB ^= operand;
                Console.WriteLine("bxl");
                break;
            case 2: // bst
                _registerB = (int)Math.Floor(Combo(operand) % 8);
                Console.WriteLine("bst");
                break;
            case 3: // jnz
                if (_registerA != 0)
                {
                    _pointer = operand;
                    increasePointer = false;
                }
                Console.WriteLine("jnz");
                break;
            case 4: // bxc*
                _registerB ^= _registerC;
                Console.WriteLine("bxc");
                break;
            case 5: // out
                _out.Add((int)Math.Floor(Combo(operand) % 8));
                Console.WriteLine("out");
                break;
            case 6: // bdv
                _registerB = (int)Math.Floor(_registerA / Math.Pow(2, Combo(operand)));
                Console.WriteLine("bdv");
                break;
            case 7: // cdv
                _registerC = (int)Math.Floor(_registerA / Math.Pow(2, Combo(operand)));
                Console.WriteLine("cdv");
                break;
        }

        if (increasePointer) _pointer += 2;
        return;

        double Combo(byte b)
        {
            return b switch
            {
                0 or 1 or 2 or 3 => b,
                4 => _registerA,
                5 => _registerB,
                6 => _registerC,
                7 => throw new ApplicationException("Combo operand 7 is reserved and invalid."),
                _ => throw new ApplicationException($"Unknown operand: {operand}")
            };
        }
    }

    private void Initialize()
    {
        foreach (var register in _input[0].Split('\n'))
        {
            var n = int.Parse(string.Join("", register.Where(char.IsDigit)));
            switch (register[9])
            {
                case 'A': _registerA = n; break;
                case 'B': _registerB = n; break;
                case 'C': _registerC = n; break;
            }
        }

        _program = _input[1][9..].Split(',').Select(byte.Parse).ToArray();
        _pointer = 0;
        _out = [];
    }

    private string SecondPart()
    {
        return string.Empty;
    }
}