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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListView;

namespace Mono.UIAutomation.Winforms.Behaviors.ListView
{
	internal class GridProviderBehavior 
		: ProviderBehavior, IGridProvider
	{
		
		#region Constructors

		public GridProviderBehavior (ListViewProvider provider)
			: base (provider)
		{
			listView = (SWF.ListView) Provider.Control;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return GridPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   new GridPatternColumnEvent ((ListViewProvider) Provider));
			Provider.SetEvent (ProviderEventType.GridPatternRowCountProperty,
			                   new GridPatternRowEvent ((ListViewProvider) Provider));
			Provider.SetEvent (ProviderEventType.GridPatternColumnReorderedEvent,
			                   new GridPatternColumnReorderedEvent ((ListViewProvider) Provider));
		}
		
		public override void Disconnect ()
		{	
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridPatternRowCountProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridPatternColumnReorderedEvent,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == GridPatternIdentifiers.ColumnCountProperty.Id)
				return ColumnCount;
			else if (propertyId == GridPatternIdentifiers.RowCountProperty.Id)
				return RowCount;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion

		#region IGridProvider implementation

		public int ColumnCount {
			get {
				if (listView.View == SWF.View.Details)
					return listView.Columns.Count;
				else { //Is View.List
					if (RowCount == 0)
						return -1;

					bool module = listView.Items.Count % RowCount > 0;
					return (listView.Items.Count / RowCount) + (module ? 1 : 0);
				}
			}
		}

		public int RowCount {
			get {
				if (listView.View == SWF.View.Details)
					return listView.Items.Count;
				else //Is View.List
					return listView.UIARows; 
			}
		}

		public IRawElementProviderSimple GetItem (int row, int column)
		{
			int rowCount = RowCount;
			int columnCount = ColumnCount;

			//According to http://msdn.microsoft.com/en-us/library/ms743401.aspx
			if (row < 0 || column < 0 || row >= rowCount || column >= columnCount)
				throw new ArgumentOutOfRangeException ();

			var provider = (ListViewProvider) Provider;

			if (listView.View == SWF.View.Details) {
				// FIXME: In Vista when listView.Groups == 0 no Groups are added,
				// and we should iterate when listView.Groups > 0
				SWF.ListViewItem item = listView.Items [row];
				ListViewProvider.ListViewListItemProvider itemProvider = null;
				if ((listView.Groups == null || listView.Groups.Count == 0) && !listView.ShowGroups)
					itemProvider = provider.GetItem (item);
				else {
					ListViewProvider.ListViewGroupProvider groupProvider
						= provider.GetGroupProviderFrom (provider.GetGroupFrom (item));
					itemProvider
						= (ListViewProvider.ListViewListItemProvider) groupProvider.GetItem (item);
				}
				return itemProvider.GetEditProviderAtColumn (column);
			} else { //Is View.List
				return provider.Navigation.GetVisibleChild ((column * rowCount) + row);
			}
		}
		
		#endregion

		#region

		private SWF.ListView listView;

		#endregion
	}
}
