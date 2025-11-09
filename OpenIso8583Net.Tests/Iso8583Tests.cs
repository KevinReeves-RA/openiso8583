// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Iso8583Tests.cs" company="John Oxley">
//   2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenIso8583Net.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Text;

    /// <summary>
    /// Iso8583 Tests
    /// </summary>
    [TestClass]
    public class Iso8583Tests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The test clear field.
        /// </summary>
        [TestMethod]
        public void TestClearField()
        {
            var msg = new Iso8583();
            msg[2] = "123456789123456";
            msg.ClearField(2);
            Assert.AreEqual(false, msg.IsFieldSet(2));
            Assert.AreEqual(null!, msg[2]);
        }

        /// <summary>
        /// The test clear field that is null.
        /// </summary>
        [TestMethod]
        public void TestClearFieldThatIsNull()
        {
            var msg = new Iso8583();
            msg.ClearField(2);
            Assert.IsNull(msg[2]);
            Assert.IsFalse(msg.IsFieldSet(2));
        }

        /// <summary>
        /// The test message extended pack.
        /// </summary>
        [TestMethod]
        public void TestMessageExtendedPack()
        {
            var msg = new Iso8583();
            msg[2] = "58889212354567816";
            msg[3] = "270010";
            msg[102] = "9012273811";
            msg.MessageType = 200;

            var actual = msg.ToMsg();

            var mtid = Encoding.ASCII.GetBytes("0200");
            var bitmap = new Bitmap();
            bitmap[2] = true;
            bitmap[3] = true;
            bitmap[102] = true;
            var bitmapData = bitmap.ToMsg();
            var msgContent = Encoding.ASCII.GetBytes("1758889212354567816270010109012273811");

            var fullMessageLength = 4 + bitmapData.Length + msgContent.Length;
            Assert.AreEqual(fullMessageLength, msg.PackedLength, "Incorrect packed length");

            var expected = new byte[fullMessageLength];

            Array.Copy(mtid, expected, 4);
            Array.Copy(bitmapData, 0, expected, 4, bitmapData.Length);
            Array.Copy(msgContent, 0, expected, 4 + bitmapData.Length, msgContent.Length);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPrivateMessagePack()
        {
            var msg = new Iso8583Post();
            msg[2] = "58889212354567816";
            msg[3] = "270010";
            msg[102] = "9012273811";
            msg.MessageType = 9820;

            var bitmap = new Bitmap();
            bitmap[2] = true;
            bitmap[3] = true;
            bitmap[102] = true;
            var bitmapData = bitmap.ToMsg();
            var msgContent = Encoding.ASCII.GetBytes("1758889212354567816270010109012273811");

            var fullMessageLength = 4 + bitmapData.Length + msgContent.Length;
            Assert.AreEqual(fullMessageLength, msg.PackedLength, "Incorrect packed length");
        }

        /// <summary>
        /// The test message extended unpack.
        /// </summary>
        [TestMethod]
        public void TestMessageExtendedUnpack()
        {
            var mtid = Encoding.ASCII.GetBytes("0200");
            var bitmap = new Bitmap();
            bitmap[2] = true;
            bitmap[3] = true;
            bitmap[102] = true;
            var bitmapData = bitmap.ToMsg();
            var msgContent = Encoding.ASCII.GetBytes("1758889290122738116270010109012273811");

            var fullMessageLength = 4 + bitmapData.Length + msgContent.Length;
            var raw = new byte[fullMessageLength];
            Array.Copy(mtid, raw, 4);
            Array.Copy(bitmapData, 0, raw, 4, bitmapData.Length);
            Array.Copy(msgContent, 0, raw, 4 + bitmapData.Length, msgContent.Length);

            var msg = new Iso8583();
            var offset = msg.Unpack(raw, 0);

            Assert.AreEqual(fullMessageLength, offset, "The offset for the message is incorrect");
            Assert.AreEqual(true, msg.IsFieldSet(2), "Expected field 2");
            Assert.AreEqual("58889290122738116", msg[2], "Expected field 2 value 58889290122738116");
            Assert.AreEqual(true, msg.IsFieldSet(3), "Expected field 3");
            Assert.AreEqual("270010", msg[3], "Expected field 3 value 270010");
            Assert.AreEqual(true, msg.IsFieldSet(102), "Expected field 102");
            Assert.AreEqual("9012273811", msg[102], "Expected field 102 value 9012273811");
        }

        /// <summary>
        /// The test message non-extended pack.
        /// </summary>
        [TestMethod]
        public void TestMessageNotExtendedPack()
        {
            var msg = new Iso8583();
            msg[2] = "58889290122738116";
            msg[3] = "270010";
            msg.MessageType = 200;

            var actual = msg.ToMsg();

            var mtid = Encoding.ASCII.GetBytes("0200");
            var bitmap = new Bitmap();
            bitmap[2] = true;
            bitmap[3] = true;
            var bitmapData = bitmap.ToMsg();
            var msgContent = Encoding.ASCII.GetBytes("1758889290122738116270010");

            var fullMessageLength = 4 + bitmapData.Length + msgContent.Length;
            var expected = new byte[fullMessageLength];

            Array.Copy(mtid, expected, 4);
            Array.Copy(bitmapData, 0, expected, 4, bitmapData.Length);
            Array.Copy(msgContent, 0, expected, 4 + bitmapData.Length, msgContent.Length);

            Assert.AreEqual(fullMessageLength, msg.PackedLength, "Incorrect packed length");

            var equal = true;
            for (var i = 0; i < fullMessageLength; i++)
            {
                equal &= expected[i] == actual[i];
            }

            Assert.AreEqual(true, equal, "Messages not equal");
        }

        /// <summary>
        /// The test message non-extended unpack.
        /// </summary>
        [TestMethod]
        public void TestMessageNotExtendedUnpack()
        {
            var mtid = Encoding.ASCII.GetBytes("0200");
            var bitmap = new Bitmap();
            bitmap[2] = true;
            bitmap[3] = true;
            var bitmapData = bitmap.ToMsg();
            var msgContent = Encoding.ASCII.GetBytes("1758889290122738116270010");

            var fullMessageLength = 4 + bitmapData.Length + msgContent.Length;
            var raw = new byte[fullMessageLength];
            Array.Copy(mtid, raw, 4);
            Array.Copy(bitmapData, 0, raw, 4, bitmapData.Length);
            Array.Copy(msgContent, 0, raw, 4 + bitmapData.Length, msgContent.Length);

            var msg = new Iso8583();
            var offset = msg.Unpack(raw, 0);

            Assert.AreEqual(fullMessageLength, offset, "The offset for the message is incorrect");
            Assert.AreEqual(true, msg.IsFieldSet(2), "Expected field 2");
            Assert.AreEqual("58889290122738116", msg[2], "Expected field 2 value 58889290122738116");
            Assert.AreEqual(true, msg.IsFieldSet(3), "Expected field 3");
            Assert.AreEqual("270010", msg[3], "Expected field 3 value 270010");
        }

        /// <summary>
        /// The test setting null clears field.
        /// </summary>
        [TestMethod]
        public void TestSettingNullClearsField()
        {
            var msg = new Iso8583();
            msg[2] = "12345678912365";
            msg[2] = null!;
            Assert.IsNull(msg[2]);
            Assert.IsFalse(msg.IsFieldSet(2));
        }

        #endregion
    }
}