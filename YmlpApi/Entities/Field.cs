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
	public struct Field {
	    public int Id { get; set; }
	    public string FieldName { get; set; }
	    public string Alias { get; set; }
	    public string defaultValue { get; set; }
	    public string CorrectUpperCase { get; set; }
	}
}
