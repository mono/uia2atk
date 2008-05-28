// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	public sealed class ProviderFactory
	{
		private ProviderFactory ()
		{
		}
		
		public static IRawElementProviderSimple GetProvider (Control control)
		{
			return GetProvider (control, true);
		}
		
		public static IRawElementProviderSimple GetProvider (Control control, 
		                                                     bool initializeEvents)
		{
			Label l;
			Button b;
			RadioButton r;
			CheckBox c;
			TextBox t;
			LinkLabel ll;
			SimpleControlProvider provider = null;
			Form f;

			if ((f = control as Form) != null)
				provider = new WindowProvider (f);
			else if ((b = control as Button) != null)
				provider = new ButtonProvider (b);
			else if ((r = control as RadioButton) != null)
				provider = new RadioButtonProvider (r);
			else if ((c = control as CheckBox) != null)
				provider = new CheckBoxProvider (c);
			else if ((t = control as TextBox) != null)
				provider = new TextBoxProvider (t);
			else if ((ll = control as LinkLabel) != null)
				provider = new LinkLabelProvider (ll);
			else if ((l = control as Label) != null)
				provider = new LabelProvider (l);
			
			if (provider != null) {
				if (initializeEvents)
					provider.InitializeEvents ();
				return provider;
			} else
				return null;
		}
	}
}
