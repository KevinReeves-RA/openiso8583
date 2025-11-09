using System;
using System.Collections.Generic;
using System.Text;

namespace ISO8583Display
{
    /// <summary>
    /// writes records with a RDW 
    /// </summary>
    public class RdwWriter : TextWriter
    {
        private readonly Encoding _encoding;
        private readonly RDWType _rdw;
        private readonly BlockingType _blockingMode;
        private readonly bool _wordAlign;
        private readonly BinaryWriter _writer;

        public override Encoding Encoding { get { return _encoding; } }

        public RdwWriter(Stream strm, Encoding? encoding = null, RDWType rdw = RDWType.None, BlockingType blockingMode = BlockingType.None, bool wordAlign = false) : base()
        {
            _encoding = encoding ?? Encoding.ASCII;
            _rdw = rdw;
            _blockingMode = blockingMode;
            _wordAlign = wordAlign;
            _writer = new BinaryWriter(strm);
        }

        public RdwWriter(BinaryWriter writer, Encoding? encoding = null, RDWType rdw = RDWType.None, BlockingType blockingMode = BlockingType.None, bool wordAlign = false) : base()
        {
            _encoding = encoding ?? Encoding.ASCII;
            _rdw = rdw;
            _blockingMode = blockingMode;
            _wordAlign = wordAlign;
            _writer = writer;
        }

        public override void Write(string? value)
        {
            if (value == null) return;
            if (value.EndsWith(this.NewLine))
                WriteLine(value[..^NewLine.Length]);
            WriteLine(value);
        }

        public override void WriteLine(string? value)
        {
            if (value == null) return;
            WriteRecord(_encoding.GetBytes(value));
        }

        public void WriteRawData(byte[] data)
        {
            _writer.Write(data);
        }

        public void WriteRecord(byte[] record, bool hasRDW = false)
        {
            if (!hasRDW && _rdw != RDWType.None)
                WriteBlocking(CreateRDW(record.Length, _rdw), _writer, _blockingMode);
            WriteBlocking(record, _writer, _blockingMode);

            // if we need to word align the records...
            if (_wordAlign && _writer.BaseStream.Position % 4L > 0)
            {
                byte[] padding = Enumerable.Repeat((byte)0x00, (4 - (int)(_writer.BaseStream.Position % 4L)) % 4).ToArray();
                WriteBlocking(padding, _writer, _blockingMode);
            }
        }

        private static void WriteBlocking(byte[] buffer, BinaryWriter writer, BlockingType blockingMode)
        {
            if (blockingMode == BlockingType.None)
            {
                writer.Write(buffer);
                return;
            }

            // add each byte, adding the blocking bytes as needed
            int i = 0;
            while (i < buffer.Length)
            {
                int left = 1014 - (int)(writer.BaseStream.Position % 1014L);
                if (blockingMode == BlockingType.File)
                    left -= 2;

                // if we're blocking add blocking stuff as appropriate
                if (left == 0L && blockingMode == BlockingType.MIP)
                {
                    writer.Write(new byte[] { 0x03, 0xF6 });
                    left += 1012;
                }
                if (left == 0 && blockingMode == BlockingType.File)
                {
                    writer.Write(new byte[] { 0x00, 0x00 });
                    left += 1012;
                }

                if (buffer.LongLength - i < left)
                {
                    writer.Write(buffer[i..]);
                    return;
                }

                // add the next byte
                writer.Write(buffer[i..left]);
                i += (int)left;
            }
        }

        public static byte[] CreateRDW(int length, RDWType rdw)
        {
            if (rdw == RDWType.LittleEndian) // little endian includes the length of the RDW
                length += 4;
            byte[] intBytes = BitConverter.GetBytes(length);
            Array.Reverse(intBytes);
            if (rdw == RDWType.LittleEndian)
                intBytes = new byte[] { intBytes[2], intBytes[3], intBytes[0], intBytes[0] };

            if (rdw == RDWType.Microfocus)
            {
                if (intBytes[0] >= 0xf0)
                    throw new InvalidDataException("Message Data length exceeds the message length of the MicroFocus messages (the first nibble of the RDW header, has a value, which is shouldn't)");
                intBytes[0] = (byte)(0x40 + (intBytes[0] & 0x0F)); // 0100 (a data record id) + length
            }
            return intBytes;
        }

    }
}
