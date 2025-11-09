using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    ///   Summary description for AlphaNumericPrintableValidatorTests
    /// </summary>
    [TestClass]
    public class AlphaNumericPrintableValidatorTests : BaseValidatorTests
    {
        public AlphaNumericPrintableValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.AlphaNumericPrintable)
        {
            ValidValues.Add("ab23cdef");
            ValidValues.Add("ABC23DEF");
            ValidValues.Add("adsf7346,.");
            ValidValues.Add("1324234");
            ValidValues.Add("ab23c def");
            ValidValues.Add(".,?#'");

            InvalidValues.Add("qwe\nrty");
            InvalidValues.Add("qwerty\t");
        }
        [TestMethod]
        public void TestAlphaNumericPrintableDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("anp", desc);
        }


        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestAnpValidValues()
#pragma warning restore S2699 
        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestAnpInvalidValues()
#pragma warning restore S2699
        {
            TestInvalidValues();
        }
    }
}