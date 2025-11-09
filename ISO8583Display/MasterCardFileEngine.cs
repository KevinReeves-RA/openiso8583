using OpenIso8583Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ISO8583Display
{
    public class MasterCardFileEngine : IFileHandlerEngine, IFileWriterEngine
#pragma warning restore S101 // Types should be named in PascalCase
    {
        public enum HeaderTypes
        {
            None,
            BankServ,
            MicroFocus
        }



        // 10k should do it for MasterCard because if all fields are full used (max lengths),
        // it comes to about 9k, then add RDW's, bitmaps and stuff
        private const int BUFFER_SIZE = 10240;

        public ILogger? Logger { get; set; } = null;


        public MasterCardFileEngine()
        {
            AutoDetectOnRead = true;
        }

        public MasterCardFileEngine(BlockingType mode) : this(mode, true, HeaderTypes.None) { }
        public MasterCardFileEngine(BlockingType mode, bool hasRDW) : this(mode, hasRDW, HeaderTypes.None) { }
        public MasterCardFileEngine(BlockingType mode, bool hasRDW, HeaderTypes headers)
        {
            AutoDetectOnRead = false;
            Mode = mode;
            RDW = (hasRDW) ? RDWType.Default : RDWType.None;
            Headers = headers;
        }

        public MasterCardFileEngine(BlockingType mode, RDWType rdw, HeaderTypes headers)
        {
            AutoDetectOnRead = false;
            Mode = mode;
            RDW = rdw;
            Headers = headers;
        }

        public RDWType RDW { get; set; } = RDWType.Default;

        public bool AutoDetectOnRead { get; set; }

        public HeaderTypes Headers { get; set; } = HeaderTypes.None;

        public BlockingType Mode { get; set; } = BlockingType.None;

        public Encoding DefaultEncoding { get; set; } = Encoding.GetEncoding(1140);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// added so we can grab just the header record(s) (e.g. Message ID 1644 DE24 697 / 695)
        /// </remarks>
        public int StopParsingAfterRecords { get; set; } = 0;

        /// <summary>
        /// changes a MicroFocus Cobol file structure to a normal RDW formatted data record, removing the Micro Focus specific stuff
        /// </summary>
        /// <remarks>
        /// see https://www.microfocus.com/documentation/visual-cobol/vc60/DevHub/HRFLRHFILE05.html
        /// </remarks>
#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity
        private object[] ReadMicrofocusFile(ref BlockedBinaryReader br, Encoding? encoding = null)
#pragma warning restore S3776
        {
            var result = new List<object>();
            encoding ??= DefaultEncoding;

            int recLen = 0;
            var rdw = new byte[4];

            br.BaseStream.Position = 0;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                rdw = br.ReadBytes(rdw.Length);

                int recType = ((int)rdw[0] >> 4); // get the first nibble of the first byte
                rdw[0] = (byte)((int)rdw[0] & 0x0F); // remove the first nibble, make it 0
                recLen = GetRDW(rdw, 0, encoding);
                if (recLen == 0)
                    break;

                var data = br.ReadBytes(recLen);
                switch (recType)
                {
                    case 1: // 0001 : A system record. This indicates a duplicate occurrence record in the data file.
                    case 2: // 0010 : Deleted record (available for reuse via the free space list).
                        break;
                    case 3: // 0011 : System record. (header)
                        // see : https://www.microfocus.com/documentation/visual-cobol/vc60/DevHub/HRFLRHFILE05.html
                        if (recLen != 124)
                            throw new InvalidDataException("invalid header length - expect 124"); // note could be header of C-ISAM files, not in scope here
                        // check the 'Organization' byte
                        // 1 = sequential
                        // 2 = indexed
                        // 3 = relative
                        if (data[39 - 4] != 1) // make sure it's sequential...
                            throw new InvalidDataException("header says the layout is not 'Variable Format Record Sequential' (header byte 39 != 1)");

                        // Data compression routine number:
                        // 0 = no compression
                        // 1 = CBLDC001
                        // 2-127 = Reserved for internal use
                        // 128-255 = User-defined compression routine number
                        if (data[41 - 4] != 0) // make sure no compression
                            throw new InvalidDataException("header says the Data compression is not off (header byte 41 != 0)");
                        break;

                    case 4: // 0100 : Normal user data
                        // parse the ISO8583 message
                        Iso8583MasterCard msg = new Iso8583MasterCard(encoding);
                        msg.Unpack(data, 0);
                        result.Add(msg);

                        // if we need to stop after reading a number of records...
                        if (StopParsingAfterRecords != 0 && result.Count >= StopParsingAfterRecords)
                            return result.ToArray();
                        break;


                    case 5:  // 0101 : Reduced user data record (indexed files only). 
                    case 6:  // 0110 : Pointer record (indexed files only).
                    case 7:  // 0111 : User data record referenced by a pointer record.
                    case 8:  // 1000 : Reduced user data record referenced by a pointer record.
                    case 9:  // 1001 : Reserved for future use
                    case 10: // 1010 : Mid-transaction user data record (As 4 except the record is from a File share transaction which has not yet been COMMITed)
                    case 11: // 1011 : Mid-transaction reduced user data record (as 5 except the record is from a File share transaction which has not yet been COMMITed)
                    case 12: // 1100 : Mid-transaction user data record referenced by a pointer record (as 7 except the record is from a File share transaction which has not yet been COMMITed)
                    case 13: // 1101 : Mid-transaction reduced user data record referenced by a pointer record (as 8 except the record is from a File share transaction which has not yet been COMMITed)
                    default:
                        // skip over the record
                        break;
                }

                // a record can have padding bytes at the end for a "Variable Format Record Sequential File"
                // "to ensure that the next record starts on a four-byte boundary"
                // basically they like to word align the data (probably because of file systems)
                if (br.BaseStream.Position % 4 > 0)
                    br.ReadBytes((int)((4 - (br.BaseStream.Position % 4)) % 4));
            }
            return result.ToArray();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <exception cref="InvalidDataException"></exception>
        /// <remarks>
        /// Does not take into account blocking 
        /// </remarks>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private object[] ReadBankservFile(ref BlockedBinaryReader br, Encoding? encoding = null)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            var result = new List<object>();
            encoding ??= DefaultEncoding;

            // does it have an RDW...
            byte[] rdw = br.ReadBytes(4);

            // check for a RDW....
            if (rdw[0] == 0x00 && rdw[1] == 0x00 && rdw[2] == 0x00 && rdw[3] == 180)
            {
                string header = encoding.GetString(br.ReadBytes(180));
                if (!header.StartsWith("01") || header[10..24].Trim() != "CARDMCI") // make sure it's a BankServ MasterCard file...
                    throw new InvalidDataException($"Bankserv header should start with 01 (received '{header[..2]}') and have CARDMCI in positions 10..25 (received '{header[10..24].Trim()}')");

                // get the message length from the RDW

                int msgLength = 180; // which was the header length...
                while (msgLength > 0)
                {
                    rdw = br.ReadBytes(4);

                    // get the RDW message length
                    msgLength = GetRDW(rdw, 0, encoding);
                    if (msgLength == 0)
                        break;

                    // get the record data
                    var data = br.ReadBytes(msgLength);

                    // if it's 180 bytes long, check if we have a footer record starting 98 or 99
                    if (msgLength == 180)
                    {
                        string recString = encoding.GetString(data);
                        // we've hit the end of the file... done...
                        if ((recString.StartsWith("98") || recString.StartsWith("99")) && recString[10..24].Trim() == "CARDMCI")
                            break;
                    }

                    // parse the ISO8583 message
                    Iso8583MasterCard msg = new Iso8583MasterCard(encoding);
                    msg.Unpack(data, 0);
                    result.Add(msg);

                    // if we need to stop after reading a number of records...
                    if (StopParsingAfterRecords != 0 && result.Count >= StopParsingAfterRecords)
                        break;
                }
                return result.ToArray();
            }

            br.BaseStream.Position = 0;
            long pos = 0;
            long filePos = 0;
            bool eof = false;
            // read the first 10k bytes
            var rawData = br.ReadBytes(BUFFER_SIZE);
            if (rawData.Length < BUFFER_SIZE)
                eof = true;

            // skip over the header record
            string header2 = encoding.GetString(rawData, 0, 180);
            if (header2.StartsWith("01") && header2[10..24].Trim() == "CARDMCI")
                pos = 180;


            while (pos < rawData.Length)
            {
                if (!eof && pos > 0)
                {
                    // read the next 10k bytes
                    filePos += pos;
                    pos = 0;
                    br.BaseStream.Position = filePos;
                    rawData = br.ReadBytes(BUFFER_SIZE);
                    if (rawData.Length < BUFFER_SIZE)
                        eof = true;
                }

                var msgData = new byte[rawData.Length - pos];
                Array.Copy(rawData, pos, msgData, 0, rawData.Length - pos);

                string recString = encoding.GetString(msgData, 0, 180);

                // do we have the footer...
                if ((recString.StartsWith("98") || recString.StartsWith("99")) && recString[10..24].Trim() == "CARDMCI")
                    break;

                var msg = new Iso8583MasterCard(encoding);
                int unpackedLen = msg.Unpack(msgData, 0);
                result.Add(msg);

                pos += unpackedLen;

                // if we need to stop after reading a number of records...
                if (StopParsingAfterRecords != 0 && result.Count >= StopParsingAfterRecords)
                    break;
            }
            return result.ToArray();
        }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public object[] ReadFile(Stream stream, Encoding? encoding = null)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            stream.Position = 0;
            if (AutoDetectOnRead)
            {
                BinaryReader detect = new BinaryReader(stream, Encoding.ASCII, true);
                DetectSettings(ref detect);
            }
            encoding ??= DefaultEncoding;

            // get the reader we're going to use
            BlockedBinaryReader br = new BlockedBinaryReader(stream, Mode, true);

            Logger?.LogTrace("Parsing ISO8583 stream {length}, with headers {headers}, blocking {blocking}, and has RDW {hasrdw}",
                stream.Length,
                Headers.ToString("G"),
                Mode.ToString("G"),
                RDW.ToString("G"));

            switch (Headers)
            {
                case HeaderTypes.BankServ:
                    return ReadBankservFile(ref br, encoding);
                case HeaderTypes.MicroFocus:
                    return ReadMicrofocusFile(ref br, encoding);
            }

            var res = new List<object>();

            int pos = 0;
            bool eof = false;
            // read the first 10k bytes
            var rawData = br.ReadBytes(BUFFER_SIZE);
            if (rawData.Length < BUFFER_SIZE)
                eof = true;

            while (pos < rawData.Length)
            {
                if (!eof && pos > 0)
                {
                    // remove the processed data, and top up the rawData to BUFFER_SIZE
                    var newRaw = new byte[BUFFER_SIZE];
                    Array.Copy(rawData, pos, newRaw, 0, rawData.Length - pos);
                    var newData = br.ReadBytes(pos);
                    if (newData.Length < pos)
                        eof = true;
                    Array.Copy(newData, 0, newRaw, rawData.Length - pos, newData.Length);
                    rawData = newRaw;
                    pos = 0;
                }

                // get the Record Descriptor Word (RDW) - 4 bytes with value of Record Length in binary
                long msgLength = -1;
                if (RDW != RDWType.None)
                {
                    msgLength = GetRDW(rawData, pos, encoding, RDW);
                    pos += 4;
                }

                // have we hit the end of the file?
                if (msgLength == 0)
                {
                    Logger?.LogTrace("RDW has length of 0, at {filePos} ({pos}), exiting parsing...", br.BaseStream.Position, pos);
                    pos += rawData.Length;
                    continue;
                }

                var msgData = new byte[rawData.Length - pos];
                Array.Copy(rawData, pos, msgData, 0, rawData.Length - pos);

                var msg = new Iso8583MasterCard(encoding);
                int unpackedLen = msg.Unpack(msgData, 0);

                pos += unpackedLen;
                res.Add(msg);
                if (StopParsingAfterRecords != 0 && res.Count >= StopParsingAfterRecords)
                    break;
            }

            return res.ToArray();
        }

        public static int GetRDW(byte[] data, int pos = 0, Encoding? encoding = null, RDWType rdwType = RDWType.Default)
        {
            if (data.Length < pos + 4)
                return 0;
            byte space = (encoding ?? Encoding.ASCII).GetBytes(" ")[0];
            if (data[pos] == space && data[pos + 1] == space && data[pos + 2] == space && data[pos + 3] == space)
                return 0;

            if (rdwType == RDWType.Microfocus)
                data[pos] = (byte)((int)data[0] & 0x0F); // remove the first nibble, make it 0

            if (rdwType == RDWType.LittleEndian)
                return (data[pos + 2] * 0x1000000) + (data[pos + 3] * 0x10000) + (data[pos + 0] * 0x100) + data[pos + 1] - 4;

            return (data[pos] * 0x1000000) + (data[pos + 1] * 0x10000) + (data[pos + 2] * 0x100) + data[pos + 3];
        }


#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity
        public void WriteStream(BinaryWriter writer, object[] data, Encoding? enc = null)
#pragma warning restore S3776
        {
            // ensure the encoding is set
            enc = enc ?? DefaultEncoding;

            var internalWriter = new RdwWriter(writer, enc, RDW, Mode, (Headers == HeaderTypes.MicroFocus || RDW == RDWType.Microfocus));

            switch (Headers)
            {
                case HeaderTypes.MicroFocus:
                    var mfHeader = new List<byte>();
                    mfHeader.AddRange(new byte[] { 0x30, 0x00, 0x00, 0x7C }); // standard header (0x30 system record, RDW length of 124)
                    mfHeader.AddRange(new byte[] { 0x00, 0x00 }); // Database sequence number, used by add-on products
                    mfHeader.AddRange(new byte[] { 0x00, 0x00 }); // Integrity flag. Indexed files only. If this is non-zero when the header is read, it indicates that the file is corrupt
                    // Creation date and time in YYMMDDHHMMSSCC format.
                    mfHeader.AddRange(enc.GetBytes(DateTime.Now.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture) + (DateTime.Now.Year / 100).ToString()));
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); // 14 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); // 14 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x00, 0x00 }); // 2 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x00 }); // 1 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x01 }); // Organization: 1 = sequential, 2 = indexed, 3 = relative
                    mfHeader.AddRange(new byte[] { 0x00 }); // 1 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x00 }); // Data compression routine number: 0 = none, 1=CBLDC001, 2-127 = reserved, 128-255 = user defined
                    mfHeader.AddRange(new byte[] { 0x00 }); // 1 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x01 }); // Recording mode: 0=Fixed format, 1=Variable format (For indexed files, the recording mode field of the .idx file takes precedence.)
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }); // 5 long - For internal use.
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x1F, 0xA0 }); // Maximum record length. Example: with a maximum record of length 80 characters, this field will contain x"00 00 00 50".
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x17 }); // Minimum record length. Example: with a minimum record length of 2 characters, this field will contain x"00 00 00 02".
                    // 46 long For internal use.
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00 }); // 4 long - Version and build data for the indexed file handler creating the file.
                    // 16 long For internal use.
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    mfHeader.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                    internalWriter.WriteRecord(mfHeader.ToArray(), true);
                    break;
                case HeaderTypes.BankServ:

                    // TODO: add Bankserv header
                    // ISSUE : need a way to determine TEST vs PROD header...
                    throw new NotImplementedException("BankServ Headers not yet implemented");
            }


            // add each record, adding the RDW as needed
            foreach (var o in data)
            {
                byte[]? message = (o is Iso8583MasterCard m) ? m.ToMsg(enc) : throw new InvalidDataException();
                internalWriter.WriteRecord(message, false);
            }



            // add the footers
            switch (Headers)
            {
                case HeaderTypes.MicroFocus:
                    // add null RDW
                    internalWriter.WriteRawData(new byte[4]);
                    // Fill file to the 8k mark with spaces
                    var space = enc.GetBytes(" ");
                    while (writer.BaseStream.Position % 8192 > 0)
                        internalWriter.WriteRawData(space);
                    break;
                case HeaderTypes.BankServ:
                    // TODO: add Bankserv footer
                    throw new NotImplementedException("Bankserv Footers not yet implemented");
            }

            // if we're doing blocking mip or file, make sure we pad to the 1014 position
            if (Mode != BlockingType.None)
            {
                // write a RDW of all nulls, so we know we're at the end...
                internalWriter.WriteRecord(new byte[4], true);

                // byte[] always initializes as nulls
                byte[] blockPadding = new byte[1014 - (writer.BaseStream.Position % 1014)];
                internalWriter.WriteRawData(blockPadding);
            }
            internalWriter.Close();
        }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private void DetectSettings(ref BinaryReader detect)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            detect.BaseStream.Position = 0;
            var detectBytes = detect.ReadBytes(1014 * 4); // enough to check 4 blocks if in blocking mode
            detect.BaseStream.Position = 0;

            // attempt to detect the blocking (if any) 
            // of blocking checks, chances are we have a blocked file (either MIP of FILE)
            List<BlockingType> modeDetect = new List<BlockingType>();
            for (int i = 0; i < detectBytes.Length / 1014; i++)
            {
                if (detectBytes[(1014 * i)] == 0x03 && detectBytes[(1014 * i) + 1] == 0xF6)
                    modeDetect.Add(BlockingType.MIP);

                if ((detectBytes[(1014 * i) + 1012] == 0x00 && detectBytes[(1014 * i) + 1013] == 0x00)     // null
                    || (detectBytes[(1014 * i) + 1012] == 0x20 && detectBytes[(1014 * i) + 1013] == 0x20)  // ascii
                    || (detectBytes[(1014 * i) + 1012] == 0x40 && detectBytes[(1014 * i) + 1013] == 0x40)) // ebcdic
                    modeDetect.Add(BlockingType.File);
            }
            if (modeDetect.Count > 0 && modeDetect.Count == detectBytes.Length / 1014)
            {
                if (modeDetect.TrueForAll(x => x == BlockingType.File))
                    Mode = BlockingType.File;
                if (modeDetect.TrueForAll(x => x == BlockingType.MIP))
                    Mode = BlockingType.MIP;
            }

            // try and detect encoding if there is none specified

            // to find out if we have ASCII or EBCDIC we check the first 40 bytes after any possible header.
            // ASCII number range 0x30 to 0x39 (decimal 48 to 57)
            // Ebcdic number range 0xF0 to 0xF9 (decimal 240 to 249)
            // 
            // so we look through the string of characters looking for the MasterCard header record type of 1644
            // we expect to find it within the first few characters after any header record
            // 
            // we check for EBCDIC fist as numbers fall into the ASCII extended range 0xF0 to 0xF9 the chances
            // of having these extended characters (ñööô in ascii) next to each other is small for ASCII encoded data.
            // also the ASCII range for numbers falls into control characters in Ebcdic
            string detectEncoding = Encoding.GetEncoding(1140).GetString(detectBytes);
            int hp = detectEncoding.IndexOf("1644"); // look for 1644 as it should be the header record message type
            if (hp == -1)
                hp = detectEncoding.IndexOf("1240"); // look for 1240 as it should be the header record message type
            if (hp >= 4 && hp < 180 + 40) // 4 bytes for the RDW, and 180 bytes for max header size (bankserv) + 40 bytes for luck
            {
                // chances are we have ebcdic, because the ASCII numbers fall into 
                DefaultEncoding = Encoding.GetEncoding(1140);
            }
            else
            {
                DefaultEncoding = Encoding.ASCII;
                detectEncoding = DefaultEncoding.GetString(detectBytes);
                hp = detectEncoding.IndexOf("1644"); // look for 1644 as it should be the header record message type
                if (hp == -1)
                    hp = detectEncoding.IndexOf("1240"); // look for 1240 as it should be the header record message type
            }
            if (hp == 4)
                RDW = RDWType.Default;

            // check for Bankserv headers
            string firstLine = DefaultEncoding.GetString(detectBytes);
            // check for a bankserv without a RDW (01yyyymmddCARDMCI       0066)
            if (firstLine.StartsWith("01") && firstLine[10..24].Trim() == "CARDMCI")
            {
                RDW = RDWType.None;
                Headers = HeaderTypes.BankServ;
                // if we have not done a full look into the file in terms of blocking (at least 4 iterations), set the blocking mode to none
                if (detectBytes.Length < (1014 * 4) && Mode != BlockingType.None)
                    Mode = BlockingType.None;
                return;
            }
            // check for a bankserv with a RDW (0x00, 0x00, 0x00, 0xB4 + "01yyyymmddCARDMCI       0066")
            if (detectBytes[0] == 0x00 && detectBytes[1] == 0x00 && detectBytes[2] == 0x00 && detectBytes[3] == 180 &&
                firstLine[4..6] == "01" && firstLine[14..28].Trim() == "CARDMCI")
            {
                RDW = RDWType.Default;
                Headers = HeaderTypes.BankServ;
                Mode = BlockingType.None;
                return;
            }

            // checked for Microfocus headers (first nibble = 0011, RDW length = 124)
            if (detectBytes[0] == 0x30 && detectBytes[1] == 0x00 && detectBytes[2] == 0x00 && detectBytes[3] == 0x7C)
            {
                RDW = RDWType.Microfocus;
                Headers = HeaderTypes.MicroFocus;
                return;
            }

            Headers = HeaderTypes.None;

            // try and figure out if we're little endian or not
            if (RDW != RDWType.None && detectBytes[2] == 0x00 && detectBytes[3] == 0x00)
                RDW = RDWType.LittleEndian;
        }

        public static MasterCardFileEngine GetMicrofocusEngine()
        {
            return new MasterCardFileEngine(BlockingType.None, true, HeaderTypes.MicroFocus) { DefaultEncoding = Encoding.ASCII };
        }

        public static MasterCardFileEngine GetBankservEngine()
        {
            return new MasterCardFileEngine(BlockingType.None, true, HeaderTypes.BankServ) { DefaultEncoding = Encoding.ASCII };
        }

        public static MasterCardFileEngine GetMasterCardEngine()
        {
            return new MasterCardFileEngine(BlockingType.File, true, HeaderTypes.None) { DefaultEncoding = Encoding.GetEncoding(1140) };
        }
    }

}
