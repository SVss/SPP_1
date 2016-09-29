using System;
using System.Reflection;

namespace TracerLib
{
    internal class MethodInfo
    {
        public MethodBase Method { get; set; }
        public long Time { get; set; }

        public MethodInfo(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            Method = method;
            Time = 0;
        }
    }
}
