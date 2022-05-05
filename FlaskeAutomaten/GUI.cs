using System;
using System.Collections.Generic;
using System.Drawing;
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
        private Bottle[] producerBufferContents;

        private Queue<GUIMessage> messageQueue = new Queue<GUIMessage>();

        public void InitializeGUI(SystemControl control)
        {
            systemControl = control;
            Console.SetWindowSize(125, 25);
            Console.SetBufferSize(125, 25);
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
            guiThread.Join();

        }

        private void MainGUILoop()
        {
            while (true)
            {
                //
                // Producer
                //

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

                //
                // Producer buffer contents
                //

                for (int u = 0; u < 12; u++)
                {
                    Console.SetCursorPosition(30, 11 - u);
                    Console.Write("        "); // Clear
                }

                producerBufferContents = systemControl.GetProducerBufferList();

                int y = 0;

                for (int j = 0; j < producerBufferContents.Length; j++)
                {
                    if (producerBufferContents[j] != null)
                    {
                        Console.SetCursorPosition(30, 11 - y);
                        Console.Write("        "); // Clear
                        Console.SetCursorPosition(30, 11 - y);
                        Console.Write(producerBufferContents[j].ToString());
                        y++;
                    }
                }

                //
                // Beer count
                //

                int beerCount = systemControl.GetBeerBufferCount();

                Console.SetCursorPosition(67, 7);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(beerCount.ToString() + "  ");
                Console.ForegroundColor = ConsoleColor.White;

                //
                // Soda count
                //

                int sodaCount = systemControl.GetSodaBufferCount();

                Console.SetCursorPosition(67, 18);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(sodaCount.ToString() + "  ");
                Console.ForegroundColor = ConsoleColor.White;

                //
                // Splitter pause
                //

                if (systemControl.IsSplitterPaused())
                {
                    Console.SetCursorPosition(49, 13);
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

                //
                // Print message queue
                //

                while(messageQueue.TryDequeue(out GUIMessage msg))
                {
                    if(msg != null)
                    {
                        Console.SetCursorPosition(msg.Location.X, msg.Location.Y);
                        Console.Write(msg.Message);
                    }
                }

                Thread.Sleep(100);
            }


        }


        public void PrintConsumerMessage(Bottletype type, string msg)
        {
            msg = msg + "                         ";

            if (type == Bottletype.Soda)
            {
                messageQueue.Enqueue(new GUIMessage(new Point(79, 16), msg));

            }
            else if (type == Bottletype.Beer)
            {
                messageQueue.Enqueue(new GUIMessage(new Point(79, 10), msg));
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
