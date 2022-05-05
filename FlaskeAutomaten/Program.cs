using System;
using System.Threading;

namespace FlaskeAutomaten
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SystemControl control = new SystemControl();
            control.Init();


            Console.ReadKey();
        }
    }
}
