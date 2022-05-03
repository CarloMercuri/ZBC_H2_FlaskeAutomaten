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

        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }


        public BottlesProducer(BottleBuffer buffer)
        {
            this.buffer = buffer;
            rand = new Random();
        }

        private string CreateSodaBottle()
        {
            string name = "Soda " + sodaIdCode.ToString();
            sodaIdCode++;
            return name;
        }

        private string CreateBeerBottle()
        {
            string name = "Beer " + beerIdCode.ToString();
            beerIdCode++;
            return name;
        }

        public void StartProducing()
        {
            Thread produceThread = new Thread(new ThreadStart(ProduceThread));
            produceThread.Name = "Produce Thread";
            produceThread.Start();
        }

        private string GetRandomBottle()
        {
            string bottle = "";

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
            
            string bottle = "";
            int attempts = 0;

            while (true)
            {
                paused = false;

                Monitor.Enter(buffer);

                try
                {
                    if (buffer.TryInsertProduct(GetRandomBottle()))
                    {
                        // Producer inserted a bottle.
                    }
                    else
                    {
                        attempts++;

                        if(attempts > 7)
                        {
                            Monitor.Pulse(buffer);
                            Monitor.Exit(buffer);

                            paused = true;
                            Thread.Sleep(3000);
                            continue;
                        }
                    }

                    Monitor.Pulse(buffer);
                    Monitor.Exit(buffer);
                }
                catch (Exception ex)
                {

                    throw;
                }
                finally
                {
                  
                }

                Thread.Sleep(500);


            }

           



        }
    }
}
