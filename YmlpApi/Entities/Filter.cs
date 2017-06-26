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
	public struct Filter {
	    public int ID { get; set; }
	    public string FilterName { get; set; }
	    public string Criterion { get; set; }
	    public string Field { get; set; }
	    public string Operand { get; set; }
	    public string Value { get; set; }
	}
}
