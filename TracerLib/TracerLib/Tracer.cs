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
        private const int FRAME_NUMBER = 1;             // not to see "StartTrace" each time
        private const string XML_ROOT_START = "<root>";
        private const string XML_ROOT_END = "</root>";
        private const string XML_THREAD_START = "<thread id=\"{0}\" time=\"{1}\">";
        private const string XML_THREAD_END = "</thread>";
        private const string TO_STRING_THREAD_FORMAT= "Thread {0} (time: {1})\nMethods:\n";

        private static StackTrace stackTracer = new StackTrace(true);
        private static Dictionary<int, ThreadsListItem> threadsDict = new Dictionary<int,ThreadsListItem>();

        public static void StartTrace()
        {
            var method = (new StackTrace(true)).GetFrame(FRAME_NUMBER).GetMethod();
            MethodInfo mi = new MethodInfo(method);

            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (!threadsDict.ContainsKey(threadId))
            {
                var item = new ThreadsListItem();
                item.CallStack = new Stack<TraceTree>();
                item.CallTree = new List<TraceTree>();
                item.timeSpan = new TimeSpan(0);

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

            threadsDict[threadId].timeSpan = threadsDict[threadId].timeSpan.Add(node.Info.timeSpan);
        }

        public static string BuildXml(string fileName)
        {
            string result = XML_ROOT_START;
            foreach (int id in threadsDict.Keys)
            {
                object[] args = new object[] { id.ToString(), threadsDict[id].Time };
                result += String.Format(XML_THREAD_START, args);

                foreach (var item in threadsDict[id].CallTree)
                {
                    result += item.ToXMLString();
                }
                result += XML_THREAD_END;
            }
            result += XML_ROOT_END;

            return result;
        }

        public static void PrintToConsole()
        {
            string result = "";
            foreach (int id in threadsDict.Keys)
            {
                object[] args = new object[] { id.ToString(), threadsDict[id].Time };
                result += "\n" + String.Format(TO_STRING_THREAD_FORMAT, args);

                foreach (var item in threadsDict[id].CallTree)
                {
                    result += item.ToString(1) + "\n";
                }
            }

            Console.Write(result);
        }
    }

}
