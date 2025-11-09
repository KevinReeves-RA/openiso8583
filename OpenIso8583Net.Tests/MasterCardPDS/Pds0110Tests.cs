using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.MasterCardPDS;
using System;

namespace OpenIso8583Net.Tests.MasterCardPDS
{
    [TestClass]
    public class Pds0110Tests
    {
        [TestMethod]
        public void Pds0110ParseTest()
        {
            var pds = Pds0110.Parse("0022306040000003166900095");
            Assert.IsNotNull(pds);
            Assert.AreEqual(2, pds.Type);
            Assert.AreEqual(new DateOnly(2023, 06, 04), pds.ReferenceDate);
            Assert.AreEqual(31669, pds.ProcessorID);
            Assert.AreEqual(95, pds.SequenceNumber);
        }
    }
}
