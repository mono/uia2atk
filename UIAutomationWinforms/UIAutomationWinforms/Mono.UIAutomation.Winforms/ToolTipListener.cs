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

namespace Mono.UIAutomation.Winforms
{

	internal static class ToolTipListener
	{
		
		#region Constructors

		static ToolTipListener ()
		{
			initialized = false;
			tooltips = new Dictionary<ToolTip, List<Control>> ();
		}
		
		#endregion
		
		#region Private Static Fields
		
		private static bool initialized;
		private static Dictionary<ToolTip, List<Control>> tooltips;

		#endregion
		
		#region Public Static Methods
		
		public static ToolTip GetToolTipFromControl (Control control)
		{
			foreach (KeyValuePair<ToolTip, List<Control>> valuePair in
			         tooltips) {
				if (valuePair.Value.Contains (control) == true)
					return valuePair.Key;
			}
			return null;
		}
		
		public static void Initialize ()
		{
			if (!AutomationInteropProvider.ClientsAreListening || initialized == true)
				return;
			
			Helper.AddPrivateEvent (typeof (ToolTip),
			                        null,
			                        "UIAUnPopup", 
			                        typeof (ToolTipListener),
			                        "OnUIAUnPopup");
			
			Helper.AddPrivateEvent (typeof (ToolTip),
			                        null,
			                        "UIAToolTipHookUp", 
			                        typeof (ToolTipListener),
			                        "OnUIAToolTipHookUp");
			
			Helper.AddPrivateEvent (typeof (ToolTip),
			                        null,
			                        "UIAToolTipUnhookUp", 
			                        typeof (ToolTipListener),
			                        "OnUIAToolTipUnhookUp");

			initialized = true;
		}
		
		#endregion
		
		#region Private Static Methods
		
#pragma warning disable 169

		private static void OnUIAPopup (object sender, PopupEventArgs args)
		{
			ToolTip tooltip = (ToolTip) sender;			
			ToolTipProvider provider 
				= (ToolTipProvider) ProviderFactory.GetProvider (tooltip);			
			provider.Show (args.AssociatedControl);
		}
		
		private static void OnUIAUnPopup (object sender, PopupEventArgs args)
		{
			ToolTip tooltip = (ToolTip) sender;
			ToolTipProvider provider 
				= (ToolTipProvider) ProviderFactory.GetProvider (tooltip);
			provider.Hide (args.AssociatedControl);
			
			ProviderFactory.ReleaseProvider (tooltip);
		}
		
		private static void OnUIAToolTipHookUp (object sender, ControlEventArgs args)
		{
			ToolTip tooltip = (ToolTip) sender;
			List<Control> list = null;

			if (tooltips.TryGetValue (tooltip, out list) == false) {
				list = new List<Control> ();
				tooltips [tooltip] = list;
				tooltip.Popup += new PopupEventHandler (OnUIAPopup);
			}
			if (list.Contains (args.Control) == false)
				list.Add (args.Control);
			
			//Let's associate ToolTip if Provider for Control already exists
			FragmentControlProvider provider = 
				(FragmentControlProvider) ProviderFactory.FindProvider (args.Control);
			if (provider != null)
				provider.ToolTip = tooltip;
		}
		
		private static void OnUIAToolTipUnhookUp (object sender, ControlEventArgs args)
		{
			ToolTip tooltip = (ToolTip) sender;
			List<Control> list = null;

			if (tooltips.TryGetValue (tooltip, out list) == true) {
				list.Remove (args.Control);
				if (list.Count == 0) {
					tooltips.Remove (tooltip);
					tooltip.Popup -= new PopupEventHandler (OnUIAPopup);
				}
				
				//Let's disassociate ToolTip if Provider for Control already exists
				FragmentControlProvider provider = 
					(FragmentControlProvider) ProviderFactory.FindProvider (args.Control);
				if (provider != null)
					provider.ToolTip = tooltip;
			}
		}
		
#pragma warning restore 169
		
		#endregion
		
	}
}
