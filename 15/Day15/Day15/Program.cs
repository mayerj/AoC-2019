using System;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            RepairBot bot = new RepairBot();

            while (true)
            {
                bot.Run();
            }
        }
    }
}
