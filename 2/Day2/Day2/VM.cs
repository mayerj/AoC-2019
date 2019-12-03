using System;
using System.Collections.Generic;
using System.Text;

namespace Day2
{
	public class VM
	{
		private readonly Memory _memory;

		public VM(Memory memory)
		{
			_memory = memory;
		}

		public void Run()
		{
			int index = 0;

			bool run = true;
			while (run)
			{
				_memory.Set(index, AddressType.OpCode);
				int opcode = _memory[index];

				switch (opcode)
				{
					case 1:
						{
							//add
							Math(index, (x, y) => x + y);
							break;
						}
					case 2:
						{
							//mult
							Math(index, (x, y) => x * y);
							break;
						}
					case 99:
						run = false;
						break;
					default:
						throw new InvalidOperationException();
				}

				index += 4;
			}
		}

		private void Math(int address, Func<int, int, int> func)
		{
			int index1 = _memory[address + 1];
			int index2 = _memory[address + 2];
			int destIndex = _memory[address + 3];

			_memory[destIndex] = func(_memory[index1], _memory[index2]);
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
