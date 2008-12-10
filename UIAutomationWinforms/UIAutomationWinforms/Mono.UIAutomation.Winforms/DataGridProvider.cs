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
using System.Data;
using System.Reflection;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.DataGrid;

//TODO: Realize IListProvider (and subclass from ListProvider??)

namespace Mono.UIAutomation.Winforms
{

	internal class DataGridProvider 
		: FragmentRootControlProvider, IListProvider, IScrollBehaviorSubject
	{
		#region Public Constructors
		
		public DataGridProvider (SWF.DataGrid datagrid) : base (datagrid)
		{
			this.datagrid = datagrid;
		}

		#endregion

		#region Public Properties

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
					                                                          "VScrollBar");
				SWF.ScrollBar hscrollbar 
					= Helper.GetPrivateProperty<SWF.DataGrid, SWF.ScrollBar> (typeof (SWF.DataGrid),
					                                                          datagrid,
					                                                          "HScrollBar");
				
				//ListScrollBehaviorObserver updates Navigation
				observer = new ScrollBehaviorObserver (this, hscrollbar, vscrollbar);			
				observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
				UpdateScrollBehavior ();
			} catch (Exception) {}
		}

		public override void Terminate ()
		{
			base.Terminate ();

			observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
		}

		public override void InitializeChildControlStructure ()
		{
			UpdateChildren (false);
			datagrid.DataSourceChanged += OnDataSourceChanged;

			try {
				Helper.AddPrivateEvent (typeof (SWF.DataGrid),
				                        datagrid,
				                        "UIAExpanded",
				                        this,
				                        "OnUIAExpanded");
			} catch (NotSupportedException) { }
			try {
				Helper.AddPrivateEvent (typeof (SWF.DataGrid),
				                        datagrid,
				                        "UIACollapsed",
				                        this,
				                        "OnUIACollapsed");
			} catch (NotSupportedException) { }
		}

		public override void FinalizeChildControlStructure ()
		{
			OnNavigationChildrenCleared (false);
			datagrid.DataSourceChanged -= OnDataSourceChanged;

			try {
				Helper.RemovePrivateEvent (typeof (SWF.DataGrid),
				                           datagrid,
				                           "UIAExpanded",
				                           this,
				                           "OnUIAExpanded");
			} catch (NotSupportedException) { }
			try {
				Helper.RemovePrivateEvent (typeof (SWF.DataGrid),
				                           datagrid,
				                           "UIACollapsed",
				                           this,
				                           "OnUIACollapsed");
			} catch (NotSupportedException) { }
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.DataGrid.Id; //FIXME: Is Table not DataGrid
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "datagrid"; //FIXME: Is "table" not "datagrid"
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
			return -1; // FIXME: Implement
		}

		public void FocusItem (object objectItem)
		{
			// FIXME: Implement
		}

		public int SelectedItemsCount {
			get { return 0; } // FIXME: Implement
		}

		public void ScrollItemIntoView (ListItemProvider item)
		{
			// FIXME: Implement:
		}

		public bool IsItemSelected (ListItemProvider item)
		{
			return false; // FIXME: Implement
		}

		public void SelectItem (ListItemProvider item)
		{
			// FIXME: Implement
		}

		public void UnselectItem (ListItemProvider item)
		{
			// FIXME: Implement
		}

		public object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId)
		{
			// FIXME: Implement at least:
			// - AutomationElementIdentifiers.NameProperty.Id
			// - AutomationElementIdentifiers.HasKeyboardFocusProperty.Id
			// - AutomationElementIdentifiers.BoundingRectangleProperty.Id
			// - AutomationElementIdentifiers.IsOffscreenProperty.Id
			// - AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
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

		private void OnUIAExpanded (object sender, EventArgs args)
		{
			Console.WriteLine ("DataGridProvider.Expanded - Rows: {0}", lastCurrencyManager.Count);

			DataSet dataset = null;
			if ((dataset = datagrid.DataSource as DataSet) != null) {
				Console.WriteLine ("Tables: {0}", dataset.Tables.Count);
			}
		}

		private void OnUIACollapsed (object sender, EventArgs args)
		{
			Console.WriteLine ("DataGridProvider.Collapsed - Rows: {0}", lastCurrencyManager.Count);
		}

#pragma warning restore 169

		private void UpdateChildren (bool raiseEvent)
		{
			if (lastDataSource != null)
				OnNavigationChildrenCleared (raiseEvent);

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

			SWF.DataGridTableStyle tableStyle
				= Helper.GetPrivateProperty<SWF.DataGrid, SWF.DataGridTableStyle> (typeof (SWF.DataGrid),
				                                                                   datagrid,
				                                                                   "CurrentTableStyle");
			// Is showing "+" to expand, this usually happens when DataSource is
			// DataSet and has more than one DataTable.
			if (tableStyle.GridColumnStyles.Count == 0) {
				Console.WriteLine ("Should ADD custom item that supports Invoke");
				DataGridCustomProvider customProvider 
					= new DataGridCustomProvider (this, 0, string.Empty);
				customProvider.Initialize ();
				OnNavigationChildAdded (raiseEvent, customProvider);
			} else {
				header = new DataGridHeaderProvider (this, tableStyle.GridColumnStyles);
				header.Initialize ();
				OnNavigationChildAdded (raiseEvent, header);

				for (int row = 0; row < lastCurrencyManager.Count; row++) {
					DataGridListItemProvider item = new DataGridListItemProvider (this,
					                                                              row,
					                                                              datagrid,
					                                                              tableStyle);
					item.Initialize ();
					OnNavigationChildAdded (raiseEvent, item);
				}
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

		#endregion Private Methods

		#region Private Fields

		private SWF.DataGrid datagrid;
		private DataGridHeaderProvider header;
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
				else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
					return false; //TODO: Implement
				else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
					return false;
				else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
					return true;
				else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
					return System.Windows.Rect.Empty; //TODO: Implement
					//return Helper.GetControlScreenBounds (listView.UIAHeaderControl, listView);
				else
					return base.GetProviderPropertyValue (propertyId);
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
					//FIXME: Implement
					return System.Windows.Rect.Empty;
				} else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
					//FIXME: Implement
					return false;
				} else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
					//FIXME: Implement
					return System.Windows.Rect.Empty;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private SWF.DataGridColumnStyle style;
			private DataGridHeaderProvider header;
		} // DataGridHeaderItemProvider

		#endregion

		#region Internal Class: List Item

		internal class DataGridListItemProvider : ListItemProvider
		{
			public DataGridListItemProvider (DataGridProvider provider, 
			                                 int row,
			                                 SWF.DataGrid dataGrid,
			                                 SWF.DataGridTableStyle style)
				: base (provider, provider, dataGrid, row)
			{
				this.provider = provider;
				this.row = row;
				this.style = style;
				name = string.Empty;

				//TODO: Support Value and Invoke patterns
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public string GetName (DataGridListItemEditProvider custom) 
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
				for (int column = 0; column < provider.HeaderProvider.ChildrenCount; column++) {
					object data = provider.DataGrid [row, column];					
					DataGridListItemEditProvider custom 
						= new DataGridListItemEditProvider (this, data);
					custom.Initialize ();
					OnNavigationChildAdded (false, custom);

					if (column == 0)
						name = GetName (custom);
				}
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private string name;			
			private DataGridProvider provider;
			private int row;
			private SWF.DataGridTableStyle style;
		} //DataGridListItemProvider

		#endregion

		#region Internal Class: List Item Edit

		internal class DataGridListItemEditProvider : FragmentRootControlProvider
		{
			public DataGridListItemEditProvider (DataGridListItemProvider provider,
			                                     object data) : base (null)
			{
				this.provider = provider;
				this.data = data;
			}

			public object Data {
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
				else
					return base.GetProviderPropertyValue (propertyId);
			}


			private DataGridListItemProvider provider;
			private object data;
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
