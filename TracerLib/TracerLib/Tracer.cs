using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace TracerLib
{
    public static class Tracer
    {
        private const bool NeedFileInfo = false;
        private const int SkipFramesCount = 1;      // to skip "StartTrace" method's stack frame
        private const string RootTag = "root";
        private const string CantStopExceptionMessage = "Can't stop trace before starting";

        private static Dictionary<int, ThreadsListItem> ThreadsDictionary = new Dictionary<int, ThreadsListItem>();

        // Public

        public static void StartTrace()
        {
            StackTrace context = new StackTrace(NeedFileInfo);

            System.Reflection.MethodBase currentMethod = context.GetFrame(SkipFramesCount).GetMethod();
            MethodInfo currentMethodInfo = new MethodInfo(currentMethod);

            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (ThreadsDictionary)
            {
                if (ThreadsDictionary.ContainsKey(threadId) == false)
                {
                    ThreadsDictionary.Add(threadId, new ThreadsListItem(threadId));
                }

                TraceTree node = new TraceTree(currentMethodInfo);
                ThreadsDictionary[threadId].PushNode(node);
            }
        }

        public static void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (ThreadsDictionary)
            {
                if (ThreadsDictionary.ContainsKey(threadId) == false)
                {
                    throw new Exception(CantStopExceptionMessage);
                }

                ThreadsDictionary[threadId].PopNode();
            }
        }

        public static XmlDocument BuildXml()
        {
            XmlDocument result = new XmlDocument();
            XmlElement root = (XmlElement)result.AppendChild(result.CreateElement(RootTag));

            lock (ThreadsDictionary)
            {
                foreach (ThreadsListItem item in ThreadsDictionary.Values)
                {
                    root.AppendChild(item.ToXmlElement(result));
                }
            }
            return result;
        }

        public static void PrintToConsole()
        {
            string result = String.Empty;
            lock (ThreadsDictionary)
            {
                foreach (ThreadsListItem item in ThreadsDictionary.Values)
                {
                    result += item.ToString() + Environment.NewLine;
                }
            }
            Console.Write(result);
        }
    }
}
