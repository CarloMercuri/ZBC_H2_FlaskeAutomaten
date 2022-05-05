using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FlaskeAutomaten
{
    public class Customer
    {
        private Bottletype drinkChoice;
        private BottleBuffer buffer;
        private GUI gui;
        public bool Paused { get; private set; }

        private int drinksConsumed = 0;
        private int drinkInterval;
        private int enoughDrinkPause;
        private int noDrinksPause;
        private int maxDrinks;

        public Customer(Bottletype type, BottleBuffer buffer, GUI gui, int interval, int enoughDrinkPause, int noDrinkPause, int maxDrinks)
        {
            this.maxDrinks = maxDrinks;
            this.drinkInterval = interval;
            this.enoughDrinkPause = enoughDrinkPause;
            this.noDrinksPause = noDrinkPause;
            this.gui = gui;
            drinkChoice = type;
            this.buffer = buffer;
        }

        public void StartConsuming()
        {
            Thread produceThread = new Thread(new ThreadStart(ConsumeThread));
            produceThread.Name = $"{Enum.GetName(typeof(Bottletype), drinkChoice)} Consumer Thread";
            produceThread.Start();
        }

        private void ConsumeThread()
        {
            int attempts = 0;

            while (true)
            {
                if(attempts > 10)
                {
                    attempts = 0;
                    Paused = true;
                    gui.PrintConsumerMessage(drinkChoice, "Not enough drinks. I'll come back later.");
                    Thread.Sleep(noDrinksPause);
                }
                else
                {
                    Paused = false;
                }

                if(drinksConsumed >= 25)
                {
                    drinksConsumed = 0;
                    Paused = true;
                    gui.PrintConsumerMessage(drinkChoice, (drinkChoice == Bottletype.Beer ? "Oh god I'm so drunk..." : "Hold on I need to burp"));
                    Thread.Sleep(enoughDrinkPause);
                }           

                try
                {
                    Monitor.Enter(buffer);

                    if (buffer.TryGetBottleType(drinkChoice, out Bottle bottle))
                    {
                        gui.PrintConsumerMessage(drinkChoice, $"Consumed {bottle.ToString()}");
                        drinksConsumed++;
                    }
                    else
                    {
                        attempts++;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger logger = new ErrorLogger();
                    logger.LogException(ex);
                }
                finally
                {
                    if (Monitor.IsEntered(buffer))
                    {
                        Monitor.Pulse(buffer);
                        Monitor.Exit(buffer);
                    }
                }

                Thread.Sleep(drinkInterval);
            }
        }
    }
}
