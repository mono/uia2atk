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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

using UiaAtkBridge;

namespace Mono.UIAutomation.Winforms
{
	public class FormListener
	{
#region Private Static Members
		
		static bool initialized = false;
		static Dictionary<Form, WindowProvider> formProviders =
			new Dictionary<Form, WindowProvider> ();
		static Monitor appMonitor = null;
		
#endregion
		
#region Public Static Methods
		
		/// <summary>
		/// Set up the FormListener class to listen to winforms
		/// events that will allow the correct UIA providers to be
		/// created and the correct UIA events to be fired.
		/// 
		/// This method is called via reflection from the
		/// System.Windows.Forms.Application class.
		/// </summary>
		public static void Initialize ()
		{
			if (initialized)
				return;
			
			Console.WriteLine ("FormListener Initialized");
			
			Type appType = typeof (Application);
			// NOTE: FormAdded is fired too frequently (such as
			//       when the form comes into focus).  A different
			//       event is probably more appropriate.
			EventInfo formAddedEvent =
				appType.GetEvent("FormAdded",
				                 BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo formAddedEventAddMethod =
				formAddedEvent.GetAddMethod(true);
			formAddedEventAddMethod.Invoke(null,
			                               new object[]{new EventHandler(OnFormAdded)});
			
			// OnRun
			EventInfo preRunEvent =
				appType.GetEvent("PreRun",
				                 BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo preRunEventAddMethod =
				preRunEvent.GetAddMethod(true);
			preRunEventAddMethod.Invoke(null,
			                               new object[]{new EventHandler(OnPreRun)});
		}
		
#endregion
		
#region Static Event Handlers
		
		static bool applicationStarted = false;
		
		/// <summary>
		/// Start GLib mainloop in its own thread just before
		/// winforms mainloop starts
		/// </summary>
		static void OnPreRun (object sender, EventArgs args)
		{
			Console.WriteLine ("PreRun fired");
			if (!applicationStarted && appMonitor != null)
				appMonitor.ApplicationStarts ();
		}
		
		static void OnFormAdded (object sender, EventArgs args)
		{
			Console.WriteLine ("Form added!");
			Form f = (Form) sender;
			if (formProviders.ContainsKey (f))
				return;
			formProviders [f] = new WindowProvider (f);
			
			bool newMonitor = false;
			if (appMonitor == null) {
				Console.WriteLine ("about to create monitor");
				appMonitor = new Monitor();
				Console.WriteLine ("just made monitor");
				newMonitor = true;
			}
			
			// Events aren't handled in the bridge yet, so don't
			// notify the appMonitor once the bridge has been launched.
			// Obviously this will change soon.
			if (!applicationStarted) {
				appMonitor.FormIsAdded (f.Text);
			}
		}
		
#endregion
	}
}
