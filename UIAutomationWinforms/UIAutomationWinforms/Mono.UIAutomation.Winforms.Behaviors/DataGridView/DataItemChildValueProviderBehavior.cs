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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGridView;

using System.Collections.Generic;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGridView
{
	internal class DataItemChildValueProviderBehavior
		: ProviderBehavior, IValueProvider
	{
		#region Constructors
		
		public DataItemChildValueProviderBehavior (DataGridViewProvider.DataGridViewDataItemEditProvider childProvider)
			: base (childProvider)
		{
			this.provider = childProvider;
		}
		
		#endregion
		
		#region ProviderBehavior Specialization
		
		public override AutomationPattern ProviderPattern {
			get { return ValuePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// FIXME: How to listen this event?
			//Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			//                   new DataItemChildValuePatternValueIsReadOnlyEvent (provider));
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   new DataItemChildValuePatternValueValueEvent (provider));
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
			if (IsReadOnly)
				throw new ElementNotEnabledException ();

			PerformSetValue (value);
		}

		public bool IsReadOnly {
			get { return provider.TextBoxCell.ReadOnly; }
		}
		
		public string Value {
			get { return provider.TextBoxCell.Value as string ?? string.Empty; }
		}
		
		#endregion

		#region Private Members
		
		private void PerformSetValue (string value)
		{
			if (provider.ItemProvider.DataGridView.InvokeRequired) {
				provider.ItemProvider.DataGridView.BeginInvoke (new SetValueDelegate (PerformSetValue),
				                                                new object [] { value });
				return;
			}

			// Notice however that this is the weird behavior in .NET, because
			// editing an IsNewRow Row *WONT ADD* a new row

			SWF.DataGridViewCell oldCell = provider.ItemProvider.DataGridView.CurrentCell;
			provider.ItemProvider.DataGridView.CurrentCell = provider.TextBoxCell;
			provider.ItemProvider.DataGridView.BeginEdit (false);
			provider.TextBoxCell.Value = value;
			provider.ItemProvider.DataGridView.EndEdit ();
			provider.ItemProvider.DataGridView.CurrentCell = oldCell;
		}


		private DataGridViewProvider.DataGridViewDataItemEditProvider provider;

		private delegate void SetValueDelegate (string val);

		#endregion

	}
}
