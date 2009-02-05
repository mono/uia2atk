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
//	Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.MenuItem
{
	internal class ToggleProviderBehavior : ProviderBehavior, IToggleProvider
	{
		private MenuItemProvider itemProvider;
		
		public ToggleProviderBehavior (MenuItemProvider itemProvider) :
			base (itemProvider)
		{
			this.itemProvider = itemProvider;
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			return base.GetPropertyValue (propertyId);
		}

		public override AutomationPattern ProviderPattern {
			get {
				return TogglePatternIdentifiers.Pattern;
			}
		}

		public override void Connect ()
		{
		}

		public override void Disconnect ()
		{
		}

		#region IToggleProvider implementation

		// NOTE: There is no guarantee that clicking the item will
		//       actually perform a toggle.
		public void Toggle ()
		{
			if (((SWF.MenuItem)Provider.Component).Enabled == false)
				throw new ElementNotEnabledException ();

			PerformClick ();
		}
		
		public ToggleState ToggleState {
			get {
				return ToggleState.On;
			}
		}
		
		#endregion	
		
		#region Private Methods
		
		private void PerformClick ()
		{
			if (itemProvider.ParentMenu != null &&
			    itemProvider.ParentMenu.Wnd != null &&
			    itemProvider.ParentMenu.Wnd.InvokeRequired) {
				itemProvider.ParentMenu.Wnd.BeginInvoke (new SWF.MethodInvoker (PerformClick));
			}
			
			SWF.MenuItem item = (SWF.MenuItem) Provider.Component;
			item.PerformClick ();
		}
		
		#endregion
	}
}
