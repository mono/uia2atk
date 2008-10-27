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
using System.Windows.Automation;
using SWF = System.Windows.Forms;
namespace Mono.UIAutomation.Winforms.Events.ListView
{
	internal class ListItemEditValuePatternValueEvent
		: BaseAutomationPropertyEvent
	{
		
		#region Constructors

		public ListItemEditValuePatternValueEvent (ListViewProvider.ListViewListItemEditProvider provider)
			: base (provider, 
			        ValuePatternIdentifiers.ValueProperty)
		{
			editProvider = provider;
		}
		
		#endregion
		
		#region ProviderEvent Methods

		public override void Connect ()
		{
			editProvider.ItemProvider.ListViewItem.UIASubItemTextChanged += OnUIATextChanged;
		}

		public override void Disconnect ()
		{
			editProvider.ItemProvider.ListViewItem.UIASubItemTextChanged -= OnUIATextChanged;
		}
		
		#endregion 
		
		#region Protected methods

		private void OnUIATextChanged (object sender, SWF.LabelEditEventArgs args)
		{
			if (args.Item == editProvider.ItemProvider.ListView.Columns.IndexOf (editProvider.ColumnHeader))
				RaiseAutomationPropertyChangedEvent ();
		}

		#endregion

		#region Private Fields

		private ListViewProvider.ListViewListItemEditProvider editProvider;

		#endregion
	}
}

