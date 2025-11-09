using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    /// Summary description for NoneValidatorTests
    /// </summary>
    [TestClass]
    public class NoneValidatorTests : BaseValidatorTests
    {
        public NoneValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.None)
        {
            ValidValues.Add("0123456789");
            ValidValues.Add("ABCDEF");
            ValidValues.Add("abcdef");
            ValidValues.Add("123468dfc");
            ValidValues.Add(" ");
            ValidValues.Add("123abcdefg");
            ValidValues.Add("./'[]");
            ValidValues.Add("\t");
            ValidValues.Add("\n");
        }
        [TestMethod]
        public void TestNoneDescription()
        {
            var desc = this.FieldValidator.Description;
            Assert.AreEqual("none", desc);
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestNoneValidValues()
#pragma warning restore S2699 

        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestNoneInvalidValues()
#pragma warning restore S2699 
        {
            TestInvalidValues();
        }
    }
}
