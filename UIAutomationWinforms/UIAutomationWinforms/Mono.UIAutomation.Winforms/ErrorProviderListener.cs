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
using SWFErrorProvider = System.Windows.Forms.ErrorProvider;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Mono.UIAutomation.Winforms
{

	internal static class ErrorProviderListener
	{
		
		#region Constructors

		static ErrorProviderListener ()
		{
			provider = null;
			initialized = false;
			errorProviders = new Dictionary<SWFErrorProvider, List <Control>> ();
		}
		
		#endregion
		
		#region Private Static Fields
		
		private static bool initialized;
		private static Dictionary<SWFErrorProvider, List<Control>> errorProviders;
		private static ErrorProvider.ErrorProviderToolTipProvider provider;

		#endregion
		
		#region Public Static Methods
		
		public static SWFErrorProvider GetErrorProviderFromControl (Control control)
		{
			foreach (KeyValuePair<SWFErrorProvider, List<Control>> valuePair in
			         errorProviders) {
				if (valuePair.Value.Contains (control) == true)
					return valuePair.Key;
			}
			return null;
		}
		
		//Method called by SWF.HelpProvider static constructor
		public static void Initialize ()
		{
			if (initialized == true)
				return;

			//Events used to associate ErrorProvider with Control
			Helper.AddPrivateEvent (typeof (SWFErrorProvider),
			                        null,
			                        "UIAErrorProviderHookUp", 
			                        typeof (ErrorProviderListener),
			                        "OnUIAErrorProviderHookUp");
			Helper.AddPrivateEvent (typeof (SWFErrorProvider),
			                        null,
			                        "UIAErrorProviderUnhookUp", 
			                        typeof (ErrorProviderListener),
			                        "OnUIAErrorProviderUnhookUp");
			
			//Events used to associate UserControls with Control
			Helper.AddPrivateEvent (typeof (SWFErrorProvider),
			                        null,
			                        "UIAControlHookUp", 
			                        typeof (ErrorProviderListener),
			                        "OnUIAControlHookUp");
			Helper.AddPrivateEvent (typeof (SWFErrorProvider),
			                        null,
			                        "UIAControlUnhookUp", 
			                        typeof (ErrorProviderListener),
			                        "OnUIAControlUnhookUp");
			
			//Events used to associate ToolTip provider			
			Helper.AddPrivateEvent (typeof (SWFErrorProvider),
			                        null,
			                        "UIAPopup", 
			                        typeof (ErrorProviderListener),
			                        "OnUIAPopup");
			Helper.AddPrivateEvent (typeof (SWFErrorProvider),
			                        null,
			                        "UIAUnPopup", 
			                        typeof (ErrorProviderListener),
			                        "OnUIAUnPopup");

			initialized = true;
		}
		
		#endregion
		
		#region Private Static Methods
		
#pragma warning disable 169
		
		private static void OnUIAControlHookUp (object sender, ControlEventArgs args)
		{
			Control control = (Control) sender;
			SWFErrorProvider errorProvider = GetErrorProviderFromControl (control);
			
			if (control.Parent != null)
				ErrorProvider.InstancesTracker.AddControl (args.Control, 
				                                           control.Parent,
				                                           errorProvider);
		}
		
		private static void OnUIAControlUnhookUp (object sender, ControlEventArgs args)
		{
			Control control = (Control) sender;
			SWFErrorProvider errorProvider = GetErrorProviderFromControl (control);
			
			if (control.Parent != null)
				ErrorProvider.InstancesTracker.RemoveControl (args.Control, 
				                                              control.Parent,
				                                              errorProvider);
		}

		private static void OnUIAErrorProviderHookUp (object sender, ControlEventArgs args)
		{
			SWFErrorProvider errorProvider = (SWFErrorProvider) sender;
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
		
		private static void OnUIAErrorProviderUnhookUp (object sender, ControlEventArgs args)
		{
			SWFErrorProvider errorProvider = (SWFErrorProvider) sender;
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

		private static void OnUIAPopup (object sender, PopupEventArgs args)
		{
			SWFErrorProvider error = (SWFErrorProvider) sender;
			
			if (provider != null)
				provider.Terminate ();
			
			//We are doing this because SWF.ErroProvider doesn't use SWF.ToolTip :(
			provider = new ErrorProvider.ErrorProviderToolTipProvider (error);
			provider.Initialize ();
			provider.Show (args.AssociatedControl);
		}
		
		private static void OnUIAUnPopup (object sender, PopupEventArgs args)
		{
			if (provider != null) {
				provider.Hide (args.AssociatedControl);
				provider.Terminate ();
			}
		}
		
#pragma warning restore 169		
		
		#endregion
	}
}
