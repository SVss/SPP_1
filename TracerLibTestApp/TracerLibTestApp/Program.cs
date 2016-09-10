using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using TracerLib;

namespace TracerLibTestApp
{
    class Program
    {
        const string OUTPUT_FILE_NAME = "result.xml";

        static void Main(string[] args)
        {
            TraceClassMethod();

            TraceThread();
            TraceThread();

            TraceMethod();

            Tracer.PrintToConsole();

            char answ = 'n';
            do
            {
                Console.Write("\nSave to xml ? [y/n]: ");
                answ = Char.ToLower(Console.ReadKey().KeyChar);
            } while ((answ != 'n') && (answ != 'y'));

            if (answ == 'y')
            {
                XmlDocument doc = Tracer.BuildXml();
                doc.Save("result.xml");

                Console.WriteLine("\n\nFile {0} successfully saved.", OUTPUT_FILE_NAME);
                Console.ReadKey();
            }
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
        }

        static int go()
        {
            Tracer.StartTrace();

            int result = 0;
            for (int i = 0; i < 3; ++i)
                result += r1();

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

    }

    class RecClass
    {
        const int REC_DEPTH = 5;
        const int SLEEP_TIME = 50;

        public static void getString(int x, int y, int z)
        {
            Tracer.StartTrace();

            String result = " abc defg";
            result += "9  ";
            result += myCoolFunction1(1).ToString();
            result += anotherAmazingMethod().ToString();
            result += creativeName().ToString();
            result = result.Trim();

            Tracer.StopTrace();
        }

        public static int myCoolFunction1(int a)
        {
            Tracer.StartTrace();

            int result;
            if (a < REC_DEPTH)
            {
                Thread.Sleep(SLEEP_TIME);
                result = myCoolFunction1(a + 1);
            }
            else
            {
                result = a;
            }

            Tracer.StopTrace();
            return result;
        }

        public static int anotherAmazingMethod()
        {
            Tracer.StartTrace();

            int result = 5;

            Tracer.StopTrace();
            return result;
        }

        public static int creativeName()
        {
            Tracer.StartTrace();

            int result = 42;

            Tracer.StopTrace();
            return result;
        }
    }

}
