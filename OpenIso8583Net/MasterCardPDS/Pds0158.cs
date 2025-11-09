using System;
using System.Globalization;

namespace OpenIso8583Net.MasterCardPDS
{
    public class Pds0158 : PdsBase<Pds0158>
    {
        public override string PdsId => "0158";

        public override string PdsName => "Business Activity";

        public override string PdsValue { get; set; } = string.Empty;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        public static new Pds0158 Parse(string data)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            var res = new Pds0158();

            if (data.Length < 4 || data.Length > 31)
                throw new ArgumentException($"PDS {res.PdsId} expects data with a length between 4 and 31");

            res.PdsValue = data;


            res.CardProgramIdentifier = data[..3]; // Subfield 1 (Card Program Identifier) : ans-3
            if (data.Length >= 4)
                res.BusinessServiceArrangementTypeCode = data[3].ToString(); // Subfield 2 (Business Service Arrangement Type Code) : ans-1
            if (data.Length >= 10)
                res.BusinessServiceIDCode = data[4..10]; // Subfield 3 (Business Service ID Code) : ans-6

            if (data.Length >= 12)
                res.InterchangeRateDesignator = data[10..12]; // Subfield 4 (Interchange Rate Designator) : ans-2

            // Subfield 5 (Central Site Business Date) : n-6; YYMMDD
            if (data.Length >= 18)
                res.CentralSiteBusinessDate = DateOnly.ParseExact(data[12..18], "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

            // Subfield 6 (Business Cycle) : n-2
            if (data.Length >= 20)
                res.BusinessCycle = int.Parse(data[18..20]);

            // Subfield 7 (Card Acceptor Classification Override Indicator) : ans-1
            if (data.Length >= 21)
                res.CardAcceptorClassificationOverrideIndicator = data[21].ToString();

            // Subfield 8 (Product Class Override Indicator) : ans-3
            if (data.Length >= 24)
                res.ProductClassOverrideIndicator = data[21..24];

            // Subfield 9 (Corporate Incentive Rates Apply Indicator) : ans-1
            if (data.Length >= 25)
                res.CorporateIncentiveRatesApplyIndicator = data[24].ToString();

            // Subfield 10 (Special Conditions Indicator) : ans-1
            if (data.Length >= 26)
                res.SpecialConditionsIndicator = data[25].ToString();

            // Subfield 11 (Mastercard Assigned ID Override Indicator) : ans-1
            if (data.Length >= 27)
                res.MastercardAssignedIDOverrideIndicator = data[26].ToString();

            // Subfield 12 (Account Level Management Account Category Code) : ans-1
            if (data.Length >= 28)
                res.AccountLevelManagementAccountCategoryCode = data[27].ToString();

            // Subfield 13 (Rate Indicator) : ans-1
            if (data.Length >= 29)
                res.RateIndicator = data[28].ToString();

            // Subfield 14 (Masterpass Incentive Indicator) : an-1
            if (data.Length >= 30)
                res.MasterPassIncentiveIndicator = data[29].ToString();

            // Subfield 15 (Digital Wallet Interchange Override Indicator) : an-1
            if (data.Length >= 31)
                res.DigitalWalletInterchangeOverrideIndicator = data[30].ToString();

            return res;
        }

        /// <summary>
        /// Card Program Identifier
        /// </summary>
        /// <remarks>
        /// PDS 0158 (Business Activity), subfield 1 (Card Program Identifier) is a three
        /// character code that identifies the card program or financial network to which 
        /// a transaction belongs.
        /// 
        /// currently valid values (7 June 2022)
        /// ------------------------------------
        ///   * CIR    - Cirrus®
        ///   * DMC    - Debit Mastercard®
        ///   * MCC    - Mastercard®   * Credit
        ///   * MSI    - Maestro® PRO Sweden domestic brand 
        ///   * PVL    - Private label
        ///   * spaces - Customers must submit all spaces in this subfield unless
        ///              specifically directed otherwise by Mastercard®
        /// 
        /// The clearing system will then determine the correct value and populate this 
        /// subfield before sending the message to the receiver.
        /// </remarks>
        public string? CardProgramIdentifier { get; set; }

        public string? BusinessServiceArrangementTypeCode { get; set; }

        public string? BusinessServiceIDCode { get; set; }
        public string? InterchangeRateDesignator { get; set; }
        public DateOnly? CentralSiteBusinessDate { get; set; }

        public int? BusinessCycle { get; set; }
        public string? CardAcceptorClassificationOverrideIndicator { get; set; }
        public string? ProductClassOverrideIndicator { get; set; }
        public string? CorporateIncentiveRatesApplyIndicator { get; set; }
        public string? SpecialConditionsIndicator { get; set; }
        public string? MastercardAssignedIDOverrideIndicator { get; set; }
        public string? AccountLevelManagementAccountCategoryCode { get; set; }
        public string? RateIndicator { get; set; }
        public string? MasterPassIncentiveIndicator { get; set; }
        public string? DigitalWalletInterchangeOverrideIndicator { get; set; }
    }
}
