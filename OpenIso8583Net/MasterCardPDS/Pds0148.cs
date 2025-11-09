using System;
using System.Collections.Generic;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0148 : PdsBase<Pds0148>
    {
        public override string PdsId => "0148";

        public override string PdsName => "Currency Exponents";

        public override string PdsValue { get; set; } = string.Empty;

        public static new Pds0148 Parse(string data)
        {
            var res = new Pds0148();

            if (data.Length != 4)
                throw new ArgumentException($"PDS {res.PdsId} expects data with a length of 4");

            res.PdsValue = data;

            res.CurrencyCode = int.Parse(data[..3]);
            res.CurrencyExponent = data[3];
            return res;
        }

        public static List<Pds0148> ParseAll(string data)
        {
            var res = new List<Pds0148>();

            if (data.Length % 4 != 0)
                throw new ArgumentException($"PDS 0148 expects data with a length that's a multiple of 4");

            for (int i = 0; i < data.Length; i += 4)
                res.Add(Pds0148.Parse(data.Substring(i, 4)));
            return res;
        }

        public int CurrencyCode { get; set; }
        public char CurrencyExponent { get; set; }
    }
}
