using System;
using System.Globalization;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0300 : PdsBase<Pds0300>
    {
        public override string PdsId => "0300";

        public override string PdsName => "Reconciled, File";
        public static new Pds0300 Parse(string data)
        {
            var res = new Pds0300();

            if (data.Length != 25)
                throw new ArgumentException($"PDS expects data with a length of 25");

            res.PdsValue = data;
            res.Type = int.Parse(data[..3]);
            res.ReferenceDate = DateOnly.ParseExact(data[3..9], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            res.ProcessorID = long.Parse(data[9..20]);
            res.SequenceNumber = int.Parse(data[20..]);
            return res;
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
        public long ProcessorID { get; set; }
        public int SequenceNumber { get; set; }
        public override string PdsValue { get; set; } = string.Empty;

        public string Display { get { return ToString("D"); } }


    }
}
