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
using System.Reflection;
using SWF = System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

//TODO: Realize IListProvider (and subclass from ListProvider??)

namespace Mono.UIAutomation.Winforms
{
	
	internal class DataGridProvider : FragmentRootControlProvider
	{
		
		public DataGridProvider (SWF.DataGrid datagrid) : base (datagrid)
		{
			this.datagrid = datagrid;
		}

		public override void InitializeChildControlStructure ()
		{
			UpdateChildren (false);
			datagrid.DataSourceChanged += OnDataSourceChanged;
		}

		public override void FinalizeChildControlStructure ()
		{
			OnNavigationChildrenCleared (false);
			datagrid.DataSourceChanged -= OnDataSourceChanged;
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

		private void UpdateChildren (bool raiseEvent)
		{
			if (lastDataSource != null)
				OnNavigationChildrenCleared (raiseEvent);

			if (header != null) {
				header.Terminate ();
				header = null;
			}

			// According to http://msdn.microsoft.com/en-us/library/fxfa9793.aspx
			// Valid data sources for the DataGrid include:
			// - DataTable class
			// - DataView class
			// - DataSet class
			// - DataViewManager class
			// Also:
			// - Any component that implements the IList interface (shows public properties)
			// - Any component that implements the IListSource interface.
			// - Any component that implements the IBindingList interface.
			IList ilist = null;

			if ((ilist = datagrid.DataSource as IList) != null)
				InitializeListDataSource (ilist, raiseEvent);

			lastDataSource = datagrid.DataSource;
		}

		private void InitializeListDataSource (IList list, bool raiseEvent)
		{
			// NOTE: ILists use public properties for data binding.
			//
			// We are using the first element as the main Bindable-Type, because 
			// you can use an ArrayList and it would contain different types.
			// http://msdn.microsoft.com/en-us/library/system.windows.forms.datagrid.datasource.aspx
			if (list.Count == 0)
				return;

			ArrayList items = new ArrayList ();
			Type listType = list [0].GetType ();
			PropertyInfo[] publicProperties 
				= listType.GetProperties (BindingFlags.Public | BindingFlags.Instance);

			for (int index = 0; index < list.Count; index++) {
				if (list [index].GetType () == listType
				    || list [index].GetType ().IsSubclassOf (listType))
					items.Add (list [index]);
			}

			// Header
			header = new DataGridHeaderProvider (this, items, publicProperties);
			header.Initialize ();
			OnNavigationChildAdded (raiseEvent, header);

			// Items
			for (int index = 0; index < items.Count; index++) {
				DataGridListItemProvider itemProvider
					= new DataGridListItemProvider (this, 
					                                items [index], 
					                                publicProperties);
				itemProvider.Initialize ();
				OnNavigationChildAdded (raiseEvent, itemProvider);
			}
		}

		private void OnDataSourceChanged (object sender, EventArgs args)
		{
			UpdateChildren (true);
		}

		private SWF.DataGrid datagrid;
		private DataGridHeaderProvider header;
		private object lastDataSource;

		class DataGridHeaderProvider : FragmentRootControlProvider
		{
			public DataGridHeaderProvider (DataGridProvider provider,
			                               ArrayList items,
			                               PropertyInfo []properties) : base (null)
			{
				this.provider = provider;
				this.properties = properties;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Header.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return null;//name
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
				foreach (PropertyInfo property in properties) {
					DataGridHeaderItemProvider headerItem
						= new DataGridHeaderItemProvider (this, property.Name);
					OnNavigationChildAdded (false, headerItem);
				}
			}

			private PropertyInfo []properties;
			private DataGridProvider provider;			
		}

		class DataGridHeaderItemProvider : FragmentControlProvider
		{
			public DataGridHeaderItemProvider (DataGridHeaderProvider header, 
			                                   string name) : base (null)
			{
				this.header = header;
				this.name = name;
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return header; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.HeaderItem.Id;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
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

			private string name;
			private DataGridHeaderProvider header;
		}

		class DataGridListItemProvider : FragmentRootControlProvider
		{
			public DataGridListItemProvider (DataGridProvider provider,
			                                 object data,
			                                 PropertyInfo[] publicProperties) : base (null)
			{
				this.provider = provider;
				this.data = data;
				this.publicProperties = publicProperties;

				//TODO: Support Value and Invoke patterns
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			public override void InitializeChildControlStructure ()
			{
				base.InitializeChildControlStructure ();

				foreach (PropertyInfo property in publicProperties) {
					DataGridListItemCustomProvider custom 
						= new DataGridListItemCustomProvider (this, property.GetValue (data, null));
					custom.Initialize ();
					OnNavigationChildAdded (false, custom);
				}
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.ListItem.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return "list item";
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					//TODO: How to return "(Collection)" when value is IList??
					return publicProperties [0].GetValue (data, null).ToString ();
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private PropertyInfo[] publicProperties;
			private DataGridProvider provider;
			private object data;
		}

		class DataGridListItemCustomProvider : FragmentRootControlProvider
		{
			public DataGridListItemCustomProvider (DataGridListItemProvider provider,
			                                       object data) : base (null)
			{
				this.provider = provider;
				this.data = data;

				//TODO: Support Value and Invoke patterns
			}

			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}

			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Custom.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return string.Empty;
				else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return data.ToString ();
				else
					return base.GetProviderPropertyValue (propertyId);
			}

			private DataGridListItemProvider provider;
			private object data;
		}

	}
}
