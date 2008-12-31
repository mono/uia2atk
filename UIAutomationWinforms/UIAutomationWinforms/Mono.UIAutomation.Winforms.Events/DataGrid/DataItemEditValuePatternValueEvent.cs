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
			try {
				// Event used add child. Usually happens when DataSource/DataMember are valid
				Helper.AddPrivateEvent (typeof (SWF.DataGrid),
				                        editProvider.ItemProvider.DataGridProvider.Control,
				                        "UIAGridCellChanged",
				                        this,
				                        "OnUIAGridCellChanged");
			} catch (NotSupportedException) {}
		}

		public override void Disconnect ()
		{
			try {
				Console.WriteLine (editProvider.ItemProvider.DataGridProvider.Control.GetType ());
				// Event used add child. Usually happens when DataSource/DataMember are valid
				Helper.RemovePrivateEvent (typeof (SWF.DataGrid),
				                           editProvider.ItemProvider.DataGridProvider.Control,
				                           "UIAGridCellChanged",
				                           this,
				                           "OnUIAGridCellChanged");
			} catch (NotSupportedException) {}

		}
		
		#endregion 
		
		#region Protected methods

#pragma warning disable 169

		private void OnUIAGridCellChanged (object sender, CollectionChangeEventArgs args)
		{
			// XXX: Seems that the value is changed, however we need to wait for 
			// OnPaint to get the change.
			SWF.DataGridCell cell = (SWF.DataGridCell) args.Element;
			if (cell.ColumnNumber == editProvider.ItemProvider.GetColumnIndexOf (editProvider)
			    && cell.RowNumber == editProvider.ItemProvider.Index)
				RaiseAutomationPropertyChangedEvent ();
		}

#pragma warning restore 169

		#endregion

		#region Private Fields

		private DataGridProvider.DataGridDataItemEditProvider editProvider;

		#endregion
	}
}

