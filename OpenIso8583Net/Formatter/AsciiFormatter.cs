using System.Text;

namespace OpenIso8583Net.Formatter
{
    /// <summary>
    ///   ASCII Field Formatter
    /// </summary>
    public class AsciiFormatter : CodePageFormatter
    {
        public AsciiFormatter() : base(Encoding.ASCII) { }

    }
}