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
// 	Neville Gao <nevillegao@gmail.com>
// 	Brad Taylor <brad@getcoded.net>
// 

using System;
using Mono.Unix;
using System.Reflection;
using SD = System.Drawing;
using System.Windows.Forms;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Collections.Generic;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using System.Windows.Forms.PropertyGridInternal;
using Mono.UIAutomation.Winforms.Events.PropertyGrid;
using Mono.UIAutomation.Winforms.Behaviors.PropertyGrid;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;
using PGI = System.Windows.Forms.PropertyGridInternal;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (PropertyGridView))]
	internal class PropertyGridViewProvider : ListProvider
	{
#region Public Properties
		public override int SelectedItemsCount {
			get { return propertyGrid.SelectedGridItem == null ? 0 : 1; }
		}

		public override int ItemsCount {
			get { return children.Count; }
		}

		public PropertyGrid PropertyGrid {
			get { return propertyGrid; }
		}
#endregion

#region Public Methods
		public PropertyGridViewProvider (PropertyGridView propertyGridView)
			: base (propertyGridView)
		{
			view = propertyGridView;
			propertyGrid = Helper.GetPrivateField<PropertyGrid> (
				typeof (PropertyGridView), view,
				"property_grid");
		}

		public override void UnselectItem (ListItemProvider item)
		{
			// We don't actually support this
		}

		public override void SelectItem (ListItemProvider item)
		{
			GridItem gridItem = item.ObjectItem as GridItem;
			if (gridItem == null)
				return;
			
			propertyGrid.SelectedGridItem = gridItem;
		}

		public override void ScrollItemIntoView (ListItemProvider item)
		{
			// No way in SWF to do this
		}

		public override IRawElementProviderSimple[] GetSelectedItems ()
		{
			foreach (PropertyGridListItemProvider item in children)
				if (IsItemSelected (item))
					return new ListItemProvider[] { item };

			return new ListItemProvider[0];
		}

		public override bool IsItemSelected (ListItemProvider item)
		{
			return (item.ObjectItem == propertyGrid.SelectedGridItem);
		}

		public override int IndexOfObjectItem (object objectItem)
		{
			for (int i = 0; i < children.Count; i++)
				if (children [i].ObjectItem == objectItem)
					return i;

			return -1;
		}
		
		public IRawElementProviderSimple GetItem (int row, int col)
		{
			if (col < 0 || col > 2) {
				return null;
			}

			if (row < 0 || row >= children.Count) {
				return null;
			}

			return children [row];
		}

		public override void FocusItem (object objectItem)
		{
			foreach (PropertyGridListItemProvider item in children)
				if (objectItem == item.ObjectItem)
					SelectItem (item);
		}

		public override object GetItemPropertyValue (ListItemProvider item, int propertyId)
		{
			PropertyGridListItemProvider pgListItem 
				= item as PropertyGridListItemProvider;
			if (pgListItem == null)
				return null;

			if (propertyId == AEIds.NameProperty.Id)
				return pgListItem.Name;
			else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
				return false;
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return null;
		}

		internal override IProviderBehavior GetBehaviorRealization (AutomationPattern behavior)
		{
			if (behavior == SelectionPatternIdentifiers.Pattern)
				return new SelectionProviderBehavior (this);
			else if (behavior == GridPatternIdentifiers.Pattern)
				return new GridProviderBehavior (this);
			else if (behavior == TablePatternIdentifiers.Pattern)
				return new TableProviderBehavior (this);
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
			else if (behavior == GridItemPatternIdentifiers.Pattern)
				return new ListItemGridItemProviderBehavior (listItem);
			else if (behavior == TableItemPatternIdentifiers.Pattern)
				return new ListItemTableItemProviderBehavior (listItem);
			else
				return base.GetListItemBehaviorRealization (behavior, listItem);
		}
#endregion

#region SimpleControlProvider Overrides
		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("data grid");

			return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region FragmentControlProvider Overrides
		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			propertyGrid.SelectedGridItemChanged
				+= OnSelectedGridItemChanged;

			AddChildren ();
		}
		
		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();

			propertyGrid.SelectedGridItemChanged
				-= OnSelectedGridItemChanged;

			RemoveChildren ();
		}
#endregion

#region Protected Properties
		protected override ScrollBar HorizontalScrollBar {
			get { return null; }
		}

		protected override ScrollBar VerticalScrollBar {
			get {
				return Helper.GetPrivateField<ScrollBar> (
					typeof (PropertyGridView), view, "vbar"
				);
			}
		}
#endregion

#region Protected Methods
		protected override ListItemProvider GetNewItemProvider (FragmentRootControlProvider rootProvider,
		                                                        ListProvider provider,
		                                                        Control control,
		                                                        object objectItem)
		{
			return new PropertyGridListItemProvider (
				this, view, (GridEntry) objectItem
			);
		}
#endregion

#region Private Members
		private void AddChildren ()
		{
			GridItem root = propertyGrid.RootGridItem;
			if (root == null)
				return;
			
			foreach (GridItem cat in root.GridItems) {
				if (cat is CategoryGridEntry && cat.GridItems != null && cat.GridItems.Count > 0) {
					var catProvider = new PropertyGridCategoryProvider (this, view, (CategoryGridEntry)cat);
					catProvider.Initialize ();
					children.Add (catProvider);
					AddChildProvider (catProvider);
				}
				
				foreach (GridItem item in cat.GridItems) {
					PropertyGridListItemProvider itemProvider
						= (PropertyGridListItemProvider) GetNewItemProvider (
							this, this, view, item);

					itemProvider.Initialize ();
					children.Add (itemProvider);
					AddChildProvider (itemProvider);
				}
			}
		}

		private void RemoveChildren ()
		{
			foreach (PropertyGridListItemProvider prov
			         in children) {
				RemoveChildProvider (prov);
				prov.Terminate ();
			}

			children.Clear ();
		}

		private void OnSelectedGridItemChanged (object o, SelectedGridItemChangedEventArgs args)
		{
			RemoveChildren ();
			AddChildren ();
		}
#endregion

#region Private Fields
		private PropertyGridView view;
		private PropertyGrid propertyGrid;
		private List<PropertyGridListItemProvider> children
			= new List<PropertyGridListItemProvider> ();
#endregion
	}

	internal class PropertyGridListItemProvider : ListItemProvider
	{
#region Public Properties
		public string Name {
			get { return entry.Label; }
		}

		public string Value {
			get { return entry.ValueText; }
			set {
				string error;
				if (!entry.SetValue (value, out error)) {
					Log.Warn ("PropertyGridListItemProvider: Unable to set value: {0}",
					          error);
				}
			}
		}

		public bool IsReadOnly {
			get { return entry.IsReadOnly && entry.IsEditable; }
		}

		public PropertyGridView PropertyGridView {
			get { return view; }
		}
		
		public PropertyGridViewProvider PropertyGridViewProvider {
			get { return viewProvider; }
		}

		public GridEntry Entry {
			get { return entry; }
		}
#endregion

#region Public Methods
		public PropertyGridListItemProvider (PropertyGridViewProvider viewProvider,
		                                     PropertyGridView view,
		                                     GridEntry entry) 
			: base (viewProvider, viewProvider, view, entry)
		{
			this.viewProvider = viewProvider;
			this.view = view;
			this.entry = entry;

			this.viewProvider.ToString ();
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();
			
			nameProvider = new PropertyGridListItemNameProvider (this, view);
			nameProvider.Initialize ();
			AddChildProvider (nameProvider);
		
			valueProvider = new PropertyGridListItemValueProvider (this, view);
			valueProvider.Initialize ();
			AddChildProvider (valueProvider);
		}

		public override void FinalizeChildControlStructure ()
		{
			base.FinalizeChildControlStructure ();
			
			nameProvider.Terminate ();
			RemoveChildProvider (nameProvider);
			
			valueProvider.Terminate ();
			RemoveChildProvider (valueProvider);
		}
#endregion

#region Protected Methods
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.DataItem.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("data item");
			else
				return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region Private Fields
		private PropertyGridListItemChildProvider nameProvider;
		private PropertyGridListItemValueProvider valueProvider;

		private PropertyGridViewProvider viewProvider;
		private PropertyGridView view;
		private GridEntry entry;
#endregion
	}

	internal abstract class PropertyGridListItemChildProvider
		: FragmentRootControlProvider
	{
		public abstract int Column {
			get;
		}

		public PropertyGridListItemChildProvider (PropertyGridListItemProvider itemProvider,
		                                          PropertyGridView view)
			: base (null)
		{
			this.itemProvider = itemProvider;
			this.propertyGrid = itemProvider.PropertyGridViewProvider.PropertyGrid;
			this.view = view;
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return itemProvider; }
		}

		public PropertyGridListItemProvider ListItemProvider {
			get { return itemProvider; }
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Edit.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("edit");
			else if (propertyId == AEIds.BoundingRectangleProperty.Id) {
				SD.Rectangle bounds = GetBounds ();
				return Helper.GetControlScreenBounds (bounds, view, true);
			} else if (propertyId == AEIds.IsOffscreenProperty.Id) {
				SD.Rectangle controlRect = view.Bounds;
				controlRect.X = controlRect.Y = 0;
				return !controlRect.IntersectsWith (GetBounds ());
			}

			return base.GetProviderPropertyValue (propertyId);
		}

		// TODO: Replace this with an internal property
		private int RowHeight {
			get {
				return Helper.GetPrivateField<int> (
					typeof (PropertyGridView), view,
					"row_height"
				);
			}
		}

		// TODO: Replace this with an internal property
		private ScrollBar VerticalScrollBar {
			get {
				return Helper.GetPrivateField<ScrollBar> (
					typeof (PropertyGridView), view, "vbar"
				);
			}
		}

		// TODO: Replace this with an internal property
		private int SplitterLocation {
			get {
				return Helper.GetPrivateProperty<PropertyGridView, int> (
					view, "SplitterLocation"
				);
			}
		}

		private SD.Rectangle GetBounds ()
		{
			GridEntry entry = itemProvider.Entry;

			int x = SplitterLocation + ENTRY_SPACING + (entry.PaintValueSupported ? VALUE_PAINT_INDENT : 0);
			int y = (-1 * VerticalScrollBar.Value) * RowHeight;

			MethodInfo mi = typeof (PropertyGridView).GetMethod ("CalculateItemY",
									     BindingFlags.NonPublic
									     | BindingFlags.Instance);
			if (mi == null) {
				Log.Warn ("Unable to find CalculateItemY method");
				return new SD.Rectangle ();
			}

			object[] args = new object[] {
				entry, propertyGrid.RootGridItem.GridItems, y
			};

			// CalculateItemY (entry, items, ref y)
			mi.Invoke (view, args);
			
			// y is a ref param
			y = (int) args[2];

			SD.Rectangle controlRect = view.Bounds;
			int width = (Column == 0) ? SplitterLocation
						  : controlRect.Width - VerticalScrollBar.Bounds.Width - x;

			return new SD.Rectangle (
				Column * x, y, width, RowHeight
			);
		}

		public PropertyGridListItemProvider itemProvider;
		public PropertyGrid propertyGrid;
		public PropertyGridView view;

		// TODO: Replace these with an internal property
		private const int ENTRY_SPACING = 2;
		private const int VALUE_PAINT_INDENT = 27;
	}

	internal class PropertyGridListItemNameProvider
		: PropertyGridListItemChildProvider
	{
		public override int Column {
			get { return 0; }
		}
		
		public PropertyGridListItemNameProvider (PropertyGridListItemProvider itemProvider,
		                                         PropertyGridView view)
			: base (itemProvider, view)
		{
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (ValuePatternIdentifiers.Pattern,
			             new ListItemNameValueProviderBehavior (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id)
				return itemProvider.Name;

			return base.GetProviderPropertyValue (propertyId);
		}
	}
	
	internal class PropertyGridListItemValueProvider
		: PropertyGridListItemChildProvider
	{
		public override int Column {
			get { return 1; }
		}

		public PropertyGridListItemValueProvider (PropertyGridListItemProvider itemProvider,
		                                          PropertyGridView view)
			: base (itemProvider, view)
		{
		}

		public override void Initialize ()
		{
			base.Initialize ();

			SetBehavior (ValuePatternIdentifiers.Pattern,
			             new ListItemChildValueProviderBehavior (this));

			SetEvent (ProviderEventType.AutomationElementNameProperty,
			          new ListItemChildValueAutomationNamePropertyEvent (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id)
				return itemProvider.Value;

			return base.GetProviderPropertyValue (propertyId);
		}
	}
	
	internal class PropertyGridCategoryProvider : PropertyGridListItemProvider
	{
		internal PGI.CategoryGridEntry entry;
		
		internal PropertyGridCategoryProvider (PropertyGridViewProvider prov, PropertyGridView grid, PGI.CategoryGridEntry entry)
			: base (prov, grid, entry)
		{
			this.entry = entry;
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (InvokePatternIdentifiers.Pattern, new CategoryInvokeProviderBehavior (this));
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.NameProperty.Id)
				return entry.Label;
			else if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.Custom.Id;

			return base.GetProviderPropertyValue (propertyId);
		}
	}
}
