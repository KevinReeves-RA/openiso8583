// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsoConvertTests.cs" company="John Oxley">
//   2012
// </copyright>
// <summary>
//   Summary description for ConvertTests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenIso8583Net.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///   IsoConvert tests
    /// </summary>
    [TestClass]
    public class IsoConvertTests
    {
        #region Public Methods and Operators

        /// <summary>
        ///   The from int to msg type test.
        /// </summary>
        [TestMethod]
        public void FromIntToMsgTypeTest()
        {
            var res = IsoConvert.FromIntToMsgType(200);
            Assert.AreEqual("0200", res);
        }

        /// <summary>
        ///   The from msg type to int.
        /// </summary>
        [TestMethod]
        public void FromMsgTypeToInt()
        {
            var res = IsoConvert.FromMsgTypeToInt("0200");
            Assert.AreEqual(200, res);
        }

        #endregion
    }
}