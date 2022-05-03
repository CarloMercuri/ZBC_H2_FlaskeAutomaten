using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class BeerBuffer : BottleBuffer
    {
        private int maxItems;
        public BeerBuffer(int maxItems) : base(maxItems)
        {
            this.maxItems = maxItems;
        }


    }
}
