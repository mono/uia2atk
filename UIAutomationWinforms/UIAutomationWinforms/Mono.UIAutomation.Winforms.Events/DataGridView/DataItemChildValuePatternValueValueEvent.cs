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

	internal class DataItemChildValuePatternValueValueEvent 
		: BaseAutomationPropertyEvent
	{
		
		#region Constructor
		
		public DataItemChildValuePatternValueValueEvent (DataGridViewProvider.DataGridViewDataItemEditProvider provider)
			: base (provider,
			        ValuePatternIdentifiers.ValueProperty)
		{
			this.provider = provider;
		}
		
		#endregion
		
		#region IConnectable Overrides
	
		public override void Connect ()
		{
			provider.ItemProvider.DataGridView.CellEndEdit += OnValueProperty;
		}

		public override void Disconnect ()
		{
			provider.ItemProvider.DataGridView.CellEndEdit -= OnValueProperty;
		}
		
		#endregion
		
		#region Private Methods
		
		private void OnValueProperty (object sender, 
		                              SWF.DataGridViewCellEventArgs args)
		{
			if (args.ColumnIndex == provider.Cell.ColumnIndex
			    && args.RowIndex == provider.Cell.RowIndex)
				RaiseAutomationPropertyChangedEvent ();
		}

		#endregion

		#region Private Fields

		private DataGridViewProvider.DataGridViewDataItemEditProvider provider;

		#endregion
	}
}
