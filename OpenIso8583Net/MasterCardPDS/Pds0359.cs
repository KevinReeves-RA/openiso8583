using System;
using System.Globalization;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0359 : PdsBase<Pds0359>
    {
        public override string PdsId => "0359";

        public override string PdsName => "Reconciled, Settlement Activity";

        public override string PdsValue { get; set; } = string.Empty;

        public static new Pds0359 Parse(string data)
        {
            var res = new Pds0359();
            if (data.Length < 67)
                throw new ArgumentException($"PDS {res.PdsId} expects data with a length of 67");

            res.PdsValue = data;
            res.AgentID = data[..11].Trim();
            res.AgentAccount = data[11..39].Trim();
            if (int.TryParse(data[39].ToString(), out int lvl))
                res.LevelCode = lvl;
            res.ServiceID = data[40..50].Trim();
            res.ExchangeRateClassCode = data[50].ToString().Trim();
            if (DateOnly.TryParseExact(data[51..57], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly rcd))
                res.ReconDate = rcd;
            if (int.TryParse(data[57..59], out int rc))
                res.ReconCycle = rc;
            if (DateOnly.TryParseExact(data[59..65], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly sd))
                res.SettlementDate = sd;
            if (int.TryParse(data[65..67], out int sc))
                res.SettlementCycle = sc;
            return res;
        }

        public string AgentID { get; set; } = string.Empty;
        public string AgentAccount { get; set; } = string.Empty;
        public int LevelCode { get; set; }
        public string ServiceID { get; set; } = string.Empty;
        public string ExchangeRateClassCode { get; set; } = string.Empty;
        public DateOnly? ReconDate { get; set; }
        public int ReconCycle { get; set; }
        public DateOnly? SettlementDate { get; set; }
        public int SettlementCycle { get; set; }

    }
}
