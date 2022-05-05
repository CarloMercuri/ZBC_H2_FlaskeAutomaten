using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FlaskeAutomaten
{
    public class BottlesProducer
    {
        private int sodaIdCode = 0;
        private int beerIdCode = 0;

        BottleBuffer buffer;
        private Random rand;

        private bool paused;
        private int interval;
        private int pauseTimer;

        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }


        public BottlesProducer(BottleBuffer buffer, int interval, int pauseTimer)
        {
            this.interval = interval;
            this.pauseTimer = pauseTimer;
            this.buffer = buffer;
            rand = new Random();
        }

        /// <summary>
        /// Creates a soda bottle with incremental ID
        /// </summary>
        /// <returns></returns>
        private Bottle CreateSodaBottle()
        {
            Bottle btl =  new Bottle(Bottletype.Soda, sodaIdCode);
            sodaIdCode++;
            return btl;
        }

        /// <summary>
        /// Creates a Beer bottle with incremental ID
        /// </summary>
        /// <returns></returns>
        private Bottle CreateBeerBottle()
        {
            Bottle btl = new Bottle(Bottletype.Beer, beerIdCode);
            beerIdCode++;
            return btl;
        }

        public void StartProducing()
        {
            Thread produceThread = new Thread(new ThreadStart(ProduceThread));
            produceThread.Name = "Produce Thread";
            produceThread.Start();
        }

        /// <summary>
        /// Returns a random (Soda or Beer) bottle.
        /// </summary>
        /// <returns></returns>
        private Bottle GetRandomBottle()
        {
            Bottle bottle = null;

            if (rand.Next(0, 100) > 50)
            {
                bottle = CreateBeerBottle();
            }
            else
            {
                bottle = CreateSodaBottle();
            }

            return bottle;
        }
              

        private void ProduceThread()
        {         
            int attempts = 0;
            paused = false;

            while (true)
            {
                if (paused)
                {
                    Thread.Sleep(pauseTimer);
                    paused = false;
                }

                try
                {
                    Monitor.Enter(buffer);
                    Bottle btl = GetRandomBottle();

                    if (buffer.TryInsertProduct(btl))
                    {
                        // Producer inserted a bottle.                        
                    }
                    else
                    {
                        attempts++;

                        if(attempts > 7)
                        {
                            paused = true;
                        }
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

                Thread.Sleep(interval);

            }
        }
    }
}
