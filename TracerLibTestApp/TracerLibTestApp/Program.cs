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
            TraceMethod();
            TraceClassMethods();
            TraceThread1();
            TraceThread2();

            PrintReport();
            SaveXmlReport();

            Console.Write("\nPress any key to exit");
            Console.ReadKey();
        }

        private static void PrintReport()
        {
            mainTracer.PrintToConsole();
        }

        private static void SaveXmlReport()
        {
            char answ = 'n';
            do
            {
                Console.Write("\nSave to xml ? [y/n]: ");
                answ = Char.ToLower(Console.ReadKey().KeyChar);
            } while ((answ != 'n') && (answ != 'y'));

            Console.WriteLine();

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
                        Console.WriteLine("\nReport successfully saved to file: {0}.", fileName);
                    }
                    catch (XmlException)
                    {
                        Console.WriteLine("\nCan't save output file.");
                    }
                }
                else
                {
                    Console.WriteLine("\nSaving aborted.");
                }
            }
        }

        private static void TraceClassMethods()
        {
            mainTracer.StartTrace();

            AnotherClass1.Method1(5, 6, 7);

            AnotherClass1.UnusualMethod();

            mainTracer.StopTrace();
        }

        private static void TraceMethod()
        {
            mainTracer.StartTrace();

            int i = Method0();

            mainTracer.StopTrace();
        }

        private static void TraceThread1()
        {
            Thread anotherThread = new Thread(() => AnotherClass1.Method1(5, 6, 7));
            anotherThread.Start();
            anotherThread.Join();
        }

        private static void TraceThread2()
        {
            AnotherClass2 objClass2 = new AnotherClass2();

            Thread anotherThread = new Thread(() => objClass2.StrangeMethod("Hello, World!"));
            anotherThread.Start();
            anotherThread.Join();
        }

        private static int Method0()
        {
            mainTracer.StartTrace();

            int result = 0;
            for (int i = 0; i < 3; ++i)
                result += Get42();

            mainTracer.StopTrace();
            return result;
        }

        private static int Get42()
        {
            mainTracer.StartTrace();

            int result = 42;

            mainTracer.StopTrace();
            return result;
        }

    }

    class AnotherClass1
    {
        const int RecursionDepth = 5;
        const int SleepTime = 50;

        static Tracer anotherTracer = Tracer.GetInstance();

        public static void Method1(int x, int y, int z)
        {
            anotherTracer.StartTrace();

            String result = " abc defg";
            result += "9  ";
            result += CoolMethod(1).ToString();
            result += AnotherCoolMethod().ToString();
            result += UnusualMethod();
            result = result.Trim();

            anotherTracer.StopTrace();
        }

        public static int CoolMethod(int a)
        {
            anotherTracer.StartTrace();

            int result;
            if (a < RecursionDepth)
            {
                Thread.Sleep(SleepTime);
                result = CoolMethod(a + 1);
            }
            else
            {
                result = a;
            }

            anotherTracer.StopTrace();
            return result;
        }

        private static int AnotherCoolMethod()
        {
            anotherTracer.StartTrace();

            Random rnd = new Random();
            int result = rnd.Next(-10, 10); ;

            anotherTracer.StopTrace();
            return result;
        }

        public static string UnusualMethod()
        {
            anotherTracer.StartTrace();

            string result = "Unusual";

            anotherTracer.StopTrace();
            return result;
        }
    }

    class AnotherClass2
    {
        private static Tracer thirdTracer = Tracer.GetInstance();

        public void StrangeMethod(string inputString)
        {
            thirdTracer.StartTrace();

            char[] characters = inputString.ToCharArray();
            for (int i = 0; i < characters.Length; ++i)
            {
                characters[i] = UpperChar(characters[i]);
            }
            string tempString = characters.ToString();

            thirdTracer.StopTrace();
        }

        char UpperChar(char c)
        {
            thirdTracer.StartTrace();

            char result = Char.ToUpper(c);

            thirdTracer.StopTrace();
            return result;
        }
    }
}
