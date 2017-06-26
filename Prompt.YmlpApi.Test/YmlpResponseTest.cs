using Prompt.Ymlp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Prompt.YmlpApi.Test
{
    
    
    /// <summary>
    ///This is a test class for YmlpResponseTest and is intended
    ///to contain all YmlpResponseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YmlpResponseTest {


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
        ///A test for YmlpResponse Constructor
        ///</summary>
        [TestMethod()]
        public void YmlpResponseConstructorTest() {
            YmlpResponse target = new YmlpResponse();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Code
        ///</summary>
        [TestMethod()]
        public void CodeTest() {
            YmlpResponse target = new YmlpResponse(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Code = expected;
            actual = target.Code;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Output
        ///</summary>
        [TestMethod()]
        public void OutputTest() {
            YmlpResponse target = new YmlpResponse(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Output = expected;
            actual = target.Output;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
