using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace OpenIso8583Net.Tests
{
    [TestClass]
    public class PostilionKeyValueTests
    {
        [TestMethod]
        public void PostilionKeyValue_Unpack()
        {
            var x = PostilionKeyValue.ParseData(@"218Postilion:MetaData247218PaymentsAPI.userId111217AdditionalEmvTags111218PaymentsAPI.userId23283caf2caf183dc86b658f0f1c6599435217AdditionalEmvTags41446<?xml version=""1.0"" encoding=""UTF-8""?><AdditionalEmvTags><EmvTag><TagId>9F34</TagId><TagValue>3F0000</TagValue></EmvTag><EmvTag><TagId>9F33</TagId><TagValue>E068C8</TagValue></EmvTag><EmvTag><TagId>9F03</TagId><TagValue>000000000000</TagValue></EmvTag><EmvTag><TagId>9F02</TagId><TagValue>000000001236</TagValue></EmvTag><EmvTag><TagId>82</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9F27</TagId><TagValue>80</TagValue></EmvTag><EmvTag><TagId>4F</TagId><TagValue>A0000000031010</TagValue></EmvTag><EmvTag><TagId>9F26</TagId><TagValue>D6C1154559DBD8E9</TagValue></EmvTag><EmvTag><TagId>5F2A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>9F1C</TagId><TagValue>3132333435363738</TagValue></EmvTag><EmvTag><TagId>9F1A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>5F24</TagId><TagValue>231031</TagValue></EmvTag><EmvTag><TagId>9C</TagId><TagValue>00</TagValue></EmvTag><EmvTag><TagId>9B</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9A</TagId><TagValue>220208</TagValue></EmvTag><EmvTag><TagId>9F12</TagId><TagValue>56697361204465626974</TagValue></EmvTag><EmvTag><TagId>9F10</TagId><TagValue>06011203A00000</TagValue></EmvTag><EmvTag><TagId>95</TagId><TagValue>0000000000</TagValue></EmvTag><EmvTag><TagId>9F37</TagId><TagValue>3E8285DA</TagValue></EmvTag><EmvTag><TagId>9F36</TagId><TagValue>01DF</TagValue></EmvTag><EmvTag><TagId>9F06</TagId><TagValue>A0000000031010</TagValue></EmvTag></AdditionalEmvTags>");

            Assert.IsTrue(x.ContainsKey("Postilion:MetaData"));
            Assert.AreEqual("218PaymentsAPI.userId111217AdditionalEmvTags111", x["Postilion:MetaData"]);

            Assert.IsTrue(x.ContainsKey("PaymentsAPI.userId"));
            Assert.AreEqual("83caf2caf183dc86b658f0f1c6599435", x["PaymentsAPI.userId"]);

            Assert.IsTrue(x.ContainsKey("AdditionalEmvTags"));
            Assert.AreEqual(@"<?xml version=""1.0"" encoding=""UTF-8""?><AdditionalEmvTags><EmvTag><TagId>9F34</TagId><TagValue>3F0000</TagValue></EmvTag><EmvTag><TagId>9F33</TagId><TagValue>E068C8</TagValue></EmvTag><EmvTag><TagId>9F03</TagId><TagValue>000000000000</TagValue></EmvTag><EmvTag><TagId>9F02</TagId><TagValue>000000001236</TagValue></EmvTag><EmvTag><TagId>82</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9F27</TagId><TagValue>80</TagValue></EmvTag><EmvTag><TagId>4F</TagId><TagValue>A0000000031010</TagValue></EmvTag><EmvTag><TagId>9F26</TagId><TagValue>D6C1154559DBD8E9</TagValue></EmvTag><EmvTag><TagId>5F2A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>9F1C</TagId><TagValue>3132333435363738</TagValue></EmvTag><EmvTag><TagId>9F1A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>5F24</TagId><TagValue>231031</TagValue></EmvTag><EmvTag><TagId>9C</TagId><TagValue>00</TagValue></EmvTag><EmvTag><TagId>9B</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9A</TagId><TagValue>220208</TagValue></EmvTag><EmvTag><TagId>9F12</TagId><TagValue>56697361204465626974</TagValue></EmvTag><EmvTag><TagId>9F10</TagId><TagValue>06011203A00000</TagValue></EmvTag><EmvTag><TagId>95</TagId><TagValue>0000000000</TagValue></EmvTag><EmvTag><TagId>9F37</TagId><TagValue>3E8285DA</TagValue></EmvTag><EmvTag><TagId>9F36</TagId><TagValue>01DF</TagValue></EmvTag><EmvTag><TagId>9F06</TagId><TagValue>A0000000031010</TagValue></EmvTag></AdditionalEmvTags>", x["AdditionalEmvTags"]);
        }

        [TestMethod]
        public void PostilionKeyValue_ToMsg()
        {
            var data = new Dictionary<string, string>()
            {
                {"Postilion:MetaData", "218PaymentsAPI.userId111217AdditionalEmvTags111" },
                {"PaymentsAPI.userId", "83caf2caf183dc86b658f0f1c6599435" },
                {"AdditionalEmvTags", @"<?xml version=""1.0"" encoding=""UTF-8""?><AdditionalEmvTags><EmvTag><TagId>9F34</TagId><TagValue>3F0000</TagValue></EmvTag><EmvTag><TagId>9F33</TagId><TagValue>E068C8</TagValue></EmvTag><EmvTag><TagId>9F03</TagId><TagValue>000000000000</TagValue></EmvTag><EmvTag><TagId>9F02</TagId><TagValue>000000001236</TagValue></EmvTag><EmvTag><TagId>82</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9F27</TagId><TagValue>80</TagValue></EmvTag><EmvTag><TagId>4F</TagId><TagValue>A0000000031010</TagValue></EmvTag><EmvTag><TagId>9F26</TagId><TagValue>D6C1154559DBD8E9</TagValue></EmvTag><EmvTag><TagId>5F2A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>9F1C</TagId><TagValue>3132333435363738</TagValue></EmvTag><EmvTag><TagId>9F1A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>5F24</TagId><TagValue>231031</TagValue></EmvTag><EmvTag><TagId>9C</TagId><TagValue>00</TagValue></EmvTag><EmvTag><TagId>9B</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9A</TagId><TagValue>220208</TagValue></EmvTag><EmvTag><TagId>9F12</TagId><TagValue>56697361204465626974</TagValue></EmvTag><EmvTag><TagId>9F10</TagId><TagValue>06011203A00000</TagValue></EmvTag><EmvTag><TagId>95</TagId><TagValue>0000000000</TagValue></EmvTag><EmvTag><TagId>9F37</TagId><TagValue>3E8285DA</TagValue></EmvTag><EmvTag><TagId>9F36</TagId><TagValue>01DF</TagValue></EmvTag><EmvTag><TagId>9F06</TagId><TagValue>A0000000031010</TagValue></EmvTag></AdditionalEmvTags>" }
            };

            var msg = PostilionKeyValue.ToMsg(data);
            Assert.AreEqual(@"218Postilion:MetaData247218PaymentsAPI.userId111217AdditionalEmvTags111218PaymentsAPI.userId23283caf2caf183dc86b658f0f1c6599435217AdditionalEmvTags41446<?xml version=""1.0"" encoding=""UTF-8""?><AdditionalEmvTags><EmvTag><TagId>9F34</TagId><TagValue>3F0000</TagValue></EmvTag><EmvTag><TagId>9F33</TagId><TagValue>E068C8</TagValue></EmvTag><EmvTag><TagId>9F03</TagId><TagValue>000000000000</TagValue></EmvTag><EmvTag><TagId>9F02</TagId><TagValue>000000001236</TagValue></EmvTag><EmvTag><TagId>82</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9F27</TagId><TagValue>80</TagValue></EmvTag><EmvTag><TagId>4F</TagId><TagValue>A0000000031010</TagValue></EmvTag><EmvTag><TagId>9F26</TagId><TagValue>D6C1154559DBD8E9</TagValue></EmvTag><EmvTag><TagId>5F2A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>9F1C</TagId><TagValue>3132333435363738</TagValue></EmvTag><EmvTag><TagId>9F1A</TagId><TagValue>0156</TagValue></EmvTag><EmvTag><TagId>5F24</TagId><TagValue>231031</TagValue></EmvTag><EmvTag><TagId>9C</TagId><TagValue>00</TagValue></EmvTag><EmvTag><TagId>9B</TagId><TagValue>0000</TagValue></EmvTag><EmvTag><TagId>9A</TagId><TagValue>220208</TagValue></EmvTag><EmvTag><TagId>9F12</TagId><TagValue>56697361204465626974</TagValue></EmvTag><EmvTag><TagId>9F10</TagId><TagValue>06011203A00000</TagValue></EmvTag><EmvTag><TagId>95</TagId><TagValue>0000000000</TagValue></EmvTag><EmvTag><TagId>9F37</TagId><TagValue>3E8285DA</TagValue></EmvTag><EmvTag><TagId>9F36</TagId><TagValue>01DF</TagValue></EmvTag><EmvTag><TagId>9F06</TagId><TagValue>A0000000031010</TagValue></EmvTag></AdditionalEmvTags>", msg);

        }
    }
}
