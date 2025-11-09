using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    ///   Summary description for AlphaNumericSpecialValidatorTests
    /// </summary>
    [TestClass]
    public class AlphaNumericSpecialValidatorTests : BaseValidatorTests
    {
        public AlphaNumericSpecialValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.AlphaNumericSpecial)
        {
            ValidValues.Add("ab23cdef");
            ValidValues.Add("ABC23DEF");
            ValidValues.Add("adsf7346,.");
            ValidValues.Add("1324234");
            ValidValues.Add("ab23c def");
            ValidValues.Add(".,?#'");
        }

        [TestMethod]
        public void TestDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("ans", desc);
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestAnsValidValues()
#pragma warning restore S2699

        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestAnsInvalidValues()
#pragma warning restore S2699
        {
            TestInvalidValues();
        }
    }
}