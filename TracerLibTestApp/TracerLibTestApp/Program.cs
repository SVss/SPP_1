using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TracerLib;

namespace TracerLibTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceMethod();
            TraceThread();

            Console.ReadKey();
        }

        static void TraceMethod()
        {
            Class1.getString(5, 6, 7);
            Tracer.PrintToConsole();
        }

        static void TraceThread()
        {
            Thread cThread = new Thread(() => Class1.getString(5, 6, 7));
            cThread.Start();
            cThread.Join();
            Tracer.PrintToConsole();    // ToDo: 
        }

    }

    class Class1
    {
        public static void getString(int x, int y, int z)
        {
            Tracer.StartTrace();

            String result = " abc defg";
            result += "9  ";
            f(0);
            result = result.Trim();

            Tracer.StopTrace();
        }

        public static void f(int a)
        {
            if (a < 100)
            {
                Thread.Sleep(1);
                f(a + 1);
            }

            return;
        }
    }

}
