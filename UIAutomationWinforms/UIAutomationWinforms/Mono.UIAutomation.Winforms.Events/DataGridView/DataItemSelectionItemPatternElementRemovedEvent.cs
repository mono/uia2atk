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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Events.DataGridView
{

	internal class DataItemSelectionItemPatternElementRemovedEvent
		: BaseAutomationEvent
	{

		#region Constructors

		public DataItemSelectionItemPatternElementRemovedEvent (FragmentControlProvider provider)
			: base (provider, 
			        SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent)
		{
			this.provider = (DataGridViewProvider.DataGridDataItemProvider) provider;
			selected = this.provider.DataGridView.SelectedCells.Contains (this.provider.Row.Cells [0]);
		}
		
		#endregion
		
		#region Overridden Methods

		public override void Connect ()
		{
			provider.DataGridView.SelectionChanged += OnSelectionChanged;
		}

		public override void Disconnect ()
		{
			provider.DataGridView.SelectionChanged -= OnSelectionChanged;
		}
		
		#endregion 
		
		#region Private members
		
		private void OnSelectionChanged (object sender, EventArgs args)
		{
			if (selected
			    && provider.DataGridView.SelectedCells.Count > 1
			    && !provider.DataGridView.SelectedCells.Contains (provider.Row.Cells [0]))
				RaiseAutomationEvent ();

			selected = provider.DataGridView.SelectedCells.Contains (provider.Row.Cells [0]);
		}

		private bool selected;
		private DataGridViewProvider.DataGridDataItemProvider provider;
		
		#endregion
	}
}
