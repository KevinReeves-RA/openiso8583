using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    /// Summary description for HexValidatorTests
    /// </summary>
    [TestClass]
    public class HexValidatorTests : BaseValidatorTests
    {
        public HexValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.Hex)
        {
            ValidValues.Add("0123456789");
            ValidValues.Add("ABCDEF");
            ValidValues.Add("abcdef");
            ValidValues.Add("123468dfc");

            InvalidValues.Add(" ");
            InvalidValues.Add("123abcdefg");
            InvalidValues.Add("./'[]");
            InvalidValues.Add("\t");
            InvalidValues.Add("\n");
        }

        [TestMethod]
        public void TestDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("hex", desc);
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestHexValidValues()
#pragma warning restore S2699 

        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestHexInvalidValues()
#pragma warning restore S2699 
        {
            TestInvalidValues();
        }
    }
}
