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
using SWF = System.Windows.Forms;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	internal static class ErrorProviderListener
	{
		
		#region Constructors

		static ErrorProviderListener ()
		{
			provider = null;
			initialized = false;
			errorProviders = new Dictionary<SWF.ErrorProvider, List <Control>> ();
		}
		
		#endregion
		
		#region Private Static Fields
		
		private static bool initialized;
		private static Dictionary<SWF.ErrorProvider, List<SWF.Control>> errorProviders;
		private static ErrorProvider.ErrorProviderToolTipProvider provider;

		#endregion
		
		#region Public Static Methods
		
		public static SWF.ErrorProvider GetErrorProviderFromControl (Control control)
		{
			foreach (KeyValuePair<SWF.ErrorProvider, List<SWF.Control>> valuePair in
			         errorProviders) {
				if (valuePair.Value.Contains (control) == true)
					return valuePair.Key;
			}
			return null;
		}
		
		//Method called by SWF.HelpProvider static constructor
		public static void Initialize ()
		{
			//FIXME: Should we use IsAccessibilityEnabled instead?
			if (!AutomationInteropProvider.ClientsAreListening || initialized == true)
				return;

			//Events used to associate ErrorProvider with Control
			SWF.ErrorProvider.UIAErrorProviderHookUp += OnUIAErrorProviderHookUp;
			SWF.ErrorProvider.UIAErrorProviderUnhookUp += OnUIAErrorProviderUnhookUp;
			
			//Events used to associate UserControls with Control
			SWF.ErrorProvider.UIAControlHookUp += OnUIAControlHookUp;
			SWF.ErrorProvider.UIAControlUnhookUp += OnUIAControlUnhookUp;
			
			//Events used to associate ToolTip provider
			SWF.ErrorProvider.UIAPopup += OnUIAPopup;
			SWF.ErrorProvider.UIAUnPopup += OnUIAUnPopup;

			initialized = true;
		}
		
		#endregion
		
		#region Private Static Methods
		
		private static void OnUIAControlHookUp (object sender, SWF.ControlEventArgs args)
		{
			SWF.Control control = (SWF.Control) sender;
			SWF.ErrorProvider errorProvider = GetErrorProviderFromControl (control);
			
			if (control.Parent != null)
				ErrorProvider.InstancesTracker.AddControl (args.Control, 
				                                           control.Parent,
				                                           errorProvider);
		}
		
		private static void OnUIAControlUnhookUp (object sender, SWF.ControlEventArgs args)
		{
			SWF.Control control = (SWF.Control) sender;
			SWF.ErrorProvider errorProvider = GetErrorProviderFromControl (control);
			
			if (control.Parent != null)
				ErrorProvider.InstancesTracker.RemoveControl (args.Control, 
				                                              control.Parent,
				                                              errorProvider);
		}

		private static void OnUIAErrorProviderHookUp (object sender, SWF.ControlEventArgs args)
		{
			SWF.ErrorProvider errorProvider = (SWF.ErrorProvider) sender;
			List<Control> list = null;

			if (errorProviders.TryGetValue (errorProvider, out list) == false) {
				list = new List<Control> ();
				errorProviders [errorProvider] = list;
			}
			if (list.Contains (args.Control) == false)
				list.Add (args.Control);
			
			//Let's associate ErrorProvider if Provider for Control already exists
			FragmentControlProvider provider = 
				(FragmentControlProvider) ProviderFactory.FindProvider (args.Control);
			if (provider != null)
				provider.ErrorProvider = errorProvider;
		}
		
		private static void OnUIAErrorProviderUnhookUp (object sender, SWF.ControlEventArgs args)
		{
			SWF.ErrorProvider errorProvider = (SWF.ErrorProvider) sender;
			List<Control> list = null;
			
			if (errorProviders.TryGetValue (errorProvider, out list) == true) {
				list.Remove (args.Control);
				if (list.Count == 0)
					errorProviders.Remove (errorProvider);
				
				//Let's disassociate ErrorProvider if Provider for Control already exists
				FragmentControlProvider provider = 
					(FragmentControlProvider) ProviderFactory.FindProvider (args.Control);
				if (provider != null)
					provider.ErrorProvider = null;
			}
		}

		private static void OnUIAPopup (object sender, SWF.PopupEventArgs args)
		{
			SWF.ErrorProvider error = (SWF.ErrorProvider) sender;
			
			if (provider != null)
				provider.Terminate ();
			
			//We are doing this because SWF.ErroProvider doesn't use SWF.ToolTip :(
			provider = new ErrorProvider.ErrorProviderToolTipProvider (error);
			provider.Initialize ();
			provider.Show (args.AssociatedControl);
		}
		
		private static void OnUIAUnPopup (object sender, SWF.PopupEventArgs args)
		{
			if (provider != null) {
				provider.Hide (args.AssociatedControl);
				provider.Terminate ();
			}
		}
		
		#endregion
	}
}
