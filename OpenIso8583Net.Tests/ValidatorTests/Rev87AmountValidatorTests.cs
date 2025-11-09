using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenIso8583Net.Tests.ValidatorTests
{
    /// <summary>
    ///   Summary description for Rev87AmountValidatorTests
    /// </summary>
    [TestClass]
    public class Rev87AmountValidatorTests : BaseValidatorTests
    {
        public Rev87AmountValidatorTests()
            : base(OpenIso8583Net.FieldValidator.FieldValidators.Rev87AmountValidator)
        {
            ValidValues.Add("C0002135");
            ValidValues.Add("D0002135");
            ValidValues.Add("C000002135");
            ValidValues.Add("D000002135");

            InvalidValues.Add("ABCDEF");
            InvalidValues.Add("abcdef");
            InvalidValues.Add("123468dfc");
            InvalidValues.Add("123456");
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
            Assert.AreEqual("amt", desc);
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestAmtValidValues()
#pragma warning restore S2699 
        {
            TestValidValues();
        }

        [TestMethod]
#pragma warning disable S2699 // Add at least one assertion to this test case
        public void TestAmtInvalidValues()
#pragma warning restore S2699 
        {
            TestInvalidValues();
        }
    }
}