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
using System;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.DataGridView;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGridView
{
	// FIXME: Implement IEmbeddedImage ??
	
	internal class DataItemChildToggleProviderBehavior  
		: ProviderBehavior, IToggleProvider
	{	
		#region Constructor
		
		public DataItemChildToggleProviderBehavior (DataGridViewProvider.DataGridViewDataItemCheckBoxProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}
		
		#endregion
		
		#region Overriden Methods
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                   new DataItemChildTogglePatternToggleStateEvent (provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TogglePatternToggleStateProperty,
			                   null);
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return TogglePatternIdentifiers.Pattern; }
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TogglePatternIdentifiers.ToggleStateProperty.Id)
				return ToggleState;
			else
				return null;
		}

		#endregion
		
		#region IToggleProvider Members
	
		public void Toggle ()
		{
			if (provider.Cell.ReadOnly)
				throw new ElementNotEnabledException ();

			switch (ToggleState) {
			case ToggleState.On:
				PerformToggle (SWF.CheckState.Unchecked);
				break;
			case ToggleState.Off:
				if (provider.CheckBoxCell.ThreeState)
					PerformToggle (SWF.CheckState.Indeterminate);
				else
					PerformToggle (SWF.CheckState.Checked);
				break;
			// Control could still have been set to intermediate
			// programatically, regardless of ThreeState value.
			case ToggleState.Indeterminate:
			default:
				PerformToggle (SWF.CheckState.Checked);
				break;
			}
		}

		public ToggleState ToggleState {
			get {
				// More information: 
				// http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridviewcheckboxcell.valuetype.aspx
					
				if (provider.CheckBoxCell.Value == null) {
					if (provider.CheckBoxCell.ThreeState)
						return ToggleState.Indeterminate;
					else
						return ToggleState.Off;
				} else {
					SWF.CheckState state = SWF.CheckState.Indeterminate;
					bool tryBoolean = false;
					try {
						state = (SWF.CheckState) provider.CheckBoxCell.Value;
					} catch (InvalidCastException) { tryBoolean = true; }

					if (tryBoolean) {
						try {
							bool val = (bool) provider.CheckBoxCell.Value;
							if (val)
								state = SWF.CheckState.Checked;
							else
								state = SWF.CheckState.Unchecked;
						} catch (InvalidCastException) {
							// We just can't get valid value
							return ToggleState.Indeterminate;
						}
					}

					switch (state) {
					case SWF.CheckState.Checked:
						return ToggleState.On;
					case SWF.CheckState.Unchecked:
						return ToggleState.Off;
					case SWF.CheckState.Indeterminate:
					default:
						return ToggleState.Indeterminate;
					}

				}
			}
		}

		#endregion
		
		#region Private Methods
		
		private void PerformToggle (SWF.CheckState state)
		{
			if (provider.ItemProvider.DataGridView.InvokeRequired) {
				provider.ItemProvider.DataGridView.BeginInvoke (new PerformToggleDelegate (PerformToggle),
				                                                new object [] { state });
				return;
			}

			// FIXME: Validate InvalidCastException ? (thrown usually because of ValueType)

			SWF.DataGridViewCell oldCell = provider.ItemProvider.DataGridView.CurrentCell;
			provider.ItemProvider.DataGridView.CurrentCell = provider.CheckBoxCell;
//			provider.ItemProvider.DataGridView.BeginEdit (false);
			provider.CheckBoxCell.Value = state;
//			provider.ItemProvider.DataGridView.EndEdit ();
			provider.ItemProvider.DataGridView.CurrentCell = oldCell;
		}

		#endregion

		#region Private Fields
	
		private DataGridViewProvider.DataGridViewDataItemCheckBoxProvider provider;
	
		#endregion

	}
	
	delegate void PerformToggleDelegate (SWF.CheckState state);
}
