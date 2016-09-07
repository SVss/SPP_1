using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TracerLib
{
    class MethodInfo
    {
        public String Name { get; set; }
        public String ClassName { get; set; }
        public int ArgsCount { get; set; }
        public long Time { get; set; }

        public int ThreadId { get; set; }

        public MethodInfo()
        {
            this.Time = 0;
        }
    }
}
