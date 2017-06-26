/*
 * Created by SharpDevelop.
 * User: Damiaan
 * Date: 11/01/2010
 * Time: 16:36
 */

using System;

namespace Prompt.Ymlp
{
	/// <summary>The default response when no Data is retrieved.  Contains the return code and a message from ymlp.</summary>
	/// <remarks>normal response:
	/// &lt;Result&gt;
	/// &lt;Code&gt;101&lt;/Code&gt;
	/// &lt;Output&gt;
	/// API access is disabled.  To enable it, select API in the menu on the left in your account
	/// &lt;/Output&gt;
	/// &lt;/Result&gt;*/
	/// </remarks>
	public class YmlpResponse {
	    int m_Code;
	    string m_Output;
	
	    public int Code {
	        get { return m_Code; }
	        set { m_Code = value; }
	    }
	    public string Output {
	        get { return m_Output; }
	        set { m_Output = value; }
	    }
	}
}
