using System;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0005 : PdsBase<Pds0005>
    {
        public override string PdsId => "0005";

        public override string PdsName => "Message Error Indicator";

        public override string PdsValue { get; set; } = string.Empty;

        public static new Pds0005 Parse(string data)
        {
            var res = new Pds0005();

            if (data.Length > 140)
                throw new ArgumentException("PDS 0005 expects data with a length less or equal to 140");

            res.PdsValue = data;



            return res;
        }



    }
}
