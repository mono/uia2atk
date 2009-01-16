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
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{
	internal class ListItemEditValueProviderBehavior
		: ProviderBehavior, IValueProvider
	{
		#region Constructors
		
		public ListItemEditValueProviderBehavior (ListViewProvider.ListViewListItemEditProvider editProvider)
			: base (editProvider)
		{
			columnHeader = editProvider.ColumnHeader;
			listView = editProvider.ItemProvider.ListView;
			listViewItem = editProvider.ItemProvider.ListViewItem;
		}
		
		#endregion
		
		#region ProviderBehavior Specialization
		
		public override AutomationPattern ProviderPattern {
			get { return ValuePatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTDOTNET: IsReadOnly Property NEVER changes in .NET, instead the 
			// behavior is removed, but in our implementation we need either 
			// IValueProvider or ITextProvider to support changes.
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   new ListItemEditValuePatternIsReadOnlyEvent ((ListViewProvider.ListViewListItemEditProvider) Provider));
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   new ListItemEditValuePatternValueEvent ((ListViewProvider.ListViewListItemEditProvider) Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.ValuePatternIsReadOnlyProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.ValuePatternValueProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == ValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else if (propertyId == ValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion

		#region IValueProvider implementation 
		
		public void SetValue (string value)
		{
			PerformSetValue (value);
		}
		
		public bool IsReadOnly {
			get { return !listView.LabelEdit; }
		}
		
		public string Value {
			get {
				int indexOf = listView.Columns.IndexOf (columnHeader);

				if (indexOf < 0 || indexOf >= listViewItem.SubItems.Count)
					return string.Empty;
				else {
					if (indexOf == 0)
						return listViewItem.Text;
					else
						return listViewItem.SubItems [indexOf - 1].Text;
				}
			}
		}
		
		#endregion

		#region Private Methods

		private void PerformSetValue (string value)
		{
			int indexOf = listView.Columns.IndexOf (columnHeader);

			if (indexOf < 0 || indexOf >= listViewItem.SubItems.Count)
				return;
			
			if (listView.InvokeRequired == true) {
				listView.BeginInvoke (new ListItemSetValueDelegate (PerformSetValue),
				                      new object [] { value } );
				return;
			}

			if (indexOf == 0)
				listViewItem.Text = value;
			else
				listViewItem.SubItems [indexOf - 1].Text = value;
		}

		#endregion
		
		#region Private Fields

		private SWF.ColumnHeader columnHeader;
		private SWF.ListView listView;
		private SWF.ListViewItem listViewItem;
		
		#endregion
	}
}
