using System.Text;

namespace OpenIso8583Net.Formatter
{
    public class CodePageFormatter : IFormatter
    {
        public Encoding Encoding { get; set; }

        public CodePageFormatter(Encoding encoding)
        {
            Encoding = encoding;
        }

        public CodePageFormatter(int codePage)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding = Encoding.GetEncoding(codePage);
        }

        public byte[] GetBytes(string value)
        {
            return Encoding.GetBytes(value);
        }

        public int GetPackedLength(int unpackedLength)
        {
            return unpackedLength;
        }

        public string GetString(byte[] data)
        {
            return Encoding.GetString(data);
        }
    }
}
