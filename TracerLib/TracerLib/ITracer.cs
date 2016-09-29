using System.Xml;

namespace TracerLib
{
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        XmlDocument BuildXml();
        void PrintToConsole();
    }
}