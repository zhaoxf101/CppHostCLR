using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Injector
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = Process.GetProcessesByName("TargetForInject")[0];
            string message="";
            Inject.DoInject(p, @"e:\ManageCodeInvoker.dll", out message);
        }

       

    }
}
