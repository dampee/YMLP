/*
 * Created by SharpDevelop.
 * User: Damiaan
 * Date: 11/01/2010
 * Time: 16:36
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace Prompt.Ymlp {

    /// <summary>
    /// Send commands to Ymlp
    /// </summary>
    public class YmlpConnector {

        #region Fields
        protected List<Group> ResponseGroupList;
        protected List<Contact> ResponseContactList;
        protected List<Field> ResponseFieldList;
        protected List<From> ResponseFromsList;
        protected List<Filter> ResponseFiltersList;
        private bool m_Secure = false;
        private string m_ApiUsername;
        private string m_ApiKey;
        #endregion

        #region Public Properties
        public string ApiUrl = "www.ymlp.com";
        public string ApiUrlPath = "api";
        public string ApiUsername {
            get { return m_ApiUsername; }
        }
        public string ApiKey {
            get { return m_ApiKey; }
        }
        public bool Secure {
            get { return m_Secure; }
        }
        #endregion

        #region Error handling
        protected List<Exception> m_exceptions = null;

        public List<Exception> Exceptions {
            get { return m_exceptions; }
        }
        public void ClearExceptions() {
            m_exceptions.Clear();
        }

        /// <summary>
        /// An event that clients can use to be notified whenever an error occures      
        /// </summary>
        public event EventHandler<EventArgs<Exception>> ErrorOccured;

        /// <summary>
        /// Invoke the ErrorOccured event; called whenever there is an error
        /// </summary>
        /// <param name="ErrorMessage">the error message</param>
        protected virtual void OnErrorOccured(string ErrorMessage) {
            OnErrorOccured(new Exception(ErrorMessage));
        }

        /// <summary>
        /// Invoke the ErrorOccured event; called whenever there is an error
        /// </summary>
        /// <param name="Error">The Error message wrapped in a Exception object</param>
        protected virtual void OnErrorOccured(Exception Error) {
            m_exceptions.Add(Error);

            if (ErrorOccured != null)
                ErrorOccured.Invoke(this, new EventArgs<Exception>(Error));
        }

        /// <summary>
        /// If the response is a code >= 100 then the API failed.  Adds an exception to m_exceptions why it has failed.
        /// </summary>  
        private void LookForGeneralErrors(YmlpResponse response) {
            switch (response.Code) {
                case 100:
                    m_exceptions.Add(new Exception("API Call Failed: Invalid or missing API Key"));
                    break;
                case 101:
                    m_exceptions.Add(new Exception("API Call Failed: API access is disabled"));
                    break;
                case 102:
                    m_exceptions.Add(new Exception("API access not allowed from this IP address"));
                    break;
            }
        }


        #endregion

        #region Constructor
        public YmlpConnector(string ApiKey, string ApiUsername, bool secure) {
            this.m_ApiKey = ApiKey;
            this.m_ApiUsername = ApiUsername;
            this.m_Secure = secure;
        }
        #endregion

        #region DoCall & ParseResult
        /// <summary>
        /// Does the call to the API.
        /// </summary>
        /// <param name="method">The method to be called.</param>
        /// <param name="commandParams">The command params.</param>
        /// <returns></returns>
        protected virtual YmlpResponse doCall(string method, Dictionary<string, object> commandParams) {

            // Add default data to command parameters
            if (commandParams == null) {
                commandParams = new Dictionary<string, object>();
            }
            commandParams["Key"] = this.m_ApiKey;
            commandParams["Username"] = this.m_ApiUsername;
            commandParams["output"] = "XML";

            ASCIIEncoding encoding = new ASCIIEncoding();
            string postData = string.Empty;
            foreach (var item in commandParams) {
            	string itemvalue =  item.Value == null ? string.Empty : item.Value.ToString();
                postData += string.Format("&{0}={1}",
                                         HttpUtility.UrlEncode(item.Key),
                                         HttpUtility.UrlEncode(itemvalue));
            }
            if (postData.Length > 0) {
                postData = postData.Substring(1); // remove first &-sign
            }

            // Get postData 
            byte[] data = encoding.GetBytes(postData);

            // Set up request
            string CallUrl = new UriBuilder(m_Secure ? "https" : "http", ApiUrl, m_Secure ? 443 : 80, ApiUrlPath + "/" + method).ToString();
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(CallUrl);
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            myRequest.ProtocolVersion = HttpVersion.Version10;
            myRequest.Method = "POST";

            // Send the data.
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            // send response stream to object
            HttpWebResponse objResponse = (HttpWebResponse)myRequest.GetResponse();
            XElement result = null;
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream())) {
                result = XElement.Parse(sr.ReadToEnd());
                // Close and clean up the StreamReader
                sr.Close();
            }

            return (result == null) ? null : ParseResult(result);
        }

        /// <summary>
        /// Parse the result and fill internal lists.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        private YmlpResponse ParseResult(XElement Input) {
            YmlpResponse response = new YmlpResponse();
            m_exceptions = new List<Exception>();
            switch (Input.Name.LocalName) {
                case "Result":
                    response.Code = Convert.ToInt32(Input.Element("Code").Value);
                    response.Output = Input.Element("Output").Value;

                    LookForGeneralErrors(response);

                    break;

                case "Groups":
                    ResponseGroupList = new List<Group>();
                    foreach (XElement item in Input.Elements("Group")) {
                        ResponseGroupList.Add(new Group() {
                            Id = Convert.ToInt32(item.Element("ID").Value),
                            GroupName = item.Element("GroupName").Value,
                            NumberOfContacts = Convert.ToInt32(item.Element("NumberOfContacts").Value)
                        });
                    }
                    break;
                case "Contact":  // Single contact returned 
                    ResponseContactList = new List<Contact>();
                    Contact singleContact = new Contact();
                    singleContact.EmailAddress = Input.Element("EMAIL").Value;
                    singleContact.State = Convert.ToBoolean(Input.Element("STATE").Value);
                    singleContact.CreateDate = DateTime.Parse(Input.Element("DATE").Value);

                    foreach (XElement item in Input.Elements()) {
                        string fieldname = item.Name.LocalName;
                        if (fieldname == "EMAIL") {
                            // skip 
                        }
                        else if (fieldname.Substring(1, 5) == "FIELD") {
                            singleContact.Fields.Add(fieldname, item.Value);
                        }
                        else if (fieldname.Substring(1, 5) == "GROUP") {
                            int groupId = Convert.ToInt32(fieldname.Remove(0, 5));
                            singleContact.Groups.Add(groupId);
                        }

                    }
                    ResponseContactList.Add(singleContact);

                    break;
                case "Contacts": // list of contacts returned
                    ResponseContactList = new List<Contact>();
                    foreach (XElement item in Input.Elements("Contact")) {
                        if (item.Element("Email") == null) {
                            Contact newcontact = new Contact() {
                                EmailAddress = item.Element("EMAIL").Value
                            };
                            int NumberOfFieldsFound = item.Elements().Count();
                            if (NumberOfFieldsFound > 0) {
                                newcontact.Fields = new Dictionary<string, string>();
                                for (int i = 1; i < NumberOfFieldsFound; i++) {
                                    string key = "FIELD" + i.ToString();
                                    newcontact.Fields.Add(key, item.Element(key).Value);
                                }
                            }
                            ResponseContactList.Add(newcontact);
                        }
                    }
                    break;

                case "Fields":
                    ResponseFieldList = new List<Field>();
                    foreach (XElement item in Input.Elements("Field")) {
                        ResponseFieldList.Add(new Field() {
                            Id = Convert.ToInt32(item.Element("ID").Value),
                            Alias = item.Element("Alias").Value,
                            FieldName = item.Element("FieldName").Value,
                            defaultValue = item.Element("DefaultValue").Value,
                            CorrectUpperCase = item.Element("CorrectUppercase").Value
                        });
                    }
                    break;
                case "Froms":
                    ResponseFromsList = new List<From>();
                    foreach (XElement item in Input.Elements("From")) {
                        ResponseFromsList.Add(new From() {
                            Id = item.Element("FromID").Value,
                            Email = item.Element("FromEmail").Value,
                            Name = item.Element("FromName").Value
                        });
                    }
                    break;

                case "Filters":
                    ResponseFiltersList = new List<Filter>();
                    foreach (XElement item in Input.Elements("Filter")) {
                        ResponseFiltersList.Add(new Filter() {
                            ID = Convert.ToInt32( item.Element("ID").Value),
                            FilterName = item.Element("FilterName").Value,
                            Field = item.Element("Field").Value,
                            Criterion = item.Element("Criterion").Value,
                            Operand  = item.Element("Operand").Value,
                            Value = item.Element("Value").Value
                        });
                    }
                  
                    break;
                default:
                    throw new NotImplementedException("Result of API call unknown: " + Input.Name.LocalName);
                    break;
            }

            return response;
        }
        #endregion

        #region Public Ping
        public virtual YmlpResponse Ping() {
            // return $this->doCall("Ping");
            return doCall("Ping", null);
        }
        #endregion

        #region Group methods
        /// <summary>
        /// Groups.GetList() lists the groups in your account, along with their group IDs and the number of contacts in each group.
        /// </summary>
        /// <param name="Groups">Groups returned and parsed from API call</param>
        /// <returns>true if succesful</returns>
        public virtual bool GroupsGetList(out List<Group> Groups) {
            YmlpResponse call = doCall("Groups.GetList", null);

            Groups = ResponseGroupList;

            LookForGeneralErrors(call);
            //no specific errors to search for

            return call.Code == 0;

        }

        /// <summary>
        /// Add a group to YMLP
        /// </summary>
        /// <param name="GroupName">the name to be added</param>
        /// <returns>true if succesfully added</returns>
        public virtual bool GroupsAdd(string GroupName) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["GroupName"] = GroupName;
            YmlpResponse call = this.doCall("Groups.Add", params_Renamed);

            // Search for errors
            LookForGeneralErrors(call);
            if (call.Code == 1) {
                m_exceptions.Add(new Exception("GroupName is Missing"));
            }
            return call.Code == 0;
        }

        public virtual bool GroupsDelete(int GroupId) {
            List<int> groupList = new List<int>() { GroupId };
            return GroupsDelete(groupList);
        }

        /// <summary>
        /// Delete group 
        /// </summary>
        /// <param name="GroupId">List of id's to remove</param>
        /// <returns></returns>
        public virtual bool GroupsDelete(List<int> GroupId) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["GroupId"] = string.Join(",", GroupId.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            YmlpResponse call = this.doCall("Groups.Delete", params_Renamed);

            // Look for specific errors
            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing GroupID");
                    break;
                case 2:
                    OnErrorOccured("No further groups can be deleted if there's just one group");
                    break;
            }

            return call.Code == 0;
        }

        /// <summary>
        /// Groups.Update() is used to update the properties of a group.
        /// </summary>
        /// <param name="GroupId">The ID of the group to update</param>
        /// <param name="GroupName">The new name</param>
        /// <returns></returns>
        public virtual bool GroupsUpdate(int GroupId, string GroupName) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["GroupId"] = GroupId;
            params_Renamed["GroupName"] = GroupName;
            YmlpResponse call = doCall("Groups.Update", params_Renamed);

            // Search for errors
            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing GroupID");
                    break;
            }
            return call.Code == 0;
        }

        public virtual bool GroupsEmpty(int GroupId) {
            List<int> groupList = new List<int>() { GroupId };
            return GroupsEmpty(groupList);
        }

        /// <summary>
        /// Groups.Empty() is used to remove all contacts in a group.
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns>true if successful</returns>
        public virtual bool GroupsEmpty(List<int> GroupId) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["GroupId"] = string.Join(",", GroupId.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            YmlpResponse call = this.doCall("Groups.Empty", params_Renamed);

            // Search for errors
            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing GroupID");
                    break;
            }
            return call.Code == 0;
        }
        #endregion Group Methods

        #region Field Methods
        public virtual bool FieldsGetList(out List<Field> FieldList) {
            YmlpResponse call = doCall("Fields.GetList", null);

            // look for general error codes
            //no specific error exist for this call, so only perform general 
            LookForGeneralErrors(call);

            FieldList = ResponseFieldList;

            return call.Code == 0;
        }

        /// <summary>
        /// Fieldses the add.
        /// </summary>
        /// <param name="NewId">The new id.</param>
        /// <param name="FieldName">Name of the field.</param>
        /// <param name="Alias">The alias.</param>
        /// <param name="DefaultValue">The default value.</param>
        /// <param name="CorrectUppercase">If set to <c>true</c>, the content of the field will be Pascal cased..</param>
        /// <returns></returns>
        public virtual bool FieldsAdd(out int NewId, string FieldName, string Alias, string DefaultValue, bool CorrectUppercase) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FieldName"] = FieldName;
            params_Renamed["Alias"] = Alias;
            params_Renamed["DefaultValue"] = DefaultValue;
            params_Renamed["CorrectUppercase"] = CorrectUppercase ? 1 : 0;
            YmlpResponse call = this.doCall("Fields.Add", params_Renamed);

            LookForGeneralErrors(call);
            NewId = 0;
            switch (call.Code) {
                case 0:
                    // Instantiate the regular expression object.
                    string pat = @"ID: (newid)";
                    Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                    Match m = r.Match(call.Output);
                    if (m.Success) {
                        NewId = Convert.ToInt32(m.Value);
                    }
                    break;
                case 1:
                    OnErrorOccured("FieldName is Missing");
                    break;
                case 2:
                    OnErrorOccured("Disallowed characters used for the field alias. Allowed characters: letters, figures and underscore.");
                    break;
                case 3:
                    OnErrorOccured("Alias already in use for another field");
                    break;
            }

            return call.Code == 0;
        }

        /// <summary>
        /// removes a field based on a given field ID.
        /// </summary>
        /// <param name="FieldID">The ID of the field to be removed</param>
        /// <returns>true if succesfull</returns>
        /// <remarks>You can use the FieldsGetList method to get the Id's</remarks>
        public virtual bool FieldsDelete(List<int> FieldID) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FieldId"] = string.Join(",", FieldID.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            YmlpResponse call = this.doCall("Fields.Delete", params_Renamed);

            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing FieldID");
                    break;
                case 2:
                    OnErrorOccured("The field storing email addresses cannot be deleted");
                    break;
            }

            return call.Code == 0;
        }

        public virtual bool FieldsUpdate(int FieldId, string FieldName, string Alias, string DefaultValue, bool CorrectUppercase) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FieldId"] = FieldId.ToString();
            params_Renamed["FieldName"] = FieldName;
            params_Renamed["Alias"] = Alias;
            params_Renamed["DefaultValue"] = DefaultValue;
            params_Renamed["CorrectUppercase"] = (CorrectUppercase == true) ? "1" : "0";
            YmlpResponse call = this.doCall("Fields.Update", params_Renamed);

            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing FieldID");
                    break;
                case 2:
                    OnErrorOccured("Disallowed characters used for the field alias. Allowed characters: letters, figures and underscore.");
                    break;
                case 3:
                    OnErrorOccured("Alias already in use for another field");
                    break;

            }

            return call.Code == 0;
        }
        #endregion
        
        #region Public Get Contact Lists
        /// <summary>
        /// Contacts.GetContact() retrieves all available information regarding a contact.
        /// </summary>
        /// <param name="Email">the e-mail address to look up</param>
        /// <returns>
        /// A list of all available information regarding this contact: 
        /// fields (FIELDX, where X is the ID of a field; use Fields.GetList() to retrieve the ID for each field), 
        /// groups (GROUPX where X is the ID of a group, use Groups.GetList() to retrieve the ID for each group; value "1": member of this group, value "0": not a member of this group) 
        /// and built-in fields (date added etc).
        /// </returns>
        public virtual bool ContactsGetContact(string Email, out Contact Contact) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["Email"] = Email;
            YmlpResponse call = this.doCall("Contacts.GetContact", params_Renamed);

            Debug.Assert(ResponseContactList.Count == 1);

            // Search for errors
            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid E-mail Address");
                    break;
                case 2:
                    OnErrorOccured("E-mail not found");
                    break;
            }

            Contact = ResponseContactList.FirstOrDefault();

            return call.Code == 0;
        }

        public virtual bool ContactsGetList(out List<Contact> ContactList, int? GroupId, List<int> FieldIDs) {
            // If groupId has value, create list otherwise pass NULL to function
            List<int> groupList = (GroupId.HasValue) ? new List<int>() { GroupId.Value } : null;

            // Do the call
            return ContactsGetList(out ContactList, groupList, FieldIDs, null, null, null, null);
        }

        /// <summary>
        /// Get the contactlist.
        /// </summary>
        /// <param name="?">The result (if there is one...)</param>
        /// <param name="GroupID">ID of the group or a comma-separated list of groups IDs; use Groups.GetList() to retrieve the ID for each group. Null = ALL Contacts</param>
        /// <param name="FieldID">ID of the field or a comma-separated list of field IDs; 
        /// use Field.GetList() to retrieve the ID for each field 
        /// (optional; default: return only email addresses)</param>
        /// <param name="Page">ID of the result page to show (optional, default: 1).</param>
        /// <param name="NumberPerPage">number of contacts per result page (optional, default: 1,000)</param>
        /// <param name="StartDate">only show contacts that were added after this date (optional)</param>
        /// <param name="StopDate">only show contacts that were added before this date</param>
        /// <returns>False if errors occured</returns>
        public virtual bool ContactsGetList(out List<Contact> ContactList, List<int> GroupID, List<int> FieldID, int? Page, int? NumberPerPage, DateTime? StartDate, DateTime? StopDate) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["GroupID"] = string.Join(",", GroupID.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            if (FieldID != null) params_Renamed["FieldID"] = string.Join(",", FieldID.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            if (Page.HasValue) params_Renamed["Page"] = Page.Value;
            if (NumberPerPage.HasValue) params_Renamed["NumberPerPage"] = NumberPerPage.Value;
            if (StartDate.HasValue) params_Renamed["StartDate"] = StartDate.Value.ToString("yyyy-MM-dd");
            if (StopDate.HasValue) params_Renamed["StopDate"] = StopDate.Value.ToString("yyyy-MM-dd");

            // Do the API call
            YmlpResponse call = this.doCall("Contacts.GetList", params_Renamed);

            ContactList = ResponseContactList;

            LookForGeneralErrors(call);

            // Look up method specific errorcodes
            switch (call.Code) {
                case 1:
                    m_exceptions.Add(new Exception("Invalid start date."));
                    break;
                case 2:
                    m_exceptions.Add(new Exception("Invalid stop date."));
                    break;
            }

            return call.Code == 0;
        }

        public virtual bool ContactsGetUnsubscribed(out List<Contact> ContactList, DateTime? StartDate, DateTime? StopDate) {
            return ContactsGetUnsubscribed(out ContactList, null, null, null, StartDate, StopDate);
        }

        /// <summary>
        /// returns the list of unsubscribed contacts in your account.
        /// </summary>
        /// <param name="ContactList">The contact list parsed into <see cref="Contact">Contact</see> objects.</param>
        /// <param name="FieldID">A list of fields to get</param>
        /// <param name="Page">a page number, Null for everything</param>
        /// <param name="NumberPerPage">Number of items per page</param>
        /// <param name="StartDate">Users subscribed after this date, or null for everything</param>
        /// <param name="StopDate">Users subscribed before this date, or null for everything</param>
        /// <returns>False if errors occured, else True</returns>
        public virtual bool ContactsGetUnsubscribed(out List<Contact> ContactList, List<int> FieldID, int? Page, int? NumberPerPage, DateTime? StartDate, DateTime? StopDate) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FieldID"] = FieldID;
            if (Page.HasValue) params_Renamed["Page"] = Page;
            if (NumberPerPage.HasValue) params_Renamed["NumberPerPage"] = NumberPerPage;
            if (StartDate.HasValue) params_Renamed["StartDate"] = StartDate.Value.ToString("yyyy-MM-dd");
            if (StopDate.HasValue) params_Renamed["StopDate"] = StopDate.Value.ToString("yyyy-MM-dd");

            YmlpResponse call = this.doCall("Contacts.GetUnsubscribed", params_Renamed);
            ContactList = ResponseContactList;

            // no specific errors, only general errors possible
            LookForGeneralErrors(call);

            return call.Code == 0;
        }

        public virtual bool ContactsGetDeleted(out List<Contact> ContactList) {
            return ContactsGetDeleted(out ContactList, null, null, null, null, null);
        }

        /// <summary>
        /// returns the list of manually removed contacts in your account.
        /// </summary>
         /// <param name="FieldID">A list of fields to get</param>
        /// <param name="Page">a page number, Null for everything</param>
        /// <param name="NumberPerPage">Number of items per page</param>
        /// <param name="StartDate">Users subscribed after this date, or null for everything</param>
        /// <param name="StopDate">Users subscribed before this date, or null for everything</param>
        /// <returns><value>true</value> if succesful</returns>
        /// <remarks>Check Exceptions property when false is returned</remarks>
        public virtual bool ContactsGetDeleted(out List<Contact> ContactList, List<int> FieldID, int? Page, int? NumberPerPage, DateTime? StartDate, DateTime? StopDate) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FieldID"] = string.Join(",", FieldID.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray()); ;
            params_Renamed["Page"] = Page;
            params_Renamed["NumberPerPage"] = NumberPerPage;
            if (StartDate != null) params_Renamed["StartDate"] = StartDate.Value.ToString("yyyy-MM-dd");
            if (StopDate != null) params_Renamed["StopDate"] = StopDate.Value.ToString("yyyy-MM-dd");
            YmlpResponse call = this.doCall("Contacts.GetDeleted", params_Renamed);

            // only general errors, no specific errors
            LookForGeneralErrors(call);

            ContactList = ResponseContactList;

            return call.Code == 0;
        }

        public virtual bool ContactsGetBounced(out List<Contact> ContactList) {
            return ContactsGetBounced(out ContactList, null, null, null, null, null);
        }

        /// <summary>
        /// returns the list of contacts removed by bounce back handling in your account.
        /// </summary>
        /// <param name="ContactList">The contact list parsed into <see cref="Contact">Contact</see> objects.</param>
        /// <param name="FieldID">A list of fields to get</param>
        /// <param name="Page">a page number, Null for everything</param>
        /// <param name="NumberPerPage">Number of items per page</param>
        /// <param name="StartDate">Users subscribed after this date, or null for everything</param>
        /// <param name="StopDate">Users subscribed before this date, or null for everything</param>
        /// <returns>true if succesful</returns>
        public virtual bool ContactsGetBounced(out List<Contact> ContactList, List<int> FieldID, int? Page, int? NumberPerPage, DateTime? StartDate, DateTime? StopDate) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FieldID"] = FieldID;
            params_Renamed["Page"] = Page;
            params_Renamed["NumberPerPage"] = NumberPerPage;
            if (StartDate != null) params_Renamed["StartDate"] = StartDate.Value.ToString("yyyy-MM-dd");
            if (StopDate != null) params_Renamed["StopDate"] = StopDate.Value.ToString("yyyy-MM-dd");
            YmlpResponse call = this.doCall("Contacts.GetBounced", params_Renamed);

            ContactList = ResponseContactList;

            // Only general errors, no specific errors
            LookForGeneralErrors(call);

            return call.Code == 0;
        }
        #endregion

        #region Public Contact changing methods
        /// <summary>
        /// Adds a contact
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="GroupID">ID of the group or a comma-separated list of groups IDs; use Groups.GetList() to retrieve the ID for each group.</param>
        /// <returns><c>True</c> if successful</returns>
        public virtual bool ContactsAdd(string Email, int GroupID) {
            Dictionary<string, string> otherFields = null;
            bool overruleUnsubscribedBounced = false;
            return ContactsAdd(Email, new List<int>() { GroupID }, otherFields, overruleUnsubscribedBounced);
        }

        /// <summary>
        /// Adds a contact
        /// </summary>
        /// <param name="Email">The contact e-mail address</param>
        /// <param name="OtherFields">FieldX: data for any other field can be sent using the following syntax: FieldX=value (e.g.: Field2=John), where X is the ID of the field (use Fields.GetList() to retrieve the ID for each field)</param>
        /// <param name="GroupID">ID of the group or a comma-separated list of groups IDs; use Groups.GetList() to retrieve the ID for each group.</param>
        /// <param name="OverruleUnsubscribedBounced">default is false); if <c>true</c>, the email address will be added even if this person previously unsubscribed or if the email address previously was removed by bounce back handling</param>
        /// <returns><c>True</c> if successful</returns>
        /// <remarks>The e-mail address is the unique identifier and can't be ommitted</remarks>
        public virtual bool ContactsAdd(string Email, List<int> GroupID, Dictionary<string, string> OtherFields, bool OverruleUnsubscribedBounced) {

            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["Email"] = Email;

            if (OtherFields != null) {
                foreach (KeyValuePair<string, string> item in OtherFields) {
                    params_Renamed[item.Key] = item.Value;
                }
            }

            //params_Renamed["GroupID"] = string.Join(",", GroupID.ToArray()); // ==> List of strings
            params_Renamed["GroupID"] = string.Join(",", GroupID.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            params_Renamed["OverruleUnsubscribedBounced"] = OverruleUnsubscribedBounced == true ? "1" : "0";
            YmlpResponse call = doCall("Contacts.Add", params_Renamed);

            LookForGeneralErrors(call);
            // Lookup specific call related errors
            switch (call.Code) {
                case 1:
                    OnErrorOccured(new Exception("Invalid Email Address"));
                    break;
                case 2:
                    OnErrorOccured(new Exception("Invalid or Missing GroupId"));
                    break;
                case 3:
                    OnErrorOccured(new Exception("E-mail address already in selected groups"));
                    break;
                case 4:
                    OnErrorOccured(new Exception("This e-mail address has previously unsubscribed"));
                    break;
            }


            return call.Code == 0;
        }

        /// <summary>
        /// Unsubscribes a given email address.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns><c>True</c> if successful</returns>
        public virtual bool ContactsUnsubscribe(string Email) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["Email"] = Email;
            YmlpResponse call = this.doCall("Contacts.Unsubscribe", params_Renamed);

            // Search for errors
            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid e-mail address");
                    break;
            }

            return call.Code == 0;
        }

        /// <summary>
        /// Removes a given email address from one or more groups.
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="GroupID"></param>
        /// <returns><c>True</c> if successful</returns>
        public virtual bool ContactsDelete(string Email, int GroupID) {
            List<int> groupList = new List<int>() { GroupID };
            return ContactsDelete(Email, groupList); // workitem 5704
        }

        /// <summary>
        /// removes a given email address from one or more groups.
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="GroupID"></param>
        /// <returns><c>True</c> if successful</returns>
        public virtual bool ContactsDelete(string Email, List<int> GroupIDs) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["Email"] = Email;
            //params_Renamed["GroupID"] = GroupID;
            params_Renamed["GroupID"] = string.Join(",", GroupIDs.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());

            YmlpResponse call = this.doCall("Contacts.Delete", params_Renamed);

            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid Email Address");
                    break;
                case 2:
                    OnErrorOccured("Invalid or Missing GroupID");
                    break;
                case 3:
                    OnErrorOccured("Email address not in selected groups");
                    break;
            }
            return call.Code == 0;
        }
        #endregion
  
        #region Filters
        /// <summary>
        /// Get the stored as a list
        /// </summary>
        /// <param name="FilterList"></param>
        /// <returns></returns>
        public virtual bool FiltersGetList(out List<Filter> FilterList) {
            YmlpResponse call = doCall("Filters.GetList", null);

            // look for general error codes
            //no specific error exist for this call, so only perform general 
            LookForGeneralErrors(call);

            FilterList = ResponseFiltersList;

            return call.Code == 0;
        }

        /// <summary>
        /// Adds a filter.
        /// </summary>
        /// <param name="NewId">The new id, generated by the system.</param>
        /// <param name="FilterName">label to use for the new filter.</param>
        /// <param name="Field">FieldX where X is the ID of the field (use Field.GetList() to retrieve the ID for each field), 
        ///          one of the built-in fields (IP,Browser,OperatingSystem,Date) or 
        ///          Contact.</param>
        /// <param name="Operand">Equals, DoesNotEqual, Contains, DoesNotContain, StartsWith, EndsWith, IsEmpty, IsNotEmpty, SmallerThen, GreaterThen, Before, After, IsIn, IsNotIn
        ///                (Before & After can only be used in combination with Date as field; IsIn & IsNotIn can only be used in combination with Contact as field).</param>
        /// <param name="Value">Some characters or a word (ex: 'yahoo' to create a filter that selects Yahoo email addresses), 
        ///                a date (YYYY-MM-DD, e.g.: 2020-05-31) in combination with Date as field & Before/Afer as operand, 
        ///                or a GroupId in combination with Contact as field and IsIn/IsNotIn as operand.</param>
        /// <returns>True on successful call, else false (the error code on the class should be verified)</returns>
        public virtual bool FiltersAdd(out int NewId, string FilterName, string Field, string Operand, string Value) {
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FilterName"] = FilterName;
            params_Renamed["Field"] = Field;
            params_Renamed["Operand"] = Operand;
            params_Renamed["Value"] = Value;
            YmlpResponse call = this.doCall("Filters.Add", params_Renamed);

            LookForGeneralErrors(call);
            NewId = 0;

            switch (call.Code) {
                case 0:
                    // Instantiate the regular expression object.
                    string pat = @"ID: (newid)";
                    Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                    Match m = r.Match(call.Output);
                    if (m.Success) {
                        NewId = Convert.ToInt32(m.Value);
                    }
                    break;
                case 1:
                    OnErrorOccured("Invalid or Missing Field");
                    break;
                case 2:
                    OnErrorOccured("Invalid or Missing Operand");
                    break;
                case 3:
                    OnErrorOccured("This combination of Field and Operand cannot be used");
                    break;
                case 4:
                    OnErrorOccured("Missing Value");
                    break;
                case 5:
                    OnErrorOccured("Invalid date. Use YYYY-MM-DD format, example 2020-05-31");
                    break;
            }

            return call.Code == 0;
        }

        /// <summary>
        /// Removes a Filter based on a given Filter ID.
        /// </summary>
        /// <param name="FilterID">ID of the filter; use Filters.GetList() to retrieve the ID for each filter.</param>
        /// <returns>true if succesful</returns>
        /// <remarks>You can use the <c>FiltersGetList</c> method to get the Id's</remarks>
        public virtual bool FiltersDelete(List<int> FilterID) {

            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FilterId"] = string.Join(",", FilterID.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            YmlpResponse call = this.doCall("Filters.Delete", params_Renamed);

            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing FilterID");
                    break;
            }

            return call.Code == 0;
        }

        #endregion

        #region NewsLetter
        /// <summary>
        /// Get the From addresses for the newsletter
        /// </summary>
        /// <param name="From">New list with from addresses</param>
        /// <returns>List of from addresses that can be used</returns>
        public virtual bool NewsLetterGetFrom(out List<From> From) {
            YmlpResponse call = doCall("Newsletter.GetFroms", null);

            From = ResponseFromsList;

            LookForGeneralErrors(call);
            //no specific errors to search for

            return call.Code == 0;

        }

        /// <summary>
        /// Adds a from address to the newsletter
        /// </summary>
        /// <param name="FromEmail">From e-mail address.</param>
        /// <param name="FromName">Name/description for the new sender address.</param>
        /// <returns></returns>
        public virtual bool NewsLetterAddFrom(out string NewId, string FromEmail, string FromName) {

            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FromEmail"] = FromEmail;
            params_Renamed["FromName"] = FromName;
            YmlpResponse call = this.doCall("Newsletter.AddFrom", params_Renamed);

            LookForGeneralErrors(call);
            NewId = string.Empty;
            switch (call.Code) {
                case 0:
                    // Instantiate the regular expression object.
                    string pat = @"ID: (newid)";
                    Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                    Match m = r.Match(call.Output);
                    if (m.Success) {
                        NewId = m.Value;
                    }
                    break;
                case 1:
                    OnErrorOccured("Invalid Email Address");
                    break;
            }

            return call.Code == 0;

        }

        /// <summary>
        /// Delete the "From" address attached to the newsletter
        /// </summary>
        /// <param name="FromId">ID of the sender address.</param>
        /// <returns>true if successful</returns>
        /// <remarks>use Newsletter.GetFroms() to retrieve the ID for each sender address.</remarks>
        public virtual bool NewsLetterDeleteFrom(string FromId) {
            /* Response
                    Success:    * Returncode 0 : "Removed ID: X" (where X is the ID of the removed sender address)
                    Failure:    * Returncode 1 : "Invalid or Missing FromID"
             */


            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["FromID"] = FromId;
            YmlpResponse call = this.doCall("NewsLetter.DeleteFrom", params_Renamed);

            LookForGeneralErrors(call);
            switch (call.Code) {
                case 1:
                    OnErrorOccured("Invalid or Missing FromID");
                    break;
            }

            return call.Code == 0;

        }

        public virtual bool NewsLetterSend(string Subject, string HTML, string PlainText, List<int> GroupId, List<int> FilterId, bool TestMessage) {
            // bool TrackOpens = false, bool TrackClicks = false, bool testMessage = true,
            // DateTime? DeliveryTime = null, string FromID = "", bool CombineFilters = false) {

            return NewsLetterSend(Subject, HTML, PlainText, GroupId, FilterId, false,false,TestMessage, null, "", false);
        }
        
        /// <summary>
        /// Send the a Newsletter.
        /// </summary>
        /// <param name="Subject">The subject.</param>
        /// <param name="HTML">HTML code for the message 
        /// (optional if a text part is specified).</param>
        /// <param name="PlainText">(Plain) text for the message 
        /// (optional if a HTML part is specified).</param>
        /// <param name="GroupId">ID of the group or a comma-separated list of groups IDs to send to; 
        /// use Groups.GetList() to retrieve the ID for each group (ignored for test messages).</param>
        /// <param name="FilterId">ID of the filter or a comma-separated list of filter IDs to apply; 
        /// use Filters.GetList() to retrieve the ID for each filter (ignored for test messages).</param>
        /// <param name="TrackOpens">if set to <c>true</c> track opens for the message 
        /// (only available if a HTML part was specified): false (default) .</param>
        /// <param name="TrackClicks">if set to <c>true</c> track clicks for the message 
        /// (only available if a HTML part was specified): false (default) .</param>
        /// <param name="testMessage">if set to <c>true</c> it's a test message.</param>
        /// <param name="DeliveryTime">Delivery time for the message 
        /// (optional, format: YYYY-MM-DD HH:MM am/pm, e.g.: 2020-05-31 04:20 pm).</param>
        /// <param name="FromID">ID of the sender address to use; use Newsletter.GetFroms() to retrieve the ID for each sender address 
        /// (optional; default: first sender address listed on the "Sending Newsletters" page).</param>
        /// <param name="CombineFilters">if set to <c>true</c> contacts must match all applied filters or just one (false =all filters, default) / (true=just one filter).</param>
        /// <returns><c>true</c> if succesfull call was made to YMLP</returns>
        public virtual bool NewsLetterSend(string Subject, string HTML, string PlainText, List<int> GroupId, List<int> FilterId, 
            bool TrackOpens, bool TrackClicks, bool testMessage,
            DateTime? DeliveryTime, string FromID, bool CombineFilters) {
            
            Dictionary<string, object> params_Renamed = new Dictionary<string, object>();
            params_Renamed["Subject"] = Subject;
            params_Renamed["Html"] = HTML;
            params_Renamed["Text"] = PlainText;
            params_Renamed["GroupID"] = GroupId;
            params_Renamed["FilterID"] = FilterId;
            params_Renamed["TrackOpens"] = TrackOpens == true ? "1" : "0";
            params_Renamed["TrackClicks"] = TrackClicks == true ? "1" : "0";
            params_Renamed["testMessage"] = testMessage == true ? "1" : "0";
            if(DeliveryTime.HasValue) params_Renamed["DeliveryTime"] = DeliveryTime.Value.ToString("yyyy-MM-dd hh:mm tt");
            if (!string.IsNullOrEmpty(FromID)) { params_Renamed["FromID"] = FromID; }
            params_Renamed["CombineFilters"] = CombineFilters == true ? "1" : "0";
            YmlpResponse call = this.doCall("Newsletter.Send", params_Renamed);

            LookForGeneralErrors(call);

            switch (call.Code) {
                case 1:
                    OnErrorOccured("Missing subject");
                    break;
                case 2:
                    OnErrorOccured("Missing message");
                    break;
                case 3:
                    OnErrorOccured("Invalid or Missing GroupID");
                    break;
                case 4:
                    OnErrorOccured("The delivery time has past");
                    break;
                case 5:
                    OnErrorOccured("There aren't sufficient credits available in your account to queue this message for delivery");
                    break;
                case 6:
                    OnErrorOccured("There are no contacts in the selected groups.");
                    break;
            }

            return call.Code == 0;

        }
        #endregion
    }
}
