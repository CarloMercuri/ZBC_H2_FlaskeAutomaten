using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class SystemControl
    {
        private GUI gui;
        private BottleBuffer producerBuffer;
        private BottlesProducer producer;
        private BottleBuffer beerBuffer;
        private BottleBuffer sodaBuffer;
        private Splitter splitter;

        private Customer beerCustomer;
        private Customer sodaCustomer;

        public void Init()
        {
            this.gui = new GUI();

            producerBuffer = new BottleBuffer(10);
            beerBuffer = new BottleBuffer(10);
            sodaBuffer = new BottleBuffer(10);

            producer = new BottlesProducer(producerBuffer, 200,   // Product making interval
                                                           5000); // Pause timer

            splitter = new Splitter(producerBuffer, beerBuffer, sodaBuffer, 10,   // Max contents
                                                                            400,  // Main interval
                                                                            5000); // Pause timer

            beerCustomer = new Customer(Bottletype.Beer, beerBuffer, gui,
                                                                     800,   // Main interval
                                                                     8000,  // Enough drinks interval
                                                                     5000,  // No drinks left pause
                                                                     15);   // Max drinks

            sodaCustomer = new Customer(Bottletype.Soda, sodaBuffer, gui,
                                                                     800,   // Main interval
                                                                     8000,  // Enough drinks interval
                                                                     5000,  // No drinks left pause
                                                                     25);   // Max drinks


            producer.StartProducing();
            splitter.StartSplitting();
            beerCustomer.StartConsuming();
            sodaCustomer.StartConsuming();


            gui.InitializeGUI(this);
        }

        /// <summary>
        /// Returns true if all the slots in the buffer are occupied.
        /// </summary>
        /// <returns></returns>
        public bool IsBeerBufferFull()
        {
            return beerBuffer.IsFull();
        }

        /// <summary>
        /// Returns true if all the slots in the buffer are occupied.
        /// </summary>
        /// <returns></returns>
        public bool IsSodaBufferFull()
        {
            return sodaBuffer.IsFull();
        }

        /// <summary>
        /// Returns true if the splitter is currently paused.
        /// </summary>
        /// <returns></returns>
        public bool IsSplitterPaused()
        {
            return splitter.Paused;
        }

        /// <summary>
        /// Returns true if the producer is currently paused.
        /// </summary>
        /// <returns></returns>
        public bool IsProducerPaused()
        {
            return producer.Paused;
        }

        /// <summary>
        /// Returns a copy of the contents of the buffer
        /// </summary>
        /// <returns></returns>
        public Bottle[] GetProducerBufferList()
        {
            return producerBuffer.GetContents();
        }

        /// <summary>
        /// Returns a count of the items in the beer buffer
        /// </summary>
        /// <returns></returns>
        public int GetBeerBufferCount()
        {
            return beerBuffer.GetCount();
        }

        /// <summary>
        /// Returns a count of the items in the soda buffer
        /// </summary>
        /// <returns></returns>
        public int GetSodaBufferCount()
        {
            return sodaBuffer.GetCount();
        }
    }
}
