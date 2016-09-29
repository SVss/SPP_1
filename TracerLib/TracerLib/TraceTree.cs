using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace TracerLib
{
    internal class TraceTree
    {
        private readonly Stopwatch _nodeStopwatch;

        // Public

        public List<TraceTree> Children { get; set; }
        public MethodInfo Info { get; set; }

        public TraceTree(MethodInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            Info = info;
            _nodeStopwatch = new Stopwatch();
            Children = new List<TraceTree>();
        }

        public void StartTimer()
        {
            _nodeStopwatch.Reset();
            _nodeStopwatch.Start();
        }

        public void StopTimer()
        {
            _nodeStopwatch.Stop();
            Info.Time = _nodeStopwatch.ElapsedMilliseconds;
        }

        public string ToString(int indentStart = 0, int indentStep = 1)
        {
            string result = string.Empty;
            for (int i = 0; i < indentStart; ++i)
            {
                result += " ";
            }

            if (Info.Method.ReflectedType != null)
            {
                object[] args = {
                    Info.Method.ReflectedType.Name,
                    Info.Method.Name,
                    Info.Method.GetParameters().Length,
                    Info.Time.ToString()
                };
                result += String.Format(StringConstants.MethodToStringFormat, args);
            }

            foreach (var child in Children)
            {
                result += Environment.NewLine + child.ToString(indentStart + indentStep, indentStep);
            }
            return result.TrimStart(Environment.NewLine.ToCharArray());
        }

        public XmlElement ToXmlElement(XmlDocument document)
        {
            XmlElement result = document.CreateElement(XmlConstants.MethodTag);
            result.SetAttribute(XmlConstants.NameAttribute, Info.Method.Name);
            result.SetAttribute(XmlConstants.TimeAttribute, Info.Time.ToString());

            string name = "method";
            if (Info.Method.ReflectedType != null)
            {
                name = Info.Method.ReflectedType.Name;
            }
            result.SetAttribute(XmlConstants.PackageAttribute, name);

            int paramsCount = Info.Method.GetParameters().Length;
            if (paramsCount > 0)
            {
                result.SetAttribute(XmlConstants.ParamsAttribute, paramsCount.ToString());
            }

            foreach (var child in Children)
            {
                result.AppendChild(child.ToXmlElement(document));
            }
            return result;
        }
    }

    // Constants

    public static partial class XmlConstants
    {
        public static string MethodTag => "method";
        public static string NameAttribute => "name";
        public static string ParamsAttribute => "params";
        public static string PackageAttribute => "package";
    }

    public static partial class StringConstants
    {
        public static string MethodToStringFormat => "{0}.{1}(paramsCount: {2}; time: {3})";
    }
}
