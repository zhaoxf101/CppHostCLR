using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Message
{
    public class Message
    {
        public static int Show(string message)
        {
            MessageBox.Show(message);
            return 100;
        }
    }
}
