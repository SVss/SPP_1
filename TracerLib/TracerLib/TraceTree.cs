using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace TracerLib
{
    class TraceTree
    {
        private Stopwatch NodeStopwatch;

        // Public

        public List<TraceTree> Children { get; set; }
        public MethodInfo Info { get; set; }

        public TraceTree(MethodInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.Info = info;
            NodeStopwatch = new Stopwatch();
            this.Children = new List<TraceTree>();
        }

        public void StartTimer()
        {
            NodeStopwatch.Reset();
            NodeStopwatch.Start();
        }

        public void StopTimer()
        {
            NodeStopwatch.Stop();
            Info.Time = NodeStopwatch.ElapsedMilliseconds;
        }

        public string ToString(int indentStart = 0, int indentStep = 1)
        {
            string result = String.Empty;
            for (int i = 0; i < indentStart; ++i)
            {
                result += " ";
            }

            object[] args = new object[] {
                Info.Method.ReflectedType.Name,
                Info.Method.Name,
                Info.Method.GetParameters().Count(),
                Info.Time.ToString()
            };
            result += String.Format(StringConstants.MethodToStringFormat, args);

            foreach (var child in Children)
            {
                result += Environment.NewLine + child.ToString(indentStart + indentStep, indentStep);
            }
            return result.TrimStart(Environment.NewLine.ToCharArray());
        }

        public XmlElement ToXMLElement(XmlDocument document)
        {
            XmlElement result = document.CreateElement(XmlConstants.MethodTag);
            result.SetAttribute(XmlConstants.NameAttribute, Info.Method.Name);
            result.SetAttribute(XmlConstants.TimeAttribute, Info.Time.ToString());
            result.SetAttribute(XmlConstants.PackageAttribute, Info.Method.ReflectedType.Name);

            int paramsCount = Info.Method.GetParameters().Count();
            if (paramsCount > 0)
            {
                result.SetAttribute(XmlConstants.ParamsAttribute, paramsCount.ToString());
            }

            foreach (var child in Children)
            {
                result.AppendChild(child.ToXMLElement(document));
            }
            return result;
        }
    }

    // Constants

    public static partial class XmlConstants
    {
        public static string MethodTag { get { return "method"; } }
        public static string NameAttribute { get { return "name"; } }
        public static string ParamsAttribute { get { return "params"; } }
        public static string PackageAttribute { get { return "package"; } }
    }

    public static partial class StringConstants
    {
        public static string MethodToStringFormat { get { return "{0}.{1}(paramsCount: {2}; time: {3})"; } }
    }
}
