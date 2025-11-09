using OpenIso8583Net.FieldValidator;
using System.Text.Json.Serialization;

namespace OpenIso8583Net
{
    /// <summary>
    ///   Postilion ISO 8583 Message
    /// </summary>
    /// <remarks>
    ///   This inherits from Iso8583 and adds a number of postilion specific fields.  In particular, field 127
    ///   the postilion private bitmap field has been added
    /// </remarks>
    /// <example>
    ///   <code>
    ///     public byte[] GetDataFromMessage()
    ///     {
    ///     Iso8583Post msg = new Iso8583Post();
    ///     msg[Iso8583Post.Bit._002_PAN] = "123456789012345";
    ///     msg.Private[Field127.Bit._002_SWITCH_KEY] = "SimKey00000001";
    ///     byte[] data = msg.ToMsg();
    ///     return data;
    ///     }
    /// 
    ///     public Iso8583Post GetMessageFromData(byte[] data)
    ///     {
    ///     Iso8583Post msg = new Iso8583Post();
    ///     msg.Unpack(data, 0);
    ///     return msg;
    ///     }
    ///   </code>
    ///   <code lang = "VB">
    ///     Public Function GetDataFromMessage() As Byte()
    ///     Dim msg As New Iso8583Post()
    ///     msg(Iso8583Post.Bit._002_PAN) = "123456789012345"
    ///     msg.[Private](Field127.Bit._002_SWITCH_KEY) = "SimKey00000001"
    ///     Dim data As Byte() = msg.ToMsg()
    ///     Return data
    ///     End Function
    /// 
    ///     Public Function GetMessageFromData(ByVal data As Byte()) As Iso8583Post
    ///     Dim msg As New Iso8583Post()
    ///     msg.Unpack(data, 0)
    ///     Return msg
    ///     End Function
    ///   </code>
    /// </example>
    public class Iso8583Post : Iso8583
    {


        /// <summary>
        ///   Creates a new Iso8583Post message
        /// </summary>
        public Iso8583Post() : base(GetDefaultIso8583PostTemplate())
        {
        }

        /// <summary>
        ///   The postilion private field, field 127
        /// </summary>
        [JsonIgnore]
        public Field127 Private
        {
            get { return (Field127)GetField(127); }
        }



        /// <summary>
        ///   Create a field of the correct type and length
        /// </summary>
        /// <param name = "field">Field number to create</param>
        /// <returns>AField representing the desired field</returns>
        protected override IField CreateField(int field)
        {
            // Deal with the postilion specific fields first
            switch (field)
            {
                case Bit._127_POSTILION_PRIVATE_FIELD:
                    return new Field127();
            }

            // Handle standard ISO fields later
            return base.CreateField(field);
        }


        /// <summary>
        /// Get the default Is8583-Post template
        /// </summary>
        /// <returns>
        /// A Template
        /// </returns>
        protected static Template GetDefaultIso8583PostTemplate()
        {
            var template = GetDefaultIso8583Template();
            // Patch 148 - to support AN
            template[Bit._032_ACQUIRING_INST_ID_CODE] = FieldDescriptor.AsciiVar(2, 11, FieldValidators.AlphaNumeric, "Acquirer ID");
            template[Bit._033_FORWARDING_INT_ID_CODE] = FieldDescriptor.AsciiVar(2, 11, FieldValidators.AlphaNumeric, "Forwarding ID");
            template[Bit._090_ORIGINAL_DATA_ELEMENTS] = FieldDescriptor.AsciiFixed(42, FieldValidators.AlphaNumeric, "Original Data");
            template[Bit._100_RECEIVING_INST_ID_CODE] = FieldDescriptor.AsciiVar(2, 11, FieldValidators.AlphaNumeric, "Rcv Inst. Code");

            // field 38 differs from the ISO standard by the field validator An (ISO) vs Anp (postilion)
            template[Bit._038_AUTH_ID_RESPONSE] = FieldDescriptor.AsciiFixed(6, FieldValidators.Anp, "Approval Code");
            // field 38 differs from the ISO standard by the field validator An (ISO) vs Anp (postilion) 
            template[Bit._037_RETRIEVAL_REF_NUM] = FieldDescriptor.AsciiFixed(12, FieldValidators.Anp, "Ret Ref No");
            template[Bit._059_ECHO_DATA] = FieldDescriptor.AsciiVar(3, 255, FieldValidators.AlphaNumericSpecial, "Echo Data");
            template[Bit._123_POS_DATA_CODE] = FieldDescriptor.AsciiVar(3, 15, FieldValidators.AlphaNumeric, "POS Data Code");

            return template;
        }


        #region Nested type: Bit

        /// <summary>
        ///   Human readable constants mapping to field numbers
        /// </summary>
        public new static class Bit
        {
            #region Constants and Fields

            /// <summary>
            ///   Primary Account Number
            /// </summary>
            public const int _002_PAN = 2;

            /// <summary>
            ///   Processing Code
            /// </summary>
            public const int _003_PROC_CODE = 3;

            /// <summary>
            ///   Transaction Amount
            /// </summary>
            public const int _004_TRAN_AMOUNT = 4;

            /// <summary>
            ///   Settlement Amount
            /// </summary>
            public const int _005_SETTLE_AMOUNT = 5;

            /// <summary>
            ///   Transmission Date and Time
            /// </summary>
            public const int _007_TRAN_DATE_TIME = 7;

            /// <summary>
            ///   Conversion Rate Settlement
            /// </summary>
            public const int _009_CONVERSION_RATE_SETTLEMENT = 9;

            /// <summary>
            ///   Systems Trace Audit Number
            /// </summary>
            public const int _011_SYS_TRACE_AUDIT_NUM = 11;

            /// <summary>
            ///   Field 12 - Time, Local Transaction
            /// </summary>
            public const int _012_LOCAL_TRAN_TIME = 12;

            /// <summary>
            ///   Field 13 - Date, Local Transaction
            /// </summary>
            public const int _013_LOCAL_TRAN_DATE = 13;

            /// <summary>
            ///   Field 14 - Date, Expiration
            /// </summary>
            public const int _014_EXPIRATION_DATE = 14;

            /// <summary>
            ///   Field 15 - Date, Settlement
            /// </summary>
            public const int _015_SELLTLEMENT_DATE = 15;

            /// <summary>
            ///   Field 16 - Date, Conversion
            /// </summary>
            public const int _016_CONVERSION_DATE = 16;

            /// <summary>
            ///   Field 18 - Merchant Type
            /// </summary>
            public const int _018_MERCHANT_TYPE = 18;

            /// <summary>
            ///   Field 22 - POS Entry Mode
            /// </summary>
            public const int _022_POS_ENTRY_MODE = 22;

            /// <summary>
            ///   Field 23 – Card Sequence Number
            /// </summary>
            public const int _023_CARD_SEQUENCE_NUM = 23;

            /// <summary>
            ///   Field 25 - POS Condition Code
            /// </summary>
            public const int _025_POS_CONDITION_CODE = 25;

            /// <summary>
            ///   Field 26 - POS PIN Capture Code
            /// </summary>
            public const int _026_POS_PIN_CAPTURE_CODE = 26;

            /// <summary>
            ///   Authorisation ID Response
            /// </summary>
            public const int _027_AUTH_ID_RSP = 27;

            /// <summary>
            ///   Transaction fee amount
            /// </summary>
            public const int _028_TRAN_FEE_AMOUNT = 28;

            /// <summary>
            ///   Settlement fee amount
            /// </summary>
            public const int _029_SETTLEMENT_FEE_AMOUNT = 29;

            /// <summary>
            ///   Transaction processing fee amount
            /// </summary>
            public const int _030_TRAN_PROC_FEE_AMOUNT = 30;

            /// <summary>
            ///   Settlement processing fee amount
            /// </summary>
            public const int _031_SETTLEMENT_PROC_FEE_AMOUNT = 31;

            /// <summary>
            ///   Field 32 - Acquiring Institution ID Code
            /// </summary>
            public const int _032_ACQUIRING_INST_ID_CODE = 32;

            /// <summary>
            ///   Field 33 - Forwarding Institution ID Code
            /// </summary>
            public const int _033_FORWARDING_INT_ID_CODE = 33;

            /// <summary>
            ///   Field 35 - Track 2 Data
            /// </summary>
            public const int _035_TRACK_2_DATA = 35;

            /// <summary>
            ///   Field 37 - Retrieval Reference Number
            /// </summary>
            public const int _037_RETRIEVAL_REF_NUM = 37;

            /// <summary>
            ///   Field 38 - Authorization ID Response
            /// </summary>
            public const int _038_AUTH_ID_RESPONSE = 38;

            /// <summary>
            ///   Field 39 - Response Code
            /// </summary>
            public const int _039_RESPONSE_CODE = 39;

            /// <summary>
            ///   Field 40 - Service Restriction Code
            /// </summary>
            public const int _040_SERVICE_RESTRICTION_CODE = 40;

            /// <summary>
            ///   Field 41 - Card Acceptor Terminal ID
            /// </summary>
            public const int _041_CARD_ACCEPTOR_TERMINAL_ID = 41;

            /// <summary>
            ///   Field 42 - Card Acceptor ID Code
            /// </summary>
            public const int _042_CARD_ACCEPTOR_ID_CODE = 42;

            /// <summary>
            ///   Field 43 - Card Acceptor Name Location
            /// </summary>
            public const int _043_CARD_ACCEPTOR_NAME_LOCATION = 43;

            /// <summary>
            ///   Field 44 - Additional Response Data
            /// </summary>
            public const int _044_ADDITIONAL_RESPONSE_DATA = 44;

            /// <summary>
            ///   Track 1 Data
            /// </summary>
            public const int _045_TRACK_1_DATA = 45;

            /// <summary>
            ///   Field 48 - Additional Data
            /// </summary>
            public const int _048_ADDITIONAL_DATA = 48;

            /// <summary>
            ///   Field 49 - Currency Code, Transaction
            /// </summary>
            public const int _049_TRAN_CURRENCY_CODE = 49;

            /// <summary>
            ///   Field 50 - Currency Code, Settlement
            /// </summary>
            public const int _050_SETTLEMENT_CURRENCY_CODE = 50;

            /// <summary>
            ///   Field 52 - PIN Data
            /// </summary>
            public const int _052_PIN_DATA = 52;

            /// <summary>
            ///   Security Related Control Information
            /// </summary>
            public const int _053_SECURITY_RELATED_CONTROL_INFORMATION = 53;

            /// <summary>
            ///   Field 54 - Additional Amounts
            /// </summary>
            public const int _054_ADDITIONAL_AMOUNTS = 54;

            /// <summary>
            ///   Field 56 - Message Reason Code
            /// </summary>
            public const int _056_MESSAGE_REASON_CODE = 56;

            /// <summary>
            ///   Authorisation Life Cycle
            /// </summary>
            public const int _057_AUTHORISATION_LIFE_CYCLE = 57;

            /// <summary>
            ///   Authorising Agent Institution
            /// </summary>
            public const int _058_AUTHORISING_AGENT_INSTITUTION = 58;

            /// <summary>
            ///   Settlement Code
            /// </summary>
            public const int _066_SETTLEMENT_CODE = 66;

            /// <summary>
            ///   Extended Payment Code
            /// </summary>
            public const int _067_EXTENDED_PAYMENT_CODE = 67;

            /// <summary>
            ///   Network Management Information Code
            /// </summary>
            public const int _070_NETWORK_MANAGEMENT_INFORMATION_CODE = 70;

            /// <summary>
            ///   Date Action
            /// </summary>
            public const int _073_DATE_ACTION = 73;

            /// <summary>
            ///   Credits, Number
            /// </summary>
            public const int _074_CREDITS_NUMBER = 74;

            /// <summary>
            ///   Credits Reversal, Number
            /// </summary>
            public const int _075_CREDITS_REVERSAL_NUMBER = 75;

            /// <summary>
            ///   Debits, Number
            /// </summary>
            public const int _076_DEBITS_NUMBER = 76;

            /// <summary>
            ///   Debits Reversal, Number
            /// </summary>
            public const int _077_DEBITS_REVERSAL_NUMBER = 77;

            /// <summary>
            ///   Transfers, Number
            /// </summary>
            public const int _078_TRANSFER_NUMBER = 78;

            /// <summary>
            ///   Transfers Reversal, Number
            /// </summary>
            public const int _079_TRANSFER_REVERSAL_NUMBER = 79;

            /// <summary>
            ///   Inquiries, Number
            /// </summary>
            public const int _080_INQUIRIES_NUMBER = 80;

            /// <summary>
            ///   Authorisations, Number
            /// </summary>
            public const int _081_AUTHORISATIONS_NUMBER = 81;

            /// <summary>
            ///   Credits, Processing Fee Amount
            /// </summary>
            public const int _082_CREDITS_PROCESSING_FEE_AMOUNT = 82;

            /// <summary>
            ///   Credits, Transaction Fee Amount
            /// </summary>
            public const int _083_CREDITS_TRANSACTION_FEE_AMOUNT = 83;

            /// <summary>
            ///   Debits, Processing Fee Amount
            /// </summary>
            public const int _084_DEBITS_PROCESSING_FEE_AMOUNT = 84;

            /// <summary>
            ///   Debits, Transaction Fee Amount
            /// </summary>
            public const int _085_DEBITS_TRANSACTION_FEE_AMOUNT = 85;

            /// <summary>
            ///   Credits, Amount
            /// </summary>
            public const int _086_CREDITS_AMOUNT = 86;

            /// <summary>
            ///   Credits Reversal, Amount
            /// </summary>
            public const int _087_CREDITS_REVERSAL_AMOUNT = 87;

            /// <summary>
            ///   Debits, Amount
            /// </summary>
            public const int _088_DEBITS_AMOUNT = 88;

            /// <summary>
            ///   Debits Reversal, Amount
            /// </summary>
            public const int _089_DEBITS_REVERSAL_AMOUNT = 89;

            /// <summary>
            ///   Original Data Elements
            /// </summary>
            public const int _090_ORIGINAL_DATA_ELEMENTS = 90;

            /// <summary>
            ///   File Update Code
            /// </summary>
            public const int _091_FILE_UPDATE_CODE = 91;

            /// <summary>
            ///   Replacement Amounts
            /// </summary>
            public const int _095_REPLACEMENT_AMOUNTS = 95;

            /// <summary>
            ///   Amount Net Settlement
            /// </summary>
            public const int _097_AMOUNT_NET_SETTLEMENT = 97;

            /// <summary>
            ///   Payee
            /// </summary>
            public const int _098_PAYEE = 98;

            /// <summary>
            ///   Field 100 - Receiving Institution ID Code
            /// </summary>
            public const int _100_RECEIVING_INST_ID_CODE = 100;

            /// <summary>
            ///   Field 101 - File Name
            /// </summary>
            public const int _101_FILE_NAME = 101;

            /// <summary>
            ///   Field 102 - Account Identification 1
            /// </summary>
            public const int _102_ACCOUNT_ID_1 = 102;

            /// <summary>
            ///   Field 103 - Account Identification 2
            /// </summary>
            public const int _103_ACCOUNT_ID_2 = 103;

            /// <summary>
            ///   Payments, Number
            /// </summary>
            public const int _118_PAYMENTS_NUMBER = 118;

            /// <summary>
            ///   Payments Reversal, Number
            /// </summary>
            public const int _119_PAYMENTS_REVERSAL_NUMBER = 119;

            #endregion

            /// <summary>
            ///   Field 59 - Echo Data
            /// </summary>
            public const int _059_ECHO_DATA = 59;

            /// <summary>
            ///   POS Data Code
            /// </summary>
            public const int _123_POS_DATA_CODE = 123;

            /// <summary>
            ///   Postilion private field
            /// </summary>
            public const int _127_POSTILION_PRIVATE_FIELD = 127;
        }

        #endregion


    }
}