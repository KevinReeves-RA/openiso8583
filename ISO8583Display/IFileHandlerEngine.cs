using System.Text;

namespace ISO8583Display
{
    public interface IFileHandlerEngine
    {
        public object[] ReadFile(string file, Encoding? encoding = null)
        {
            return ReadFile(File.OpenRead(file), encoding);
        }

        public object[] ReadFile(Stream stream, Encoding? encoding = null);

        public object[] ReadString(string data, Encoding? encoding = null)
        {
            encoding ??= Encoding.ASCII;
            using var strm = new MemoryStream(encoding.GetBytes(data));
            return ReadFile(strm, encoding);
        }
    }
}
