// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingCodeTest.cs" company="John Oxley">
//   2012
// </copyright>
// <summary>
//   This is a test class for ProcessingCodeTest and is intended to contain all ProcessingCodeTest Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenIso8583Net.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// This is a test class for ProcessingCodeTest and is intended to contain all ProcessingCodeTest Unit Tests
    /// </summary>
    [TestClass]
    public class ProcessingCodeTest
    {

        #region Public Methods and Operators

        /// <summary>
        /// The test data too long.
        /// </summary>
        [TestMethod]
        public void TestDataTooLong()
        {
            const string Data = "1234567";
            Assert.ThrowsExactly<ArgumentException>(() => new ProcessingCode(Data));
        }

        /// <summary>
        /// The test data too short.
        /// </summary>
        [TestMethod]
        public void TestDataTooShort()
        {
            const string Data = "12345";
            Assert.ThrowsExactly<ArgumentException>(() => new ProcessingCode(Data));
        }

        /// <summary>
        /// The test valid constructor.
        /// </summary>
        [TestMethod]
        public void TestValidConstructor()
        {
            const string Data = "112233";
            var proc = new ProcessingCode(Data);
            Assert.AreEqual("11", proc.TranType);
            Assert.AreEqual("22", proc.FromAccountType);
            Assert.AreEqual("33", proc.ToAccountType);
        }

        #endregion
    }
}