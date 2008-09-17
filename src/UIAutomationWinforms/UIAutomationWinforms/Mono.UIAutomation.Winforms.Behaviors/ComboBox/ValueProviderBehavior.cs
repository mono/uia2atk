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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ComboBox;

namespace Mono.UIAutomation.Winforms.Behaviors.ComboBox
{

	internal class ValueProviderBehavior 
		: ProviderBehavior, IValueProvider
	{
		
		#region Constructors
	
		public ValueProviderBehavior (FragmentControlProvider provider)
			: base (provider) 
		{
		}

		#endregion		

		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern {
			get { return ValuePatternIdentifiers.Pattern; }
		}		
		
		public override void Connect (Control control) 
		{
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   new ValuePatternValuePropertyEvent (Provider));
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   new ValuePatternValueIsReadOnlyEvent (Provider));
		}
		
		public override void Disconnect (Control control)
		{
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else if (propertyId == ValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
			
		#region IValueProvider Interface
		
		public bool IsReadOnly {
			get { return Provider.Control.Enabled == false; }
		}

		public string Value {
			get { return Provider.Control.Text; }
		}

		public void SetValue (string value)
		{
			if (IsReadOnly == true)
				throw new ElementNotEnabledException ();

			PerformSetValue (value);
		}

		#endregion
		
		#region Private Methods
		
		private void PerformSetValue (string value) 
		{
			if (Provider.Control.InvokeRequired == true) {
				Provider.Control.BeginInvoke (new ComboBoxSetValueDelegate (PerformSetValue),
				                              new object [] { value } );
				return;
			}
			
			Provider.Control.Text = value;
		}
		
		#endregion
	}
	
	delegate void ComboBoxSetValueDelegate (string value);
}
