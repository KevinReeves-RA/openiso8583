using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace OpenIso8583Net.Tests
{
    [TestClass]
    public class MasterCardMessageTests
    {
        public MasterCardMessageTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [TestMethod]
        public void PDSTests()
        {
            // TODO: 

            var pds = Iso8583MasterCard.GetPDSFields("0001011Hello World0002019Goodbye Cruel world");
            Assert.AreEqual(2, pds.Count);
            var pds1 = pds["0001"];
            var pds2 = pds["0002"];
            Assert.AreEqual("Hello World", pds1);
            Assert.AreEqual("Goodbye Cruel world", pds2);
        }

        [TestMethod]
        public void EbcdicTest()
        {
            // create ebcdic message
            var msg = new Iso8583MasterCard(Encoding.GetEncoding(1140));

            byte[] rawMsg = "F1F6F4F480000100000100000200000000000000F6F9F7F0F8F0F0F1F0F5F0F2F5F0F0F2F2F3F0F6F0F4F0F0F0F0F0F0F3F1F6F6F9F0F0F0F9F5F0F1F1F0F0F2F5F0F0F2F2F3F0F6F0F4F0F0F0F0F0F0F3F1F6F6F9F0F0F0F9F5F0F1F2F2F0F0F1E3F0F1F9F1F0F0F1F2F0F0F0F0F0F0F0F1".ToByteArray();
            msg.Unpack(rawMsg, 0);

            Assert.AreEqual(1644, msg.MessageType);

            Assert.AreEqual("697", msg[24]);
            Assert.AreEqual("01050250022306040000003166900095011002500223060400000031669000950122001T01910012", msg[48]);
            Assert.AreEqual("00000001", msg[71]);

        }

        [TestMethod]
        public void EbcdicToAsciiTest()
        {
            var msg = new Iso8583MasterCard(Encoding.GetEncoding(1140));

            byte[] rawMsg = "F1F6F4F480000100000100000200000000000000F6F9F7F0F8F0F0F1F0F5F0F2F5F0F0F2F2F3F0F6F0F4F0F0F0F0F0F0F3F1F6F6F9F0F0F0F9F5F0F1F1F0F0F2F5F0F0F2F2F3F0F6F0F4F0F0F0F0F0F0F3F1F6F6F9F0F0F0F9F5F0F1F2F2F0F0F1E3F0F1F9F1F0F0F1F2F0F0F0F0F0F0F0F1".ToByteArray();
            msg.Unpack(rawMsg, 0);

            byte[] resMsg = msg.ToMsg(Encoding.ASCII);
            Assert.AreEqual("313634348000010000010000020000000000000036393730383030313035303235303032323330363034303030303030333136363930303039353031313030323530303232333036303430303030303033313636393030303935303132323030315430313931303031323030303030303031", resMsg.ToHex());

        }

        [TestMethod]
        public void AsciiToEbcdicTest()
        {
            var msg = new Iso8583MasterCard(Encoding.ASCII);

            byte[] rawMsg = "313634348000010000010000020000000000000036393730383030313035303235303032323330363034303030303030333136363930303039353031313030323530303232333036303430303030303033313636393030303935303132323030315430313931303031323030303030303031".ToByteArray();
            msg.Unpack(rawMsg, 0);

            byte[] resMsg = msg.ToMsg(Encoding.GetEncoding(1140));
            Assert.AreEqual("F1F6F4F480000100000100000200000000000000F6F9F7F0F8F0F0F1F0F5F0F2F5F0F0F2F2F3F0F6F0F4F0F0F0F0F0F0F3F1F6F6F9F0F0F0F9F5F0F1F1F0F0F2F5F0F0F2F2F3F0F6F0F4F0F0F0F0F0F0F3F1F6F6F9F0F0F0F9F5F0F1F2F2F0F0F1E3F0F1F9F1F0F0F1F2F0F0F0F0F0F0F0F1", resMsg.ToHex());

        }

    }
}
