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
	public class EventArgs<T> : EventArgs {
	    public EventArgs(T value) {
	        m_value = value;
	    }
	
	    private T m_value;
	
	    public T Value {
	        get { return m_value; }
	    }
	}
}
