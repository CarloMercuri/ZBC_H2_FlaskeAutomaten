using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FlaskeAutomaten
{
    public class Splitter
    {
        private BottleBuffer generalBuffer;
        private bool paused;
        private bool generalBufferEmpty;
        private bool beerBufferFull;
        private bool sodaBufferFull;
        private BeerBuffer beerBuffer;
        private SodaBuffer sodaBuffer;
        private int maxContents;
        

        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }

        private Queue<string> bottles = new Queue<string>();
        private Queue<string> beerBottles = new Queue<string>();
        private Queue<string> sodaBottles = new Queue<string>();

        


        public Splitter(BottleBuffer buffer, BeerBuffer beerBuffer, SodaBuffer sodaBuffer, int maxContents)
        {
            this.generalBuffer = buffer;
            this.beerBuffer = beerBuffer;
            this.sodaBuffer = sodaBuffer;
            this.maxContents = maxContents;
        }

        public void Initialize()
        {
            Thread splitThread = new Thread(new ThreadStart(SplittingThread));
            splitThread.Name = "Splitter Thread";
            splitThread.Start();
        }

        public int GetCount()
        {
            return bottles.Count;
        }

        private void SplittingThread()
        {
            int generalAttempts = 0;
            int sodaAttempts = 0;
            int beerAttempts = 0;

            paused = false;

            while (true)
            {
                if (paused)
                {
                    Thread.Sleep(3000);
                    paused = false;
                }
                
                try
                {
                    if(beerBottles.Count < maxContents)
                    {
                        Monitor.Enter(generalBuffer);

                        if (generalBuffer.TryGetProduct(out string Bottle))
                        {
                            if (Bottle.ToLower().Contains("soda"))
                            {
                                sodaBottles.Enqueue(Bottle);
                            }
                            else
                            {
                                beerBottles.Enqueue(Bottle);
                            }
                            
                        }
                        else
                        {
                            generalAttempts++;

                            if (generalAttempts > 7)
                            {
                                generalBufferEmpty = true;
                                generalAttempts = 0;
                            }
                        }

                        Monitor.Exit(generalBuffer);
                        Monitor.Pulse(generalBuffer);
                    }

                    if(beerBottles.Count > 0)
                    {
                        Monitor.Enter(beerBuffer);

                        if (!beerBuffer.IsFull())
                        {
                            beerBufferFull = false;
                            beerBuffer.TryInsertProduct(beerBottles.Dequeue());

                        }
                        else
                        {
                            beerAttempts++;

                            if (beerAttempts > 7)
                            {
                                beerBufferFull = true;
                                beerAttempts = 0;
                            }
                        }

                        Monitor.Exit(beerBuffer);
                        Monitor.Pulse(beerBuffer);
                    }

                    if (sodaBottles.Count > 0)
                    {
                        Monitor.Enter(sodaBuffer);

                        if (!sodaBuffer.IsFull())
                        {
                            sodaBufferFull = false;
                            sodaBuffer.TryInsertProduct(sodaBottles.Dequeue());

                        }
                        else
                        {
                            sodaAttempts++;

                            if (sodaAttempts > 7)
                            {
                                sodaBufferFull = true;
                                sodaAttempts = 0;
                            }
                        }

                        Monitor.Exit(sodaBuffer);
                        Monitor.Pulse(sodaBuffer);
                    }

                    

                    if(sodaBufferFull && beerBufferFull && generalBufferEmpty)
                    {
                        paused = true;
                    }
                    
                }
                catch (Exception ex)
                {


                }

                Thread.Sleep(500);


            }
        }


    }
}
