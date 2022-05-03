using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class SystemControl
    {
        private GUI gui;
        private BottleBuffer bottlesBuffer;
        private BottlesProducer producer;
        private BeerBuffer beerBuffer;
        private SodaBuffer sodaBuffer;
        private Splitter splitter;

        public SystemControl()
        {

        }

        public void Init(GUI gui)
        {
            bottlesBuffer = new BottleBuffer(10);
            beerBuffer = new BeerBuffer(10);
            sodaBuffer = new SodaBuffer(10);
            splitter = new Splitter(bottlesBuffer, beerBuffer, sodaBuffer, 10);
            
            producer = new BottlesProducer(bottlesBuffer);
            producer.StartProducing();

            splitter.Initialize();

            this.gui = gui;
            gui.InitializeGUI(this);
        }

        public bool IsBeerBufferFull()
        {
            return beerBuffer.IsFull();
        }

        public bool IsSodaBufferFull()
        {
            return sodaBuffer.IsFull();
        }

        public bool IsSplitterPaused()
        {
            return splitter.Paused;
        }

        public bool IsProducerPaused()
        {
            return producer.Paused;
        }

        public string[] GetBottleBufferList()
        {
            return bottlesBuffer.GetContents();
        }

        public int GetBeerBufferCount()
        {
            return beerBuffer.GetCount();
        }

        public int GetSodaBufferCount()
        {
            return sodaBuffer.GetCount();
        }

        public int GetSplitterCount()
        {
            return splitter.GetCount();
        }
    }
}
