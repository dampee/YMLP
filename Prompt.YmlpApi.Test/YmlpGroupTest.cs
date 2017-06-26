using Prompt.Ymlp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Prompt.YmlpApi.Test
{
    
    
    /// <summary>
    ///This is a test class for YmlpGroupTest and is intended
    ///to contain all YmlpGroupTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YmlpGroupTest {


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
        ///A test for GroupName
        ///</summary>
        [TestMethod()]
        public void GroupNameTest() {
            Group target = new Group(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.GroupName = expected;
            actual = target.GroupName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Id
        ///</summary>
        [TestMethod()]
        public void IdTest() {
            Group target = new Group(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NumberOfContacts
        ///</summary>
        [TestMethod()]
        public void NumberOfContactsTest() {
            Group target = new Group(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.NumberOfContacts = expected;
            actual = target.NumberOfContacts;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
