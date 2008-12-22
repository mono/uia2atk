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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.DataGrid;

namespace Mono.UIAutomation.Winforms
{

	internal class DataGridProvider 
		: FragmentRootControlProvider, IListProvider, IScrollBehaviorSubject
	{
		#region Public Constructors
		
		public DataGridProvider (SWF.DataGrid datagrid) : base (datagrid)
		{
			this.datagrid = datagrid;
			items = new Dictionary<object, ListItemProvider> ();
		}

		#endregion

		#region Public Properties

		public SWF.DataGridTableStyle CurrentTableStyle {
			get {
				// FIXME: Remove reflection after patch applied
				return Helper.GetPrivateProperty<SWF.DataGrid, SWF.DataGridTableStyle> (typeof (SWF.DataGrid),
				                                                                        datagrid,
				                                                                        "UIACurrentTableStyle");
			}
		}

		public SWF.CurrencyManager CurrencyManager {
			get { return lastCurrencyManager; }
		}		

		public SWF.DataGrid DataGrid {
			get { return datagrid; }
		}

		public DataGridHeaderProvider HeaderProvider {
			get { return header; }
		}

		#endregion

		#region Public Methods

		public IRawElementProviderSimple[] GetSelectedItems ()
		{
			if (SelectedItemsCount == 0)
				return new IRawElementProviderSimple [0];
			else {
				List<IRawElementProviderSimple> selection = new List<IRawElementProviderSimple> ();
				foreach (KeyValuePair<object, ListItemProvider> pair in items) {
					if (IsItemSelected (pair.Value))
						selection.Add (pair.Value);
				}
				return selection.ToArray ();
			}
		}

		public IRawElementProviderSimple GetChildProviderAt (int row, int column)
		{
			int rowCount = CurrencyManager == null ? 0 : CurrencyManager.Count;
			int columnCount = CurrentTableStyle == null ? 0 : CurrentTableStyle.GridColumnStyles.Count;

			//According to http://msdn.microsoft.com/en-us/library/ms743401.aspx
			if (row < 0 || column < 0 || row >= rowCount || column >= columnCount)
			    throw new ArgumentOutOfRangeException ();

			object objectcell = datagrid [row, 0];
			ListItemProvider itemProvider;
			if (items.TryGetValue (objectcell, out itemProvider))
				return itemProvider.GetChildProviderAt (column);
			else
				return null;
		}

		#endregion

		#region IScrollBehaviorSubject specialization

		public IScrollBehaviorObserver ScrollBehaviorObserver { 
			get { return observer; }
		}
		
		public FragmentControlProvider GetScrollbarProvider (SWF.ScrollBar scrollbar)
		{
			return new DataGridScrollBarProvider (scrollbar, this);
		}

		#endregion

		#region SimpleControlProvider specialization

		public override void Initialize ()
		{
			base.Initialize ();

			try {
				SWF.ScrollBar vscrollbar
					= Helper.GetPrivateProperty<SWF.DataGrid, SWF.ScrollBar> (typeof (SWF.DataGrid),
					                                                          datagrid,
					                                                          "UIAVScrollBar");
				SWF.ScrollBar hscrollbar 
					= Helper.GetPrivateProperty<SWF.DataGrid, SWF.ScrollBar> (typeof (SWF.DataGrid),
					                                                          datagrid,
					                                                          "UIAHScrollBar");

				//ListScrollBehaviorObserver updates Navigation
				observer = new ScrollBehaviorObserver (this, hscrollbar, vscrollbar);			
				observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
				UpdateScrollBehavior ();
			} catch (NotSupportedException) { }

			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new SelectionProviderBehavior (this));
			SetBehavior (GridPatternIdentifiers.Pattern,
			             new GridProviderBehavior (this));
			// Table Pattern is *only* supported when Header exists
		}

		public override void Terminate ()
		{
			base.Terminate ();

			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}

		public override void InitializeChildControlStructure ()
		{
			datagrid.DataSourceChanged += OnDataSourceChanged;
			UpdateChildren (false);

			try {
				Helper.AddPrivateEvent (typeof (SWF.DataGrid),
				                        datagrid,
				                        "UIACollectionChanged",
				                        this,
				                        "OnUIACollectionChanged");

				Helper.AddPrivateEvent (typeof (SWF.DataGrid),
				                        datagrid,
				                        "UIAColumnHeadersVisibleChanged",
				                        this,
				                        "OnUIAColumnHeadersVisibleChanged");
			} catch (NotSupportedException) {}
		}

		public override void FinalizeChildControlStructure ()
		{
			OnNavigationChildrenCleared (false);
			datagrid.DataSourceChanged -= OnDataSourceChanged;

			try {
				Helper.RemovePrivateEvent (typeof (SWF.DataGrid),
				                           datagrid,
				                           "UIACollectionChanged",
				                           this,
				                           "OnUIACollectionChanged");

				Helper.RemovePrivateEvent (typeof (SWF.DataGrid),
				                           datagrid,
				                           "UIAColumnHeadersVisibleChanged",
				                           this,
				                           "OnUIAColumnHeadersVisibleChanged");
			} catch (NotSupportedException) {}
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			// NOTDOTNET: Using DataGrid instead of Table
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "datagrid";
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		#endregion

		#region ScrollBehaviorObserver Methods
		
		private void OnScrollPatternSupportChanged (object sender, EventArgs args)
		{
			UpdateScrollBehavior ();
		}
		
		private void UpdateScrollBehavior ()
		{
			if (observer.SupportsScrollPattern == true)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			else
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
		}
		
		#endregion

		#region IListProvider realization

		public int IndexOfObjectItem (object objectItem)
		{
			if (lastCurrencyManager == null)
				return -1;

			for (int index = 0; index < lastCurrencyManager.Count; index++) {
				if (datagrid [index, 0] == objectItem)
					return index;
			}

			return -1;
		}

		public void FocusItem (object objectItem)
		{
			// FIXME: Implement
		}

		public int SelectedItemsCount {
			get { 
				int selectedRows = 0;
				try {
					selectedRows 
						= Helper.GetPrivateProperty<SWF.DataGrid, int> (typeof (SWF.DataGrid),
						                                                datagrid,
						                                                "UIASelectedRows");
				} catch (NotSupportedException) {}
	
				return selectedRows;
			}
		}

		public void ScrollItemIntoView (ListItemProvider item)
		{
			// FIXME: Implement:
		}

		public bool IsItemSelected (ListItemProvider item)
		{
			if (!items.ContainsValue (item))
				return false;
			
			return datagrid.IsSelected (item.Index);
		}

		public void SelectItem (ListItemProvider item)
		{
			if (items.ContainsValue (item))
				datagrid.Select (item.Index);
		}

		public void UnselectItem (ListItemProvider item)
		{
			if (items.ContainsValue (item))
				datagrid.UnSelect (item.Index);
		}

		public object GetItemPropertyValue (ListItemProvider item,
		                                    int propertyId)
		{
			// FIXME: Implement at least:
			// - AutomationElementIdentifiers.HasKeyboardFocusProperty.Id
			// - AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
				int index = item.Index;
				// We need to union last column and first column
				SD.Rectangle rect0 = datagrid.GetCellBounds (index, 0);
				SD.Rectangle rectN = datagrid.GetCellBounds (index, header.ChildrenCount);
				return Helper.GetControlScreenBounds (SD.Rectangle.Union (rect0, rectN), datagrid);
			} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				Rect bounds 
					= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				return Helper.IsOffScreen (bounds, datagrid, true);
			} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return Helper.GetClickablePoint (this);
			else
				return null;
		}

		public ToggleState GetItemToggleState (ListItemProvider item)
		{
			// FIXME: Implement
			return ToggleState.Indeterminate;
		}
		
		public void ToggleItem (ListItemProvider item)
		{
			// FIXME: Implement
		}

		public IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                         ListItemProvider listItem)
		{
			if (behavior == SelectionItemPatternIdentifiers.Pattern)
				return new ListItemSelectionItemProviderBehavior (listItem);
			else
				return null;
		}

		public IConnectable GetListItemEventRealization (ProviderEventType eventType, 
		                                                 ListItemProvider provider)
		{
			return null;
		}
		
		#endregion

		#region Private Methods

#pragma warning disable 169

		private void OnUIACollectionChanged (object sender, CollectionChangeEventArgs args)
		{
			if (lastCurrencyManager == null || args.Action == CollectionChangeAction.Refresh)
				return;

			// FIXME: Remove CurrentTableStyle property after committing SWF patch
			SWF.DataGridTableStyle tableStyle = CurrentTableStyle;
			CreateHeader (true, tableStyle);
			
			if (args.Action == CollectionChangeAction.Add) {
				if (tableStyle.GridColumnStyles.Count == 0)
					return;

				// Usually rows are added at end.
				for (int index = lastCurrencyManager.Count - 1; index >= 0; index--) {
					object datagridcell = datagrid [index, 0];
					if (!items.ContainsKey (datagridcell))
						CreateListItem (true, 
						                index, 
						                tableStyle);
				}
			} else if (args.Action == CollectionChangeAction.Remove) {
				// TODO: Is there a better way to do this?

				Dictionary<object, ListItemProvider> newItems 
					= new Dictionary<object, ListItemProvider> ();
				for (int index = 0; index < lastCurrencyManager.Count; index++) {
					object datagridcell = datagrid [index, 0];
					newItems [datagridcell] = items [datagridcell];
					items.Remove (datagridcell);
				}

				foreach (ListItemProvider item in items.Values)
					OnNavigationChildRemoved (true, item);

				items = newItems;
			}
		}

		private void OnUIAColumnHeadersVisibleChanged (object sender, EventArgs args)
		{
			if (datagrid.ColumnHeadersVisible)
				CreateHeader (true, CurrentTableStyle);
			else {
				OnNavigationChildRemoved (true, header);
				header.Terminate ();
				header = null;

				SetBehavior (TablePatternIdentifiers.Pattern, null);
			}
		}

#pragma warning restore 169

		private void UpdateChildren (bool raiseEvent)
		{
			if (lastDataSource != null) {
				OnNavigationChildrenCleared (raiseEvent);
				items.Clear ();
			}

			if (header != null) {
				header.Terminate ();
				header = null;
			}

			if (lastCurrencyManager == null) { 
				// First time, otherwise we can't do anything.
				lastCurrencyManager = RequestCurrencyManager ();
				if (lastCurrencyManager == null)
					return;
			}

			// FIXME: Remove CurrentTableStyle property after committing SWF patch
			SWF.DataGridTableStyle tableStyle = CurrentTableStyle;
			
			// Is showing "+" to expand, this usually happens when DataSource is
			// DataSet and has more than one DataTable.
			if (tableStyle.GridColumnStyles.Count == 0) {
				Console.WriteLine ("Should ADD custom item that supports Invoke");
				DataGridCustomProvider customProvider 
					= new DataGridCustomProvider (this, 0, string.Empty);
				customProvider.Initialize ();
				OnNavigationChildAdded (raiseEvent, customProvider);
			} else {
				CreateHeader (raiseEvent, tableStyle);

				for (int row = 0; row < lastCurrencyManager.Count; row++)
					CreateListItem (raiseEvent, row, tableStyle);
			}

			lastDataSource = datagrid.DataSource;
		}
		
		private void OnDataSourceChanged (object sender, EventArgs args)
		{
			bool refreshChildren = false;
			SWF.CurrencyManager manager = null;

			// Happens when Navigating DataGrid
			if (lastDataSource == datagrid.DataSource) {
				manager = RequestCurrencyManager ();
				// Only when rendering something different we refresh children
				if (manager != null && manager != lastCurrencyManager)
					refreshChildren = true;
			} else {
				manager = RequestCurrencyManager ();
				refreshChildren = true;
			}

			Console.WriteLine ("> UIA: DataSource changed. Refreshing {0}", refreshChildren);
			if (refreshChildren) {
				lastCurrencyManager = manager;
				UpdateChildren (true);
			}
		}

		private SWF.CurrencyManager RequestCurrencyManager () 
		{
			return (SWF.CurrencyManager) datagrid.BindingContext [datagrid.DataSource,
			                                                      datagrid.DataMember];
		}

		private void CreateHeader (bool raiseEvent, SWF.DataGridTableStyle tableStyle)
		{
			if (!datagrid.ColumnHeadersVisible || header != null)
				return;
	
			header = new DataGridHeaderProvider (this, tableStyle.GridColumnStyles);
			header.Initialize ();
			OnNavigationChildAdded (raiseEvent, header);

			SetBehavior (TablePatternIdentifiers.Pattern,
			             new TableProviderBehavior (this));
		}

		private void CreateListItem (bool raiseEvent, int row, SWF.DataGridTableStyle tableStyle)
		{
			object data = datagrid [row, 0];
			DataGridDataItemProvider item = new DataGridDataItemProvider (this,
			                                                              row,
			                                                              datagrid,
			                                                              tableStyle, 
			                                                              data);
			item.Initialize ();
			OnNavigationChildAdded (raiseEvent, item);
			items.Add (data, item);
		}

		#endregion Private Methods

		#region Private Fields

		private SWF.DataGrid datagrid;
		private DataGridHeaderProvider header;
		private Dictionary<object, ListItemProvider> items;
		private SWF.CurrencyManager lastCurrencyManager; 
		private object lastDataSource;
		private ScrollBehaviorObserver observer;

		#endregion

		#region Internal Class: DataGridHeaderProvider

		internal class DataGridHeaderProvider : FragmentRootControlProvider
		{
			public DataGridHeaderProvider (DataGridProvider provider,
			                               SWF.GridColumnStylesCollection styles) : base (null)
			{
				this.provider = provider;
				this.styles = styles;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public SWF.GridColumnStylesCollection GridColumnStyles {
				get { return styles; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Header.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return "Header";
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "header";
				else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
					return OrientationType.Horizontal;
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, provider.DataGrid, true);
				} else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					// FIXME: Remove reflection after committing SWF patch.
					SD.Rectangle rectangle
						= Helper.GetPrivateProperty<SWF.DataGrid, SD.Rectangle> (typeof (SWF.DataGrid),
						                                                         provider.DataGrid,
						                                                         "UIAColumnHeadersArea");
					rectangle.X += provider.DataGrid.Bounds.X;
					rectangle.Y += provider.DataGrid.Bounds.Y;

					return Helper.GetControlScreenBounds (rectangle, provider.DataGrid);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			public IRawElementProviderSimple[] GetHeaderItems ()
			{
				if (ChildrenCount == 0)
					return new IRawElementProviderSimple [0];
				else {
					IRawElementProviderSimple []children = new IRawElementProviderSimple [ChildrenCount];
					for (int index = 0; index < ChildrenCount; index++)
						children [index] = GetChildProviderAt (index);

					return children;
				}
			}

			public override void InitializeChildControlStructure ()
			{
				foreach (SWF.DataGridColumnStyle style in styles) {
					DataGridHeaderItemProvider headerItem
						= new DataGridHeaderItemProvider (this, style);
					OnNavigationChildAdded (false, headerItem);
				}
			}

			private SWF.GridColumnStylesCollection styles;
			private DataGridProvider provider;			
		} // DataGridHeaderProvider

		#endregion

		#region Internal Class: Header Item

		internal class DataGridHeaderItemProvider : FragmentControlProvider
		{
			public DataGridHeaderItemProvider (DataGridHeaderProvider header, 
			                                   SWF.DataGridColumnStyle style) : base (null)
			{
				this.header = header;
				this.style = style;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return header; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.HeaderItem.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return style.HeaderText;
				else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
					return null;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "header item";
				else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
					return OrientationType.Horizontal;
				else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id) {
					// FIXME: Remove reflection after committing SWF patch.
					int indexOf = header.GridColumnStyles.IndexOf (style);
					SD.Rectangle rectangle
						= Helper.GetPrivateProperty<SWF.DataGrid, SD.Rectangle> (typeof (SWF.DataGrid),
						                                                         style.DataGridTableStyle.DataGrid,
						                                                         "UIAColumnHeadersArea");
					rectangle.Width = header.GridColumnStyles [indexOf].Width;
					
					for (int index = 0; index < indexOf; index++)
						rectangle.X += header.GridColumnStyles [index].Width;

					return Helper.GetControlScreenBounds (rectangle, style.DataGridTableStyle.DataGrid);
				} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, style.DataGridTableStyle.DataGrid, true);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridColumnStyle style;
			private DataGridHeaderProvider header;
		} // DataGridHeaderItemProvider

		#endregion

		#region Internal Class: Data Item

		internal class DataGridDataItemProvider : ListItemProvider
		{
			public DataGridDataItemProvider (DataGridProvider provider,
			                                 int row,
			                                 SWF.DataGrid dataGrid,
			                                 SWF.DataGridTableStyle style,
			                                 object data)
				: base (provider, provider, dataGrid, data)
			{
				this.provider = provider;
				this.row = row;
				this.style = style;
				name = string.Empty;

				//TODO: Support Value and Invoke patterns
			}

			public DataGridProvider DataGridProvider {
				get { return provider; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public string GetName (DataGridDataItemEditProvider custom) 
			{
				int indexOf = GetChildProviderIndexOf (custom);
				if (indexOf == -1)
					return string.Empty;
				
				if (custom.Data == null || string.IsNullOrEmpty (custom.Data.ToString ()))
					return style.GridColumnStyles [indexOf].NullText;
				else
					return custom.Data.ToString ();
			}

			public override void InitializeChildControlStructure ()
			{
				for (int column = 0; column < provider.CurrentTableStyle.GridColumnStyles.Count; column++) {
					object data = provider.DataGrid [row, column];
					
					DataGridDataItemEditProvider custom 
						= new DataGridDataItemEditProvider (this, data);
					custom.Initialize ();
					OnNavigationChildAdded (false, custom);

					if (column == 0)
						name = GetName (custom);
				}
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				//FIXME: What about ItemTypeProperty & ItemStatusProperty ?
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				// NOTDOTNET: Using DataItem instead, the spec says that we should use text, 
				// however the implementation uses ListItem.
				else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.DataItem.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "data item";
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private string name;			
			private DataGridProvider provider;
			private int row;
			private SWF.DataGridTableStyle style;
		} //DataGridListItemProvider

		#endregion

		#region Internal Class: Data Item Edit

		internal class DataGridDataItemEditProvider : FragmentRootControlProvider
		{
			public DataGridDataItemEditProvider (DataGridDataItemProvider provider,
			                                     object data) : base (null)
			{
				this.provider = provider;
				this.data = data == null ? string.Empty : data.ToString ();
			}

			public string Data {
				get { return data; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//TODO: Support Invoke pattern
				SetBehavior (ValuePatternIdentifiers.Pattern,
				             new ListItemEditValueProviderBehavior (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Edit.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "edit";
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return provider.GetName (this);
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return Helper.GetControlScreenBounds (provider.DataGridProvider.DataGrid.GetCellBounds (provider.Index,
					                                                                                        provider.GetChildProviderIndexOf (this)),
					                                      provider.DataGridProvider.DataGrid);
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					Rect bounds 
						= (Rect) GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
					return Helper.IsOffScreen (bounds, provider.DataGridProvider.DataGrid, true);
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					return Helper.GetClickablePoint (this);
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private DataGridDataItemProvider provider;
			private string data;
		} //DataGridListItemCustomProvider

		#endregion
		
		#region Internal Class: ScrollBar provider

		class DataGridScrollBarProvider : ScrollBarProvider
		{
			public DataGridScrollBarProvider (SWF.ScrollBar scrollbar,
			                                  DataGridProvider provider)
				: base (scrollbar)
			{
				this.provider = provider;
				//TODO: i18n?
				name = scrollbar is SWF.HScrollBar ? "Horizontal Scroll Bar"
					: "Vertical Scroll Bar";
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}			
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			private DataGridProvider provider;
			private string name;
		} // DataGridScrollBarProvider
		
		#endregion

		#region Internal Class: Custom 

		internal class DataGridCustomProvider : FragmentRootControlProvider
		{
			public DataGridCustomProvider (DataGridProvider provider,
			                               int row,
			                               string name) : base (null)
			{
				this.provider = provider;
				this.row = row;
				this.name = name;
			}

			public int Row {
				get { return row; }
			}

			public DataGridProvider DataGridProvider {
				get { return provider; }
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return DataGridProvider; }
			}

			public override void Initialize ()
			{
				base.Initialize ();

				//TODO: Support Value Pattern ?
				SetBehavior (InvokePatternIdentifiers.Pattern, 
				             new CustomInvokeProviderBehavior (this));
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Custom.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return string.Empty;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private string name;
			private DataGridProvider provider;
			private int row;
		} //DataGridCustomProvider

		#endregion
	}
}
