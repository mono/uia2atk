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
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGrid
{
	internal class HeaderItemInvokePatternInvokedEvent 
		: BaseAutomationEvent
	{
		#region Constructors

		public HeaderItemInvokePatternInvokedEvent (DataGridProvider.DataGridHeaderItemProvider provider)
			: base (provider, 
			        InvokePatternIdentifiers.InvokedEvent)
		{
			this.provider = provider;
			dataGridProvider = provider.HeaderProvider.DataGridProvider;
		}
		
		#endregion
		
		#region ProviderEvent Methods

		public override void Connect ()
		{
			dataGridProvider.DataGrid.MouseDown += OnDataGridMouseDown;
		}

		public override void Disconnect ()
		{
			dataGridProvider.DataGrid.MouseDown -= OnDataGridMouseDown;
		}
		
		#endregion 
		
		#region Private methods
		
		private void OnDataGridMouseDown (object sender, SWF.MouseEventArgs args)
		{
			SWF.DataGrid.HitTestInfo hitTestInfo
				= dataGridProvider.DataGrid.HitTest (args.X, args.Y);

			if (args.Button == SWF.MouseButtons.Left
				   && hitTestInfo.Type == SWF.DataGrid.HitTestType.ColumnHeader) {
				if (hitTestInfo.Column
				    == provider.HeaderProvider.GridColumnStyles.IndexOf (provider.ColumnStyle))
					RaiseAutomationEvent ();
			}
		}

		private DataGridProvider dataGridProvider;
		private DataGridProvider.DataGridHeaderItemProvider provider;

		#endregion
	}
}