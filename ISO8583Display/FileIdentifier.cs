using System;
using System.Collections.Generic;

namespace ISO8583Display
{


    public class FileIdentifier
    {
        public string UATNameStartsWith { get; set; } = string.Empty;
        public string ProdNameStartsWith { get; set; } = string.Empty;
        public IFileHandlerEngine? Engine { get; set; }
    }
}
