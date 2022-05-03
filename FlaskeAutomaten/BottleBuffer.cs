using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class BottleBuffer
    {
        private Queue<string> Bottles = new Queue<string>();

        private bool itemsPresent;
        private int maxItems;

        public bool ItemsPresent
        {
            get { return itemsPresent; }
            set { itemsPresent = value; }
        }

        public BottleBuffer(int maxItems)
        {
            this.maxItems = maxItems;
        }

        public bool IsFull()
        {
            return Bottles.Count >= maxItems;
        }

        public int GetCount()
        {
            return Bottles.Count;
        }

        public bool TryInsertProduct(string bottle)
        {
            if(Bottles.Count < maxItems)
            {
                Bottles.Enqueue(bottle);
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] GetContents()
        {
            return Bottles.ToArray();
        }

        public bool TryGetProduct(out string foundProduct)
        {
            foundProduct = null;

            if (Bottles.TryDequeue(out string product))
            {
                itemsPresent = true;
                foundProduct = product;
                return true;
            }
            else
            {
                itemsPresent = false;
                return false;
            }
        }
    }
}
