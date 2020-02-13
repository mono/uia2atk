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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	public class FormListener
	{
		static bool initialized = false;

		/// <summary>
		/// Set up the FormListener class to listen to winforms
		/// events that will allow the correct UIA providers to be
		/// created and the correct UIA events to be fired.
		/// </summary>
		public static void Initialize ()
		{
			if (!AutomationInteropProvider.ClientsAreListening || initialized)
				return;
			
			// We are using this event to tell the bridge that should release all
			// the FormProvider provider that aren't yet removed.
			Application.ApplicationExit += Application_ApplicationExit;

			// FIXME: FormAdded is fired too frequently (such as
			//        when the form comes into focus).  A different
			//        event is probably more appropriate.
			Application.FormAdded += Application_FormAdded;

			initialized = true;
		}
		
		private static void Application_ApplicationExit (object sender, EventArgs args)
		{
			ProviderFactory.DesktopProvider.Terminate ();
		}

		private static void Application_FormAdded (object sender, EventArgs args)
		{
			UiTaskSchedulerHolder.InitOnceFromCurrentSyncContext ();

			var form = (Form) sender;

			// Some sort of optimisation for frequently called `Application.FormAdded`.
			if (IsFormProviderAlreadyCreated (form))
				return;

			// NOTE: Form Provider Releasing is done by FormProvider
			
			// Pass false in last argument so that InitializeChildControlStructure
			// isn't called when the provider is created.  We'll do
			// that manually after alerting the bridge to the presence
			// of the new form.

			FormProvider.UpdateOwnerProviderOfForm (form);
		}

		private static bool IsFormProviderAlreadyCreated (Form form)
		{
			var provider = (FormProvider) ProviderFactory.FindProvider (form);
			return provider != null && provider.Navigate (NavigateDirection.Parent) != null;
		}
	}
}
