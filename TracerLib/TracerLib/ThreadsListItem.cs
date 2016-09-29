using System;
using System.Collections.Generic;
using System.Xml;

namespace TracerLib
{
    internal class ThreadsListItem
    {
        private readonly int _id;
        private Stack<TraceTree> CallStack { get; }
        private List<TraceTree> CallTree { get; }   // List to keep several methods in MainThread

        // Public

        public long Time { get; set; }

        public ThreadsListItem(int id)
        {
            _id = id;
            CallStack = new Stack<TraceTree>();
            CallTree = new List<TraceTree>();
            Time = 0;
        }

        public void PushNode(TraceTree node)
        {
            if (CallStack.Count == 0)
            {
                CallTree.Add(node);
            }
            else
            {
                CallStack.Peek().Children.Add(node);
            }
            CallStack.Push(node);
            node.StartTimer();
        }

        public TraceTree PopNode()
        {
            if (CallStack.Count <= 0)
            {
                throw new Exception(ExceptionMessages.CantPopExceptionMessage);
            }
            TraceTree result = CallStack.Pop();

            result.StopTimer();
            UpdateThreadTime(result);

            return result;
        }

        public XmlElement ToXmlElement(XmlDocument document)
        {
            XmlElement result = document.CreateElement(XmlConstants.ThreadTag);
            result.SetAttribute(XmlConstants.ThreadIdAttribute, _id.ToString());
            result.SetAttribute(XmlConstants.TimeAttribute, Time.ToString());

            foreach (TraceTree item in CallTree)
            {
                result.AppendChild(item.ToXmlElement(document));
            }
            return result;
        }

        public override string ToString()
        {
            object[] args = { _id.ToString(), Time };
            var result = string.Format(StringConstants.ThreadToStringFormat, args);
            result += Environment.NewLine + StringConstants.MethodsListStart;

            foreach (TraceTree item in CallTree)
            {
                result += Environment.NewLine + item.ToString(1);
            }
            return result;
        }

        // Private

        private void UpdateThreadTime(TraceTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (CallStack.Count == 0)
            {
                Time += node.Info.Time;
            }
        }
    }

    // Constants

    public static partial class XmlConstants
    {
        public static string ThreadTag => "thread";
        public static string ThreadIdAttribute => "id";
    }

    public static partial class StringConstants
    {
        public static string ThreadToStringFormat => "Thread {0} (time: {1})";
        public static string MethodsListStart => "Methods:";
    }

    public static partial class ExceptionMessages
    {
        public static string CantPopExceptionMessage => "Can't pop item from empty CallStack";
    }
}
