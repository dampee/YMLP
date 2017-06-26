/*
 * Created by SharpDevelop.
 * User: DAPE
 * Date: 26/07/2010
 * Time: 15:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Prompt.Ymlp;

namespace SampleConsole
{
	class Program
	{ 
		const string YmlpUser = "myuser";
		const string YmlpKey= "mykey";
		
         
		private static void PrintExceptions(List<Exception> ex){
			foreach (var element in ex) {
          	Console.WriteLine(element.ToString());
			}             	
			 Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
       }
         
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
						  
       	bool secure = false;
          YmlpConnector proxy = new Prompt.Ymlp.YmlpConnector(YmlpKey, YmlpUser, secure);
          
          Console.WriteLine("Available fields");
          List<Field> fl;
          if (proxy.FieldsGetList(out fl) == false) {
          	Console.WriteLine("Error getting Field list");
            	PrintExceptions(proxy.Exceptions);
            	return;
          }
          else {
            	foreach (var i in fl) {
            		Console.WriteLine("id: {0} / field: {1}", i.Id, i.FieldName);
            	}
          }

			List<Group> grouplist;
          if(proxy.GroupsGetList(out grouplist) == false) {
          	Console.WriteLine("Error getting groups, exit!");
            	PrintExceptions(proxy.Exceptions);
            	return;
          }
            
          foreach (var group in grouplist) {
            	// Get all information about all contacts in a group
            	Console.WriteLine("GROUP  ---  {0} (id: {1}, # contacts: {2})" , group.GroupName, group.Id, group.NumberOfContacts);
            	
            	// Get all the available fields 
	       	var fieldlist = fl.Select(f=>f.Id).ToList();
	       	List<Contact> cl;
            	if (proxy.ContactsGetList(out cl, group.Id, fieldlist) == false) {
            		Console.WriteLine("Error getting contacts");
            		PrintExceptions(proxy.Exceptions);
            		return;
				}
            	else {
            		// Loop all contacts
            		foreach (var contact in cl) {
            			
            			Console.WriteLine("contact e-mail: {0}", contact.EmailAddress);
            			Console.WriteLine("        state : {0}", contact.State);
            			Console.WriteLine("        create: {0}", contact.CreateDate);
            			if(contact.Fields.Count >0){
            				Console.WriteLine("        Extra fields found");
            			}
						foreach (var field in contact.Fields) {
							// For every contact print all fields from that contact            				
            				Console.WriteLine(" {0,22}: {1}", field.Key, field.Value);
            			}
            		}
            	}
           }
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
		}
	}
}