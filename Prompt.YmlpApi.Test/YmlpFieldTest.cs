using Prompt.Ymlp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Prompt.YmlpApi.Test
{
    
    
    /// <summary>
    ///This is a test class for YmlpFieldTest and is intended
    ///to contain all YmlpFieldTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YmlpFieldTest {


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
        ///A test for Alias
        ///</summary>
        [TestMethod()]
        public void AliasTest() {
            Field target = new Field(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Alias = expected;
            actual = target.Alias;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CorrectUpperCase
        ///</summary>
        [TestMethod()]
        public void CorrectUpperCaseTest() {
            Field target = new Field(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.CorrectUpperCase = expected;
            actual = target.CorrectUpperCase;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FieldName
        ///</summary>
        [TestMethod()]
        public void FieldNameTest() {
            Field target = new Field(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.FieldName = expected;
            actual = target.FieldName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Id
        ///</summary>
        [TestMethod()]
        public void IdTest() {
            Field target = new Field(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for defaultValue
        ///</summary>
        [TestMethod()]
        public void defaultValueTest() {
            Field target = new Field(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.defaultValue = expected;
            actual = target.defaultValue;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
