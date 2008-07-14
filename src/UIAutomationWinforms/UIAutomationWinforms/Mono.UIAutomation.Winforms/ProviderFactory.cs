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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public sealed class ProviderFactory
	{
		// NOTE: This may not be the best place to track this...however
		//       I forsee this factory class evolving into a builder
		//       class that takes raw providers and attaches provider
		//       behaviors depending on the control type, and maybe
		//       it makes sense for the builder to keep track of
		//       this mapping?
		private static Dictionary<Control, IRawElementProviderFragment>
			controlProviders;
		
		static ProviderFactory ()
		{
			controlProviders =
				new Dictionary<Control,IRawElementProviderFragment> ();
		}
		
		public static IRawElementProviderFragment GetProvider (Control control)
		{
			return GetProvider (control, true);
		}
		
		
		public static IRawElementProviderFragment GetProvider (Control control, 
		                                                     bool initializeEvents)
		{
			Label l;
			Button b;
			RadioButton r;
			CheckBox c;
			TextBox t;
			LinkLabel ll;
			FragmentControlProvider provider = null;
			Form f;
			GroupBox gb;
			StatusBar sb;
			ComboBox cb;
			ListBox lb;
			ScrollBar scb;
			
			if (control == null)
				return null;

			provider = (FragmentControlProvider) FindProvider (control);
			if (provider != null)
				return provider;

			if ((f = control as Form) != null)
				provider = new WindowProvider (f);
			else if ((gb = control as GroupBox) != null)
				provider = new GroupBoxProvider (gb);
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
			else if ((sb = control as StatusBar) != null)
				provider = new StatusBarProvider (sb);
			else if ((cb = control as ComboBox) != null)
				provider = new ComboBoxProvider (cb);
			else if ((lb = control as ListBox) != null)
				provider = new ListBoxProvider (lb);
			else if ((scb = control as ScrollBar) != null) {
				if ((lb = scb.Parent as ListBox) != null)
					provider = new ListBoxProvider.ListBoxScrollBarProvider (scb);
				else
					provider = new ScrollBarProvider (scb);
			} else //TODO: We have to solve the problem when there's a Custom control
				throw new NotImplementedException ("Provider not implemented for control");
			
			if (provider != null) {
				// TODO: Make tracking in dictionary optional
				controlProviders [control] = provider;
				if (initializeEvents)
					provider.InitializeEvents ();
				return provider;
			} else
				return null;
		}
		
		public static void ReleaseProvider (Control control)
		{
			controlProviders.Remove (control);
		}
		
		public static IRawElementProviderFragment FindProvider (Control control)
		{
			IRawElementProviderFragment provider;
			
			if (control == null)
				return null;
			else if (controlProviders.TryGetValue (control, out provider))
				return provider;

			return null;
		}
	}
}
