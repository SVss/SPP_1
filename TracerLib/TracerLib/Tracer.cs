using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace TracerLib
{
    public static class Tracer
    {
        private static StackTrace stackTracer = new StackTrace(true);

        static Stopwatch sw = new Stopwatch();
        static MethodInfo mi = new MethodInfo();

        public static void StartTrace()
        {
            var method = stackTracer.GetFrame(2).GetMethod();

            mi.Name = method.Name;
            mi.ClassName = method.ReflectedType.Name;
            mi.ArgsCount = method.GetParameters().Count();
            mi.ThreadId = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine("*** Trace started");

            sw.Reset();
            sw.Start();
        }

        public static void StopTrace()
        {
            sw.Stop();
            mi.Time = sw.ElapsedMilliseconds;

            Console.WriteLine("^^^ Trace stopped");
        }

        public static void BuildXml()
        {

        }

        public static void PrintToConsole()
        {
            Console.WriteLine("{0}:\t{1}.{2}(args count: {3})\t{4}", mi.ThreadId, mi.ClassName, mi.Name, mi.ArgsCount, mi.Time);
        }
    }

}
