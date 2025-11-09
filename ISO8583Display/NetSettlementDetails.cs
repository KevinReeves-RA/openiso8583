using OpenIso8583Net;
using OpenIso8583Net.MasterCardPDS;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISO8583Display
{
    public class NetSettlementDetails
    {
        /// <summary>
        /// the File ID
        /// </summary>
        public string? MasterCardFile { get; set; }

        /// <remarks>PDS 0359 S1</remarks>
        public string? AgentID { get; set; }

        /// <remarks>PDS 0359 S2</remarks>
        public string? AgentAccount { get; set; }

        /// <remarks>PDS 0359 S3</remarks>
        public int? LevelCode { get; set; }

        /// <remarks>PDS 0359 S5</remarks>
        public string? ServiceID { get; set; }

        /// <remarks>PDS 0359 S5</remarks>
        public string? ExchangeRateClass { get; set; }

        /// <summary>
        /// Reconciliation Date
        /// </summary>
        /// <remarks>PDS 0359 S6</remarks>
        public DateOnly? ReconDate { get; set; }

        /// <summary>Reconciliation Cycle</summary>
        /// <remarks>PDS 0359 S7</remarks>
        public int? ReconCycle { get; set; }

        /// <summary>Settlement Date </summary>
        /// <remarks>PDS 0359 S8</remarks>
        public DateOnly? SettlementDate { get; set; }

        /// <summary>Settlement Cycle</summary>
        /// <remarks>PDS 0359 S9</remarks>
        public int? SettlementCycle { get; set; }

        /// <summary>
        /// related reconciliation totals represent acquiring activity, issuing activity, or fee collection activity
        /// </summary>
        public string? Activity { get; set; }

        /// <summary>
        /// the settlement currency
        /// </summary>
        /// <remarks>PDS 0148 S1</remarks>
        public int? Currency { get; set; }

        /// <summary>
        /// the number of decimals for the settlement currency
        /// </summary>
        /// <remarks>PDS 0148 S2</remarks>
        public int? CurrencyDecimals { get; set; }

        /// <summary>
        ///  total of transaction amounts that have a debit impact on processing
        /// </summary>
        /// <remarks>PDS 0390</remarks>
        public decimal? DebitTransactionAmount { get; set; }

        /// <summary>
        /// the total of transaction amounts that have a credit impact on processing.
        /// </summary>
        /// <remarks>PDS 0391</remarks>
        public decimal? CreditTransactionAmount { get; set; }

        /// <summary>
        /// total of fee amounts that have a debit impact on processing.
        /// </summary>
        /// <remarks>PDS 0392</remarks>
        public decimal? DebitFeeAmount { get; set; }
        public string? DebitFeeType { get; set; }

        /// <summary>
        ///  total of fee amounts that have a credit impact on processing
        /// </summary>
        /// <remarks>PDS 0393</remarks>
        public decimal? CreditFeeAmount { get; set; }
        public string? CreditFeeType { get; set; }

        /// <summary>
        /// net total of the transaction amounts that have a debit impact on processing and those that have a credit impact on processing
        /// </summary>
        /// <remarks>PDS 0394</remarks>
        public decimal? AmountNetTransaction { get; set; }


        /// <summary>
        /// total of fee amounts that have a debit impact on processing and those that have a credit impact on processing
        /// </summary>
        /// <remarks>PDS 0395</remarks>
        public decimal? AmountNetFee { get; set; }


        /// <summary>
        /// total of transaction and fee amounts
        /// </summary>
        /// <remarks>PDS 0396</remarks>
        public decimal? AmountNetTotal { get; set; }

        public static NetSettlementDetails FromMasterCardMessage(Iso8583MasterCard mc)
        {
            NetSettlementDetails res = new();


            if (mc.PDSFields.ContainsKey("0148") && int.TryParse(mc.PDSFields["0148"], out int curr))
            {
                res.Currency = curr / 10;
                res.CurrencyDecimals = curr % 10;
            }

            res.MasterCardFile = (mc.PDSFields.ContainsKey("0300")) ? mc.PDSFields["0300"] : null;
            // reformat into the pretty format 000/yyyy-MM-dd/00000031669/seqno
            try
            {
                if (!string.IsNullOrWhiteSpace(res.MasterCardFile) && !res.MasterCardFile.Contains('/'))
                    res.MasterCardFile = Pds0300.Parse(res.MasterCardFile).ToString();
            }
            catch { /* do nothing, leave value as is */ }

            res.Activity = (mc.PDSFields.ContainsKey("0302")) ? mc.PDSFields["0302"] : null;

            if (mc.PDSFields.ContainsKey("0359"))
            {
                var sd = Pds0359.Parse(mc.PDSFields["0359"]);
                res.AgentID = sd.AgentID;
                res.AgentAccount = sd.AgentAccount;
                res.LevelCode = sd.LevelCode;
                res.ServiceID = sd.ServiceID;
                res.ExchangeRateClass = sd.ExchangeRateClassCode;
                res.ReconDate = sd.ReconDate;
                res.SettlementDate = sd.SettlementDate;
                res.ReconCycle = sd.ReconCycle;
                res.SettlementCycle = sd.SettlementCycle;
            }

            res.DebitTransactionAmount = GetAmount(mc, "0390", res.CurrencyDecimals!.Value);
            res.CreditTransactionAmount = GetAmount(mc, "0391", res.CurrencyDecimals!.Value);
            res.DebitFeeAmount = GetAmount(mc, "0392", res.CurrencyDecimals!.Value);
            res.DebitFeeType = (mc.PDSFields.ContainsKey("0392")) ? mc.PDSFields["0392"][..2] : null;
            res.CreditFeeAmount = GetAmount(mc, "0393", res.CurrencyDecimals!.Value);
            res.CreditFeeType = (mc.PDSFields.ContainsKey("0393")) ? mc.PDSFields["0393"][..2] : null;
            res.AmountNetTransaction = GetAmount(mc, "0394", res.CurrencyDecimals!.Value);
            res.AmountNetFee = GetAmount(mc, "0395", res.CurrencyDecimals!.Value);
            res.AmountNetTotal = GetAmount(mc, "0396", res.CurrencyDecimals!.Value);

            return res;
        }

        private static decimal? GetAmount(Iso8583MasterCard mc, string pds, int decimalPoints)
        {
            if (!mc.PDSFields.ContainsKey(pds))
                return null;
            return GetAmount(mc.PDSFields[pds], decimalPoints);
        }

        private static decimal? GetAmount(string? data, int decimalPoints)
        {
            if (data == null)
                return null;

            string amtPart = data;
            if (amtPart.Contains('C'))
                amtPart = amtPart.Substring(amtPart.IndexOf('C') + 1);
            if (amtPart.Contains('D'))
                amtPart = amtPart.Substring(amtPart.IndexOf('D') + 1);

            if (decimal.TryParse(amtPart, out decimal amount))
                return amount / (decimal)Math.Pow(10, decimalPoints);

            return null;
        }



    }
}
