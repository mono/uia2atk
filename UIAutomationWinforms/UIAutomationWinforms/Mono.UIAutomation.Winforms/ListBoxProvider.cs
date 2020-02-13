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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListBox;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ListBox;
using Mono.UIAutomation.Winforms.Navigation;
using Mono.UIAutomation.Services;

namespace Mono.UIAutomation.Winforms
{

	[MapsComponent (typeof (ListBox))]
	internal class ListBoxProvider : ListProvider
	{		
	
		#region Constructor 

		public ListBoxProvider (ListBox listbox) : base (listbox)
		{
			listboxControl = listbox;
		}

		#endregion

		#region Scroll Methods and Properties

		protected override ScrollBar HorizontalScrollBar { 
			get { return listboxControl.UIAHScrollBar; }
		}

		protected override ScrollBar VerticalScrollBar { 
			get { return listboxControl.UIAVScrollBar; }
		}

		#endregion 
		
		#region Public Methods
		
		public ScrollBar GetInternalScrollBar (Orientation orientation)
		{
			if (orientation == Orientation.Horizontal)
				return HorizontalScrollBar;
			else
				return VerticalScrollBar;
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.List.Id;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id) {
				if (string.IsNullOrEmpty (listboxControl.AccessibleName))
					return Helper.StripAmpersands (listboxControl.Text);
				else
					return listboxControl.AccessibleName;
			} else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion
		
		#region FragmentControlProvider: Specializations
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			//TODO
			Log.Warn ("ListBoxProvider:ElementProviderFromPoint not implemented");
			return null;
		}
		
		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override IRawElementProviderFragment GetFocus ()
		{
			return GetItemProviderFrom (this, listboxControl.Items [listboxControl.FocusedItem]);
		}
		
		public override void FocusItem (object objectItem)
		{
			listboxControl.FocusedItem = listboxControl.Items.IndexOf (objectItem);
		}

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			listboxControl.Items.UIACollectionChanged += OnCollectionChanged;
			
			foreach (object objectItem in listboxControl.Items) {
				ListItemProvider item = GetItemProviderFrom (this, objectItem);
				AddChildProvider (item);
			}
		}
		
		protected override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();

			listboxControl.Items.UIACollectionChanged -= OnCollectionChanged;
		}
		
		#endregion
		
		#region ListItem: Properties Methods
		
		public override object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return listboxControl.GetItemText (item.ObjectItem);
			
			if (ContainsItem (item) == false)
				return null;

			if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return listboxControl.Focused && item.Index == listboxControl.FocusedItem;
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				System.Drawing.Rectangle itemRec = System.Drawing.Rectangle.Empty;
				System.Drawing.Rectangle rectangle = listboxControl.Bounds;

				itemRec = listboxControl.GetItemRectangle (item.Index);
				itemRec.X += rectangle.X;
				itemRec.Y += rectangle.Y;
				
				if (listboxControl.FindForm () == listboxControl.Parent)
					itemRec = listboxControl.TopLevelControl.RectangleToScreen (itemRec);
				else
					itemRec = listboxControl.Parent.RectangleToScreen (itemRec);
	
				return Helper.RectangleToRect (itemRec);
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return Helper.IsListItemOffScreen ((Rect) item.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id),
				                                   listboxControl,
				                                   false,
				                                   System.Drawing.Rectangle.Empty,
				                                   ScrollBehaviorObserver);
			else
				return null;
		}
		
		#endregion
		
		#region ListProvider: Specializations
		
		public override int SelectedItemsCount { 
			get { return listboxControl.SelectedItems.Count; }
		}
		
		public override int ItemsCount {
			get { return listboxControl.Items.Count; }
		}

		public override int IndexOfObjectItem (object objectItem)
		{
			return listboxControl.Items.IndexOf (objectItem);
		}
		
		public override IRawElementProviderSimple[] GetSelectedItems ()
		{
			ListItemProvider []items;

			if (listboxControl == null || listboxControl.SelectedIndices.Count == 0)
				return new ListItemProvider [0];
			
			items = new ListItemProvider [listboxControl.SelectedIndices.Count];		
			for (int index = 0; index < items.Length; index++) 
				items [index] = GetItemProviderFrom (this, listboxControl.Items [listboxControl.SelectedIndices [index]]);
			
			return items;
		}
		
		public override void SelectItem (ListItemProvider item)
		{
			listboxControl.SetSelected (item.Index, true);
		}

		public override void UnselectItem (ListItemProvider item)
		{
			listboxControl.SetSelected (item.Index, false);
		}
		
		public override bool IsItemSelected (ListItemProvider item)
		{
			return listboxControl.SelectedIndices.Contains (item.Index);
		}

		public override IConnectable GetListItemEventRealization (ProviderEventType eventType, 
		                                                          ListItemProvider provider)
		{
			if (eventType == ProviderEventType.AutomationElementHasKeyboardFocusProperty)
			    return new ListItemAutomationHasKeyboardFocusPropertyEvent (provider);
			else if (eventType == ProviderEventType.AutomationElementHasKeyboardFocusProperty)
			    return new ListItemAutomationIsOffscreenPropertyEvent (provider);
			else if (eventType == ProviderEventType.AutomationElementNameProperty)
			    return new ListItemAutomationIsOffscreenPropertyEvent (provider);
			else
				return base.GetListItemEventRealization (eventType, provider);
		}

		public override void ScrollItemIntoView (ListItemProvider item)
		{
			if (ContainsItem (item) == true)
				listboxControl.TopIndex = item.Index;
		}
		
		#endregion
		
		#region Internal Methods: Get Behaviors
		
		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (behavior == SelectionPatternIdentifiers.Pattern)
				return new SelectionProviderBehavior (this);
			else if (behavior == ScrollPatternIdentifiers.Pattern)
				return new ScrollProviderBehavior (this);
			else 
				return null;
		}		
		
		public override IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                  ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new ListItemSelectionItemProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}
		
		#endregion
		
		#region Private Fields
		
		private ListBox listboxControl;
		
		#endregion
	}

}
