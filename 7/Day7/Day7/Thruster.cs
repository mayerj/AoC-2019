using System;
using System.Collections;
using System.Linq;

namespace Day7
{
    public class Thruster
    {
        private readonly string _input;
        private readonly int _phaseSetting;

        public Thruster(string input, int phaseSetting)
        {
            _input = input;
            _phaseSetting = phaseSetting;
        }

        public int Run(int input)
        {
            IEnumerator enumerator = new[] { _phaseSetting, input }.GetEnumerator();

            Memory memory = new Memory(_input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList());

            int result = 0;
            VM vm = new VM(memory, () => { enumerator.MoveNext(); return (int)enumerator.Current; }, x => result = x);
            vm.Run();

            return result;
        }
    }
}