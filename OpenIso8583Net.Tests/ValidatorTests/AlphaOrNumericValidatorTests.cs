using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    ///   Summary description for AlphaOrNumericTests
    /// </summary>
    [TestClass]
    public class AlphaOrNumericValidatorTests : BaseValidatorTests
    {
        public AlphaOrNumericValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.AlphaOrNumeric)
        {
            ValidValues.Add("1234567890");
            ValidValues.Add("ABCdef");

            InvalidValues.Add("1234a");
            InvalidValues.Add("1324.234");
            InvalidValues.Add("abcdef1");
            InvalidValues.Add("ZYX ");
            InvalidValues.Add(".,?#'");
        }

        [TestMethod]
        public void TestDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("a or n", desc);
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestAlphaOrNumericValidValues()
#pragma warning restore S2699 

        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestAlphaOrNumericInvalidValues()
#pragma warning restore S2699
        {
            TestInvalidValues();
        }
    }
}