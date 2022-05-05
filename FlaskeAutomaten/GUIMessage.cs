using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FlaskeAutomaten
{
    public class GUIMessage
    {
        public Point Location { get; set; }
        public string Message { get; set; }

        public GUIMessage(Point location, string message)
        {
            this.Location = location;   
            this.Message = message;
        }
    }
}
