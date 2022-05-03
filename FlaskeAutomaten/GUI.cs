using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FlaskeAutomaten
{
    public class GUI
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        // Ascii

        private string[] bottleProducerAscii;
        private string[] producerBufferAscii;
        private string[] splitArrayAscii;
        private string[] customerBeerBufferAscii;
        private string[] customerSodaBufferAscii;

        // rest
        private SystemControl systemControl;
        private string[] bottlesBufferContents;

        public void InitializeGUI(SystemControl control)
        {
            systemControl = control;
            Console.SetWindowSize(100, 25);
            Console.SetBufferSize(100, 25);
            Console.CursorVisible = false;
            LockConsole();

            bottleProducerAscii = new string[]
            {
                $" ______________",
                $"|    BOTTLE    |",
                $"|   PRODUCER   |",
                $"|______________|",
            };

            producerBufferAscii = new string[]
            {
                $" ______________",
                $"|   PRODUCER   |",
                $"|    BUFFER    |",
                $"|______________|",
            };

            customerBeerBufferAscii = new string[]
            {
                $" ______________",
                $"|     BEER     |",
                $"|    BUFFER    |",
                $"|______________|",
            };

            customerSodaBufferAscii = new string[]
           {
                $" ______________",
                $"|     SODA     |",
                $"|    BUFFER    |",
                $"|______________|",
           };

            splitArrayAscii = new string[]
            {
                @"  /",
                @" /",
                @"/",
                @"",
                @"\",
                @" \",
                @"  \",
            };

            ConsoleTools.PrintArray(bottleProducerAscii, 5, 12, null, ConsoleColor.White);
            Console.SetCursorPosition(21, 14);
            Console.WriteLine("----");

            ConsoleTools.PrintArray(producerBufferAscii, 25, 12, null, ConsoleColor.White);

            Console.SetCursorPosition(41, 14);
            Console.WriteLine("--- SPLITTER ---");

            ConsoleTools.PrintArray(splitArrayAscii, 57, 11, null, ConsoleColor.White);
            ConsoleTools.PrintArray(customerSodaBufferAscii, 60, 14, null, ConsoleColor.White);
            ConsoleTools.PrintArray(customerBeerBufferAscii, 60, 8, null, ConsoleColor.White);

            Thread guiThread = new Thread(new ThreadStart(MainGUILoop));
            guiThread.Start();

        }

        private void MainGUILoop()
        {
            while (true)
            {
                // Producer

                if (systemControl.IsProducerPaused())
                {
                    Console.SetCursorPosition(9, 11);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("P");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.SetCursorPosition(9, 11);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  "); // clear
                }

                bottlesBufferContents = systemControl.GetBottleBufferList();

                for (int i = 0; i < bottlesBufferContents.Length; i++)
                {
                    Console.SetCursorPosition(25, 11 - i);
                    Console.WriteLine(bottlesBufferContents[i]);
                }

                // Splitter count
                int splitterCount = systemControl.GetSplitterCount();

                Console.SetCursorPosition(48, 13);
                Console.ForegroundColor = ConsoleColor.Yellow;
                string s = splitterCount.ToString();
                Console.Write(splitterCount.ToString() + "  ");
                Console.ForegroundColor = ConsoleColor.White;

                // Beer count
                int beerCount = systemControl.GetBeerBufferCount();

                Console.SetCursorPosition(67, 7);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(beerCount.ToString() + "  ");
                Console.ForegroundColor = ConsoleColor.White;

                // Soda count
                int sodaCount = systemControl.GetBeerBufferCount();

                Console.SetCursorPosition(67, 18);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(sodaCount.ToString() + "  ");
                Console.ForegroundColor = ConsoleColor.White;

                // Splitter pause

                if (systemControl.IsSplitterPaused())
                {
                    Console.SetCursorPosition(62, 15);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("P");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.SetCursorPosition(62, 15);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("  "); // clear
                }

                Thread.Sleep(100);
            }


        }


        /// <summary>
        /// Makes it so you cannot resize or maximize it
        /// </summary>
        public void LockConsole()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
        }
    }
}
