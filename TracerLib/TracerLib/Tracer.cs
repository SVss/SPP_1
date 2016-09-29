using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace TracerLib
{
    public sealed class Tracer: ITracer
    {
        private static Dictionary<int, ThreadsListItem> _threadsDictionary;
        private static readonly object LockObj = new object();

        private static Tracer _instance;

        private Tracer()
        {
            _threadsDictionary = new Dictionary<int, ThreadsListItem>();
        }

        // Public

        public static Tracer GetInstance()
        {
            if (_instance == null)
            {
                lock (LockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new Tracer();
                    }
                }
            }
            return _instance;
        }

        public void StartTrace()
        {
            StackTrace context = new StackTrace(ConfigConstants.NeedFileInfoFlag);

            MethodBase currentMethod = context.GetFrame(ConfigConstants.SkipFramesCount).GetMethod();
            MethodInfo currentMethodInfo = new MethodInfo(currentMethod);

            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (LockObj)
            {
                if (_threadsDictionary.ContainsKey(threadId) == false)
                {
                    _threadsDictionary.Add(threadId, new ThreadsListItem(threadId));
                }

                TraceTree node = new TraceTree(currentMethodInfo);
                _threadsDictionary[threadId].PushNode(node);
            }
        }

        public void StopTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (LockObj)
            {
                if (_threadsDictionary.ContainsKey(threadId) == false)
                {
                    throw new Exception(ExceptionMessages.CantStopExceptionMessage);
                }

                _threadsDictionary[threadId].PopNode();
            }
        }

        public XmlDocument BuildXml()
        {
            XmlDocument result = new XmlDocument();
            XmlElement root = (XmlElement)result.AppendChild(result.CreateElement(XmlConstants.RootTag));
            lock (LockObj)
            {
                foreach (ThreadsListItem item in _threadsDictionary.Values)
                {
                    root.AppendChild(item.ToXmlElement(result));
                }
            }
            return result;
        }

        public void PrintToConsole()
        {
            string result = String.Empty;
            lock (LockObj)
            {
                foreach (ThreadsListItem item in _threadsDictionary.Values)
                {
                    result += item + Environment.NewLine;
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
        public static string RootTag => "root";
        public static string TimeAttribute => "time";
    }

    public static partial class ExceptionMessages
    {
        public static string CantStopExceptionMessage => "Can't stop trace before starting";
    }
}
