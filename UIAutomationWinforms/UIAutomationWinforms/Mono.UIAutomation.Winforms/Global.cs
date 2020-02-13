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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andrés G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{
	
	public class Global
	{
		static KeyFilter keyFilter = new KeyFilter ();

		/// <summary>
		/// Set up the all the *Listener classes to winforms
		/// events that will allow the correct UIA providers to be
		/// created and the correct UIA events to be fired.
		/// 
		/// This method is called via reflection from the
		/// System.Windows.Forms.Application class.
		/// </summary>
		public static void Initialize ()
		{
			Application.PreRun += new EventHandler (OnPreRun);
			FormListener.Initialize ();
			ToolTipListener.Initialize ();
			HelpProviderListener.Initialize ();
			ErrorProviderListener.Initialize ();
		}

		/// <summary>
		/// Start GLib mainloop in its own thread just before
		/// winforms mainloop starts, but after gtk_init ()
		/// has been called by MWF
		/// </summary>
		static void OnPreRun (object sender, EventArgs args)
		{
			// FIXME: Change this temporary hack to pass on the PreRun event
			AutomationInteropProvider.RaiseAutomationEvent (null, null);

			Application.AddKeyFilter (keyFilter);
		}
	}
}
