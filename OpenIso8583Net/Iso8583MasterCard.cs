using OpenIso8583Net.Emv;
using OpenIso8583Net.Exceptions;
using OpenIso8583Net.FieldValidator;
using OpenIso8583Net.Formatter;
using OpenIso8583Net.LengthFormatters;
using OpenIso8583Net.MasterCardPDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIso8583Net
{
    public class Iso8583MasterCard : AMessage
    {
        private static Template GetDefaultTemplate(int codePage)
        {
            return GetDefaultTemplate(new Formatter.CodePageFormatter(codePage));
        }

        private static Template GetDefaultTemplate(Encoding encoding)
        {
            return GetDefaultTemplate(new Formatter.CodePageFormatter(encoding));
        }

        private static Template GetDefaultTemplate()
        {
            return GetDefaultTemplate(new Formatter.EbcdicFormatter());
        }

        private static Template GetDefaultTemplate(IFormatter formatter)
        {
            var DefaultTemplate =
                new Template(formatter)
                {
                    { MC._002_PAN, FieldDescriptor.VarilableLength(2, 19, FieldValidators.N, formatter, "PAN") },
                    { MC._003_PROC_CODE, FieldDescriptor.FixedLength(6, FieldValidators.N, formatter, "Processing Code") },
                    { MC._004_TRAN_AMOUNT, FieldDescriptor.FixedLength(12, FieldValidators.N, formatter,"Txn Amount") },
                    { MC._005_SETTLE_AMOUNT, FieldDescriptor.FixedLength(12, FieldValidators.N, formatter, "Settle Amount") },
                    { MC._006_AMOUNT_CARDHOLDER_BILLING, FieldDescriptor.FixedLength(12, FieldValidators.N, formatter, "Billing Amount") },
                   // { MC._007_TRAN_DATE_TIME, FieldDescriptor.GenFixed(10, FieldValidators.N, formatter, "TXN_DATETIME") },
                    { MC._009_CONVERSION_RATE_RECONCILIATION, FieldDescriptor.FixedLength(8, FieldValidators.N, formatter, "CONV_RATE_RECON") },
                    { MC._010_CONVERSION_RATE_CARDHOLDER_BILLING, FieldDescriptor.FixedLength(8, FieldValidators.N, formatter,"CONV_RATE_BILL") },
                    //{ 11, FieldDescriptor.GenFixed(6, FieldValidators.N, formatter,"Field11") },
                    { MC._012_LOCAL_TRAN_DATETIME, FieldDescriptor.FixedLength(12, FieldValidators.N, formatter, "Local Txn Time") },
                    { MC._014_EXPIRY_DATE, FieldDescriptor.FixedLength(4, FieldValidators.N, formatter, "Exp Date") },
                    { MC._022_POS_ENTRY_MODE, FieldDescriptor.FixedLength(12, FieldValidators.Ans, formatter, "POS Entry Mode") },
                    { MC._023_CARD_SEQ_NR, FieldDescriptor.FixedLength(3, FieldValidators.N, formatter, "Card Seq No") },
                    { MC._024_FUNC_CODE, FieldDescriptor.FixedLength(3, FieldValidators.N, formatter, "Function Code") },
                    { MC._025_MESSAGE_REASON_CODE, FieldDescriptor.FixedLength(4, FieldValidators.N, formatter, "Reason Code") },
                    { MC._026_MCC, FieldDescriptor.FixedLength(4, FieldValidators.N, formatter, "MCC") },
                    //{ 27, FieldDescriptor.GenFixed(1, FieldValidators.N, formatter,"Field27") },
                    { MC._030_AMOUNTS_ORIGINAL, FieldDescriptor.FixedLength(24, FieldValidators.N, formatter, "Original Amount") },
                    { MC._031_ACQ_REFERENCE_DATA, FieldDescriptor.VarilableLength(2, 23, FieldValidators.N, formatter, "Acq Ref Data") },
                    { MC._032_ACQ_INST_ID_CODE, FieldDescriptor.VarilableLength(2, 11, FieldValidators.N, formatter, "Acq Institution ID") },
                    { MC._033_FORWARDING_INST_ID_CODE, FieldDescriptor.VarilableLength(2, 11, FieldValidators.N, formatter, "Forwarding Institution ID") },
                    { MC._037_RET_REF_NR, FieldDescriptor.FixedLength(12, FieldValidators.Anp, formatter, "Ret Ref No") },
                    { MC._038_APPROVAL_CODE, FieldDescriptor.FixedLength(6, FieldValidators.Anp, formatter, "Approval Code") },
                    { MC._040_SERVICE_CODE, FieldDescriptor.FixedLength(3, FieldValidators.N, formatter, "Service Code") },
                    { MC._041_TERMINAL_ID, FieldDescriptor.FixedLength(8, FieldValidators.Ans, formatter, "Terminal ID") },
                    { MC._042_CARD_ACCEPTOR_ID, FieldDescriptor.FixedLength(15, FieldValidators.Ans, formatter, "Card Acceptor ID") },
                    { MC._043_CARD_NAME_LOCATION, FieldDescriptor.VarilableLength(2, 99, FieldValidators.Ans, formatter, "Card Name Locations") },
                    { MC._048_PRIVATE_ADDITIONAL_DATA, FieldDescriptor.VarilableLength(3, 999, FieldValidators.None, formatter, "MasterCard PDS Data 1") },
                    { MC._049_TRAN_CURRENCY_CODE, FieldDescriptor.FixedLength(3, FieldValidators.N, formatter, "Txn Currency Code") },
                    { MC._050_SETTLEMENT_CURRENCY_CODE, FieldDescriptor.FixedLength(3, FieldValidators.N, formatter, "Settlement Currency") },
                    { MC._051_CURRENCY_CODE_CARDHOLDER_BILLING, FieldDescriptor.FixedLength(3, FieldValidators.N, formatter, "Cardholder Billing Currency") },
                    { MC._054_ADDITIONAL_AMOUNTS, FieldDescriptor.VarilableLength(3, 240, FieldValidators.Ansp, formatter, "Additional Amounts") },
                    { MC._055_ICC_DATA, FieldDescriptor.BinaryVariableLength(3, 510, FieldValidators.Hex, formatter, "ICC Data") },
                    { MC._062_ADDITIONAL_DATA_2, FieldDescriptor.VarilableLength(3, 999, FieldValidators.None, formatter, "MasterCard PDS Data 2") },
                    { MC._063_TRANSACTION_LIFE_CYCLE_ID, FieldDescriptor.VarilableLength(3, 16, FieldValidators.Ansp, formatter, "Txn Lifecycle ID") },
                    { MC._071_MSG_NR, FieldDescriptor.FixedLength(8, FieldValidators.N, formatter, "Message No") },
                    { MC._072_DATA_RECORD, FieldDescriptor.VarilableLength(3, 999, FieldValidators.None, formatter, "Data Record") },
                    { MC._073_DATE_ACTION, FieldDescriptor.FixedLength(6, FieldValidators.N, formatter, "Action Date") },
                    { MC._093_TXN_DESTINATION_INSTITUTION_ID, FieldDescriptor.VarilableLength(2, 11, FieldValidators.N, formatter, "Txn Dest Institution") },
                    { MC._094_TXN_ORIGINATOR_INSTITUTION_ID, FieldDescriptor.VarilableLength(2, 11, FieldValidators.N, formatter, "Txn Orig Institution")},
                    { MC._095_CARD_ISSUER_REFERENCE_DATA, FieldDescriptor.VarilableLength(2, 10, FieldValidators.N, formatter, "Card Issue Ref Data") },
                    { MC._100_RECEIVING_INST_ID_CODE, FieldDescriptor.VarilableLength(2, 11, FieldValidators.N, formatter, "Rcv Institution ID") },
                    { MC._105_MULTI_USE_TRANSACTION_IDENTIFICATION, FieldDescriptor.VarilableLength(3, 999, FieldValidators.An, formatter, "Multi Use Txn ID") },
                    { MC._111_FEE_AMOUNTS_DEBITS, FieldDescriptor.VarilableLength(3, 12, FieldValidators.Ans, formatter, "Fees Debits") },
                    //{ MC._122_ADDITIONAL_RECORD_DATA, FieldDescriptor.GenVar(3, 999, FieldValidators.None, formatter, "Additional Record Data") },
                    { MC._123_ADDITIONAL_DATA_3,  FieldDescriptor.VarilableLength(3, 999, FieldValidators.None, formatter, "MasterCard PDS Data 3") },
                    { MC._124_ADDITIONAL_DATA_4,  FieldDescriptor.VarilableLength(3, 999, FieldValidators.None, formatter,"MasterCard PDS Data 4") },
                    { MC._125_ADDITIONAL_DATA_5,  FieldDescriptor.VarilableLength(3, 999, FieldValidators.None, formatter, "MasterCard PDS Data 5") },
                    { MC._127_NETWORK_DATA, FieldDescriptor.VarilableLength(3, 999, FieldValidators.Ans, formatter,"Network Data") },
                };
            return DefaultTemplate;
        }



        /// <summary>
        ///   Creates a new instance of the Iso8583 class
        /// </summary>
        public Iso8583MasterCard() : this(GetDefaultTemplate())
        {
        }

        public Iso8583MasterCard(IFormatter MsgTypeFormatter) : this(GetDefaultTemplate(MsgTypeFormatter))
        {
        }

        public Iso8583MasterCard(int codePage) : this(GetDefaultTemplate(codePage))
        {

        }

        public Iso8583MasterCard(Encoding encoding) : this(GetDefaultTemplate(encoding))
        {

        }

        /// <summary>
        ///   Create a new instance of the Iso8583Rev93 class with the specified template overrides
        /// </summary>
        /// <param name = "template">Template override</param>
        public Iso8583MasterCard(Template template) : base(template)
        {
        }

        /// <summary>
        ///   Gets or sets the message type
        /// </summary>
        public int MessageType { get; set; }


        public new virtual byte[] ToMsg(Encoding? enc)
        {
            // if no encoding, just return the message bytes
            if (enc == null)
                return ToMsg();

            if (this.Template.MsgTypeFormatter is CodePageFormatter cpp && cpp.Encoding != enc)
            {
                this.Template.MsgTypeFormatter = new CodePageFormatter(enc);
                foreach (var t in Template.Values)
                {
                    if (t.Formatter is CodePageFormatter cp)
                        cp.Encoding = enc;
                    if (t.LengthFormatter is VariableLengthFormatter var && var.LengthFormatter is CodePageFormatter cpf)
                        cpf.Encoding = enc;
                }
            }

            return ToMsg();
        }


        /// <summary>
        /// Gets the message as a byte array ready to send over the network
        /// </summary>
        /// <returns>
        /// byte[] representing the message 
        /// </returns>
        public new virtual byte[] ToMsg()
        {
            #region PDS Fields to the appropriate ISO8583 fields
            // Add in the PDS fields, so when we work out the full message length it's correct
            List<int> pdsFieldLists = new List<int> { 48, 62, 123, 124, 125 };
            // start by clearing the PDS fields for repopulation
            foreach (var pds in pdsFieldLists)
                this.ClearField(pds);
            int nextDE = 0;

            // start building the values to place in the PDS DE's
            StringBuilder sb = new StringBuilder();

            foreach (var kv in PDSFields.OrderBy(kp => kp.Key))
            {
                string nextPDS = SetPDSField(kv);
                if (sb.Length + nextPDS.Length > 999)
                {
                    this.SetFieldValue(pdsFieldLists[nextDE], sb.ToString());
                    nextDE++;
                    sb.Clear();
                    if (nextDE > pdsFieldLists.Count)
                        throw new ArgumentException("Unable to set all PDS fields, exceeds the possible PDS sizes");
                }
                sb.Append(nextPDS);
            }

            // add the last values (if there are any)
            if (sb.Length > 0)
                this.SetFieldValue(pdsFieldLists[nextDE], sb.ToString());
            #endregion

            var mtidLength = this.Template.MsgTypeFormatter.GetPackedLength(4);
            var packedLen = this.PackedLength + mtidLength;
            var data = new byte[packedLen];

            var buffer = Template.MsgTypeFormatter.GetBytes(IsoConvert.FromIntToMsgType(this.MessageType));
            Array.Copy(buffer, 0, data, 0, mtidLength);
            var offset = mtidLength;

            // bitmap
            var bmap = this.bitmap.ToMsg();
            Array.Copy(bmap, 0, data, offset, this.bitmap.PackedLength);
            offset += this.bitmap.PackedLength;

            // Fields
            for (var i = 2; i <= 128; i++)
            {
                if (this.bitmap[i])
                {
                    var field = this.fields[i];
                    Array.Copy(field.ToMsg(), 0, data, offset, field.PackedLength);
                    offset += field.PackedLength;
                }
            }

            return data;
        }

        /// <summary>
        /// Unpacks the message from a byte array
        /// </summary>
        /// <param name="msg">
        /// message data to unpack 
        /// </param>
        /// <param name="startingOffset">
        /// the offset in the array to start 
        /// </param>
        /// <returns>
        /// the offset in the array representing the start of the next message 
        /// </returns>
        public override int Unpack(byte[] msg, int startingOffset)
        {
            // get mtid
            var mtidLength = this.Template.MsgTypeFormatter.GetPackedLength(4);
            var buffer = new byte[mtidLength];
            var offset = startingOffset;
            Array.Copy(msg, offset, buffer, 0, mtidLength);
            string msgTypeString = this.Template.MsgTypeFormatter.GetString(buffer);
            this.MessageType = IsoConvert.FromMsgTypeToInt(msgTypeString);
            offset += mtidLength;
            int res = base.Unpack(msg, offset);

            List<int> pdsFieldLists = new List<int> { 48, 62, 123, 124, 125 };
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
            foreach (var pdsFld in pdsFieldLists)
            {
                if (this.IsFieldSet(pdsFld))
                {
                    var pds = GetPDSFields(this[pdsFld]);
                    foreach (var kv in pds)
                    {
                        if (!PDSFields.ContainsKey(kv.Key))
                            PDSFields.Add(kv.Key, kv.Value);
                        else
                            PDSFields[kv.Key] = (PDSFields[kv.Key] ?? "") + " || " + kv.Value;
                    }
                }
            }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

            return res;
        }

        protected override IField CreateField(int field)
        {
            if (Template.ContainsKey(field))
            {
                return new Field(field, Template[field]);
            }
            throw new UnknownFieldException(field.ToString());
        }



        readonly Dictionary<string, string> _pdsFields = new Dictionary<string, string>();
        public Dictionary<string, string> PDSFields
        {
            get { return _pdsFields; }
        }

        public static Dictionary<string, string> GetPDSFields(string? data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return new Dictionary<string, string>();

            var res = new Dictionary<string, string>();
            string dataLeft = data;
            while (dataLeft.Length > 0)
            {
                var tag = dataLeft[..4];
                var lens = dataLeft[4..7];
                var len = int.Parse(lens);
                var val = dataLeft.Substring(7, len);
                res.Add(tag, val);
                dataLeft = dataLeft.Substring(len + 7);
            }
            return res;
        }

        private static string SetPDSField(KeyValuePair<string, string> val)
        {
            return SetPDSField(val.Key, val.Value);
        }

        private static string SetPDSField(string field, string value)
        {
            int id = -1;
            if (!int.TryParse(field, out id))
                throw new ArgumentException("The PDS ID needs to be numeric", nameof(field));

            if (field.Length > 4 && field.Length < 1)
                throw new ArgumentException("The PDS ID needs to be 1 to 4 digits long", nameof(field));

            if (value.Length > 990)
                throw new ArgumentException("Value is too long, it needs to be a max of 992 long (PDS Id (4) + length field(3) + length of value must be less than 999)", nameof(value));

            return $"{id:0000}{value.Length:000}{value}";
        }



        public override string ToString(string? prefix)
        {
            var sb = new StringBuilder();
            sb.Append((prefix ?? "") + IsoConvert.FromIntToMsgType(this.MessageType) + ":" + Environment.NewLine);
            for (var i = 2; i <= 128; i++)
            {
                if (this.bitmap[i])
                {
                    sb.AppendLine(ToString(i, prefix ?? ""));

                    switch (i)
                    {
                        case 55:
                            var icc = EmvUtils.UnpackEmvTags(this[i]!.ToByteArray());
                            sb.AppendLine(icc.ToString((prefix ?? "") + "                                       EMV "));
                            break;
                        case 48:
                        case 62:
                        case 123:
                        case 124:
                        case 125:
                            PDSValueToString(prefix, sb, i);
                            break;
                    }
                }
            }
            foreach (var kv in PDSFields)
            {
                sb.AppendLine((prefix ?? "") + "                                       PDS " + kv.Key + " = [" + kv.Value + "]");
            }
            return sb.ToString();
        }

        private void PDSValueToString(string? prefix, StringBuilder sb, int i)
        {
            foreach (var kv in GetPDSFields(this[i]))
            {
                object? val = null;
                switch (kv.Key)
                {
                    case "0105":
                        val = Pds0105.Parse(kv.Value);
                        break;
                    case "0110":
                        val = Pds0110.Parse(kv.Value);
                        break;
                    case "0158":
                        val = Pds0158.Parse(kv.Value);
                        break;
                    case "0164":
                        val = Pds0164List.Parse(kv.Value);
                        break;
                    case "0300":
                        val = Pds0300.Parse(kv.Value);
                        break;
                    case "0359":
                        val = Pds0359.Parse(kv.Value);
                        break;
                }
                if (val != null)
                    sb.AppendLine(val.ToString());
                else
                    sb.AppendLine((prefix ?? "") + "                                       PDS " + kv.Key + " = [" + kv.Value + "]");
            }
        }

        #region Nested type: Bit

        /// <summary>
        ///   Human readable constants mapping to field numbers
        /// </summary>
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
        public static class MC
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
        {
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
            /// Amount, Card Holder Billing
            /// </summary>
            public const int _006_AMOUNT_CARDHOLDER_BILLING = 6;

            /// <summary>
            /// Transmission Date and Time
            /// </summary>
            public const int _007_TRAN_DATE_TIME = 7;

            /// <summary>
            /// Field 9 - Conversion Rate Reconciliation
            /// </summary>
            public const int _009_CONVERSION_RATE_RECONCILIATION = 9;

            /// <summary>
            /// Field 10 - Conversion Rate, Cardholder Billing
            /// </summary>
            public const int _010_CONVERSION_RATE_CARDHOLDER_BILLING = 10;
            /// <summary>
            ///   Field 12 - Time, Local Transaction
            /// </summary>
            public const int _012_LOCAL_TRAN_DATETIME = 12;

            /// <summary>
            ///   Field 14 - Expiry Date
            /// </summary>
            public const int _014_EXPIRY_DATE = 14;

            /// <summary>
            ///   Field 22 - POS Entry Mode
            /// </summary>
            public const int _022_POS_ENTRY_MODE = 22;

            /// <summary>
            ///   Field 23 - Card Sequence Number
            /// </summary>
            public const int _023_CARD_SEQ_NR = 23;

            /// <summary>
            ///   Field 24 - Function Code
            /// </summary>
            public const int _024_FUNC_CODE = 24;

            /// <summary>
            ///   Field 25 - Message Reason Code
            /// </summary>
            public const int _025_MESSAGE_REASON_CODE = 25;

            /// <summary>
            /// Field 26 - Card Acceptor Business Code (MCC)
            /// </summary>
            public const int _026_MCC = 26;

            /// <summary>
            ///   Field 30 - Original Amounts
            /// </summary>
            public const int _030_AMOUNTS_ORIGINAL = 30;

            /// <summary>
            /// Field 31 - Acquirer Reference Data
            /// </summary>
            public const int _031_ACQ_REFERENCE_DATA = 31;

            /// <summary>
            ///   Field 32 - Acquiring Institution ID Code
            /// </summary>
            public const int _032_ACQ_INST_ID_CODE = 32;

            /// <summary>
            /// Field 33 - Forwarding Institution ID Code
            /// </summary>
            public const int _033_FORWARDING_INST_ID_CODE = 33;

            /// <summary>
            ///   Field 37 - Retrieval Reference Number
            /// </summary>
            public const int _037_RET_REF_NR = 37;

            /// <summary>
            ///   Field 38 - Approval Code
            /// </summary>
            public const int _038_APPROVAL_CODE = 38;

            /// <summary>
            ///   Field 40 - Service Code
            /// </summary>
            public const int _040_SERVICE_CODE = 40;

            /// <summary>
            ///   Field 41 - Terminal ID
            /// </summary>
            public const int _041_TERMINAL_ID = 41;

            /// <summary>
            ///   Field 42 - Card Acceptor ID
            /// </summary>
            public const int _042_CARD_ACCEPTOR_ID = 42;

            /// <summary>
            /// Field 43 - Card Acceptor Name / Location
            /// </summary>
            public const int _043_CARD_NAME_LOCATION = 43;

            /// <summary>
            ///   Field 48 - Private Additional Data
            /// </summary>
            public const int _048_PRIVATE_ADDITIONAL_DATA = 48;

            /// <summary>
            ///   Field 49 - Transaction Currency Code
            /// </summary>
            public const int _049_TRAN_CURRENCY_CODE = 49;

            /// <summary>
            ///   Field 50 - Settlement Currency Code
            /// </summary>
            public const int _050_SETTLEMENT_CURRENCY_CODE = 50;

            /// <summary>
            /// Field 51 - Currency Code, Cardholder Billing
            /// </summary>
            public const int _051_CURRENCY_CODE_CARDHOLDER_BILLING = 51;

            /// <summary>
            ///   Field 54 - Additional Amounts
            /// </summary>
            public const int _054_ADDITIONAL_AMOUNTS = 54;

            /// <summary>
            ///   Field 55 - ICC Data
            /// </summary>
            public const int _055_ICC_DATA = 55;

            /// <summary>
            ///   Field 62 - Hotcard Capacity
            /// </summary>
            public const int _062_ADDITIONAL_DATA_2 = 62;

            /// <summary>
            ///   Field 63 - TermApp.ISO Private Data
            /// </summary>
            public const int _063_TRANSACTION_LIFE_CYCLE_ID = 63;

            /// <summary>
            ///   Field 71 - Message Number
            /// </summary>
            public const int _071_MSG_NR = 71;

            /// <summary>
            ///   Field 72 - Data Record
            /// </summary>
            public const int _072_DATA_RECORD = 72;

            /// <summary>
            /// Field 73 - Data, Action
            /// </summary>
            public const int _073_DATE_ACTION = 73;

            /// <summary>
            /// Field 93 - Transaction Destination Institution ID Code
            /// </summary>
            public const int _093_TXN_DESTINATION_INSTITUTION_ID = 93;

            /// <summary>
            /// Field 94 - Transaction Originator Institution ID Code
            /// </summary>
            public const int _094_TXN_ORIGINATOR_INSTITUTION_ID = 94;

            /// <summary>
            /// Field 95 - Card Issuer Reference Data
            /// </summary>
            public const int _095_CARD_ISSUER_REFERENCE_DATA = 95;

            /// <summary>
            ///   Field 100 - Receiving Institution ID Code
            /// </summary>
            public const int _100_RECEIVING_INST_ID_CODE = 100;

            /// <summary>
            /// Field 105 - Multi-Use Transaction Identification Data
            /// </summary>
            public const int _105_MULTI_USE_TRANSACTION_IDENTIFICATION = 105;

            /// <summary>
            /// Field 111 - Amount, Currency Conversion Assessment
            /// </summary>
            public const int _111_FEE_AMOUNTS_DEBITS = 111;

            /// <summary>
            /// Field 122 - Additional Record Data
            /// </summary>
            public const int _122_ADDITIONAL_RECORD_DATA = 122;

            /// <summary>
            /// Field 123 - Additional Data
            /// </summary>
            public const int _123_ADDITIONAL_DATA_3 = 123;

            /// <summary>
            /// Field 124 -  Additional Data
            /// </summary>
            public const int _124_ADDITIONAL_DATA_4 = 124;

            /// <summary>
            /// Field 125 - Additional Data
            /// </summary>
            public const int _125_ADDITIONAL_DATA_5 = 125;

            /// <summary>
            /// Field 127 - Network Data (is reserved for internal clearing system use)
            /// </summary>
            public const int _127_NETWORK_DATA = 127;

        }

        #endregion

        #region Nested type: MsgType

        /// <summary>
        ///   Human readable constants mapping to message types
        /// </summary>
        public static class MsgType
        {
            /// <summary>
            ///   Invalid Message
            /// </summary>
            public const int _0000_INVALID_MSG = 0;

            /// <summary>
            ///   Authorisation Request
            /// </summary>
            public const int _1100_AUTH_REQ = 0x1100;

            /// <summary>
            ///   Authorisation Request Response
            /// </summary>
            public const int _1110_AUTH_REQ_RSP = 0x1110;

            /// <summary>
            ///   Authorisation Advice
            /// </summary>
            public const int _1120_AUTH_ADV = 0x1120;

            /// <summary>
            ///   Authorisation Advice Response
            /// </summary>
            public const int _1130_AUTH_ADV_RSP = 0x1130;

            /// <summary>
            ///   Transaction Request
            /// </summary>
            public const int _1200_TRAN_REQ = 0x1200;

            /// <summary>
            ///   Transaction Request Response
            /// </summary>
            public const int _1210_TRAN_REQ_RSP = 0x1210;

            /// <summary>
            ///   Transaction Advice
            /// </summary>
            public const int _1220_TRAN_ADV = 0x1220;

            /// <summary>
            ///   Transaction Advice Response
            /// </summary>
            public const int _1230_TRAN_ADV_RSP = 0x1230;

            /// <summary>
            ///   File Action Request
            /// </summary>
            public const int _1304_FILE_ACTION_REQ = 0x1304;

            /// <summary>
            ///   File Action Request Response
            /// </summary>
            public const int _1314_FILE_ACTION_REQ_RSP = 0x1314;

            /// <summary>
            ///   Reversal Advice
            /// </summary>
            public const int _1420_TRAN_REV_ADV = 0x1420;

            /// <summary>
            ///   Reversal Advice Response
            /// </summary>
            public const int _1430_TRAN_REV_ADV_RSP = 0x1430;

            /// <summary>
            ///   Reconciliation Request
            /// </summary>
            public const int _1500_RECON_REQ = 0x1500;

            /// <summary>
            ///   Reconciliation Request Response
            /// </summary>
            public const int _1510_RECON_REQ_RSP = 0x1510;

            /// <summary>
            ///   Reconciliation Advice
            /// </summary>
            public const int _1520_RECON_ADV = 0x1520;

            /// <summary>
            ///   Reconciliation Advice Response
            /// </summary>
            public const int _1530_RECON_ADV_RSP = 0x1530;

            /// <summary>
            ///   Administration Request
            /// </summary>
            public const int _1604_ADMIN_REQ = 0x1604;

            /// <summary>
            ///   Administration Request Response
            /// </summary>
            public const int _1614_ADMIN_REQ_RSP = 0x1614;

            /// <summary>
            ///   Administration Advice
            /// </summary>
            public const int _1624_ADMIN_ADV = 0x1624;

            /// <summary>
            ///   Administration Advice Response
            /// </summary>
            public const int _1634_ADMIN_ADV_RSP = 0x1634;

            /// <summary>
            ///   Network Management Request
            /// </summary>
            public const int _1804_NWRK_MNG_REQ = 0x1804;

            /// <summary>
            ///   Network Management Request Response
            /// </summary>
            public const int _1814_NWRK_MNG_REQ_RSP = 0x1814;
        }

        #endregion
    }
}
