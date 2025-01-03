﻿namespace AdventOfCode;

/// <summary>
/// --- Day 17: Chronospatial Computer ---
/// </summary>
public class Day17 : BaseDay
{
    private int _pointer;
    private long _registerA;
    private long _registerB;
    private long _registerC;
    private byte[] _program;
    private List<byte> _out;

    private readonly string[] _input;

    public Day17()
    {
        _input = File.ReadAllText(InputFilePath).Split("\n\n");
        // _input = "Register A: 729\nRegister B: 0\nRegister C: 0\n\nProgram: 0,1,5,4,3,0".Split("\n\n").ToArray();
        // _input = "Register A: 2024\nRegister B: 0\nRegister C: 0\n\nProgram: 0,3,5,4,3,0".Split("\n\n").ToArray();
    }

    public override ValueTask<string> Solve_1() => new($"{Run()}"); // Test: "4,6,3,5,6,3,5,2,1,0"

    public override ValueTask<string> Solve_2() => new($"{MatchOutputToProgram()}"); // Test: 117440

    private string Run(long a = 0L)
    {
        Initialize(a);
        while (0 <= _pointer && _pointer < _program.Length - 1)
        {
            var (opcode, operand) = Read();
            Execute(opcode, operand);
        }

        return string.Join(",", _out);
    }

    private void Initialize(long a = 0)
    {
        foreach (var register in _input[0].Split('\n'))
        {
            var n = long.Parse(string.Join("", register.Where(char.IsDigit)));
            switch (register[9])
            {
                case 'A': _registerA = a == 0 ? n : a; break;
                case 'B': _registerB = n; break;
                case 'C': _registerC = n; break;
            }
        }

        _program = _input[1][9..].Split(',').Select(byte.Parse).ToArray();
        _pointer = 0;
        _out = [];
    }

    private (byte opcode, byte operand) Read() => (_program[_pointer], _program[_pointer + 1]);

    private void Execute(byte opcode, byte operand)
    {
        var increasePointer = true;
        switch (opcode)
        {
            case 0: _registerA = (long)(_registerA / Math.Pow(2, Combo(operand))); break;
            case 1: _registerB ^= operand; break;
            case 2: _registerB = (long)(Combo(operand) % 8); break;
            case 3:
                if (_registerA != 0)
                {
                    _pointer = operand;
                    increasePointer = false;
                }

                break;
            case 4: _registerB ^= _registerC; break;
            case 5: _out.Add((byte)(Combo(operand) % 8)); break;
            case 6: _registerB = (long)(_registerA / Math.Pow(2, Combo(operand))); break;
            case 7: _registerC = (long)(_registerA / Math.Pow(2, Combo(operand))); break;
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
                _ => throw new ApplicationException($"Unknown operand: {operand}.")
            };
        }
    }

    private long MatchOutputToProgram()
    {
        var a = 1L;
        var digitsToMatch = 1;
        var iterations = 1;
        while (a < long.MaxValue && digitsToMatch <= _program.Length)
        {
            iterations++;
            Run(a);

            if (_out.Count == _program.Length && _out.SequenceEqual(_program))
            {
                // Console.WriteLine($"{a} => {string.Join(",", _out)}. Solved in {iterations} iterations.");
                return a;
            }

            if (_out.SequenceEqual(_program.TakeLast(digitsToMatch)))
            {
                // Console.WriteLine($"{a} => {string.Join(",", _out)}. Jumping to a = {a * 8}");
                a *= 8;
                digitsToMatch += 1;
                continue;
            }

            a++;
        }

        return 0L;
    }
}