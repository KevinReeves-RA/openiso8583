using System;

namespace OpenIso8583Net.Exceptions
{
    /// <summary>
    ///   Exception class for an incorrectly formatted field
    /// </summary>
    [Serializable]
    public class FieldFormatException : FormatException
    {
        /// <summary>
        ///   Create a new instance of the FieldFormatException class
        /// </summary>
        /// <param name = "fieldNumber"></param>
        /// <param name = "message"></param>
        public FieldFormatException(int fieldNumber, string message)
            : this("", fieldNumber, message)
        {

        }

        /// <summary>
        ///   Create a new instance of the FieldFormatException class
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name = "fieldNumber"></param>
        /// <param name = "message"></param>
        public FieldFormatException(string prefix, int fieldNumber, string message)
            : base("Field Number : " + prefix + fieldNumber + Environment.NewLine + message)
        {
            FieldNumber = fieldNumber;
        }

        protected FieldFormatException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }

        /// <summary>
        ///   Field number that the exception applies to
        /// </summary>
        public int FieldNumber { get; private set; }
    }
}