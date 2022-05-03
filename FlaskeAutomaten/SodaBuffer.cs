using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class SodaBuffer : BottleBuffer
    {
        private int maxItems;
        public SodaBuffer(int maxItems) : base(maxItems)
        {
            this.maxItems = maxItems;
        }

    }
}
