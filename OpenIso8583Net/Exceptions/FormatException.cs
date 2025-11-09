using System;

namespace OpenIso8583Net.Exceptions
{
    /// <summary>
    /// Used to show a format exception in building the ISO message
    /// </summary>
    [Serializable]
    public class FormatException : Exception
    {
        /// <summary>
        /// Creates a new instance of the FormatException class
        /// </summary>
        /// <param name="message">Message to include in the exception</param>
        public FormatException(string message)
            : base(message)
        {
        }

        protected FormatException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }

    }
}