using System.Text;

namespace ISO8583Display
{
    public class ArchiveFileHandlerEngine : IFileHandlerEngine
    {
        public object[] ReadFile(Stream stream, Encoding? encoding = null)
        {
            return Array.Empty<object>();
        }
    }
}
