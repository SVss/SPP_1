using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TracerLib
{
    class MethodInfo
    {
        public System.Reflection.MethodBase Method { get; set; }
        public TimeSpan timeSpan { get; set; }
        public long Time
        {
            get { return 1000*1000*timeSpan.Minutes + 1000*timeSpan.Seconds + timeSpan.Milliseconds; }
        }

        public MethodInfo(System.Reflection.MethodBase method)
        {
            this.Method = method;
            this.timeSpan = new TimeSpan(0);
        }
    }
}
