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
        private const int FRAME_NUMBER = 1;
        private const string XML_ROOT_START = "<root>";
        private const string XML_ROOT_END = "</root>";
        private const string XML_THREAD_START = "<thread id=\"{0}\" time=\"{1}\">";
        private const string XML_THREAD_END = "</thread>";

        private static StackTrace stackTracer = new StackTrace(true);
        private static Dictionary<int, ThreadsListItem> threadsDict = new Dictionary<int,ThreadsListItem>();

        public static void StartTrace()
        {
            //int i = 0;
            //foreach (var item in (new StackTrace(true)).GetFrames())
            //{
            //    for (int j = 0; j < i; ++j)
            //    {
            //        Console.Write(" ");
            //    }
            //    Console.WriteLine(item.GetMethod().Name);
            //    ++i;
            //}

            //return;

            var method = (new StackTrace(true)).GetFrame(FRAME_NUMBER).GetMethod();
            MethodInfo mi = new MethodInfo(method);

            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!threadsDict.ContainsKey(threadId))
            {
                var item = new ThreadsListItem();
                item.CallStack = new Stack<TraceTree>();
                item.CallTree = new List<TraceTree>();

                threadsDict.Add(threadId, item);
            }

            var threadInfo = threadsDict[threadId];

            TraceTree node = new TraceTree(mi);
            if (threadInfo.CallStack.Count == 0)
            {
                threadInfo.CallTree.Add(node);
            }
            else
            {
                threadInfo.CallStack.Peek().Children.Add(node);
            }

            threadInfo.CallStack.Push(node);
            node.startTimer();
        }

        public static void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!threadsDict.ContainsKey(threadId))
            {
                throw new Exception("Can't stop trace before starting");
            }

            var node = threadsDict[threadId].CallStack.Pop();
            node.stopTimer();
        }

        public static void BuildXml()
        {
            //mi.Name = method.Name;
            //mi.ClassName = method.ReflectedType.Name;
            //mi.ArgsCount = method.GetParameters().Count();
        }

        public static void PrintToConsole()
        {
            string result = XML_ROOT_START; // ToDo: Move to BuildXml()
            foreach (int id in threadsDict.Keys)
            {
                object[] args = new object[] { id.ToString(), "xxx" };  // ToDo: Thread Time (in StopTrace())
                result += "\n" + String.Format(XML_THREAD_START, args);

                foreach (var item in threadsDict[id].CallTree)
	            {
                    result += "\n" + item.ToXMLString();
	            }

                result += XML_THREAD_END;
            }

            result += XML_ROOT_END;

            Console.WriteLine(result);
        }
    }

}
