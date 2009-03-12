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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGridView
{
	internal class DataItemChildSelectionItemPatternElementSelectedEvent
		: BaseAutomationEvent
	{
		#region Constructors

		public DataItemChildSelectionItemPatternElementSelectedEvent (DataGridViewProvider.DataGridViewDataItemChildProvider provider)
			: base (provider, 
			        SelectionItemPatternIdentifiers.ElementSelectedEvent)
		{
			this.provider = provider;
		}
		
		#endregion
		
		#region Overridden Methods

		public override void Connect ()
		{
			provider.DataGridViewProvider.DataGridView.CellStateChanged += OnCellStateChanged;
		}

		public override void Disconnect ()
		{
			provider.DataGridViewProvider.DataGridView.CellStateChanged -= OnCellStateChanged;
		}
		
		#endregion 
		
		#region Private members
		
		private void OnCellStateChanged (object sender, 
		                                 SWF.DataGridViewCellStateChangedEventArgs args)
		{
			if (provider.DataGridViewProvider.DataGridView.SelectedCells.Count == 1
			    && args.Cell.RowIndex == provider.Cell.RowIndex
			    && args.Cell.ColumnIndex == provider.Cell.ColumnIndex
			    && provider.Cell.Selected)
			    RaiseAutomationEvent ();
		}

		private DataGridViewProvider.DataGridViewDataItemChildProvider provider;

		#endregion
	}
}
