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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGridView
{
	
	internal class DataItemComboBoxListItemSelectionItemPatternElementSelectedEvent
		: BaseAutomationEvent
	{

		#region Constructor

		public DataItemComboBoxListItemSelectionItemPatternElementSelectedEvent (ListItemProvider provider)
			: base (provider, 
			        SelectionItemPatternIdentifiers.ElementSelectedEvent)
		{
			this.itemProvider = provider;
			this.provider = (DataGridViewProvider.DataGridViewDataItemComboBoxListBoxProvider) provider.ListProvider;
		}
		
		#endregion

		#region IConnectable Overrides
		
		public override void Connect ()
		{
			provider.ComboboxProvider.ComboBoxCell.DataGridView.CellValueChanged += OnCellValueChanged;
		}

		public override void Disconnect ()
		{
			provider.ComboboxProvider.ComboBoxCell.DataGridView.CellValueChanged -= OnCellValueChanged;
		}
		
		#endregion
		
		#region Private Methods

		private void OnCellValueChanged (object sender, SWF.DataGridViewCellEventArgs args)
		{
			if (args.ColumnIndex == provider.ComboboxProvider.ComboBoxCell.ColumnIndex
			    && args.RowIndex == provider.ComboboxProvider.ComboBoxCell.RowIndex
			    && provider.IsItemSelected (itemProvider))
				RaiseAutomationEvent ();
		}
		
		#endregion

		#region Private Fields

		private ListItemProvider itemProvider;
		private DataGridViewProvider.DataGridViewDataItemComboBoxListBoxProvider provider;

		#endregion
	}
}
