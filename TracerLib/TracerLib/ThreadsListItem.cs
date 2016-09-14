using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TracerLib
{
    class ThreadsListItem
    {
        private int Id;
        private Stack<TraceTree> CallStack { get; set; }
        private List<TraceTree> CallTree { get; set; }   // List to keep several methods in MainThread

        // Public

        public long Time { get; set; }

        public ThreadsListItem(int id)
        {
            this.Id = id;
            this.CallStack = new Stack<TraceTree>();
            this.CallTree = new List<TraceTree>();
            this.Time = 0;
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
            result.SetAttribute(XmlConstants.ThreadIdAttribute, Id.ToString());
            result.SetAttribute(XmlConstants.TimeAttribute, Time.ToString());

            foreach (TraceTree item in CallTree)
            {
                result.AppendChild(item.ToXMLElement(document));
            }
            return result;
        }

        override public string ToString()
        {
            string result = String.Empty;
            object[] args = new object[] { Id.ToString(), Time };
            result = String.Format(StringConstants.ThreadToStringFormat, args);
            result += Environment.NewLine + StringConstants.MethodsListStart;

            foreach (TraceTree item in CallTree)
            {
                result += Environment.NewLine + item.ToString(1, 1);
            }
            return result;
        }

        // Private

        private void UpdateThreadTime(TraceTree node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
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
        public static string ThreadTag { get { return "thread"; } }
        public static string ThreadIdAttribute { get { return "id"; } }
    }

    public static partial class StringConstants
    {
        public static string ThreadToStringFormat { get { return "Thread {0} (time: {1})"; } }
        public static string MethodsListStart { get { return "Methods:"; } }
    }

    public static partial class ExceptionMessages
    {
        public static string CantPopExceptionMessage { get { return "Can't pop item from empty CallStack"; } }
    }
}
