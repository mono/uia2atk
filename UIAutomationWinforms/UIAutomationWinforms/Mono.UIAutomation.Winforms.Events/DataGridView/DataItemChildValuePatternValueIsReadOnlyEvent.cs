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
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Events.DataGridView
{

	internal class DataItemChildValuePatternValueIsReadOnlyEvent 
		: BaseAutomationPropertyEvent
	{
		
		#region Constructor
		
		public DataItemChildValuePatternValueIsReadOnlyEvent (DataGridViewProvider.DataGridViewDataItemEditProvider provider)
			: base (provider,
			        ValuePatternIdentifiers.IsReadOnlyProperty)
		{
			this.provider = provider;
			isreadonly = (bool) provider.GetPropertyValue (ValuePatternIdentifiers.IsReadOnlyProperty.Id);
		}
		
		#endregion
		
		#region IConnectable Overrides
	
		public override void Connect ()
		{
			provider.ItemProvider.DataGridView.CellStateChanged += OnIsReadOnlyProperty;
			provider.ItemProvider.DataGridView.EnabledChanged += OnEnabledChanged;
		}

		public override void Disconnect ()
		{
			provider.ItemProvider.DataGridView.CellStateChanged -= OnIsReadOnlyProperty;
			provider.ItemProvider.DataGridView.EnabledChanged -= OnEnabledChanged;	
		}
		
		#endregion
		
		#region Private Methods
		
		private void OnIsReadOnlyProperty (object sender, 
		                                   SWF.DataGridViewCellStateChangedEventArgs args)
		{
			bool newValue = args.Cell.ReadOnly;

			if (args.Cell.ColumnIndex == provider.Cell.ColumnIndex
			    && args.Cell.RowIndex == provider.Cell.RowIndex
			    && isreadonly != newValue) {
				isreadonly = newValue;
				RaiseAutomationPropertyChangedEvent ();
			}
		}

		private void OnEnabledChanged (object sender, EventArgs args)
		{
			SWF.Control datagrid = (SWF.Control) sender;
			isreadonly = !datagrid.Enabled;
			RaiseAutomationPropertyChangedEvent ();
		}

		#endregion

		#region Private Fields

		private bool isreadonly;
		private DataGridViewProvider.DataGridViewDataItemEditProvider provider;

		#endregion
	}
}
