using Prompt.Ymlp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Prompt.YmlpApi.Test
{
    
    
    /// <summary>
    ///This is a test class for YmlpContactTest and is intended
    ///to contain all YmlpContactTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YmlpContactTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for EmailAddress
        ///</summary>
        [TestMethod()]
        public void EmailAddressTest() {
            Contact target = new Contact(); 
            string expected = string.Empty; 
            string actual;
            target.EmailAddress = expected;
            actual = target.EmailAddress;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Fields
        ///</summary>
        [TestMethod()]
        public void FieldsTest() {
            Contact target = new Contact(); 
            Dictionary<string, string> expected = new Dictionary<string,string>();
            expected.Add("FIELD1", "value1");
            expected.Add("FIELD2", "value2");

            Dictionary<string, string> actual;
            target.Fields = expected;
            actual = target.Fields;
            Assert.AreEqual(expected, actual);         
        }
    }
}
