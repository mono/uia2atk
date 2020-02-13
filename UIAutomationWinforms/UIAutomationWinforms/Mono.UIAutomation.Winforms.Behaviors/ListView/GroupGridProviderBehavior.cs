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

	internal class GroupGridProviderBehavior
		: ProviderBehavior, IGridProvider
	{
		
		#region Constructors

		public GroupGridProviderBehavior (ListViewProvider.ListViewGroupProvider provider)
			: base (provider)
		{
			viewGroupProvider = provider;
		}

		#endregion
		
		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return GridPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.GridItemPatternRowProperty,
			                   new GroupGridPatternRowEvent (viewGroupProvider));
			Provider.SetEvent (ProviderEventType.GridItemPatternColumnProperty,
			                   new GroupGridPatternColumnEvent (viewGroupProvider));
		}
		
		public override void Disconnect ()
		{	
			Provider.SetEvent (ProviderEventType.GridPatternColumnCountProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridPatternRowCountProperty,
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
		
		public IRawElementProviderSimple GetItem (int row, int column)
		{
			//According to http://msdn.microsoft.com/en-us/library/ms743401.aspx
			int rowCount = RowCount;
			int columnCount = ColumnCount;

			if (row >= rowCount || column >= columnCount || row < 0 || column < 0)
			    throw new ArgumentOutOfRangeException ();

			return viewGroupProvider.Navigation.GetVisibleChild ((row * columnCount) + column);
		}
		
		public int ColumnCount {
			get {
				if (viewGroupProvider.ListView.View == SWF.View.Details)
					return viewGroupProvider.ListView.Columns.Count;
				else {
					int maxColums = viewGroupProvider.ListView.UIAColumns;
					int itemsInGroup = ItemsInGroupCount ();
	
					if (itemsInGroup < maxColums)
						return itemsInGroup;
					else
						return maxColums;
				}
			}
		}
		
		public int RowCount {
			get {
				int columnCount = ColumnCount;

				if (columnCount == 0)
					return -1;
				else {
					int itemsCount = ItemsInGroupCount ();
					int rowCount = itemsCount / columnCount;
					int module = itemsCount % columnCount;
					if (module > 0)
						rowCount++;

					return rowCount;
				}
			}
		}
		
		#endregion

		#region Private Methods

		private int ItemsInGroupCount ()
		{
			ListViewProvider listViewProvider
				= (ListViewProvider) ProviderFactory.FindProvider (viewGroupProvider.ListView);
			if (listViewProvider.IsDefaultGroup (viewGroupProvider.Group) == true) {
				int itemsInGroup = 0;
				for (int index = 0; index < viewGroupProvider.ListView.Items.Count; index++) {
					if (viewGroupProvider.ListView.Items [index].Group == null)
						itemsInGroup++;
				}
				return itemsInGroup;
			} else
				return viewGroupProvider.Group.Items.Count;
		}

		#endregion

		#region Private Fields

		private ListViewProvider.ListViewGroupProvider viewGroupProvider;

		#endregion

	}
				                      
}
