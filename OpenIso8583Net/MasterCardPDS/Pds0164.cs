using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0164List : PdsBase<List<Pds0164>>
    {
        public override string PdsId => "0164[]";

        public override string PdsName => "Currency Cross Rates";

        public override string PdsValue
        {
            get
            {
                return ToMsg();
            }
            set
            {
                _PdsDataList.Clear();
                _PdsDataList.AddRange(Pds0164.ParseAll(value));
            }
        }

        private readonly List<Pds0164> _PdsDataList = new();

        public static new Pds0164List Parse(string data)
        {
            var res = new Pds0164List();
            res.PdsValue = data;
            return res;
        }

        public new string ToMsg()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in _PdsDataList)
                sb.Append(d.ToMsg());
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            foreach (var d in _PdsDataList)
                builder.Append(d.ToString());
            return builder.ToString();
        }
    }

    public class Pds0164 : PdsBase<Pds0164>
    {
        public override string PdsId => "0164";

        public override string PdsName => "Currency Cross Rate";

        public override string PdsValue { get; set; } = string.Empty;

        public static new Pds0164 Parse(string data)
        {
            if (data.Length != 23)
                throw new InvalidDataException($"Data length should be 23 - got length of {data.Length}");

            var res = new Pds0164
            {
                PdsValue = data[..23],
                CurrentyCode = int.Parse(data[..3]),
                ConversionRate = FirstIndicatesPoint2Decimal(data[3..14]),
                ConversionType = data[14].ToString(),
                BusinessDate = DateOnly.ParseExact(data[15..21], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                DeliveryCycle = int.Parse(data[21..23])
            };
            return res;
        }

        public static List<Pds0164> ParseAll(string data)
        {
            if (data.Length % 23 != 0)
                throw new InvalidDataException($"Data length should be a multiple of 23 - got length of {data.Length}");
            var res = new List<Pds0164>();
            for (int pos = 0; pos < data.Length; pos += 23)
                res.Add(Parse(data.Substring(pos, 23)));
            return res;
        }

        public new string ToMsg()
        {
            if (CurrentyCode == 0
                || ConversionRate == 0.0m
                || ConversionType == null
                || BusinessDate == default(DateOnly)
                || DeliveryCycle == 0)
                throw new InvalidDataException("data is invalid! all values needed!");

            return $"{CurrentyCode:000}{Decimal2FirstIndicatesDecimalPoint(ConversionRate, 11)}{ConversionType![0]}{BusinessDate:yyMMdd}{DeliveryCycle:00}";
        }

        private static decimal FirstIndicatesPoint2Decimal(string from)
        {
            int _decimalPosition = (int)char.GetNumericValue(from[0]);
            if (_decimalPosition < 0 || _decimalPosition > 9)
                throw new InvalidDataException($"decimal point position indicator ({from[0]}) is not between 0 and 9 - {_decimalPosition}");
            if (from.Length < _decimalPosition + 1)
                throw new InvalidDataException($"input length ({from.Length}) too short to insert decimal at position {_decimalPosition}");

            // it's the number of places from the END of the string...
            _decimalPosition = from.Length - 1 - _decimalPosition;
            var val = string.Concat(from.Substring(1, _decimalPosition), ".", from[(_decimalPosition + 1)..]).Trim();

            if (decimal.TryParse(val, out decimal res))
                return res;

            throw new InvalidDataException($"input length ({from.Length}) too short to insert decimal at position {_decimalPosition}");
        }

        private static string Decimal2FirstIndicatesDecimalPoint(decimal val, int destinationLength)
        {
            string res = val.ToString(CultureInfo.InvariantCulture).PadLeft(destinationLength, '0');
            int _decimalPosition = res.IndexOf('.');
            return (destinationLength - _decimalPosition - 1).ToString() + res.Replace(".", "").PadRight(destinationLength - 1, '0');
        }

        public int CurrentyCode { get; set; }
        public decimal ConversionRate { get; set; }
        public string? ConversionType { get; set; }
        public DateOnly BusinessDate { get; set; }
        public int DeliveryCycle { get; set; }
    }
}
