using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    ///   Summary description for AlphaNumericTests
    /// </summary>
    [TestClass]
    public class AlphaNumericValidatorTests : BaseValidatorTests
    {
        public AlphaNumericValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.AlphaNumeric)
        {
            ValidValues.Add("ab23cdef");
            ValidValues.Add("ABC23DEF");

            InvalidValues.Add("adsf7346,.");
            InvalidValues.Add("1324.234");
            InvalidValues.Add("ab23c def");
            InvalidValues.Add(".,?#'");
        }

        [TestMethod]
        public void TestDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("an", desc);
        }


        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestANValidValues()
#pragma warning restore S2699 

        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestANInvalidValues()
#pragma warning restore S2699 
        {
            TestInvalidValues();
        }
    }
}