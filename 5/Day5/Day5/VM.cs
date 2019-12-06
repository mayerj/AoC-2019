using System;
using System.Collections.Generic;
using System.Text;

namespace Day5
{
    public class VM
    {
        private readonly Memory _memory;
        private readonly Func<int> _input;
        private readonly Action<int> _output;

        public VM(Memory memory, Func<int> input, Action<int> output)
        {
            _memory = memory;
            _input = input;
            _output = output;
        }

        public void Run()
        {
            int index = 0;

            bool run = true;
            while (run)
            {
                _memory.Set(index, AddressType.OpCode);
                int opcode = _memory[index];

                int parsedOpcode = GetOpcode(opcode);

                int opcodeLength;
                switch (parsedOpcode)
                {
                    case 1:
                        {
                            //add
                            Math(index, (x, y) => x + y);
                            opcodeLength = 4;
                            break;
                        }
                    case 2:
                        {
                            //mult
                            Math(index, (x, y) => x * y);
                            opcodeLength = 4;
                            break;
                        }
                    case 3:
                        {
                            int location = _memory[index + 1];
                            _memory[location] = _input();
                            opcodeLength = 2;
                            break;
                        }
                    case 4:
                        {
                            GetModes(opcode, out var mode1, out _, out _);
                            int location = _memory[index + 1];

                            var value = mode1 ? location : _memory[location];

                            Console.WriteLine($"{index:D4} - Opcode: {opcode:D5}: {location}, {value}");

                            _output(value);
                            opcodeLength = 2;
                            break;
                        }
                    case 99:
                        run = false;
                        opcodeLength = 1;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                index += opcodeLength;
            }
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

            Console.WriteLine($"{address:D4} - Opcode: {opcode:D5}: {index1} ({v1}), {index2} ({v2}), {destIndex} (result: {result})");

            _memory[destIndex] = result;
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
