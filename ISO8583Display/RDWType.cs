using System;
using System.Collections.Generic;
using System.Text;

namespace ISO8583Display
{
    /// <summary>
    /// Record Data Word (RDW) indicates how the 
    /// </summary>
#pragma warning disable S2342 // Rename this enumeration to match the regular expression: '^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$'
    public enum RDWType
#pragma warning restore S2342 // Rename this enumeration to match the regular expression: '^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$'
    {
        None,
        Default,
        LittleEndian,
        BigEndian,
        Microfocus
    }
}
