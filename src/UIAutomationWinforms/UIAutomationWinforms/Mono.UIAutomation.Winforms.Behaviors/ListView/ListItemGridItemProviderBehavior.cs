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
	internal class ListItemGridItemProviderBehavior
		: ProviderBehavior, IGridItemProvider
	{
		#region Constructors
		
		public ListItemGridItemProviderBehavior (ListItemProvider itemProvider)
			: base (itemProvider)
		{
			this.itemProvider = itemProvider;
			viewProvider = (ListViewProvider) itemProvider.ListProvider;
			view = (SWF.ListView) viewProvider.Control;

			//We need to keep a reference to Group because when removed the 
			//group is set to null and we need to update the values.
			group = ((SWF.ListViewItem) itemProvider.ObjectItem).Group;
			if (group == null)
				group = viewProvider.GetDefaultGroup ();
		}
		
		#endregion
		
		#region ProviderBehavior Specialization
		
		public override AutomationPattern ProviderPattern {
			get { return GridItemPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			// NOTE: RowSpan Property NEVER changes.
			// NOTE: ColumnSpan Property NEVER changes.
			// NOTE: ContainingGrid Property NEVER changes.			
			Provider.SetEvent (ProviderEventType.GridItemPatternRowProperty,
			                   new ListItemGridItemPatternRowEvent ((ListItemProvider) Provider));
			Provider.SetEvent (ProviderEventType.GridItemPatternColumnProperty,
			                   new ListItemGridItemPatternColumnEvent ((ListItemProvider) Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.GridItemPatternRowProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridItemPatternColumnProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridItemPatternRowSpanProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridItemPatternColumnProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.GridItemPatternContainingGridProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == GridItemPatternIdentifiers.RowProperty.Id)
				return Row;
			else if (propertyId == GridItemPatternIdentifiers.ColumnProperty.Id)
				return Column;
			else if (propertyId == GridItemPatternIdentifiers.RowSpanProperty.Id)
				return RowSpan;
			else if (propertyId == GridItemPatternIdentifiers.ColumnSpanProperty.Id)
				return ColumnSpan;
			else if (propertyId == GridItemPatternIdentifiers.ContainingGridProperty.Id)
				return ContainingGrid;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		#endregion
		
		#region IGridItemProvider Specialization
		
		public int Row {
			get { 
				SWF.ListView view = (SWF.ListView) viewProvider.Control;

				if (view.View == SWF.View.List) //From Top to Bottom
					return MaxRows == 0 ? -1 : IndexOf % MaxRows;					
				else //From Left to Right
					return MaxColumns == 0 ? -1 : IndexOf / MaxColumns;
			}
		}
		
		public int Column {
			get {
				SWF.ListView view = (SWF.ListView) viewProvider.Control;

				if (view.View == SWF.View.List) //From Top to Bottom
					return MaxRows == 0 ? -1 : IndexOf / MaxRows; 
				else //From Left to Right
					return IndexOf - (Row * MaxColumns); 
			}
		}
		
		public int RowSpan {
			get { return 1; }
		}
		
		public int ColumnSpan {
			get { return 1; }
		}
		
		public IRawElementProviderSimple ContainingGrid {
			get { return Provider.FragmentRoot; }
		}
		
		#endregion
		
		#region Private Methods
		
		private int IndexOf {
			get {
				SWF.ListViewItem item = (SWF.ListViewItem) itemProvider.ObjectItem;

				if (view.View == SWF.View.List || view.ShowGroups == false)
					return view.Items.IndexOf (item);

				if (viewProvider.IsDefaultGroup (group) == true) {					
					int indexOf = 0;
					bool found = false;					
					
					//TODO: Is this OK??
					for (int index = 0; index < view.Items.Count; index++) {
						if (view.Items [index].Group == null) {
							if (view.Items [index] == item) {
								found = true;
								break;
							}
							indexOf++;
						}
					}
					
					return found == false ? -1 : indexOf;
				} else
					return group.Items.IndexOf (item);
			}
		}
		
		private int MaxColumns {
			get {
				return Helper.GetPrivateProperty<SWF.ListView, int> (typeof (SWF.ListView),
				                                                     (SWF.ListView) viewProvider.Control,
				                                                     "UIAColumns");
			}
		}

		private int MaxRows {
			get {
				return Helper.GetPrivateProperty<SWF.ListView, int> (typeof (SWF.ListView),
				                                                     (SWF.ListView) viewProvider.Control,
				                                                     "UIARows");
			}
		}
		
		#endregion
		
		#region Private Fields

		private SWF.ListViewGroup group;
		private SWF.ListView view;
		private ListItemProvider itemProvider;
		private ListViewProvider viewProvider;
		
		#endregion
		
	}
}
