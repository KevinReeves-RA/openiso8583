using System;

namespace OpenIso8583Net.Exceptions
{
    /// <summary>
    ///   This exception is thrown when the field that is being created is unknown
    /// </summary>
    [Serializable]
    public class UnknownFieldException : Exception
    {
        /// <summary>
        ///   Create a new instance of the UnknownFieldException class
        /// </summary>
        /// <param name = "fieldNumber">Field number that was created</param>
        public UnknownFieldException(string fieldNumber)
            : base("Field " + fieldNumber + " is unknown")
        {
            FieldNumber = fieldNumber;
        }

        protected UnknownFieldException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            FieldNumber = string.Empty;
        }

        /// <summary>
        ///   Field number that was attempted to be created
        /// </summary>
        public string FieldNumber { get; set; }


    }
}