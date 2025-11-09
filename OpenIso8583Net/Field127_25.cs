using OpenIso8583Net.Exceptions;
using OpenIso8583Net.FieldValidator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using FormatException = OpenIso8583Net.Exceptions.FormatException;

namespace OpenIso8583Net
{
#pragma warning disable S101 // should be pascal case
    public class Field127_25 : AMessage, IField
#pragma warning restore S101 // should be pascal case
    {
        public Field127_25() : base(GetField127_25Template()) { }
        #region IField implementation

        [JsonIgnore]
        public int FieldNumber => 25;

        [JsonIgnore]
        public string Value { get => null!; set => throw new NotImplementedException(); }
        #endregion

        /// <summary>
        ///   Gets the packed length of the message
        /// </summary>
        [JsonIgnore]
        public new int PackedLength
        {
            get { return 4 + base.PackedLength; }
        }

        /// <summary>
        ///   Gets the message as a byte array ready to send over the network
        /// </summary>
        /// <returns>byte[] representing the message</returns>
        public override byte[] ToMsg()
        {
            var msg = new byte[PackedLength];
            var contentLength = PackedLength - 4;
            var lenHdr = Encoding.ASCII.GetBytes(contentLength.ToString().PadLeft(4, '0'));
            Array.Copy(lenHdr, msg, 4);
            var baseMsg = base.ToMsg();
            Array.Copy(baseMsg, 0, msg, 4, baseMsg.Length);
            return msg;
        }


        /// <summary>
        ///   Unpacks the message from a byte array
        /// </summary>
        /// <param name = "msg">message data to unpack</param>
        /// <param name = "startingOffset">the offset in the array to start</param>
        /// <returns>the offset in the array representing the start of the next message</returns>
#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity
        public override int Unpack(byte[] msg, int startingOffset)
#pragma warning restore S3776 // Refactor this method to reduce its Cognitive Complexity
        {

            // Field 127.25 could be an XML or could be a bitmap...
            // it has a length indicator of 4 bytes (max 8000) so get the data
            // and figure out if it's an xml or not..
            //
            // if it's a bitmap, parse it normal unpacking
            // if it's an XML, we do things differently

            // get the message length
            var buffer = new byte[4];
            Array.Copy(msg, startingOffset, buffer, 0, buffer.Length);
            if (int.TryParse(Template.MsgTypeFormatter.GetString(buffer), out int fieldLen)
                && fieldLen > 0 && fieldLen < 8000 && startingOffset + fieldLen <= msg.Length)
            {
                var msgData = new byte[fieldLen];
                Array.Copy(msg, startingOffset + 4, msgData, 0, msgData.Length);
                string possibleXml = Template.MsgTypeFormatter.GetString(msgData);
                if (possibleXml.Contains("</IccData>"))
                {
                    if (possibleXml.StartsWith("17IccData")) // is it a packed dictionary?
                    {
                        var d = PostilionKeyValue.ParseData(possibleXml);
                        if (!d.ContainsKey("IccData")) // strange O.o?
                            throw new FormatException("Could not find IccData!");
                        possibleXml = d["IccData"];
                    }

                    // we can has XML!
                    UnpackXML(possibleXml);
                    return startingOffset + 4 + fieldLen;
                }

                int fldLen = base.Unpack(msgData, 0);
                if (fldLen != fieldLen)
                    throw new FormatException($"Unpacked 127.25 length {fieldLen} not equal to header field len {fldLen}");
                return startingOffset + 4 + fieldLen;
            }


            // fallback to 
            throw new FormatException("Field 127.25 is not in an expected format");
        }


        private void UnpackXML(string iccDataXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(iccDataXML);
            XmlNodeList ns = xmlDoc.SelectNodes("IccData/IccRequest/*")!;
            if (ns == null || ns.Count == 0)
                ns = xmlDoc.SelectNodes("IccData/IccResponse/*")!;
#pragma warning disable S3267 // Loops should be simplified using the "Where" LINQ method
            foreach (XmlNode nsNode in ns)
            {
                if (xmlMapping.ContainsKey(nsNode.Name))
                    this[xmlMapping[nsNode.Name]] = nsNode.InnerText.Trim();
            }
#pragma warning restore S3267 // Loops should be simplified using the "Where" LINQ method
        }


        protected override IField CreateField(int field)
        {
            if (Template.ContainsKey(field))
                return new Field(field, Template[field]);

            throw new UnknownFieldException("127.25." + field);
        }

        /// <summary>
        /// Defines the Template used to describe the content of Field 127.49 - Realtime Private Field, Card Acceptor Additional Data
        /// </summary>
        /// <returns>A Template defining the subfields contained in Field 127.49</returns>
        protected static Template GetField127_25Template()
        {
            var template = new Template
            {
                { Bit._002_AmountAuthorized, FieldDescriptor.AsciiFixed(12, FieldValidators.Numeric, "Amount Authorized") },
                { Bit._003_AmountOther, FieldDescriptor.AsciiFixed(12, FieldValidators.Numeric, "Amount Other") },
                { Bit._004_ApplicationIdentifier, FieldDescriptor.AsciiVar(2,32, FieldValidators.AlphaNumericSpecial,"Application Id")},
                { Bit._005_ApplicationInterchangeProfile, FieldDescriptor.AsciiFixed(4, FieldValidators.AlphaNumericSpecial, "App Intchng Profile")},
                { Bit._006_ApplicationTransactionCounter, FieldDescriptor.AsciiFixed(4, FieldValidators.AlphaNumericSpecial, "App Txn Cntr")},
                { Bit._007_ApplicationUsageControl, FieldDescriptor.AsciiFixed(4, FieldValidators.AlphaNumericSpecial, "App Usage Ctrl")},
                { Bit._008_AuthorizationResponse, FieldDescriptor.AsciiFixed(2, FieldValidators.AlphaNumeric, "Auth Resp Cde")},
                { Bit._009_CardAuthenticationReliability, FieldDescriptor.AsciiFixed(1, FieldValidators.Numeric, "Crd Auth Reliable")},
                { Bit._010_CardAuthenticationResults, FieldDescriptor.AsciiFixed(1, FieldValidators.AlphaNumericSpecial, "Crd Auth Res")},
                { Bit._011_ChipCondition, FieldDescriptor.AsciiFixed(1, FieldValidators.Numeric, "Chip Cond")},
                { Bit._012_Cryptogram, FieldDescriptor.AsciiFixed(16, FieldValidators.AlphaNumericSpecial, "Cryptogram")},
                { Bit._013_CryptogramInformationData, FieldDescriptor.AsciiFixed(2, FieldValidators.AlphaNumericSpecial, "Crypto Info")},
                { Bit._014_CvmList, FieldDescriptor.AsciiVar(3,504, FieldValidators.AlphaNumericSpecial, "Cvm List") },
                { Bit._015_CvmResults, FieldDescriptor.AsciiFixed(6, FieldValidators.AlphaNumeric, "Cvm Results")},
                { Bit._016_InterfaceDeviceSerialNumber, FieldDescriptor.AsciiFixed(8, FieldValidators.AlphaNumericSpecial, "Device Serial")},
                { Bit._017_IssuerActionCode, FieldDescriptor.AsciiFixed(11, FieldValidators.AlphaNumericSpecial, "Iss Action Cde")},
                { Bit._018_IssuerApplicationData, FieldDescriptor.AsciiVar(2,64, FieldValidators.AlphaNumericSpecial, "Iss App Data") },
                { Bit._019_IssuerScriptResults, FieldDescriptor.AsciiVar(4,507, FieldValidators.Hex, "Iss Scr Res") },
                { Bit._020_TerminalApplicationVersionNumber, FieldDescriptor.AsciiFixed(4, FieldValidators.AlphaNumericSpecial, "Term App Ver")},
                { Bit._021_TerminalCapabilities, FieldDescriptor.AsciiFixed(6, FieldValidators.AlphaNumericSpecial, "Term Capabil")},
                { Bit._022_TerminalCountryCode, FieldDescriptor.AsciiFixed(3, FieldValidators.Numeric, "Term Country")},
                { Bit._023_TerminalType, FieldDescriptor.AsciiFixed(2, FieldValidators.Numeric, "Term Type")},
                { Bit._024_TerminalVerificationResult, FieldDescriptor.AsciiFixed(10, FieldValidators.AlphaNumericSpecial, "Term Verify Res")},
                { Bit._025_TransactionCategoryCode, FieldDescriptor.AsciiFixed(1, FieldValidators.AlphaNumericSpecial, "Txn Cat Code")},
                { Bit._026_TransactionCurrencyCode, FieldDescriptor.AsciiFixed(3, FieldValidators.Numeric, "Txn Currency")},
                { Bit._027_TransactionDate, FieldDescriptor.AsciiFixed(6, FieldValidators.Numeric, "Txn Date")},
                { Bit._028_TransactionSequenceCounter, FieldDescriptor.AsciiVar(1,8, FieldValidators.Numeric, "Txn Seq Num") },
                { Bit._029_TransactionType, FieldDescriptor.AsciiFixed(2, FieldValidators.Numeric, "Txn Type")},
                { Bit._030_UnpredictableNumber, FieldDescriptor.AsciiFixed(8, FieldValidators.AlphaNumericSpecial, "Unpredictable#")},
                { Bit._031_IssuerAuthenticationData, FieldDescriptor.AsciiVar(2,32, FieldValidators.AlphaNumericSpecial, "Iss Auth Data") },
                { Bit._032_IssuerScriptTemplate1, FieldDescriptor.AsciiVar(4,3354, FieldValidators.AlphaNumericSpecial, "Iss Script 1") },
                { Bit._033_IssuerScriptTemplate2, FieldDescriptor.AsciiVar(4,3354, FieldValidators.AlphaNumericSpecial, "Iss Script 2") },
                { Bit._034_PostilionPrivateICCResponseData, FieldDescriptor.AsciiVar(2,23, FieldValidators.AlphaNumericSpecial, "Pstl ICC Res") },
                { Bit._035_CustomerExclusiveData, FieldDescriptor.AsciiVar(2,64, FieldValidators.AlphaNumericSpecial, "Cust Data") },
                { Bit._036_FormFactorIndicator, FieldDescriptor.AsciiVar(2,64, FieldValidators.AlphaNumericSpecial, "Form Factor") },
            };
            template.BitmapFormatter = new Formatter.AsciiFormatter();
            return template;
        }

        private static readonly Dictionary<string, int> xmlMapping = new()
        {
            {"AmountAuthorized", Bit._002_AmountAuthorized},
            {"AmountOther", Bit._003_AmountOther},
            {"ApplicationIdentifier", Bit._004_ApplicationIdentifier},
            {"ApplicationInterchangeProfile", Bit._005_ApplicationInterchangeProfile},
            {"ApplicationTransactionCounter", Bit._006_ApplicationTransactionCounter},
            {"ApplicationUsageControl", Bit._007_ApplicationUsageControl},
            {"AuthorizationResponseCode", Bit._008_AuthorizationResponse},
            {"CardAuthenticationReliabilityIndicator", Bit._009_CardAuthenticationReliability},
            {"CardAuthenticationResultsCode", Bit._010_CardAuthenticationResults},
            {"ChipConditionCode", Bit._011_ChipCondition},
            {"Cryptogram", Bit._012_Cryptogram},
            {"CryptogramInformationData", Bit._013_CryptogramInformationData},
            {"CvmList", Bit._014_CvmList},
            {"CvmResults", Bit._015_CvmResults},
            {"InterfaceDeviceSerialNumber", Bit._016_InterfaceDeviceSerialNumber},
            {"IssuerActionCode", Bit._017_IssuerActionCode},
            {"IssuerApplicationData", Bit._018_IssuerApplicationData},
            {"IssuerScriptResults", Bit._019_IssuerScriptResults},
            {"TerminalApplicationVersionNumber", Bit._020_TerminalApplicationVersionNumber},
            {"TerminalCapabilities", Bit._021_TerminalCapabilities},
            {"TerminalCountryCode", Bit._022_TerminalCountryCode},
            {"TerminalType", Bit._023_TerminalType},
            {"TerminalVerificationResult", Bit._024_TerminalVerificationResult},
            {"TransactionCategoryCode", Bit._025_TransactionCategoryCode},
            {"TransactionCurrencyCode", Bit._026_TransactionCurrencyCode},
            {"TransactionDate", Bit._027_TransactionDate},
            {"TransactionSequenceCounter", Bit._028_TransactionSequenceCounter},
            {"TransactionType", Bit._029_TransactionType},
            {"UnpredictableNumber", Bit._030_UnpredictableNumber},
            {"IssuerAuthenticationData", Bit._031_IssuerAuthenticationData},
            {"IssuerScriptTemplate1", Bit._032_IssuerScriptTemplate1},
            {"IssuerScriptTemplate2", Bit._033_IssuerScriptTemplate2},
            
            // the following 2 are unconfirmed not documented properly in the interface guide
            // names assumed
            {"CustomerExclusiveData", Bit._035_CustomerExclusiveData },
            {"FormFactorIndicator", Bit._036_FormFactorIndicator }
        };

        public static class Bit
        {
            public const int _002_AmountAuthorized = 2;
            public const int _003_AmountOther = 3;
            public const int _004_ApplicationIdentifier = 4;
            public const int _005_ApplicationInterchangeProfile = 5;
            public const int _006_ApplicationTransactionCounter = 6;
            public const int _007_ApplicationUsageControl = 7;
            public const int _008_AuthorizationResponse = 8;
            public const int _009_CardAuthenticationReliability = 9;
            public const int _010_CardAuthenticationResults = 10;
            public const int _011_ChipCondition = 11;
            public const int _012_Cryptogram = 12;
            public const int _013_CryptogramInformationData = 13;
            public const int _014_CvmList = 14;
            public const int _015_CvmResults = 15;
            public const int _016_InterfaceDeviceSerialNumber = 16;
            public const int _017_IssuerActionCode = 17;
            public const int _018_IssuerApplicationData = 18;
            public const int _019_IssuerScriptResults = 19;
            public const int _020_TerminalApplicationVersionNumber = 20;
            public const int _021_TerminalCapabilities = 21;
            public const int _022_TerminalCountryCode = 22;
            public const int _023_TerminalType = 23;
            public const int _024_TerminalVerificationResult = 24;
            public const int _025_TransactionCategoryCode = 25;
            public const int _026_TransactionCurrencyCode = 26;
            public const int _027_TransactionDate = 27;
            public const int _028_TransactionSequenceCounter = 28;
            public const int _029_TransactionType = 29;
            public const int _030_UnpredictableNumber = 30;
            public const int _031_IssuerAuthenticationData = 31;
            public const int _032_IssuerScriptTemplate1 = 32;
            public const int _033_IssuerScriptTemplate2 = 33;
            public const int _034_PostilionPrivateICCResponseData = 34;
            public const int _035_CustomerExclusiveData = 35;
            public const int _036_FormFactorIndicator = 36;
        }
    }
}
