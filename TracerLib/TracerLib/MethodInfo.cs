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
        public long Time { get; set; }

        public MethodInfo(System.Reflection.MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            this.Method = method;
            this.Time = 0;
        }
    }
}
