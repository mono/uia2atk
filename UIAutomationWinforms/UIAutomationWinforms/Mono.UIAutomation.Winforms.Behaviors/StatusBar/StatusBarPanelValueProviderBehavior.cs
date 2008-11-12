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
//	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.StatusBar;

namespace Mono.UIAutomation.Winforms.Behaviors.StatusBar
{
	internal class StatusBarPanelValueProviderBehavior : ProviderBehavior, IValueProvider
	{
		#region Constructor
		
		public StatusBarPanelValueProviderBehavior (StatusBarProvider.StatusBarPanelProvider provider)
			: base (provider)
		{
			this.statusBarPanel = (SWF.StatusBarPanel) provider.Component;
		}
		
		#endregion
		
		#region IProviderBehavior Interface
		
		public override AutomationPattern ProviderPattern {
			get { return ValuePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: IsReadOnly Property never changes.
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   new StatusBarPanelValuePatternValueEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else if (propertyId == ValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region IValueProvider Members
		
		public bool IsReadOnly {
			get { return true; }
		}
		
		public string Value {
			get { return statusBarPanel.Text; }
		}

		public void SetValue (string value)
		{
			throw new ElementNotEnabledException ();
		}
		
		#endregion
		
		#region Private Fields
		
		private SWF.StatusBarPanel statusBarPanel;
		
		#endregion
	}
}
