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
            TraceClassMethod();
            TraceMethod();
            TraceThread();

            Tracer.PrintToConsole();
            Console.ReadKey();
        }

        static int go()
        {
            Tracer.StartTrace();
            int result = r1();
            Tracer.StopTrace();

            return result;
        }

        static int r1()
        {
            Tracer.StartTrace();
            int result = 42;
            Tracer.StopTrace();

            return result;
        }

        static void TraceClassMethod()
        {
            Tracer.StartTrace();
            RecClass.getString(5, 6, 7);
            Tracer.StopTrace();
        }

        static void TraceMethod()
        {
            Tracer.StartTrace();

            int x = go();
            Console.WriteLine(x);

            Tracer.StopTrace();
        }

        static void TraceThread()
        {
            Thread cThread = new Thread(() => RecClass.getString(5, 6, 7));
            cThread.Start();
            cThread.Join();
            //Tracer.PrintToConsole();    // ToDo: 
        }

    }

    class RecClass
    {
        const int REC_DEPTH = 5;

        public static void getString(int x, int y, int z)
        {
            Tracer.StartTrace();

            String result = " abc defg";
            result += "9  ";
            result += f(1).ToString();
            result = result.Trim();

            Tracer.StopTrace();
        }

        public static int f(int a)
        {
            Tracer.StartTrace();

            int result;
            if (a < REC_DEPTH)
            {
                Thread.Sleep(1);
                result = f(a + 1);
            }
            else
            {
                result = a + b();
            }

            Tracer.StopTrace();
            return result;
        }

        public static int b()
        {
            Tracer.StartTrace();

            int result = c();

            Tracer.StopTrace();
            return result;
        }

        public static int c()
        {
            Tracer.StartTrace();

            int result = 42;

            Tracer.StopTrace();
            return result;
        }
    }

}
