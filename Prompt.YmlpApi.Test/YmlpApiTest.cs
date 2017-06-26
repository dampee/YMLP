using Prompt.Ymlp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using PHP;

namespace Prompt.YmlpApi.Test {


    /// <summary>
    ///This is a test class for YmlpApiTest and is intended
    ///to contain all YmlpApiTest Unit Tests
    ///</summary>
    [TestClass()]
    public class YmlpApiTest {


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

        private static Prompt.Ymlp.YmlpConnector InitializeApi() {
            string ApiKey = DefaultValues.YmlpKey;
            string ApiUsername = DefaultValues.YmlpUser;
            bool secure = false;
            Prompt.Ymlp.YmlpConnector target = new Prompt.Ymlp.YmlpConnector(ApiKey, ApiUsername, secure);
            return target;
        }

        /// <summary>
        ///A test for YmlpApi Constructor
        ///</summary>
        [TestMethod()]
        public void YmlpApiConstructorTest() {
            string ApiKey = DefaultValues.YmlpKey;
            string ApiUsername = DefaultValues.YmlpUser;
            bool secure = false;
            Prompt.Ymlp.YmlpConnector target = new Prompt.Ymlp.YmlpConnector(ApiKey, ApiUsername, secure);

            Assert.AreEqual(ApiKey, target.ApiKey);
            Assert.AreEqual(ApiUsername, target.ApiUsername);
            Assert.AreEqual(secure, target.Secure);
        }

        /// <summary>
        ///A test for ClearExceptions
        ///</summary>
        [TestMethod()]
        public void ClearExceptionsTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            target.ClearExceptions();

            Assert.Equals(target.Exceptions.Count, 0);
        }

        /// <summary>
        ///A test for ContactsAdd
        ///</summary>
        [TestMethod()]
        public void ContactsAddAndRemoveTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            string Email = DateTime.Now.ToString("yymmdd-hhMMss") + "@test.com";
            Dictionary<string, string> OtherFields = null; // new Dictionary<string, string>() ;
            //int GroupID = 1; 
            List<int> GroupID = new List<int>() { 1 };
            bool OverruleUnsubscribedBounced = false;
            bool expectedAdd = true;
            bool actualAdd;

            // Add contact to group
            actualAdd = target.ContactsAdd(Email, GroupID, OtherFields, OverruleUnsubscribedBounced);

            Assert.AreEqual(expectedAdd, actualAdd);

            // remove Contact from group
            bool actualDelete = true;
            actualDelete = target.ContactsDelete(Email, GroupID);

            Assert.IsTrue(actualDelete);
        }

        /// <summary>
        ///A test for ContactsGetBounced
        ///</summary>
        [TestMethod()]
        public void ContactsGetBouncedTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Contact> contacts;
            //List<int> FieldID = new List<int>();
            //int? Page = 0; 
            //int? NumberPerPage = 0; 
            //Nullable<DateTime> StartDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            //Nullable<DateTime> StopDate = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value

            bool actual;
            actual = target.ContactsGetBounced(out contacts);
            Assert.IsTrue(actual);
        }

        /// <summary>
        ///A test for ContactsGetContact
        ///</summary>
        [TestMethod()]
        public void ContactsGetContactTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            string Email = "dummy@dummy.com";
            Contact expected = new Contact() { EmailAddress = Email }; // TODO: Initialize to an appropriate value
            Contact actual;
            bool retVal = target.ContactsGetContact(Email, out actual);
            Assert.IsTrue(retVal);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for ContactsGetDeleted
        ///</summary>
        [TestMethod()]
        public void ContactsGetDeletedTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Contact> ContactList = null;

            bool expected = true;
            bool actual;
            actual = target.ContactsGetDeleted(out ContactList);

            Assert.AreEqual(expected, actual);
            Assert.IsNotNull(ContactList);

        }

        /// <summary>
        ///A test for ContactsGetList
        ///</summary>
        [TestMethod()]
        public void ContactsGetListTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Contact> ContactList = null;
            int? GroupID = 1;
            List<int> FieldID = null;
            //            Nullable<int> Page = new Nullable<int>(); 
            //            Nullable<int> NumberPerPage = new Nullable<int>(); 
            //            Nullable<DateTime> StartDate = DateTime.Now; 
            //            Nullable<DateTime> StopDate = DateTime.Now; 

            bool expected = true;
            bool actual;
            actual = target.ContactsGetList(out ContactList, GroupID, FieldID);
            Assert.IsNotNull(ContactList);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ContactsGetUnsubscribed
        ///</summary>
        [TestMethod()]
        public void ContactsGetUnsubscribedTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Contact> ContactList = null;
            List<int> FieldID = null;
            int? Page = null;
            int? NumberPerPage = null;
            Nullable<DateTime> StartDate = null;
            Nullable<DateTime> StopDate = null;

            bool expected = true;
            bool actual;
            actual = target.ContactsGetUnsubscribed(out ContactList, FieldID, Page, NumberPerPage, StartDate, StopDate);
            Assert.AreEqual(expected, actual);
            Assert.IsNotNull(ContactList);
            Assert.IsNull(target.Exceptions);
        }

        /// <summary>
        ///A test for ContactsUnsubscribe
        ///</summary>
        [TestMethod()]
        public void ContactsUnsubscribeTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            string Email = "dummy@dummy.com";

            bool expected = true;
            bool actual;
            actual = target.ContactsUnsubscribe(Email);
            Assert.AreEqual(expected, actual);
            Assert.IsNull(target.Exceptions);
        }

        /// <summary>
        ///A test for FieldsAdd
        ///</summary>
        [TestMethod()]
        public void FieldsAddTest() {
            YmlpConnector target = InitializeApi();

            int NewId;
            string FieldName = "TestField";
            string Alias = "Testfield";
            string DefaultValue = "";
            bool CorrectUppercase = false;

            bool expected = true;
            bool actual;
            actual = target.FieldsAdd(out NewId, FieldName, Alias, DefaultValue, CorrectUppercase);
            Assert.AreEqual(expected, actual);
            Assert.IsNull(target.Exceptions);
            Assert.AreNotEqual(NewId, 0);  // TODO: NewId > 0

            actual = target.FieldsDelete(new List<int> { NewId });
            Assert.IsTrue(actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FieldsGetList
        ///</summary>
        [TestMethod()]
        public void FieldsGetListTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Field> FieldList = null; // TODO: Initialize to an appropriate value
            List<Field> FieldListExpected = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.FieldsGetList(out FieldList);
            Assert.AreEqual(FieldListExpected, FieldList);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FieldsUpdate
        ///</summary>
        [TestMethod()]
        public void FieldsUpdateTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            int FieldId = 1;
            string FieldName = "testfield1";
            string Alias = string.Empty;
            string DefaultValue = string.Empty;
            bool CorrectUppercase = false;
            bool expected = true;
            bool actual;
            actual = target.FieldsUpdate(FieldId, FieldName, Alias, DefaultValue, CorrectUppercase);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GroupsAdd
        ///</summary>
        [TestMethod()]
        public void GroupsAddTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            string GroupName = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GroupsAdd(GroupName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GroupsDelete
        ///</summary>
        [TestMethod()]
        public void GroupsDeleteTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            int GroupId = 2; // TODO: Initialize to an appropriate value
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.GroupsDelete(GroupId);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GroupsEmpty
        ///</summary>
        [TestMethod()]
        public void GroupsEmptyTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            int GroupId = 2; 
            bool expected = true; 
            bool actual;
            actual = target.GroupsEmpty(GroupId);
            Assert.AreEqual(expected, actual);
        
            // Todo: check # contacts in group ==> now 0
            //target.ContactsGetList(List<YmlpContact> contactslist, GroupId, 
        }

        /// <summary>
        ///A test for GroupsGetList
        ///</summary>
        [TestMethod()]
        public void GroupsGetListTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Group> Groups;
            //List<YmlpGroup> GroupsExpected = null; // 
            bool expected = true; // should be true (no errors)
            bool actual;
            actual = target.GroupsGetList(out Groups);

            //Assert.AreEqual(GroupsExpected, Groups);  // Can't guess the result form a webservice
            Assert.AreEqual(expected, actual);
            Assert.IsNull(target.Exceptions);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GroupsUpdate
        ///</summary>
        [TestMethod()]
        public void GroupsUpdateTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            int GroupId = 1;
            string GroupName = "TestName";

            bool actual;
            actual = target.GroupsUpdate(GroupId, GroupName);
            Assert.IsTrue(actual);

        }

        /// <summary>
        ///A test for LookForGeneralErrors
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Prompt.YmlpApi.dll")]
        public void LookForGeneralErrorsTest() {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            YmlpConnector_Accessor target = new YmlpConnector_Accessor(param0); // TODO: Initialize to an appropriate value
            YmlpResponse response = null; // TODO: Initialize to an appropriate value
            target.LookForGeneralErrors(response);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ParseResult
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Prompt.YmlpApi.dll")]
        public void ParseResultTest() {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            YmlpConnector_Accessor target = new YmlpConnector_Accessor(param0); // TODO: Initialize to an appropriate value
            XElement Input = null; // TODO: Initialize to an appropriate value
            YmlpResponse expected = null; // TODO: Initialize to an appropriate value
            YmlpResponse actual;
            actual = target.ParseResult(Input);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Ping
        ///</summary>
        [TestMethod()]
        public void PingTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            YmlpResponse actual;
            actual = target.Ping();

            Assert.AreEqual(0, actual.Code);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for doCall
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Prompt.YmlpApi.dll")]
        public void doCallTest() {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            YmlpConnector_Accessor target = new YmlpConnector_Accessor(param0); // TODO: Initialize to an appropriate value
            string method = string.Empty; // TODO: Initialize to an appropriate value
            OrderedMap commandParams = null; // TODO: Initialize to an appropriate value
            YmlpResponse expected = null; // TODO: Initialize to an appropriate value
            YmlpResponse actual;
            actual = target.doCall(method, commandParams);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ApiKey
        ///</summary>
        [TestMethod()]
        public void ApiKeyTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            string actual;
            actual = target.ApiKey;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ApiUsername
        ///</summary>
        [TestMethod()]
        public void ApiUsernameTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            string actual;
            actual = target.ApiUsername;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Exceptions
        ///</summary>
        [TestMethod()]
        public void ExceptionsTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            List<Exception> actual;
            actual = target.Exceptions;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Secure
        ///</summary>
        [TestMethod()]
        public void SecureTest() {
            Prompt.Ymlp.YmlpConnector target = InitializeApi();

            bool actual;
            actual = target.Secure;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
