using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.Exceptions;
using System;
using System.Text;

namespace OpenIso8583Net.Tests
{
    /// <summary>
    ///   Summary description for Iso8583PostTests
    /// </summary>
    [TestClass]
    public class Iso8583PostTests
    {

        [TestMethod]
        public void TestIso8583PostToMsg()
        {
            var msg = new Iso8583Post();
            msg.MessageType = 200;
            msg[3] = "000000";
            msg.Private[2] = "hello";
            var actual = msg.ToMsg();

            var mtid = Encoding.ASCII.GetBytes("0200");

            var bitmap = new Bitmap();
            bitmap[3] = true;
            bitmap[127] = true;
            var primaryBitmap = bitmap.ToMsg();
            var primaryMessageContent = Encoding.ASCII.GetBytes("000000");

            bitmap = new Bitmap();
            bitmap[2] = true;
            var privateBitmap = bitmap.ToMsg();
            var privateContent = Encoding.ASCII.GetBytes("05hello");
            var privateLength = privateBitmap.Length + privateContent.Length;
            var privateMessage = new byte[privateLength];
            Array.Copy(privateBitmap, privateMessage, privateBitmap.Length);
            Array.Copy(privateContent, 0, privateMessage, privateBitmap.Length, privateContent.Length);
            var privateMessageLengthHeader = Encoding.ASCII.GetBytes(privateLength.ToString().PadLeft(6, '0'));

            var messageLength = 4 + primaryBitmap.Length + 6 + 6 + privateMessage.Length;

            var message = new byte[messageLength];
            var offset = 0;
            Array.Copy(mtid, 0, message, offset, mtid.Length);
            offset += mtid.Length;

            Array.Copy(primaryBitmap, 0, message, offset, primaryBitmap.Length);
            offset += primaryBitmap.Length;

            Array.Copy(primaryMessageContent, 0, message, offset, primaryMessageContent.Length);
            offset += primaryMessageContent.Length;

            Array.Copy(privateMessageLengthHeader, 0, message, offset, privateMessageLengthHeader.Length);
            offset += privateMessageLengthHeader.Length;

            Array.Copy(privateMessage, 0, message, offset, privateMessage.Length);

            Assert.AreEqual(messageLength, msg.PackedLength, "Message length not equal");

            var equals = true;
            for (var i = 0; i < messageLength; i++)
                equals &= message[i] == actual[i];

            Assert.AreEqual(true, equals, "Messages not equal");

            // push the code coverage
            Assert.IsFalse(string.IsNullOrWhiteSpace(msg.DescribePacking()));
        }

        [TestMethod]
        public void TestIso8583PostUnpack()
        {
            var mtid = Encoding.ASCII.GetBytes("0200");

            var bitmap = new Bitmap();
            bitmap[3] = true;
            bitmap[127] = true;
            var primaryBitmap = bitmap.ToMsg();
            var primaryMessageContent = Encoding.ASCII.GetBytes("000000");

            bitmap = new Bitmap();
            bitmap[2] = true;
            var privateBitmap = bitmap.ToMsg();
            var privateContent = Encoding.ASCII.GetBytes("05hello");
            var privateLength = privateBitmap.Length + privateContent.Length;
            var privateMessage = new byte[privateLength];
            Array.Copy(privateBitmap, privateMessage, privateBitmap.Length);
            Array.Copy(privateContent, 0, privateMessage, privateBitmap.Length, privateContent.Length);
            var privateMessageLengthHeader = Encoding.ASCII.GetBytes(privateLength.ToString().PadLeft(6, '0'));

            var messageLength = 4 + primaryBitmap.Length + 6 + 6 + privateMessage.Length;

            var message = new byte[messageLength];
            var offset = 0;
            Array.Copy(mtid, 0, message, offset, mtid.Length);
            offset += mtid.Length;

            Array.Copy(primaryBitmap, 0, message, offset, primaryBitmap.Length);
            offset += primaryBitmap.Length;

            Array.Copy(primaryMessageContent, 0, message, offset, primaryMessageContent.Length);
            offset += primaryMessageContent.Length;

            Array.Copy(privateMessageLengthHeader, 0, message, offset, privateMessageLengthHeader.Length);
            offset += privateMessageLengthHeader.Length;

            Array.Copy(privateMessage, 0, message, offset, privateMessage.Length);

            var msg = new Iso8583Post();
            msg.Unpack(message, 0);

            Assert.AreEqual(message.Length, msg.PackedLength);
            Assert.AreEqual("000000", msg[3]);
            Assert.AreEqual("hello", msg.Private[2]);
        }

        [TestMethod]
        public void TestDontKnow()
        {
            var msg = new Iso8583Post();
            msg.MessageType = 200;
            msg[3] = "270000";
            //msg.TransactionAmount = 400;
            //msg.TransmissionDateTime.SetNow();
            msg[11] = "123456";
            msg[12] = "151518";
            msg[13] = "1212";
            msg[22] = "012";
            msg[25] = "00";
            msg[26] = "12";
            msg[32] = "588892";
            msg[33] = "123456";
            msg[37] = "123456789123";
            msg[41] = "21458796";
            msg[42] = "100200300400500";
            msg[43] = new string('x', 40);
            msg[48] = "A";
            msg[49] = "716";
            msg[100] = "123456";
            msg[102] = "9012273811";
            msg[103] = "010203040506";
            msg[123] = "100111100130119";

            msg.Private[Field127.Bit._002_SWITCH_KEY] = DateTime.Now.ToString("yyyyMMDDHHmmss");

#pragma warning disable 168
            var data = msg.ToMsg();
#pragma warning restore 168
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void TestIso8583PostTemplateRetrievalReferenceNumber()
        {
            var iso8583Post = new Iso8583Post();

            iso8583Post.MessageType = 200;
            iso8583Post[Iso8583.Bit._003_PROC_CODE] = "000000";
            iso8583Post[Iso8583.Bit._037_RETRIEVAL_REF_NUM] = "RRN       12";
            iso8583Post[Iso8583.Bit._038_AUTH_ID_RESPONSE] = "123456";

            var rawBytes = iso8583Post.ToMsg();

            Assert.IsNotNull(rawBytes);

            var iso8583 = new Iso8583();
            FieldFormatException expected = null!;
            try
            {
                iso8583.Unpack(rawBytes, 0);
            }
            catch (FieldFormatException ffe)
            {
                expected = ffe;
            }

            Assert.IsNotNull(expected);
            Assert.AreEqual(Iso8583.Bit._037_RETRIEVAL_REF_NUM, expected.FieldNumber);
        }

        [TestMethod]
        public void TestIso8583PostTemplateAuthIdResponse()
        {
            var iso8583Post = new Iso8583Post();

            iso8583Post.MessageType = 200;
            iso8583Post[Iso8583.Bit._003_PROC_CODE] = "000000";
            iso8583Post[Iso8583.Bit._037_RETRIEVAL_REF_NUM] = "123456789012";
            iso8583Post[Iso8583.Bit._038_AUTH_ID_RESPONSE] = "12 abc";

            var rawBytes = iso8583Post.ToMsg();

            Assert.IsNotNull(rawBytes);

            var iso8583 = new Iso8583();
            FieldFormatException expected = null!;
            try
            {
                iso8583.Unpack(rawBytes, 0);
            }
            catch (FieldFormatException ffe)
            {
                expected = ffe;
            }

            Assert.IsNotNull(expected);
            Assert.AreEqual(Iso8583.Bit._038_AUTH_ID_RESPONSE, expected.FieldNumber);
        }

        [TestMethod]
        public void TestIso8583PostTemplateEchoData()
        {
            var iso8583Post = new Iso8583Post();

            iso8583Post.MessageType = 200;
            iso8583Post[Iso8583.Bit._003_PROC_CODE] = "000000";
            iso8583Post[Iso8583.Bit._037_RETRIEVAL_REF_NUM] = "123456789012";
            iso8583Post[Iso8583.Bit._038_AUTH_ID_RESPONSE] = "123456";
            iso8583Post[Iso8583Post.Bit._059_ECHO_DATA] = "Echo Data";

            var rawBytes = iso8583Post.ToMsg();

            Assert.IsNotNull(rawBytes);

            var iso8583 = new Iso8583();
            UnknownFieldException expected = null!;
            try
            {
                iso8583.Unpack(rawBytes, 0);
            }
            catch (UnknownFieldException e)
            {
                expected = e;
            }

            Assert.IsNotNull(expected);
            Assert.AreEqual("59", expected.FieldNumber);
        }

        [TestMethod]
        public void Parse127Bitmaps()
        {
            byte[] rawMsg = "30323030f23c46c129e0900000000000000000223136353232313138323033353930363130343030303030303030303030303030363030303039323631313339333139333135303731333339333130393236323730373539393930353130303030303036303232313337353232313138323033353930363130343d3237303732303630303030303030383831303030303030303039303136363738323036373432373032333330303030303030303032303030303152657461696c204173736973742054657374204d6572634361706520546f776e2020202057435a41373130647c79c832afd58830313541313031303135313333343131303130303033353700080480000080005743202020383030312020202020373130303031313032313654656e64657244657461696c4755494432333643433139323633352d374433392d343241312d394338342d3842373238444232343443323232324f726967696e616c4164646974696f6e616c446174613232343633343534383334303932363133333932343030303239373031373837433142354636433030303030303030303030303030303036303030303030303030303030303030313441303030303030303034313031303339303030303936453341453835313634384233454445393830343230333030373432373032333333363038313041303432303041433334303430303030303030303030303030303030303046463030303245304638433837313032323030303032343830303037313032333039323630303432423138313441303332280032353137204e6577204368757263682053742043617065746f776e574320".ToByteArray();
            Iso8583Post baseMsg = new();
            baseMsg.Unpack(rawMsg);

            Assert.IsNotNull(baseMsg.Private?.CardAcceptorAdditionalData);
            Assert.IsNotNull(baseMsg.Private?.ICCData);

            Assert.AreEqual("WC ", baseMsg.Private.CardAcceptorAdditionalData[5]); // 
            Assert.AreEqual("42B1814A", baseMsg.Private.ICCData[30]); // unpredictable number
        }

        [TestMethod]
        public void Parse127_49()
        {
            byte[] rawMsg = "39323230f23e46958ee08520000000020000002231363532383439373131303031323239353930303030303030303030303030303535303131303138303734373038303130303331303934373038313031383235313231303138353939393031313030323030433030303030303030433030303030303030303630333136363930363033313636393030303030303032353738353931323531303030393132333435363731323334353637382020202020202053686f703253686f7020202020202020202020202020204350542020202020202020202057435a413731303032303030353337313044303030303030303035353031303034313531303131364d43433030303048412020203130313820202020202020202020203931323531303030303030303030303235373835303331363639202020202030313130303130303330313031383037343730383030303030303331363639303030303030333136363931303138303734373038202020202020303030303030303035353031303030303030303030303030433030303030303030433030303030303030303135313131323031313030303031303031303031323439641c1620010080003130303030303032353738355332535372632020202020204d4343536e6b2020202020203030303135323031303033314d43496e74437265646974203131303953686f703253686f702020202020202020202020202020202020202020202020202032303233313031383030363530323138506f7374696c696f6e3a4d657461446174613331363332333052657461696c4173736973742e4f726967696e616c5377697463684b6579313131323132506f7374696c696f6e3a544d3131313231385061796d656e74734150492e7573657249643131313233335061796d656e74734150492e6d65726368616e745472616e73616374696f6e4964313131323238506f7374696c696f6e3a50757263686173696e67436172644461746131313131374375726c54786e31313132333052657461696c4173736973742e4f726967696e616c5377697463684b65793233323538623432313533636637393466386261303635303934656233343961396362323132506f7374696c696f6e3a544d32313431365372634d54493134303230303231385061796d656e74734150492e7573657249643233323833636166326361663138336463383662363538663066316336353939343335323238506f7374696c696f6e3a50757263686173696e674361726444617461333139383c3f786d6c2076657273696f6e3d22312e302220656e636f64696e673d225554462d38223f3e3c50757263686173696e67436172644461746120436172644163636570746f7254617849643d2274617849642220446973636f756e74416d6f756e743d2230303030303030303030303022205368697070696e67416d6f756e743d22303030303030303030303030222044757479416d6f756e743d223030303030303030303030302220546f74616c416d6f756e743d22303030303030303035353031222f3e3233335061796d656e74734150492e6d65726368616e745472616e73616374696f6e49643231393030303030303737373730303032373037323231374375726c54786e313454727565425546552042757920757320202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202020202070686f6e652020202020202020202020202020202020202020202020202020202020205931303531324d31323332384336373131383341303044303546314335384132304330364232314437364331324d30323331313233343536373820202020202020303030313532313031383037343730383232325072696f726974697a654f726967696e616c5265667331313031313567fc303570686f6e653133362048657265207374726565743032337777772e72657461696c6173736973742e676c6f62616c313030323137313239393034303130616464436f6e746163743030357461784964303034706172743030394361706520546f776e5743435a414630303434333231".ToByteArray();
            Iso8583Post baseMsg = new();
            baseMsg.Unpack(rawMsg);

            Assert.IsNotNull(baseMsg.Private);
            Assert.IsNotNull(baseMsg.Private.CardAcceptorAdditionalData);

            Assert.AreEqual("phone", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._002_CardAcceptorPhoneNumber]);
            Assert.AreEqual("6 Here street", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._003_StreetAddress]);
            // 2023-10-18 : could not create these from ConnectUp API - perhaps in the future we can update the test to include these
            //Assert.AreEqual("", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._004_RetailerPhoneNumber]);
            //Assert.AreEqual("", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._005_CountrySubdivisionCode]);
            Assert.AreEqual("www.retailassist.global", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._006_URL]);
            Assert.AreEqual("0217129904", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._007_CustomerServicePhoneNumber]);
            Assert.AreEqual("addContact", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._008_AdditionalContactInfo]);
            Assert.AreEqual("taxId", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._009_TaxId]);
            Assert.AreEqual("part", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._010_PartnerIdCode]);
            Assert.AreEqual("Cape Town", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._011_ServiceLocationCityName]);
            Assert.AreEqual("WCC", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._012_ServiceLocationCountrySubdivisionCode]);
            Assert.AreEqual("ZAF", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._013_ServiceLocationCountryCode]);
            Assert.AreEqual("4321", baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._014_ServiceLocationPostalCode]);
            // TODO : find an example of this
            Assert.IsNull(baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._015_CardAcceptorGeoCoordinates]);
            Assert.IsNull(baseMsg.Private.CardAcceptorAdditionalData[Field127_49.Bit._016_ServiceLocationGeoCoordinates]);
        }
    }
}