using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TracerLib
{
    class ThreadsListItem   // ToDo: add Time
    {
        public Stack<TraceTree> CallStack { get; set; }
        public List<TraceTree> CallTree { get; set; }   // List to keep several methods in MainThread
        public TimeSpan timeSpan{ get; set; }
        public long Time
        {
            get { return 1000*1000*timeSpan.Minutes + 1000*timeSpan.Seconds + timeSpan.Milliseconds; }
        }
    }
}
