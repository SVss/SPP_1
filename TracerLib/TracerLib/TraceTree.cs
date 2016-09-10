using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace TracerLib
{
    class TraceTree
    {
        const string TO_STRING_FORMAT = "{0}.{1}(paramsCount: {2}; time: {3})";

        private Stopwatch sw;

        public List<TraceTree> Children { get; set; }
        public MethodInfo Info { get; set; }

        public TraceTree(MethodInfo info)
        {
            sw = new Stopwatch();
            this.Info = info;
            this.Children = new List<TraceTree>();
        }

        public void startTimer()
        {
            sw.Reset();
            sw.Start();
        }

        public void stopTimer()
        {
            sw.Stop();
            //Console.WriteLine(sw.Elapsed.ToString());
            Info.Time = sw.ElapsedMilliseconds;
        }

        public string ToString(int indent)
        {
            string result = "";
            for (int i = 0; i < indent; ++i)
            {
                result += " ";
            }

            object[] args = new object[] {
                Info.Method.ReflectedType.Name,
                Info.Method.Name,
                Info.Method.GetParameters().Count(),
                Info.Time.ToString()
            };

            result += String.Format(TO_STRING_FORMAT, args);

            foreach (var child in Children)
            {
                result += "\n" + child.ToString(indent + 1);
            }

            return result;
        }

        public XmlElement ToXMLElement(XmlDocument root)
        {
            XmlElement result = root.CreateElement("method");
            result.SetAttribute("name", Info.Method.Name);
            result.SetAttribute("time", Info.Time.ToString());
            result.SetAttribute("package", Info.Method.ReflectedType.Name);

            int paramsCount = Info.Method.GetParameters().Count();
            if (paramsCount > 0)
                result.SetAttribute("params", paramsCount.ToString());

            foreach (var child in Children)
            {
                result.AppendChild(child.ToXMLElement(root));
            }
            return result;
        }
    }
}
