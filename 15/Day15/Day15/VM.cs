using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day15
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
        private Instructions? _break = null;
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

        public void Run(bool step = false)
        {
            bool run = true;
            while (run)
            {
                run = !step;

                int opcode = (int)_memory[_index];

                Instructions parsedOpcode = GetOpcode(opcode);

                if(_break != null && parsedOpcode == _break.Value)
                {
                    return;
                }

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

                            if (_debug)
                            {
                                WriteLine($"{_index:D4} - Opcode: {opcode:D5} ({parsedOpcode}): {_index + 1}, {value}");
                            }

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

                //if (_debug)
                //{
                //    WriteLine($"{opcode:D5} {opcodeLength}");
                //}

                _index += opcodeLength;
            }
        }

        public void Step()
        {
            Run(true);
        }

        public void RunUntil(Instructions input)
        {
            _break = input;
            Run();
            _break = null;
        }

        public string GetState()
        {
            return _relativeBaseOffset.ToString();
        }

        private long ReadWithMode(long address, ReadMode mode)
        {
            return mode switch
            {
                ReadMode.Position => _memory[address],
                ReadMode.Immediate => address,
                ReadMode.Relative => _memory[address + _relativeBaseOffset],
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };
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

        static readonly Instructions?[] _fastOpcodeCache = new Instructions?[100000];

        private Instructions GetOpcode(int opcode)
        {
            if(_fastOpcodeCache[opcode] != null)
            {
                return _fastOpcodeCache[opcode].Value;
            }

            return (_fastOpcodeCache[opcode] = GetOpcodeSlow(opcode)).Value;
        }
        private Instructions GetOpcodeSlow(int opcode)
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

            if (_debug)
            {
                WriteLine($"{address:D4} - Opcode: {opcode:D5}: {index1} ({v1}), {index2} ({v2}), {destIndex} (result: {result})");
            }

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

        static readonly Dictionary<long, (ReadMode mode1, ReadMode mode2, ReadMode mode3)> _cache = new Dictionary<long, (ReadMode mode1, ReadMode mode2, ReadMode mode3)>();

        static readonly (ReadMode mode1, ReadMode mode2, ReadMode mode3)?[] _fastCache = new (ReadMode mode1, ReadMode mode2, ReadMode mode3)?[100000];
        private void GetModes(long opcode, out ReadMode mode1, out ReadMode mode2, out ReadMode mode3)
        {
            //if (_cache.TryGetValue(opcode, out var result))
            //{
            //    mode1 = result.mode1;
            //    mode2 = result.mode2;
            //    mode3 = result.mode3;
            //    return;
            //}

            if (_fastCache[opcode] != null)
            {
                (mode1, mode2, mode3) = _fastCache[opcode].Value;
                return;
            }

            string code = opcode.ToString("D5");

            static ReadMode MapMode(char mode)
            {
                return mode switch
                {
                    '0' => ReadMode.Position,
                    '1' => ReadMode.Immediate,
                    '2' => ReadMode.Relative,
                    _ => throw new ArgumentOutOfRangeException(nameof(mode)),
                };
            }

            mode1 = MapMode(code[2]);
            mode2 = MapMode(code[1]);
            mode3 = MapMode(code[0]);

            _cache[opcode] = (mode1, mode2, mode3);
            _fastCache[opcode] = (mode1, mode2, mode3);
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
        private long[] _flat;

        public Memory(List<long> data)
        {
            _flat = data.ToArray();
        }

        public long this[long address]
        {
            get
            {
                if (_flat.Length > (int)address)
                {
                    return _flat[(int)address];
                }

                Array.Resize(ref _flat, ((int)address) + 1);

                return this[address];
            }
            set
            {
                if (_flat.Length > (int)address)
                {
                    _flat[(int)address] = value;
                    return;
                }

                Array.Resize(ref _flat, ((int)address) + 1);

                this[address] = value;
            }
        }

        public bool HasValue(int index)
        {
            return _flat.Length > (int)index;
        }
    }
}
