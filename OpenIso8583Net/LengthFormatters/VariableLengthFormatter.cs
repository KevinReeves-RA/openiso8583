using OpenIso8583Net.Formatter;
using System;

namespace OpenIso8583Net.LengthFormatters
{
    /// <summary>
    ///   Variable length formatter
    /// </summary>
    public class VariableLengthFormatter : ILengthFormatter
    {
        private readonly int _maxLength;

        /// <summary>
        ///   Variable length field formatter
        /// </summary>
        /// <param name = "lengthIndicator">Length of the length indicator</param>
        /// <param name = "maxLength">Maximum length of the field</param>
        /// <param name = "lengthFormatter">The field formatter used to pack the field</param>
        public VariableLengthFormatter(int lengthIndicator, int maxLength, IFormatter lengthFormatter)
        {
            _maxLength = maxLength;
            LengthFormatter = lengthFormatter;
            LengthOfLengthIndicator = LengthFormatter.GetPackedLength(lengthIndicator);
        }

        public IFormatter LengthFormatter { get; set; }

        #region ILengthFormatter Members

        /// <summary>
        ///   Get the length of the packed length indicator
        /// </summary>
        public int LengthOfLengthIndicator { get; private set; }

        /// <summary>
        ///   The maximum length of the field displayed as a string for descriptors
        /// </summary>
        public string MaxLength
        {
            get { return ".." + _maxLength; }
        }

        /// <summary>
        ///   Descriptor for the length formatter used in ToString methods
        /// </summary>
        public string Description
        {
            get
            {
                var places = (int)Math.Log10(_maxLength);
                return new string('L', 1 + places) + "Var";
            }
        }

        /// <summary>
        ///   Get the length of the field
        /// </summary>
        /// <param name = "msg">Byte array of message data</param>
        /// <param name = "offset">offset to start parsing</param>
        /// <returns>The length of the field</returns>
        public int GetLengthOfField(byte[] msg, int offset)
        {
            var len = LengthOfLengthIndicator;
            var lenData = new byte[len];
            Array.Copy(msg, offset, lenData, 0, len);
            var lenStr = LengthFormatter.GetString(lenData);
            if (lenData[0] == 0 && lenData[1] == 0)
                return 0;
            return int.Parse(lenStr);
        }

        /// <summary>
        ///   Pack the length header into the message
        /// </summary>
        /// <param name = "msg">Byte array of the message</param>
        /// <param name = "length">The length to pack into the message</param>
        /// <param name = "offset">Offset to start the packing</param>
        /// <returns>offset for the start of the field</returns>
        public int Pack(byte[] msg, int length, int offset)
        {
            var lengthStr = length.ToString().PadLeft(LengthOfLengthIndicator, '0');
            var header = LengthFormatter.GetBytes(lengthStr);
            Array.Copy(header, 0, msg, offset, LengthOfLengthIndicator);
            return offset + LengthOfLengthIndicator;
        }

        /// <summary>
        ///   Check the length of the field is valid
        /// </summary>
        /// <param name = "packedLength">the packed length of the field</param>
        /// <returns>true if valid, false otherwise</returns>
        public bool IsValidLength(int packedLength)
        {
            return packedLength <= _maxLength;
        }

        #endregion
    }
}