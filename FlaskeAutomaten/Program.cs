using System;

namespace FlaskeAutomaten
{
    internal class Program
    {
        static void Main(string[] args)
        {

            GUI gUI = new GUI();
            SystemControl control = new SystemControl();
            control.Init(gUI);

            Console.ReadKey();
        }
    }
}
