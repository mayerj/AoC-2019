using System;
using System.Collections.Generic;
using System.Text;

namespace Day7
{
    public class VM
    {
        private readonly Memory _memory;
        private int _index = 0;
        private readonly Func<int> _input;
        private readonly Action<int> _output;
        private readonly bool _debug;

        public bool IsHalted { get; private set; }

        public VM(Memory memory, Func<int> input, Action<int> output, bool debug = false)
        {
            _memory = memory;
            _input = input;
            _output = output;
            _debug = debug;
        }

        public void Run()
        {
            bool run = true;
            while (run)
            {
                _memory.Set(_index, AddressType.OpCode);
                int opcode = _memory[_index];

                int parsedOpcode = GetOpcode(opcode);

                int opcodeLength;
                switch (parsedOpcode)
                {
                    case 1:
                        {
                            //add
                            Math(_index, (x, y) => x + y);
                            opcodeLength = 4;
                            break;
                        }
                    case 2:
                        {
                            //mult
                            Math(_index, (x, y) => x * y);
                            opcodeLength = 4;
                            break;
                        }
                    case 3:
                        {
                            int location = _memory[_index + 1];
                            _memory[location] = _input();
                            opcodeLength = 2;
                            break;
                        }
                    case 4:
                        {
                            GetModes(opcode, out var mode1, out _, out _);
                            int location = _memory[_index + 1];

                            var value = mode1 ? location : _memory[location];

                            WriteLine($"{_index:D4} - Opcode: {opcode:D5}: {location}, {value}");

                            _output(value);
                            opcodeLength = 2;
                            run = false;
                            break;
                        }
                    case 5:
                        //jump if true
                        {
                            int result = EvaluateIf(_index, out int address);
                            if (result != 0)
                            {
                                _index = address;
                                opcodeLength = 0;
                                break;
                            }

                            opcodeLength = 3;
                            break;
                        }
                    case 6:
                        //jump if false
                        {
                            int result = EvaluateIf(_index, out int address);
                            if (result == 0)
                            {
                                _index = address;
                                opcodeLength = 0;
                                break;
                            }

                            opcodeLength = 3;
                            break;
                        }
                    case 7:
                        //less than
                        {
                            Evaluate(_index, (x, y) => x < y);
                            opcodeLength = 4;
                            break;
                        }
                    case 8:
                        //equals
                        {
                            Evaluate(_index, (x, y) => x == y);
                            opcodeLength = 4;
                            break;
                        }
                    case 99:
                        IsHalted = true;
                        run = false;
                        opcodeLength = 1;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                _index += opcodeLength;
            }
        }

        private void Evaluate(int index, Func<int, int, bool> condition)
        {
            GetModes(_memory[index], out bool mode1, out bool mode2, out _);

            int index1 = _memory[index + 1];
            int index2 = _memory[index + 2];
            int index3 = _memory[index + 3];

            int val1 = mode1 ? index1 : _memory[index1];
            int val2 = mode2 ? index2 : _memory[index2];

            if (condition(val1, val2))
            {
                _memory[index3] = 1;
            }
            else
            {
                _memory[index3] = 0;
            }
        }

        private int EvaluateIf(int index, out int address)
        {
            GetModes(_memory[index], out bool mode1, out bool mode2, out _);

            int index1 = _memory[index + 1];
            int index2 = _memory[index + 2];

            int val = mode1 ? index1 : _memory[index1];

            address = mode2 ? index2 : _memory[index2];

            return val;
        }

        private int GetOpcode(int opcode)
        {
            return opcode % 100;
        }

        private void Math(int address, Func<int, int, int> func)
        {
            int opcode = _memory[address];

            int index1 = _memory[address + 1];
            int index2 = _memory[address + 2];
            int destIndex = _memory[address + 3];

            GetModes(opcode, out bool mode1, out bool mode2, out _);

            var v1 = mode1 ? index1 : _memory[index1];
            var v2 = mode2 ? index2 : _memory[index2];

            var result = func(v1, v2);

            WriteLine($"{address:D4} - Opcode: {opcode:D5}: {index1} ({v1}), {index2} ({v2}), {destIndex} (result: {result})");

            _memory[destIndex] = result;
        }

        private void WriteLine(FormattableString str)
        {
            if (_debug)
            {
                Console.WriteLine(str);
            }
        }

        private void GetModes(int opcode, out bool mode1, out bool mode2, out bool mode3)
        {
            string code = opcode.ToString("D5");

            mode1 = code[2] == '1';
            mode2 = code[1] == '1';
            mode3 = code[0] == '1';
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
        private readonly List<int> _memory;
        private readonly AddressType[] _memoryType;

        public Memory(List<int> data)
        {
            _memory = data;
            _memoryType = new AddressType[data.Count];
        }

        public List<int> ReadAll()
        {
            return new List<int>(_memory);
        }

        public int this[int address]
        {
            get { return _memory[address]; }
            set
            {
                _memory[address] = value;
                Set(address, AddressType.Variable);
            }
        }

        public void Set(int address, AddressType variable)
        {
            if (_memoryType[address] != variable && _memoryType[address] != AddressType.Unknown)
            {
                if (_memoryType[address] == AddressType.OpCode && variable == AddressType.Variable)
                { }
                else
                {
                    Console.WriteLine($"Opcode written to {address}");
                }
            }

            _memoryType[address] = variable;
        }
    }
}
