using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    /// Summary description for AlphaTests
    /// </summary>
    [TestClass]
    public class AlphaValidatorTests : BaseValidatorTests
    {
        public AlphaValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.Alpha)
        {
            ValidValues.Add("abcdef");
            ValidValues.Add("ABCDEF");

            InvalidValues.Add("adsf234");
            InvalidValues.Add("1324234");
            InvalidValues.Add(".,?#'");
            InvalidValues.Add(" ");
            InvalidValues.Add("adsf fasdf");
        }

        [TestMethod]
        public void TestDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("a", desc);
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestAlphaValidValues()
#pragma warning restore S2699

        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestAlphaInvalidValues()
#pragma warning restore S2699
        {
            TestInvalidValues();
        }

    }
}
