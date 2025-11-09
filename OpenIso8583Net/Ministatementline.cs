using System;
using System.Collections.Generic;

namespace OpenIso8583Net
{
    /// <summary>
    ///   Class representing a Mini statement Line
    /// </summary>
    [Serializable]
    public class MinistatementLine : Dictionary<string, string?>
    {
        public MinistatementLine() : base() { }
        protected MinistatementLine(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
    }
}