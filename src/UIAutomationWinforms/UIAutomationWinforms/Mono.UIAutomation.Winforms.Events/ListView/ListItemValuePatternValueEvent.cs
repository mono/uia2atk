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
	internal class ListItemValuePatternValueEvent
		: BaseAutomationPropertyEvent
	{
		
		#region Constructors

		public ListItemValuePatternValueEvent (ListItemProvider provider)
			: base (provider, 
			        ValuePatternIdentifiers.ValueProperty)
		{
			viewItem = (SWF.ListViewItem) provider.ObjectItem;
		}
		
		#endregion
		
		#region ProviderEvent Methods

		public override void Connect ()
		{
			viewItem.ListView.AfterLabelEdit += OnAfterLabelEdit;
		}

		public override void Disconnect ()
		{
			viewItem.ListView.AfterLabelEdit -= OnAfterLabelEdit;
		}
		
		#endregion 
		
		#region Protected methods

		private void OnAfterLabelEdit (object sender, SWF.LabelEditEventArgs args)
		{
			if (viewItem.ListView.Items.IndexOf (viewItem) == args.Item) {
				newText = args.Label;
				RaiseAutomationPropertyChangedEvent ();
			}
		}

		//We are doing this because AfterLabelEdit event is generated before commiting the text
		protected override object GetNewPropertyValue ()
		{
			return newText;
		}		

		#endregion

		#region Private Fields

		private string newText;
		private SWF.ListViewItem viewItem;

		#endregion
	}
}

