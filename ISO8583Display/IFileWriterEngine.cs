using System.Text;

namespace ISO8583Display
{
    public interface IFileWriterEngine
    {
        public void WriteStream(BinaryWriter writer, object[] data, Encoding? enc = null);
    }
}
