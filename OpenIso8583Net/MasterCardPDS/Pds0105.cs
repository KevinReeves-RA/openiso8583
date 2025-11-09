using System;
using System.Globalization;

namespace OpenIso8583Net.MasterCardPDS
{

    public class Pds0105 : PdsBase<Pds0105>
    {
        public override string PdsId => "0105";
        public override string PdsName => "File ID";

        public static new Pds0105 Parse(string data)
        {
            var res = new Pds0105();

            // we have it in display format.... "002/23-06-04/00000031669/00095"
            if (data.Length == 30 && data[3] == '/' && data[12] == '/' && data[^6] == '/' && data[6] == '-' && data[9] == '-')
            {
                res.Type = int.Parse(data[..3]);
                res.ReferenceDate = DateOnly.ParseExact(data[4..12], "yy-MM-dd", CultureInfo.InvariantCulture);
                res.ProcessorID = long.Parse(data[13..24]);
                res.SequenceNumber = int.Parse(data[^5..]);
                res.PdsValue = res.ToMsg();
                return res;
            }

            if (data.Length != 25)
                throw new ArgumentException($"PDS expects data with a length of 25");

            res.PdsValue = data;
            res.Type = int.Parse(data[..3]);
            res.ReferenceDate = DateOnly.ParseExact(data[3..9], "yyMMdd", CultureInfo.InvariantCulture);
            res.ProcessorID = long.Parse(data[9..20]);
            res.SequenceNumber = int.Parse(data[20..]);
            return res;
        }

        public new string ToMsg()
        {
            if (Type > 999)
                throw new ArgumentException("Type needs to be max 3 digits");
            if (ReferenceDate < new DateOnly(2000, 1, 1))
                throw new ArgumentException("ReferenceDate needs to be after 1 Jan 2000");
            if (ProcessorID < 0 || ProcessorID > 99999999999)
                throw new ArgumentException("ProcessorID needs to be between 0 and 99999999999");
            if (SequenceNumber < 0 || SequenceNumber > 99999)
                throw new ArgumentException("SequenceNumber needs to be between 0 and 99999");

            return $"{Type:000}{ReferenceDate:yyMMdd}{ProcessorID:00000000000}{SequenceNumber:00000}";
        }


        public string ToString(string format = "D")
        {
            return format switch
            {
                "D" or "d" => $"{Type:000}/{ReferenceDate:yy-MM-dd}/{ProcessorID:00000000000}/{SequenceNumber:00000}",
                _ => ToString(),
            };
        }

        public int Type { get; set; }
        public DateOnly ReferenceDate { get; set; }
        public long ProcessorID { get; set; } = 31669; // Retail Assist's processor id
        public int SequenceNumber { get; set; }
        public override string PdsValue { get; set; } = string.Empty;
        public string Display { get { return ToString("D"); } }
    }
}
