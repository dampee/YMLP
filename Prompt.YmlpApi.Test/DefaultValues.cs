using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prompt.YmlpApi.Test {
   internal class DefaultValues {
         
        public const string YmlpKey= "MyAPIKey";
        public const string YmlpUser = "MyUserName";

        private static Prompt.Ymlp.YmlpConnector InitializeApi() {
            string ApiKey = DefaultValues.YmlpKey;
            string ApiUsername = DefaultValues.YmlpUser;
            bool secure = false;
            Prompt.Ymlp.YmlpConnector target = new Prompt.Ymlp.YmlpConnector(ApiKey, ApiUsername, secure);
            return target;
        }

        //    List<YmlpField> fl;
        //    if (proxy.FieldsGetList(out fl) == false) {
        //        Console.WriteLine("Error getting Field list");
        //        // ToDo: Check the exceptions (proxy.Exceptions)
        //    }
        //    else {
        //        foreach (var i in fl) {
        //            Console.WriteLine("id: {0} / field: {1}", i.Id, i.FieldName);
        //        }
        //    }

        //    List<YmlpContact> cl;
        //    var Group = gl.First();
        //    if (proxy.ContactsGetList(out cl, Group.Id, new List<int>() { 0, 1, 2 }) == false) {
        //        Console.WriteLine("Error getting contacts");
        //    }
        //    else {
        //        foreach (var i in cl) {
        //            Console.WriteLine("contact e-mal: {0} / # fields: {1}", i.EmailAddress, i.Fields.Count);
        //        }
        //    }

    }
}
