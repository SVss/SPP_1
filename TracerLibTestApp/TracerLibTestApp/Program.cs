using System;
using System.IO;
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

            string fileName = "";

            bool isExists = false;
            char answ = 'n';
            do
            {
                Console.Write("Enter filename to save trace to: ");
                fileName = Console.ReadLine();

                if (isExists = System.IO.File.Exists(fileName))
                {
                    Console.Write("File already exists. Rewrite it? ");
                    ConsoleKeyInfo key = Console.ReadKey();
                    answ = Char.ToLower(key.KeyChar);
                }
            } while ((answ == 'n') && (isExists));

            if ((!isExists) || (answ == 'y'))
            {
                string text = Tracer.BuildXml(fileName);
                System.IO.File.WriteAllText(fileName, text);

                Console.WriteLine("\nStatistics successfully saved");
            }
            else
            {
                Console.WriteLine("[y/n] <- is it so hard ?");
            }
            Console.ReadKey();
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
                Thread.Sleep(1);
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
