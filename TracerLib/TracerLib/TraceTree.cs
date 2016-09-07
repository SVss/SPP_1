using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TracerLib
{
    class TraceTree
    {
        const string XML_METHOD_START = "<method name=\"{0}\" time=\"{1}\" package=\"{2}\"{3}>";
        const string XML_PARAMS_COUNT = " paramsCount=";
        const string XML_METHOD_END = "</method>";

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
            Info.Time = sw.Elapsed;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string ToXMLString()
        {
            long time = 1000*1000*Info.Time.Minutes + 1000*Info.Time.Seconds + Info.Time.Milliseconds;

            string paramsCountString = "";
            int paramsCount = Info.Method.GetParameters().Count();
            if (paramsCount > 0)
                 paramsCountString = XML_PARAMS_COUNT + "\"" + Info.Method.GetParameters().Count() + "\"";

            object[] args = new object[] { Info.Method.Name, time, Info.Method.ReflectedType.Name, paramsCountString };

            string result = String.Format(XML_METHOD_START, args);
            foreach (var child in Children)
            {
                result += "\n" + child.ToXMLString();
            }
            result += XML_METHOD_END;

            return result;
        }
    }
}
