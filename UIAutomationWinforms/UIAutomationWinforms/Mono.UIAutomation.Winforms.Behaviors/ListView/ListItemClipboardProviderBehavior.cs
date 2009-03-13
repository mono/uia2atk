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
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{
	internal class ListItemClipboardProviderBehavior
		: ProviderBehavior, IClipboardProvider
	{
		#region Constructors
		
		public ListItemClipboardProviderBehavior (ListItemProvider itemProvider)
			: base (itemProvider)
		{
			viewItem = (SWF.ListViewItem) itemProvider.ObjectItem;
		}
		
		#endregion
		
		#region ProviderBehavior Specialization
		
		public override AutomationPattern ProviderPattern {
			get { return ClipboardPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
		}
		
		public override void Disconnect ()
		{
		}

		#endregion

		#region IClipboardProvider Implementation	

		public void Copy (int start, int end)
		{
			string text = viewItem.Text;
			start = (int) System.Math.Max (start, 0);
			end = (int) System.Math.Min (end, text.Length);
			SWF.Clipboard.SetText (text.Substring (start, end - start));
		}
		
		public void Paste (int position)
		{
			// TODO: What about viewItem.ListView.LabelEdit = false ?
			string text = viewItem.Text;
			position = (int) System.Math.Max (position, 0);
			position = (int) System.Math.Min (position, text.Length);

			string newValue = viewItem.Text.Insert (position, SWF.Clipboard.GetText ());
			PerformSetValue (newValue);
		}

		#endregion

		#region Private Methods

		private void PerformSetValue (string value)
		{
			if (viewItem.ListView.InvokeRequired == true) {
				viewItem.ListView.BeginInvoke (new ListItemSetValueDelegate (PerformSetValue),
				                               new object [] { value } );
				return;
			}
			
			viewItem.Text = value;
		}

		#endregion
		
		#region Private Fields
		
		private SWF.ListViewItem viewItem;
		
		#endregion
	}

	delegate void ListItemSetValueDelegate (string value);
}
