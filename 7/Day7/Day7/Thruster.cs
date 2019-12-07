using System;
using System.Collections;
using System.Linq;

namespace Day7
{
    public class Thruster
    {
        private readonly string _program;
        private readonly int _phaseSetting;
        private readonly VM _vm;
        private int _input;
        private int _output;

        public Thruster(string program, int phaseSetting)
        {
            _program = program;
            _phaseSetting = phaseSetting;

            bool hasConfigured = false;
            int GetNext()
            {
                if (hasConfigured)
                {
                    return _input;
                }
                else
                {
                    hasConfigured = true;
                    return _phaseSetting;
                }
            }

            Memory memory = new Memory(program.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList());

            _vm = new VM(memory, GetNext, x => _output = x);
        }

        public bool IsHalted => _vm.IsHalted;

        public int Run(int input)
        {
            _input = input;

            _vm.Run();

            return _output;
        }
    }
}