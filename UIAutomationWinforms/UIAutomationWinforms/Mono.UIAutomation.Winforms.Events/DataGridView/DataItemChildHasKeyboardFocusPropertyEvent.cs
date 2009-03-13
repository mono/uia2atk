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

	internal class DataItemChildHasKeyboardFocusPropertyEvent
		: AutomationHasKeyboardFocusPropertyEvent
	{
		
		#region Constructors

		public DataItemChildHasKeyboardFocusPropertyEvent (DataGridViewProvider.DataGridViewDataItemChildProvider itemProvider)
			: base (itemProvider)
		{
			this.itemProvider = itemProvider;
		}
		
		#endregion
		
		#region IConnectable Overrides
	
		public override void Connect ()
		{
			itemProvider.DataGridViewProvider.DataGridView.GotFocus += OnCurrentCellChanged;
			itemProvider.DataGridViewProvider.DataGridView.LostFocus += OnCurrentCellChanged;
			itemProvider.DataGridViewProvider.DataGridView.CurrentCellChanged += OnCurrentCellChanged;
		}

		public override void Disconnect ()
		{
			itemProvider.DataGridViewProvider.DataGridView.GotFocus -= OnCurrentCellChanged;
			itemProvider.DataGridViewProvider.DataGridView.LostFocus -= OnCurrentCellChanged;
			itemProvider.DataGridViewProvider.DataGridView.CurrentCellChanged -= OnCurrentCellChanged;
		}
		
		#endregion
		
		#region Private Fields
		
		private void OnCurrentCellChanged (object sender, EventArgs args)
		{
			RaiseAutomationPropertyChangedEvent ();
		}
		
		private DataGridViewProvider.DataGridViewDataItemChildProvider itemProvider;

		#endregion
	}
}
