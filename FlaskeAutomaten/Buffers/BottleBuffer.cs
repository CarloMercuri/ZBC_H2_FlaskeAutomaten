using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class BottleBuffer
    {
        private Bottle[] Bottles;

        private bool itemsPresent;

        public bool ItemsPresent
        {
            get { return itemsPresent; }
            set { itemsPresent = value; }
        }
        
        public BottleBuffer(int maxItems)
        {
            Bottles = new Bottle[maxItems];

            for (int i = 0; i < Bottles.Length; i++)
            {
                Bottles[i] = null;
            }
        }

        /// <summary>
        /// Check if it is full.
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            for (int i = 0; i < Bottles.Length; i++)
            {
                if (Bottles[i] == null) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the number of bottles contained in the buffer
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            int count = 0;

            for (int i = 0; i < Bottles.Length; i++)
            {
                if (Bottles[i] != null) count++;
            }

            return count;
        }

        /// <summary>
        /// Attempts to insert a bottle in the buffer
        /// </summary>
        /// <param name="bottle"></param>
        /// <returns></returns>
        public bool TryInsertProduct(Bottle bottle)
        {
            for (int i = 0; i < Bottles.Length; i++)
            {
                if (Bottles[i] == null)
                {
                    Bottles[i] = bottle;
                    return true;
                }
            }

            return false;
        }

        public Bottle[] GetContents()
        {
            Bottle[] returnArray = new Bottle[Bottles.Length];
            Array.Copy(Bottles, returnArray, Bottles.Length);
            return returnArray;
        }

        /// <summary>
        /// Attempts to return the first occurrence of a bottle.
        /// </summary>
        /// <param name="foundProduct"></param>
        /// <returns></returns>
        public bool TryGetBottle(out Bottle foundProduct)
        {
            foundProduct = null;

            for (int i = 0; i < Bottles.Length; i++)
            {
                if (Bottles[i] != null)
                {
                    foundProduct = Bottles[i];
                    Bottles[i] = null;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to return the first occurrence of a bottle of a specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="foundProduct"></param>
        /// <returns></returns>
        public bool TryGetBottleType(Bottletype type, out Bottle foundProduct)
        {
            foundProduct = null;

            for (int i = 0; i < Bottles.Length; i++)
            {
                if (Bottles[i] != null)
                {
                    if(Bottles[i].Type == type)
                    {
                        foundProduct = Bottles[i];
                        Bottles[i] = null;
                        return true;
                    }                    
                }
            }

            return false;
        }
    }
}
