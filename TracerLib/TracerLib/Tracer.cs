using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace TracerLib
{
    public static class Tracer
    {
        private const int FRAME_NUMBER = 1;             // not to see "StartTrace" each time
        private const string TO_STRING_THREAD_FORMAT= "Thread {0} (time: {1})\nMethods:\n";

        private static StackTrace stackTracer = new StackTrace(true);
        private static Dictionary<int, ThreadsListItem> threadsDict = new Dictionary<int,ThreadsListItem>();

        public static void StartTrace()
        {
            var method = (new StackTrace(true)).GetFrame(FRAME_NUMBER).GetMethod();
            MethodInfo mi = new MethodInfo(method);

            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (threadsDict)
            {
                if (!threadsDict.ContainsKey(threadId))
                {
                    var item = new ThreadsListItem();
                    item.CallStack = new Stack<TraceTree>();
                    item.CallTree = new List<TraceTree>();
                    item.Time = 0;

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
        }

        public static void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (threadsDict)
            {
                if (!threadsDict.ContainsKey(threadId))
                {
                    throw new Exception("Can't stop trace before starting");
                }

                var node = threadsDict[threadId].CallStack.Pop();
                node.stopTimer();

                if (threadsDict[threadId].CallStack.Count == 0)
                    threadsDict[threadId].Time += node.Info.Time;
            }
        }

        public static XmlDocument BuildXml()
        {
            XmlDocument result = new XmlDocument();
            XmlElement root = (XmlElement)result.AppendChild(result.CreateElement("root"));

            foreach (int id in threadsDict.Keys)
            {
                XmlElement thread = result.CreateElement("thread");
                thread.SetAttribute("id", id.ToString());
                thread.SetAttribute("time", threadsDict[id].Time.ToString());

                root.AppendChild(thread);

                foreach (var item in threadsDict[id].CallTree)
                {
                    thread.AppendChild(item.ToXMLElement(result));
                }
            }

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
