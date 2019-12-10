using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day9
{
    public enum ReadMode
    {
        Position,
        Immediate,
        Relative,
    }

    public class VM
    {

        private readonly Memory _memory;
        private long _index = 0;
        private long _relativeBaseOffset = 0;
        private readonly Func<long> _input;
        private readonly Action<long> _output;
        private readonly bool _debug;
        private readonly Dictionary<Instructions, InstructionDescription> _instructions = GetInstructions();

        private static Dictionary<Instructions, InstructionDescription> GetInstructions()
        {
            return new Dictionary<Instructions, InstructionDescription>
            {
                { Instructions.Add, new InstructionDescription(Instructions.Add, 4) },
                { Instructions.Multiply, new InstructionDescription(Instructions.Multiply, 4) },
                { Instructions.Input, new InstructionDescription(Instructions.Input, 2) },
                { Instructions.Output, new InstructionDescription(Instructions.Output, 2) },
                { Instructions.JumpIfTrue, new InstructionDescription(Instructions.JumpIfTrue, 3) },
                { Instructions.JumpIfFalse, new InstructionDescription(Instructions.JumpIfFalse, 3) },
                { Instructions.LessThan, new InstructionDescription(Instructions.LessThan, 4) },
                { Instructions.Equals, new InstructionDescription(Instructions.Equals, 4) },
                { Instructions.SetRelativeOffset, new InstructionDescription(Instructions.SetRelativeOffset, 2) },
                { Instructions.Halt, new InstructionDescription(Instructions.Halt, 1) },
            };
        }

        public bool IsHalted { get; private set; }

        public VM(Memory memory, Func<long> input, Action<long> output, bool debug = false)
        {
            _memory = memory;
            _input = input;
            _output = output;
            _debug = debug;
        }

        public void Decompile()
        {
            int index = 0;
            while (_memory.HasValue(index))
            {
                int opcode = (int)_memory[index];

                Instructions parsedOpcode = GetOpcode(opcode);

                GetModes(_memory[index], out var mode1, out var mode2, out var mode3);

                if (_instructions.TryGetValue(parsedOpcode, out var value))
                {
                    WriteLine($"{index:X5}:\t{opcode:D5}:\t{parsedOpcode}:\t{value.Describe(_memory, index, mode1, mode2, mode3)}");

                    index += _instructions[parsedOpcode].OpcodeLength;
                }
                else { index += 1; }
            }
        }

        public void Run()
        {
            bool run = true;
            while (run)
            {
                int opcode = (int)_memory[_index];

                Instructions parsedOpcode = GetOpcode(opcode);

                GetModes(_memory[_index], out _, out _, out _);

                int opcodeLength;
                switch (parsedOpcode)
                {
                    case Instructions.Add:
                        {
                            //add
                            Math(_index, (x, y) => x + y);
                            opcodeLength = 4;
                            break;
                        }
                    case Instructions.Multiply:
                        {
                            //mult
                            Math(_index, (x, y) => x * y);
                            opcodeLength = 4;
                            break;
                        }
                    case Instructions.Input:
                        {
                            GetModes(opcode, out var mode1, out _, out _);

                            long location = _memory[_index + 1];

                            switch (mode1)
                            {
                                case ReadMode.Position:
                                    _memory[location] = _input();
                                    break;
                                case ReadMode.Relative:
                                    _memory[location + _relativeBaseOffset] = _input();
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }
                            opcodeLength = 2;
                            break;
                        }
                    case Instructions.Output:
                        {
                            GetModes(opcode, out var mode1, out _, out _);

                            var value = ReadWithMode(_memory[_index + 1], mode1);

                            WriteLine($"{_index:D4} - Opcode: {opcode:D5}: {_index + 1}, {value}");

                            _output(value);
                            opcodeLength = 2;
                            run = false;
                            break;
                        }
                    case Instructions.JumpIfTrue:
                        //jump if true
                        {
                            long result = EvaluateIf(_index, out long address);
                            if (result != 0)
                            {
                                _index = address;
                                opcodeLength = 0;
                                break;
                            }

                            opcodeLength = 3;
                            break;
                        }
                    case Instructions.JumpIfFalse:
                        //jump if false
                        {
                            long result = EvaluateIf(_index, out long address);
                            if (result == 0)
                            {
                                _index = address;
                                opcodeLength = 0;
                                break;
                            }

                            opcodeLength = 3;
                            break;
                        }
                    case Instructions.LessThan:
                        //less than
                        {
                            Evaluate(_index, (x, y) => x < y);
                            opcodeLength = 4;
                            break;
                        }
                    case Instructions.Equals:
                        //equals
                        {
                            Evaluate(_index, (x, y) => x == y);
                            opcodeLength = 4;
                            break;
                        }
                    case Instructions.SetRelativeOffset:
                        //relative base offset
                        {
                            GetModes(opcode, out var mode1, out _, out _);

                            var delta = ReadWithMode(_memory[_index + 1], mode1);

                            _relativeBaseOffset += delta;
                            opcodeLength = 2;
                            break;
                        }
                    case Instructions.Halt:
                        IsHalted = true;
                        run = false;
                        opcodeLength = 1;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                WriteLine($"{opcode:D5} {opcodeLength}");

                _index += opcodeLength;
            }
        }

        private long ReadWithMode(long address, ReadMode mode)
        {
            switch (mode)
            {
                case ReadMode.Position:
                    return _memory[address];
                case ReadMode.Immediate:
                    return address;
                case ReadMode.Relative:
                    return _memory[address + _relativeBaseOffset];
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private void Evaluate(long index, Func<long, long, bool> condition)
        {
            GetModes(_memory[index], out var mode1, out var mode2, out var mode3);

            long index1 = _memory[index + 1];
            long index2 = _memory[index + 2];
            long index3 = _memory[index + 3];

            var val1 = ReadWithMode(index1, mode1);
            var val2 = ReadWithMode(index2, mode2);

            var result = condition(val1, val2) ? 1 : 0;

            switch (mode3)
            {
                case ReadMode.Immediate:
                    throw new InvalidOperationException();
                case ReadMode.Position:
                    _memory[index3] = result;
                    break;
                case ReadMode.Relative:
                    _memory[_relativeBaseOffset + index3] = result;
                    break;
            }
        }

        private long EvaluateIf(long index, out long address)
        {
            GetModes(_memory[index], out var mode1, out var mode2, out _);

            long index1 = _memory[index + 1];
            long index2 = _memory[index + 2];

            var val = ReadWithMode(index1, mode1);
            address = ReadWithMode(index2, mode2);

            return val;
        }

        private Instructions GetOpcode(int opcode)
        {
            return (Instructions)(opcode % 100);
        }

        private void Math(long address, Func<long, long, long> func)
        {
            long opcode = _memory[address];

            long index1 = _memory[address + 1];
            long index2 = _memory[address + 2];
            long destIndex = _memory[address + 3];

            GetModes(opcode, out var mode1, out var mode2, out var mode3);
            if (mode3 != ReadMode.Position)
            {

            }
            var v1 = ReadWithMode(index1, mode1);
            var v2 = ReadWithMode(index2, mode2);

            var result = func(v1, v2);

            WriteLine($"{address:D4} - Opcode: {opcode:D5}: {index1} ({v1}), {index2} ({v2}), {destIndex} (result: {result})");

            switch (mode3)
            {
                case ReadMode.Immediate:
                    throw new InvalidOperationException();
                case ReadMode.Relative:
                    _memory[_relativeBaseOffset + destIndex] = result;
                    break;
                case ReadMode.Position:
                    _memory[destIndex] = result;
                    break;
            }
        }

        private void WriteLine(FormattableString str)
        {
            if (_debug)
            {
                Console.WriteLine(str);
            }
        }

        private void GetModes(long opcode, out ReadMode mode1, out ReadMode mode2, out ReadMode mode3)
        {
            string code = opcode.ToString("D5");

            ReadMode MapMode(char mode)
            {
                switch (mode)
                {
                    case '0':
                        return ReadMode.Position;
                    case '1':
                        return ReadMode.Immediate;
                    case '2':
                        return ReadMode.Relative;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode));
                }
            }

            mode1 = MapMode(code[2]);
            mode2 = MapMode(code[1]);
            mode3 = MapMode(code[0]);

            if (mode3 != ReadMode.Position)
            {

            }
        }
    }

    public enum Instructions
    {
        Add = 1,
        Multiply = 2,
        Input = 3,
        Output = 4,
        JumpIfTrue = 5,
        JumpIfFalse = 6,
        LessThan = 7,
        Equals = 8,
        SetRelativeOffset = 9,

        Halt = 99,
    }

    public class InstructionDescription
    {
        private readonly Instructions _instruction;
        private readonly int _opCodeLength;

        public InstructionDescription(Instructions instruction, int opCodeLength)
        {
            _instruction = instruction;
            _opCodeLength = opCodeLength;
        }

        public int OpcodeLength { get { return _opCodeLength; } }

        public string Describe(Memory memory, long addr, ReadMode mode1, ReadMode mode2, ReadMode mode3)
        {
            List<string> strings = new List<string>();
            var modes = new[] { ReadMode.Immediate, mode1, mode2, mode3 };
            for (int i = 1; i < _opCodeLength; i++)
            {
                strings.Add($"{addr + i} {memory[addr + i]} ({modes[i]})");
            }

            return string.Join(",", strings);
        }
    }

    public enum AddressType
    {
        Unknown,
        OpCode,
        Variable,
    }

    public class Memory
    {
        private readonly Dictionary<long, long> _memory;

        public Memory(List<long> data)
        {
            _memory = new Dictionary<long, long>();

            for (int i = 0; i < data.Count; i++)
            {
                _memory[i] = data[i];
            }
        }

        public long this[long address]
        {
            get
            {
                if (_memory.TryGetValue(address, out long val))
                {
                    return val;
                }

                return 0;
            }
            set
            {
                _memory[address] = value;
            }
        }

        public bool HasValue(int index)
        {
            return _memory.ContainsKey(index);
        }

        internal List<long> ReadAll()
        {
            return new List<long>(_memory.OrderBy(x => x.Key).Select(x => x.Value));
        }
    }
}
