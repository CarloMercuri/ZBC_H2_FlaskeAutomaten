using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FlaskeAutomaten
{
    public class Splitter
    {
        private BottleBuffer producerBuffer;
        private bool paused;
        private bool producerHasSoda;
        private bool producerHasBeer;
        private bool beerBufferFull;
        private bool sodaBufferFull;
        private BottleBuffer beerBuffer;
        private BottleBuffer sodaBuffer;
        private int maxContents;
        private int interval;
        private int pauseTimer;


        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }


        public Splitter(BottleBuffer buffer, BottleBuffer beerBuffer, BottleBuffer sodaBuffer, int maxContents, int interval, int pauseTimer)
        {
            this.interval = interval;
            this.pauseTimer = pauseTimer;
            this.producerBuffer = buffer;
            this.beerBuffer = beerBuffer;
            this.sodaBuffer = sodaBuffer;
            this.maxContents = maxContents;
        }

        /// <summary>
        /// Begins the process
        /// </summary>
        public void StartSplitting()
        {
            Thread splitThread = new Thread(new ThreadStart(SplittingProcess));
            splitThread.Name = "Splitter Thread";
            splitThread.Start();
        }

        /// <summary>
        /// Attempts to insert the specified bottle into the Beer buffer
        /// </summary>
        /// <param name="bottle"></param>
        /// <returns></returns>
        private bool TryInsertInBeer(Bottle bottle)
        {
            bool success = false;

            try
            {
                Monitor.Enter(beerBuffer);

                if (beerBuffer.TryInsertProduct(bottle))
                {
                    success = true;
                }
                else
                {
                    success = false;
                }

                return success;
            }
            catch (Exception ex)
            {
                ErrorLogger logger = new ErrorLogger();
                logger.LogException(ex);
                return success; // error might have happened after we inserted one successfully
            }
            finally
            {
                if (Monitor.IsEntered(beerBuffer))
                {
                    Monitor.Pulse(beerBuffer);
                    Monitor.Exit(beerBuffer);
                }
            }
            
        }

        /// <summary>
        /// Attempts to insert the specified bottle into the Soda buffer
        /// </summary>
        /// <param name="bottle"></param>
        /// <returns></returns>
        private bool TryInsertInSoda(Bottle bottle)
        {
            bool success = false;

            try
            {
                Monitor.Enter(sodaBuffer);

                if (sodaBuffer.TryInsertProduct(bottle))
                {
                    success = true;
                }
                else
                {
                    success = false;
                }

                return success;
            }
            catch (Exception ex)
            {
                ErrorLogger logger = new ErrorLogger();
                logger.LogException(ex);
                return success; // error might have happened after we inserted one successfully
            }
            finally
            {
                if (Monitor.IsEntered(sodaBuffer))
                {
                    Monitor.Pulse(sodaBuffer);
                    Monitor.Exit(sodaBuffer);
                }
            }
        }

        /// <summary>
        /// Attempts to retreive the specified bottle type from the producer
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bottle"></param>
        /// <returns></returns>
        private bool TryGetFromProducer(Bottletype type, out Bottle bottle)
        {
            bool success = false;
            bottle = null;
            try
            {
                Monitor.Enter(producerBuffer);

                if (producerBuffer.TryGetBottleType(type, out bottle))
                {
                    success = true;
                }

                return success;
            }
            catch (Exception ex)
            {
                ErrorLogger logger = new ErrorLogger();
                logger.LogException(ex);
                return success;
            }
            finally
            {
                if (Monitor.IsEntered(producerBuffer))
                {
                    Monitor.Pulse(producerBuffer);
                    Monitor.Exit(producerBuffer);
                }
            }
        }

        private void SplittingProcess()
        {
            paused = false;

            while (true)
            {
                if (paused)
                {
                    Thread.Sleep(pauseTimer);
                    paused = false;
                }

                sodaBufferFull = false;
                beerBufferFull = false;

                // Try get a beer bottle from producer. If it finds out, attempt to place it in the 
                // right buffer.

                if(TryGetFromProducer(Bottletype.Beer, out Bottle beerBottle))
                {
                    beerBufferFull = beerBuffer.IsFull();

                    if (!beerBufferFull)
                    {
                        TryInsertInBeer(beerBottle);
                    }

                }

                // Try get a soda bottle from producer. If it finds out, attempt to place it in the 
                // right buffer.

                if (TryGetFromProducer(Bottletype.Soda, out Bottle sodaBottle))
                {
                    sodaBufferFull = sodaBuffer.IsFull();

                    if (!sodaBufferFull)
                    {
                        TryInsertInSoda(sodaBottle);
                    }
                }          

                // If both are full, going to standby mode
                if(sodaBufferFull && beerBufferFull)
                {
                    paused = true;                    
                }

                Thread.Sleep(interval);

            }
        }

       


    }
}
