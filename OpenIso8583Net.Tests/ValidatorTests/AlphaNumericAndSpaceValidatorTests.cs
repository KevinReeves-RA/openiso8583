using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenIso8583Net.FieldValidator;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    [TestClass]
    public class AlphaNumericAndSpaceValidatorTests : BaseValidatorTests
    {
        public AlphaNumericAndSpaceValidatorTests() : base(new AlphaNumericAndSpaceFieldValidator())
        {
            ValidValues.Add("ab23cdef");
            ValidValues.Add("ABC23DEF");
            ValidValues.Add("adsf7 346");
            ValidValues.Add("1324234");
            ValidValues.Add("ab23c def");
            InvalidValues.Add(".,?#'");

        }

        [TestMethod]
        public void TestAlphaNumericPrintableDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("ansp", desc);
        }


        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestAlphaNumericPrintableValidValues()
#pragma warning restore S2699
        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case.
        public void TestAlphaNumericPrintableInvalidValues()
#pragma warning restore S2699
        {
            TestInvalidValues();
        }
    }
}
