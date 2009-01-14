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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Automation;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Events.DataGrid
{
	internal class DataItemEditValuePatternValueEvent
		: BaseAutomationPropertyEvent
	{
		
		#region Constructors

		public DataItemEditValuePatternValueEvent (DataGridProvider.DataGridDataItemEditProvider provider)
			: base (provider, 
			        ValuePatternIdentifiers.ValueProperty)
		{
			editProvider = provider;
		}
		
		#endregion
		
		#region ProviderEvent Methods

		public override void Connect ()
		{
			editProvider.ItemProvider.DataGridProvider.DataGrid.UIAGridCellChanged += OnUIAGridCellChanged;
			editProvider.ItemProvider.DataGridProvider.DataGrid.UIACollectionChanged += OnUIACollectionChanged;
			editProvider.ItemProvider.DataGridProvider.DataGrid.Click += OnRowHeaderClick;
		}

		public override void Disconnect ()
		{
			editProvider.ItemProvider.DataGridProvider.DataGrid.UIAGridCellChanged -= OnUIAGridCellChanged;
			editProvider.ItemProvider.DataGridProvider.DataGrid.UIACollectionChanged -= OnUIACollectionChanged;
			editProvider.ItemProvider.DataGridProvider.DataGrid.Click -= OnRowHeaderClick;
		}
		
		#endregion 
		
		#region Protected methods

		private void OnUIAGridCellChanged (object sender, CollectionChangeEventArgs args)
		{
			SWF.DataGridCell cell = (SWF.DataGridCell) args.Element;
			if (cell.ColumnNumber == editProvider.ItemProvider.GetColumnIndexOf (editProvider)
			    && cell.RowNumber == editProvider.ItemProvider.Index)
				RaiseAutomationPropertyChangedEvent ();
		}

		private void OnUIACollectionChanged (object sender, CollectionChangeEventArgs args)
		{
			RaiseAutomationPropertyChangedEvent ();
		}

		private void OnRowHeaderClick (object sender, EventArgs args)
		{
			Point point = editProvider.ItemProvider.DataGridProvider.DataGrid.PointToClient (SWF.Cursor.Position);
			SWF.DataGrid.HitTestInfo hitTest = editProvider.ItemProvider.DataGridProvider.DataGrid.HitTest (point);
			if (editProvider.ItemProvider.GetColumnIndexOf (editProvider) == hitTest.Column 
			    && hitTest.Type == SWF.DataGrid.HitTestType.ColumnHeader)
				RaiseAutomationPropertyChangedEvent ();
		}

		#endregion

		#region Private Fields

		private DataGridProvider.DataGridDataItemEditProvider editProvider;

		#endregion
	}
}

