using System;
using System.Globalization;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0110 : PdsBase<Pds0110>
    {
        public override string PdsId => "0110";

        public override string PdsName => "Transmission ID";

        public override string PdsValue { get; set; } = string.Empty;

        public static new Pds0110 Parse(string data)
        {
            var res = new Pds0110();

            if (data.Length != 25)
                throw new ArgumentException("PDS 0110 expects data with a length of 25");

            res.PdsValue = data;
            res.PdsValue = data;
            res.Type = int.Parse(data[..3]);
            res.ReferenceDate = DateOnly.ParseExact(data[3..9], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
            res.ProcessorID = long.Parse(data[9..20]);
            res.SequenceNumber = int.Parse(data[20..]);
            return res;
        }

        public int Type { get; set; }
        public DateOnly ReferenceDate { get; set; }
        public long ProcessorID { get; set; }
        public int SequenceNumber { get; set; }
    }
}
