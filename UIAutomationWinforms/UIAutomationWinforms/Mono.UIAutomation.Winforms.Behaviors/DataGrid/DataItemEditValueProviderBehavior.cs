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
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGrid;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGrid
{

	internal class DataItemEditValueProviderBehavior 
		: ProviderBehavior, IValueProvider
	{
		
		public DataItemEditValueProviderBehavior (DataGridProvider.DataGridDataItemEditProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}

		#region ProviderBehavior Specialization

		public override AutomationPattern ProviderPattern {
			get { return ValuePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: IsReadOnly Property NEVER changes.
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   new DataItemEditValuePatternValueEvent (provider));
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

		#region IValueProvider implementation 
		
		public void SetValue (string value)
		{
			// According to our tests you can edit the value, however
			// this doesn't mean that you can change the value, so, in other
			// words an editable style doesn't necessarily mean that you can 
			// set the value. In Vista Wwhen the cell is not editable and you call
			// SetValue it will throw an exception so, we are swallowing all the exceptions.
			PerformSetValue (value);
		}
		
		public bool IsReadOnly {
			get { return false; }
		}
		
		public string Value {
			get { return (string) Provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id); }
		}
		
		#endregion

		#region Private Methods

		private void PerformSetValue (string value)
		{
			if (provider.ItemProvider.Control.InvokeRequired) {
				provider.ItemProvider.Control.BeginInvoke (new DataItemSetValueDelegate (PerformSetValue),
				                                           new object [] { value } );
				return;
			}
		 	try {
				provider.ItemProvider.SetEditValue (provider, value);
			} catch (Exception e) {
				// DataSource may throw any exception.
				Log.Warn ("{0}: Caught exception:\n{1}", this.GetType (), e);
			}
		}

		#endregion

		#region Private Methods

		private DataGridProvider.DataGridDataItemEditProvider provider;
		
		#endregion
	}

	delegate void DataItemSetValueDelegate (string value);

}
