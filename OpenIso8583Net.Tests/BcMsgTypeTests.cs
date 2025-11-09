namespace OpenIso8583Net.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenIso8583Net.Tests.TestMessages;

    [TestClass]
    public class BcMsgTypeTests
    {
        [TestMethod]
        public void TestBcdPackUnpack()
        {
            var msg = new BCDIsoMsg();
            msg.MessageType = 100;
            byte[] packed = msg.ToMsg();

            var unpacked = new BCDIsoMsg();
            unpacked.Unpack(packed, 0);
            Assert.AreEqual(100, unpacked.MessageType);
        }
    }
}