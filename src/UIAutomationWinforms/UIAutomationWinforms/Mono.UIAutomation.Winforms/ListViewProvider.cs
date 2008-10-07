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
using Mono.UIAutomation.Winforms.Behaviors.ListView;

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
		
		#region ListProvider: Internal Methods: Get Behaviors
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (MultipleViewPatternIdentifiers.Pattern == behavior)
				return new MultipleViewProviderBehavior (this);
			else if (GridPatternIdentifiers.Pattern == behavior) {         
				if (listView.View == SWF.View.Details || listView.View == SWF.View.List)
					return null; //TODO: Return realization
				else
					return null;
			} else
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
		
		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			try {
				Helper.AddPrivateEvent (typeof (SWF.ListView), 
				                        listView,
				                        "UIAViewChanged",
				                        this, 
				                        "OnUIAViewChanged");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: UIAViewChanged not defined", GetType ());
			}			
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			try {
				Helper.RemovePrivateEvent (typeof (SWF.ListView), 
				                           listView,
				                           "UIAViewChanged",
				                           this, 
				                           "OnUIAViewChanged");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: UIAViewChanged not defined", GetType ());
			}			
		}
		
		public override int ItemsCount { 
			get { return listView.Items.Count; }
		}
		
		#endregion
		
		#region ListItem: Selection Methods and Properties
		
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
		
		#endregion
		
		public override IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider)
		{
			return null;
		}
		
		#region ListProvider: ListItem: Scroll Methods
		
		public override void ScrollItemIntoView (ListItemProvider item)
		{

		}
		
		#endregion
		
		#region ListProvider: Protected Methods
		
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
		
		#endregion
		
		#region Private Methods
		
		private void OnUIAViewChanged (object sender, EventArgs args)
		{
			
			
			// Selection Pattern always supported
			// Scroll Pattern depends on visible/enable scrollbars

			if (listView.View == SWF.View.Details || listView.View == SWF.View.List)
				SetBehavior (GridPatternIdentifiers.Pattern,
				             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			else
				SetBehavior (GridPatternIdentifiers.Pattern,
				             null);
			
			
			//SmallIcon = MultipleView, Selection
			//LargeIcon = Multipleview, Scroll, Selection
			//Tile: MultipleView, Scroll, Selection
			//---
			//Details = MultipleView, Scroll, Selection, Grid
			//List = MultipleView, Scroll, Selection, Grid
		}
		
		#endregion
		
		#region Private Fields
		
		private SWF.ListView listView;
		
		#endregion
		
	}
}
