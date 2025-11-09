using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISO8583Display
{
    /// <summary>
    /// Read variable Length Record, Blocked data. 
    /// </summary>
    /// <remarks>
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
    public class BlockedBinaryReader : IDisposable
    {


        private readonly BlockingType _blockingMode;
        private readonly int _blockSize = 1014;
        private readonly int _ignoreBytes = 2;

        private readonly Stream _stream;
        private readonly bool _leaveOpen;
        private bool _disposed;

        public BlockedBinaryReader(Stream input) : this(input, BlockingType.File, false)
        {
        }

        public BlockedBinaryReader(Stream input, BlockingType mode) : this(input, mode, false)
        {
        }

        public BlockedBinaryReader(Stream input, BlockingType mode, bool leaveOpen)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (!input.CanRead)
                throw new ArgumentException("Stream not readable!");

            _blockingMode = mode;
            _stream = input;
            _leaveOpen = leaveOpen;
        }

        public virtual Stream BaseStream => _stream;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && !_leaveOpen)
                {
                    _stream.Close();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <remarks>
        /// Override Dispose(bool) instead of Close(). This API exists for compatibility purposes.
        /// </remarks>
        public virtual void Close()
        {
            Dispose(true);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BaseStream));
            }
        }

#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity
        public virtual byte[] ReadBytes(int count)
#pragma warning restore S3776 // Refactor this method to reduce its Cognitive Complexity
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Need non-negative number");
            }
            ThrowIfDisposed();

            if (count == 0)
            {
                return Array.Empty<byte>();
            }
            byte[] result = new byte[count];

            int numRead = 0;
            for (int i = 0; i < count; i++)
            {
                while (_blockingMode == BlockingType.File && _stream.Position % _blockSize >= _blockSize - _ignoreBytes)
                {
                    int ignore = _stream.ReadByte();
                    if (ignore == -1)
                        break;
                }

                while (_blockingMode == BlockingType.MIP && _stream.Position % _blockSize < _ignoreBytes)
                {
                    // 0x03 0xF6
                    int ignore = _stream.ReadByte();
                    if (ignore == -1)
                        break;
                }

                int val = _stream.ReadByte();
                if (val == -1)
                    break;

                result[i] = (byte)val;
                numRead++;
            }

            if (numRead != result.Length)
            {
                // Trim array.  This should happen on EOF & possibly net streams.
                byte[] copy = new byte[numRead];
                Buffer.BlockCopy(result, 0, copy, 0, numRead);
                result = copy;
            }

            return result;
        }

    }
}
