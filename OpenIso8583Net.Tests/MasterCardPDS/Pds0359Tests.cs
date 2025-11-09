using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.MasterCardPDS;
using System;

namespace OpenIso8583Net.Tests.MasterCardPDS
{
    [TestClass]
    public class Pds0359Tests
    {
        [TestMethod]
        public void Parse()
        {
            var pds = Pds0359.Parse("1264       009738479                   3ME00071001N2306230323062301");
            Assert.IsNotNull(pds);

            Assert.AreEqual("1264", pds.AgentID);
            Assert.AreEqual("009738479", pds.AgentAccount);
            Assert.AreEqual(3, pds.LevelCode);
            Assert.AreEqual("ME00071001", pds.ServiceID);
            Assert.AreEqual("N", pds.ExchangeRateClassCode);
            Assert.AreEqual(new DateOnly(2023, 06, 23), pds.ReconDate);
            Assert.AreEqual(3, pds.ReconCycle);
            Assert.AreEqual(new DateOnly(2023, 06, 23), pds.SettlementDate);
            Assert.AreEqual(1, pds.SettlementCycle);
        }
    }
}
