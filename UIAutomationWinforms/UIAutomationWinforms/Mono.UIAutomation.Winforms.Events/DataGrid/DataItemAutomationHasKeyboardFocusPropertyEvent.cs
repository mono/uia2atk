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
// Copyright (c) 2008-2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGrid
{

	internal class DataItemAutomationHasKeyboardFocusPropertyEvent
		: AutomationHasKeyboardFocusPropertyEvent
	{
		
		#region Constructors

		public DataItemAutomationHasKeyboardFocusPropertyEvent (ListItemProvider listItemProvider)
			: base (listItemProvider)
		{
			itemProvider = (DataGridProvider.DataGridDataItemProvider) listItemProvider;
		}
		
		#endregion
		
		#region IConnectable Overrides
	
		public override void Connect ()
		{
			((SWF.DataGrid) Provider.Control).CurrentCellChanged += OnCurrentCellChanged;
		}

		public override void Disconnect ()
		{
			((SWF.DataGrid) Provider.Control).CurrentCellChanged -= OnCurrentCellChanged;
		}
		
		#endregion
		
		#region Private Methods
		
		private void OnCurrentCellChanged (object sender, EventArgs args)
		{
			SWF.DataGridCell currentCell = itemProvider.DataGridProvider.DataGrid.CurrentCell;
			if (currentCell.ColumnNumber == 0 && currentCell.RowNumber == itemProvider.Index)
				RaiseAutomationPropertyChangedEvent ();
		}
		
		#endregion

		#region Private Methods

		private DataGridProvider.DataGridDataItemProvider itemProvider;

		#endregion
	}
}
