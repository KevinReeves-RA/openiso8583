using System;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0390 : PdsBase<Pds0390>
    {
        public override string PdsId => "0390";

        public override string PdsName => "Debits, Transaction Amount in Reconciliation Currency";

        public override string PdsValue { get; set; } = string.Empty;

        public static new Pds0390 Parse(string data)
        {
            var res = new Pds0390();

            res.DebitCredit = data[0];
            if (decimal.TryParse(data[1..], out decimal value))
                res.Amount = value;

            if (res.Amount.HasValue)
                res.Amount *= (decimal)Math.Pow(10, res.Decimals);

            return res;
        }

        private int _decimals = 2;
        public int Decimals
        {
            get { return _decimals; }
            set
            {
                if (Amount.HasValue && Amount.Value != default(decimal))
                    Amount *= (decimal)Math.Pow(10, _decimals);
                _decimals = value;
                Amount /= (decimal)Math.Pow(10, _decimals);
            }
        }

        public char? DebitCredit { get; set; }
        public decimal? Amount { get; set; }

    }
}
