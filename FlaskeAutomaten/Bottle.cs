using System;
using System.Collections.Generic;
using System.Text;

namespace FlaskeAutomaten
{
    public class Bottle
    {
        public Bottletype Type { get; set; }

        public int ID { get; private set; }


        public Bottle(Bottletype type, int id)
        {
            ID = id;
            Type = type;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(Bottletype), Type) + ID;
        }

    }
}
