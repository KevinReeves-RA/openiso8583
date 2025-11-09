using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.MasterCardPDS;

namespace OpenIso8583Net.Tests.MasterCardPDS
{
    [TestClass]
    public class Pds0164Tests
    {

        [TestMethod]
        public void ParseAll()
        {
            var data = Pds0164.ParseAll("97891020430000M2210210698083656860000M2210210698192735000000M2210210698594886250000M2210210698695244250000M2210210699698361535000M22102106");

            Assert.AreEqual(6, data.Count);
            Assert.AreEqual(978, data[0].CurrentyCode);
            Assert.AreEqual(1.020430000m, data[0].ConversionRate);

            Assert.AreEqual(980, data[1].CurrentyCode);
            Assert.AreEqual(36.56860000m, data[1].ConversionRate);

            try
            {
                _ = Pds0164.Parse("123456");
                Assert.Fail();
            }
            catch { /* expected */ }

            try
            {
                Pds0164.ParseAll("123456");
                Assert.Fail();
            }
            catch { /* expected */ }
        }

        [TestMethod]
        public void ToMsg()
        {
            string rawData = "97891020430000M2210210698083656860000M2210210698192735000000M2210210698594886250000M2210210698695244250000M2210210699698361535000M22102106";
            var data = Pds0164.ParseAll(rawData);
            string res = string.Empty;
            foreach (var item in data)
            {
                res += item.ToMsg();
            }
            Assert.AreEqual(rawData, res);
        }
    }
}
