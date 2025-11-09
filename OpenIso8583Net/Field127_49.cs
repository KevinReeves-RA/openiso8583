using OpenIso8583Net.Exceptions;
using OpenIso8583Net.FieldValidator;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenIso8583Net
{
    /// <summary>
    /// Postilion Field 127.49 - 
    /// </summary>
#pragma warning disable S101 // ename class 'Field127_49' to match pascal case naming rules (can't make 172.49!!!)
    public class Field127_49 : AMessage, IField
#pragma warning restore S101
    {
        public Field127_49() : base(GetField127_49Template()) { }
        #region IField implementation

        [JsonIgnore]
        public int FieldNumber => 49;

        [JsonIgnore]
        public string Value { get => null!; set => throw new NotImplementedException(); }
        #endregion
        /// <summary>
        ///   Gets the packed length of the message
        /// </summary>
        public new int PackedLength
        {
            get { return 3 + base.PackedLength; }
        }

        /// <summary>
        ///   Gets the message as a byte array ready to send over the network
        /// </summary>
        /// <returns>byte[] representing the message</returns>
        public override byte[] ToMsg()
        {
            var msg = new byte[PackedLength];
            var contentLength = PackedLength - 3;
            var lenHdr = Encoding.ASCII.GetBytes(contentLength.ToString().PadLeft(3, '0'));
            Array.Copy(lenHdr, msg, 3);
            var baseMsg = base.ToMsg();
            Array.Copy(baseMsg, 0, msg, 3, baseMsg.Length);
            return msg;
        }

        /// <summary>
        ///   Unpacks the message from a byte array
        /// </summary>
        /// <param name = "msg">message data to unpack</param>
        /// <param name = "startingOffset">the offset in the array to start</param>
        /// <returns>the offset in the array representing the start of the next message</returns>
        public override int Unpack(byte[] msg, int startingOffset)
        {
            // Field 127.49 is actually a bitmap message but inside a field in Iso8583Post.
            // That field has a length indicator of 3 bytes, so lets just ignore it.
            // Yes I should be checking that everything adds up
            // Future: Stop being a Muppet and check the length indicator
            return base.Unpack(msg, 3 + startingOffset);
        }

        protected override IField CreateField(int field)
        {
            if (Template.ContainsKey(field))
                return new Field(field, Template[field]);

            throw new UnknownFieldException("127.49." + field);
        }

        /// <summary>
        /// Defines the Template used to describe the content of Field 127.49 - Realtime Private Field, Card Acceptor Additional Data
        /// </summary>
        /// <returns>A Template defining the subfields contained in Field 127.49</returns>
        protected static Template GetField127_49Template()
        {
            var template = new Template
            {
                { Bit._002_CardAcceptorPhoneNumber, FieldDescriptor.AsciiVar(2,16, FieldValidators.AlphaNumericSpecial, "Acceptor Phone #")},
                { Bit._003_StreetAddress, FieldDescriptor.AsciiVar(2,48, FieldValidators.AlphaNumericSpecial,           "Street Addr")},
                { Bit._004_RetailerPhoneNumber, FieldDescriptor.AsciiVar(2,16, FieldValidators.AlphaNumericSpecial,     "Retailer Phone #")},
                { Bit._005_CountrySubdivisionCode, FieldDescriptor.AsciiFixed(3, FieldValidators.AlphaNumericSpecial,     "CntryRegionCode")},
                { Bit._006_URL, FieldDescriptor.AsciiVar(3,255, FieldValidators.AlphaNumericSpecial, "URL") },
                { Bit._007_CustomerServicePhoneNumber, FieldDescriptor.AsciiVar(2,16, FieldValidators.AlphaNumericSpecial,"CustSrv Phone") },
                { Bit._008_AdditionalContactInfo, FieldDescriptor.AsciiVar(3,25,FieldValidators.AlphaNumericSpecial, "More Contact") },
                { Bit._009_TaxId, FieldDescriptor.AsciiVar(3,21,FieldValidators.AlphaNumericSpecial, "Tax ID") },
                { Bit._010_PartnerIdCode, FieldDescriptor.AsciiVar(3,6, FieldValidators.AlphaNumericSpecial, "Partner ID") },
                { Bit._011_ServiceLocationCityName, FieldDescriptor.AsciiVar(3,13,FieldValidators.AlphaNumericSpecial, "SvcLoc City") },
                { Bit._012_ServiceLocationCountrySubdivisionCode, FieldDescriptor.AsciiFixed(3, FieldValidators.AlphaNumericSpecial, "SvcLoc Subdiv") },
                { Bit._013_ServiceLocationCountryCode, FieldDescriptor.AsciiFixed(3,FieldValidators.AlphaNumericSpecial,"SvcLoc Cntry Code") },
                { Bit._014_ServiceLocationPostalCode, FieldDescriptor.AsciiVar(3,10, FieldValidators.AlphaNumericSpecial, "SvcLoc PO Code") },
                { Bit._015_CardAcceptorGeoCoordinates, FieldDescriptor.AsciiVar(3,20, FieldValidators.AlphaNumericSpecial, "GPS Card Acceptor") },
                { Bit._016_ServiceLocationGeoCoordinates, FieldDescriptor.AsciiVar(3,20, FieldValidators.AlphaNumericSpecial, "GPS Service Loc") },
            };

            // the postilion code adds these for future expansion...
            for (int fld = 17; fld <= 64; fld++)
                template.Add(fld, FieldDescriptor.AsciiVar(3, 999, FieldValidators.None, $"Field {fld}"));

            template.BitmapLength = 2;
            return template;
        }



        public static class Bit
        {
            public const int _002_CardAcceptorPhoneNumber = 2;
            public const int _003_StreetAddress = 3;
            public const int _004_RetailerPhoneNumber = 4;
            public const int _005_CountrySubdivisionCode = 5;
            public const int _006_URL = 6;
            public const int _007_CustomerServicePhoneNumber = 7;
            public const int _008_AdditionalContactInfo = 8;
            public const int _009_TaxId = 9;
            public const int _010_PartnerIdCode = 10;
            public const int _011_ServiceLocationCityName = 11;
            public const int _012_ServiceLocationCountrySubdivisionCode = 12;
            public const int _013_ServiceLocationCountryCode = 13;
            public const int _014_ServiceLocationPostalCode = 14;
            public const int _015_CardAcceptorGeoCoordinates = 15;
            public const int _016_ServiceLocationGeoCoordinates = 16;
        }
    }
}
