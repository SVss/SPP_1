using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace TracerLib
{
    class TraceTree
    {
        private const string MethodToStringFormat = "{0}.{1}(paramsCount: {2}; time: {3})";
        private const string MethodTag = "method";
        private const string NameAttribute = "name";
        private const string TimeAttribute = "time";
        private const string ParamsAttribute = "params";
        private const string PackageAttribute = "package";

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
            result += String.Format(MethodToStringFormat, args);

            foreach (var child in Children)
            {
                result += Environment.NewLine + child.ToString(indentStart + indentStep, indentStep);
            }
            return result.TrimStart(Environment.NewLine.ToCharArray());
        }

        public XmlElement ToXMLElement(XmlDocument document)
        {
            XmlElement result = document.CreateElement(MethodTag);
            result.SetAttribute(NameAttribute, Info.Method.Name);
            result.SetAttribute(TimeAttribute, Info.Time.ToString());
            result.SetAttribute(PackageAttribute, Info.Method.ReflectedType.Name);

            int paramsCount = Info.Method.GetParameters().Count();
            if (paramsCount > 0)
            {
                result.SetAttribute(ParamsAttribute, paramsCount.ToString());
            }

            foreach (var child in Children)
            {
                result.AppendChild(child.ToXMLElement(document));
            }
            return result;
        }
    }
}
