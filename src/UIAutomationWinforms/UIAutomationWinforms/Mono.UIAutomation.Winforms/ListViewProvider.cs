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
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{
	
	internal class ListViewProvider : ListProvider
	{
		
		#region Constructors

		public ListViewProvider (SWF.ListView listView) : base (listView)
		{
			this.listView = listView;
		}
		
		#endregion
		
		#region 
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			return null;
		}
		
		#endregion
		
		#region ListItem: Properties Methods
		
		public override object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			return null;
		}
		
		#endregion 
		
		#region ListProvider specializations
		
		public override int ItemsCount { 
			get { return listView.Items.Count; }
		}
		
		public override int SelectedItemsCount { 
			get { return listView.SelectedItems.Count; }
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return false;
		}
		
		public override ListItemProvider[] GetSelectedItems ()
		{
			return null;
		}
		
		public override void SelectItem (ListItemProvider item)
		{
		}

		public override void UnselectItem (ListItemProvider item)
		{
		}
		
		public override IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider)
		{
			return null;
		}
		
		protected override Type GetTypeOfObjectCollection ()
		{
			//return typeof (SWF.ListView.ListViewItemCollection);
			return null;
		}
		
		protected override object GetInstanceOfObjectCollection ()	
		{
			//return listView.Items;
			return null;
		}
		
		public override void ScrollItemIntoView (ListItemProvider item)
		{

		}
		
		#endregion
		
		#region Private Methods
		
		private SWF.ListView listView;
		
		#endregion
		
	}
}
