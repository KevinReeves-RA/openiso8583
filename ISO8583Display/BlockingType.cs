using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO8583Display
{
    /// <summary>
    /// Blocking Type
    /// </summary>
    /// <remarks>
    /// Variable Length Record - None
    ///           |<- Record 1 ->|         |<- rec 2 ->|         |<- rec 3 ->|
    /// +---------+--------------+---------+-----------+---------+-----------+
    /// | 4 Bytes |              | 4 Bytes | 906 bytes | 4 bytes | 350 bytes |
    /// | of RDW  | 1108 bytes   | of RDW  |  of data  | of RDW  |  of data  |
    /// |   1     |   of data    |   2     |           |   3     |           |
    /// +---------+--------------+---------+-----------+---------+-----------+
    /// 
    /// Variable Length Record - Blocking on File
    /// 
    ///           |<--- Record 1 ---------------------->|         |<- rec 2 ->|                              |<- rec 3 ->|
    /// +---------+--------------+--------+  +----------+---------+-----------+---------+--------+  +--------+-----------+--------+--------+--------+
    /// | 4 Bytes |              |   2    |  | 99 bytes | 4 Bytes | 906 bytes | 3 bytes |   2    |  | 1 byte | 350 bytes | 4 byte | 655    |    2   |
    /// | of RDW  | 1008 bytes   | unused |  | of data  | of RDW  |  of data  | of RDW  | unused |  |  of    |    of     |  EOD   | unused | unused |
    /// |   1     |   of data    | bytes  |  |          |   2     |           |    3    |  bytes |  | RDW 3  |   data    | Mark   | bytes  |  bytes |
    /// +---------+--------------+--------+  +----------+---------+-----------+---------+--------+  +--------+-----------+--------+--------+--------+
    /// |<-1012 byte data block->|        |  |<--- 1012 byte data block --------------->|        |  |<--- 1012 byte data block ----------->|        |
    /// |<------- 1014 byte block ------->|  |<-------- 1014 byte block ------------------------>|  |<-------- 1014 byte block -------------------->|
    ///
    /// 
    /// Variable Length Record - Blocking on MIP
    /// 
    ///                       |<--- Record 1 ----------->|        |<- rec 2 ->|                                 |<- rec 3 ->|
    /// +-----------+---------+------------+  +----------+--------+-----------+---------+  +-----------+--------+-----------+--------+---------+
    /// | 2 bytes   | 4 bytes |            |  |          | 4 byte | 906 bytes | 3 bytes |  | 2 bytes   | 1 byte | 350 bytes | 4 byte |   657   |
    /// | MIP Block | of RDW  | 1008 bytes |  | 99 bytes |  RDW   |  of data  | of RDW  |  | MIP Block | of RDW |    of     |  EOD   | unused  |
    /// | Length    |    1    |   of data  |  | of data  |   2    |           |   3     |  | Length    |   3    |   data    | Marker |  bytes  |
    /// +-----------+---------+------------+  +----------+--------+-----------+---------+  +-----------+--------+-----------+--------+---------+
    /// |           |<-- 1012 byte data -->|  |          |<--- 1012 byte data block --->|  |           |<------- 1012 byte data block -------->|
    /// |<---- 1014 byte block ----------->|  |<---- 1014 byte block ------------------>|  |<---- 1014 byte block ---------------------------->|
    /// </remarks>
    public enum BlockingType
    {
        /// <summary>
        /// no blocking applied
        /// </summary>
        /// <remarks>
        ///           |<- Record 1 ->|         |<- rec 2 ->|         |<- rec 3 ->|
        /// +---------+--------------+---------+-----------+---------+-----------+
        /// | 4 Bytes |              | 4 Bytes | 906 bytes | 4 bytes | 350 bytes |
        /// | of RDW  | 1108 bytes   | of RDW  |  of data  | of RDW  |  of data  |
        /// |   1     |   of data    |   2     |           |   3     |           |
        /// +---------+--------------+---------+-----------+---------+-----------+
        /// </remarks>
        None,

        /// <summary>
        /// File blocking (1014 byte blocks with 2 unused bytes at the end)
        /// </summary>
        /// <remarks>
        ///           |<--- Record 1 ---------------------->|         |<- rec 2 ->|                              |<- rec 3 ->|
        /// +---------+--------------+--------+  +----------+---------+-----------+---------+--------+  +--------+-----------+--------+--------+--------+
        /// | 4 Bytes |              |   2    |  | 99 bytes | 4 Bytes | 906 bytes | 3 bytes |   2    |  | 1 byte | 350 bytes | 4 byte | 655    |    2   |
        /// | of RDW  | 1008 bytes   | unused |  | of data  | of RDW  |  of data  | of RDW  | unused |  |  of    |    of     |  EOD   | unused | unused |
        /// |   1     |   of data    | bytes  |  |          |   2     |           |    3    |  bytes |  | RDW 3  |   data    | Mark   | bytes  |  bytes |
        /// +---------+--------------+--------+  +----------+---------+-----------+---------+--------+  +--------+-----------+--------+--------+--------+
        /// |<-1012 byte data block->|        |  |<--- 1012 byte data block --------------->|        |  |<--- 1012 byte data block ----------->|        |
        /// |<------- 1014 byte block ------->|  |<-------- 1014 byte block ------------------------>|  |<-------- 1014 byte block -------------------->|
        /// </remarks>
        File,

        /// <summary>
        /// MIP blocking (1014 byte blocks with 2 bytes at the start 0x03F6 - which is 1014)
        /// </summary>
        /// <remarks>
        ///                       |<--- Record 1 ----------->|        |<- rec 2 ->|                                 |<- rec 3 ->|
        /// +-----------+---------+------------+  +----------+--------+-----------+---------+  +-----------+--------+-----------+--------+---------+
        /// | 2 bytes   | 4 bytes |            |  |          | 4 byte | 906 bytes | 3 bytes |  | 2 bytes   | 1 byte | 350 bytes | 4 byte |   657   |
        /// | MIP Block | of RDW  | 1008 bytes |  | 99 bytes |  RDW   |  of data  | of RDW  |  | MIP Block | of RDW |    of     |  EOD   | unused  |
        /// | Length    |    1    |   of data  |  | of data  |   2    |           |   3     |  | Length    |   3    |   data    | Marker |  bytes  |
        /// +-----------+---------+------------+  +----------+--------+-----------+---------+  +-----------+--------+-----------+--------+---------+
        /// |           |<-- 1012 byte data -->|  |          |<--- 1012 byte data block --->|  |           |<------- 1012 byte data block -------->|
        /// |<---- 1014 byte block ----------->|  |<---- 1014 byte block ------------------>|  |<---- 1014 byte block ---------------------------->|
        /// </remarks>
        MIP
    }
}
