// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsTest.cs" company="John Oxley">
//   2012
// </copyright>
// <summary>
//   This is a test class for UtilsTest and is intended to contain all UtilsTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenIso8583Net.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for UtilsTest and is intended to contain all UtilsTest Unit Tests
    /// </summary>
    [TestClass]
    public class UtilsTest
    {
        #region Public Methods and Operators

        /// <summary>
        /// A test for MaskPan
        /// </summary>
        [TestMethod]
        public void MaskPanTest()
        {
            const string Pan = "1234567890123456";
            const string Expected = "12345678******56";
            var actual = Utils.MaskPan(Pan);
            Assert.AreEqual(Expected, actual);

            const string ShortPan = "1234567890";
            var actualShort = Utils.MaskPan(ShortPan);
            Assert.AreEqual("******7890", actualShort);
        }

        #endregion
    }
}