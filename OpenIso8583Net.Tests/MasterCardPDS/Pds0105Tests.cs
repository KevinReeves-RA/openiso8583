using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.MasterCardPDS;
using System;

namespace OpenIso8583Net.Tests.MasterCardPDS
{
    [TestClass]
    public class Pds0105Tests
    {

        [TestMethod]
        public void PdsParse()
        {

            var pds = Pds0105.Parse("0022306040000003166900095");

            Assert.AreEqual(2, pds.Type);
            Assert.AreEqual(new DateOnly(2023, 06, 04), pds.ReferenceDate);
            Assert.AreEqual(31669, pds.ProcessorID);
            Assert.AreEqual(95, pds.SequenceNumber);
        }

        [TestMethod]
        public void PdsParseDisplay()
        {
            string orig = "002/23-06-04/00000031669/00095";
            var pds = Pds0105.Parse(orig);

            Assert.AreEqual(2, pds.Type);
            Assert.AreEqual(new DateOnly(2023, 06, 04), pds.ReferenceDate);
            Assert.AreEqual(31669, pds.ProcessorID);
            Assert.AreEqual(95, pds.SequenceNumber);
            Assert.AreEqual("0022306040000003166900095", pds.PdsValue);
            Assert.AreEqual(orig, pds.ToString("D"));
        }
    }
}
