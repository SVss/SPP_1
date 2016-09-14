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
            StackTrace context = new StackTrace(ConfigConstants.NeedFileInfoFlag);

            System.Reflection.MethodBase currentMethod = context.GetFrame(ConfigConstants.SkipFramesCount).GetMethod();
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
                    throw new Exception(ExceptionMessages.CantStopExceptionMessage);
                }

                ThreadsDictionary[threadId].PopNode();
            }
        }

        public XmlDocument BuildXml()
        {
            XmlDocument result = new XmlDocument();
            XmlElement root = (XmlElement)result.AppendChild(result.CreateElement(XmlConstants.RootTag));
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

    // Constants

    public static class ConfigConstants
    {
        public static bool NeedFileInfoFlag = false;
        public const int SkipFramesCount = 1;      // to skip "StartTrace" method's stack 
    }

    public static partial class XmlConstants
    {
        public static string RootTag { get { return "root"; } }
        public static string TimeAttribute { get { return "time"; } }
    }

    public static partial class ExceptionMessages
    {
        public static string CantStopExceptionMessage { get { return "Can't stop trace before starting"; } }
    }
}
