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

namespace Prompt.Ymlp
{
	public struct Contact {
	    public string EmailAddress { get; set; }
	    public Dictionary<string, string> Fields { get; set; }
	    public List<int> Groups { get; set; }
	    public bool? State { get; set; }
	    public DateTime? CreateDate { get; set; }
	}
}
