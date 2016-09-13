using System;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using TracerLib;

namespace TracerLibTestApp
{
    class Program
    {
        static Tracer mainTracer = Tracer.GetInstance();

        [STAThread]
        static void Main(string[] args)
        {
            TraceClassMethod();

            TraceThread();
            TraceThread();

            TraceMethod();

            mainTracer.PrintToConsole();

            char answ = 'n';
            do
            {
                Console.Write("\nSave to xml ? [y/n]: ");
                answ = Char.ToLower(Console.ReadKey().KeyChar);
            } while ((answ != 'n') && (answ != 'y'));

            if (answ == 'y')
            {
                XmlDocument doc = mainTracer.BuildXml();
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Xml files|*.xml";

                DialogResult saveAnswer = saveDialog.ShowDialog();
                if (saveAnswer == DialogResult.OK)
                {
                    string fileName = saveDialog.FileNames[0];
                    try
                    {
                        doc.Save(fileName);
                        Console.WriteLine("\n\nReport successfully saved to file\n{0}.", fileName);
                    }
                    catch(XmlException)
                    {
                        Console.WriteLine("\n\nCan't save output file.");
                    }
                }
                else
                {
                    Console.WriteLine("\n\nSaving aborted.");
                }

                Console.ReadKey();
            }
        }

        static void TraceClassMethod()
        {
            mainTracer.StartTrace();

            RecClass.getString(5, 6, 7);

            mainTracer.StopTrace();
        }

        static void TraceMethod()
        {
            mainTracer.StartTrace();

            int x = go();
            Console.WriteLine(x);

            mainTracer.StopTrace();
        }

        static void TraceThread()
        {
            Thread cThread = new Thread(() => RecClass.getString(5, 6, 7));
            cThread.Start();
            cThread.Join();
        }

        static int go()
        {
            mainTracer.StartTrace();

            int result = 0;
            for (int i = 0; i < 3; ++i)
                result += r1();

            mainTracer.StopTrace();
            return result;
        }

        static int r1()
        {
            mainTracer.StartTrace();

            int result = 42;

            mainTracer.StopTrace();
            return result;
        }

    }

    class RecClass
    {
        const int REC_DEPTH = 5;
        const int SLEEP_TIME = 50;

        static Tracer anotherTracer = Tracer.GetInstance();

        public static void getString(int x, int y, int z)
        {
            anotherTracer.StartTrace();

            String result = " abc defg";
            result += "9  ";
            result += myCoolFunction1(1).ToString();
            result += anotherAmazingMethod().ToString();
            result += creativeName().ToString();
            result = result.Trim();

            anotherTracer.StopTrace();
        }

        public static int myCoolFunction1(int a)
        {
            anotherTracer.StartTrace();

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

            anotherTracer.StopTrace();
            return result;
        }

        public static int anotherAmazingMethod()
        {
            anotherTracer.StartTrace();

            int result = 5;

            anotherTracer.StopTrace();
            return result;
        }

        public static int creativeName()
        {
            anotherTracer.StartTrace();

            int result = 42;

            anotherTracer.StopTrace();
            return result;
        }
    }

}
