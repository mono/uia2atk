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
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	public class FormListener
	{
#region Private Static Members
		
		static bool initialized = false;
		static Dictionary<Form, FormProvider> formProviders =
			new Dictionary<Form, FormProvider> ();
		
#endregion
		
#region Public Static Methods
		
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
			Application.ApplicationExit += delegate (object sender, EventArgs args) {
				foreach (FormProvider provider in ProviderFactory.GetFormProviders ()) {
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
					                                   provider);
				}
			};
			
			// FIXME: FormAdded is fired too frequently (such as
			//        when the form comes into focus).  A different
			//        event is probably more appropriate.
			Application.FormAdded += new EventHandler (OnFormAdded);

			initialized = true;
		}
		
#endregion
		
#region Static Event Handlers
		
		static void OnFormAdded (object sender, EventArgs args)
		{
			var f = (Form) sender;
			TryAddFormProvider (f);
		}

		private static bool TryAddFormProvider (Form form)
		{
			var provider = (FormProvider) ProviderFactory.GetProvider (form);
			var added = formProviders.TryAdd (form, provider);
			if (!added)
				return false;

			// NOTE: Form Provider Releasing is done by FormProvider
			
			// Pass false in last argument so that InitializeChildControlStructure
			// isn't called when the provider is created.  We'll do
			// that manually after alerting the bridge to the presence
			// of the new form.

			provider.ProviderClosed += (s,e) => {
				var l_formProvider = (FormProvider) s;
				var l_form = (Form) l_formProvider.Control;
				RemoveFormProvider (l_form);
			};
			
			form.VisibleChanged  += (s,e) => {
				var l_form = (Form) s;
				if (l_form.Visible) {
					TryAddFormProvider (l_form);
				} else {
					RemoveFormProvider (l_form);
				}
			};

			if (form.Owner == null) { //For example is not MessageBox, f.ShowDialog or XXXXXDialog
				// TODO: Fill in rest of eventargs
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded, provider);
				provider.InitializeChildControlStructure ();
			} else {
				var ownerProvider = (FormProvider) ProviderFactory.FindProvider (form.Owner);
				ownerProvider.AddChildProvider (provider);
			}

			return true;
		}

		private static void RemoveFormProvider (Form form)
		{
			formProviders.Remove (form);
			if (form.Owner != null) {
				var provider = (FormProvider) ProviderFactory.GetProvider (form);
				var ownerProvider = (FormProvider) ProviderFactory.FindProvider (form.Owner);
				ownerProvider.RemoveChildProvider (provider);
			}
		}

#endregion
	}
}
