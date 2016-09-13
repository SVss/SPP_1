using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace TracerLib
{
    interface ITracer
    {
        void StartTrace();
        void StopTrace();
        XmlDocument BuildXml();
        void PrintToConsole();
    }

    public sealed class Tracer: ITracer
    {
        private const bool NeedFileInfo = false;
        private const int SkipFramesCount = 1;      // to skip "StartTrace" method's stack frame
        private const string RootTag = "root";
        private const string CantStopExceptionMessage = "Can't stop trace before starting";

        private static Dictionary<int, ThreadsListItem> ThreadsDictionary;
        private static object lockObj = new object();

        private static Tracer instance = null;

        private Tracer()
        {
            ThreadsDictionary = new Dictionary<int, ThreadsListItem>();
        }

        // Public

        public static Tracer GetInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new Tracer();
                    }
                }
            }
            return Tracer.instance;
        }

        public void StartTrace()
        {
            StackTrace context = new StackTrace(NeedFileInfo);

            System.Reflection.MethodBase currentMethod = context.GetFrame(SkipFramesCount).GetMethod();
            MethodInfo currentMethodInfo = new MethodInfo(currentMethod);

            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObj)
            {
                if (ThreadsDictionary.ContainsKey(threadId) == false)
                {
                    ThreadsDictionary.Add(threadId, new ThreadsListItem(threadId));
                }

                TraceTree node = new TraceTree(currentMethodInfo);
                ThreadsDictionary[threadId].PushNode(node);
            }
        }

        public void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (lockObj)
            {
                if (ThreadsDictionary.ContainsKey(threadId) == false)
                {
                    throw new Exception(CantStopExceptionMessage);
                }

                ThreadsDictionary[threadId].PopNode();
            }
        }

        public XmlDocument BuildXml()
        {
            XmlDocument result = new XmlDocument();
            XmlElement root = (XmlElement)result.AppendChild(result.CreateElement(RootTag));
            lock (lockObj)
            {
                foreach (ThreadsListItem item in ThreadsDictionary.Values)
                {
                    root.AppendChild(item.ToXmlElement(result));
                }
            }
            return result;
        }

        public void PrintToConsole()
        {
            string result = String.Empty;
            lock (lockObj)
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
