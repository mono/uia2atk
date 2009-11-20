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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;

namespace UiaAtkBridge
{
	public class Tree : ComponentParentAdapter, Atk.SelectionImplementor, Atk.TableImplementor
	{
		private IRawElementProviderFragment	provider;
		private ISelectionProvider		selectionProvider;
		private SelectionProviderUserHelper	selectionHelper;
		private Adapter selectedItem;
		private bool hasFocus = false;
		private TableImplementorHelper tableExpert = null;


#region UI Automation Properties supported


		// AutomationElementIdentifiers.BoundingRectangleProperty.Id

		// AutomationElementIdentifiers.AutomationIdProperty.Id
		public string AutomationId
		{
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.AutomationIdProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		public bool IsKeyboardFocusable
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.NameProperty.Id
		// already handled by the Atk object

		// AutomationElementIdentifiers.ClickablePointProperty.Id
		public System.Windows.Point ClickablePoint
		{ 
			get {
				return (System.Windows.Point) provider.GetPropertyValue (AutomationElementIdentifiers.ClickablePointProperty.Id);
			}
		}

		// AutomationElementIdentifiers.ControlTypeProperty.Id
		public int ControlTypeId
		{ 
			get {
				return (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			}
		}

		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		public string LocalizedControlType
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		public bool IsContentElement
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsContentElementProperty.Id);
			}
		}

		// AutomationElementIdentifiers.IsControlElementProperty.Id
		public bool IsControlElement
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsControlElementProperty.Id);
			}
		}

		// AutomationElementIdentifiers.HelpTextProperty.Id
		public string HelpText
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.HelpTextProperty.Id);
			}
		}

		// SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id
		public bool IsSelectionRequired
		{ 
			get {
				return (bool) provider.GetPropertyValue (SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id);
			}
		}

		// SelectionPatternIdentifiers.CanSelectMultipleProperty.Id
		public bool CanSelectMultiple
		{ 
			get {
				return (bool) provider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			}
		}


#endregion


		public Tree (IRawElementProviderFragment provider) : base (provider)
		{
			this.provider = provider;
			
			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new ArgumentException ("Tree should always implement ISelectionProvider");

			tableExpert = new TableImplementorHelper (this);

			Role = Atk.Role.TreeTable;
			
			selectionHelper = new SelectionProviderUserHelper (provider, selectionProvider);

			hasFocus = (bool) Provider.GetPropertyValue (
				AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
		}
		
		internal override void AddOneChild (Atk.Object child)
		{
			base.AddOneChild (child);
			
			Adapter adapter = child as Adapter;
			if (adapter == null)
				return;
			
			for (int i = 0; i < NAccessibleChildren; i++) {
				Adapter currentChild = RefAccessibleChild (i) as Adapter;
				if (adapter.ParentProvider == currentChild.Provider)
					((TreeItem)currentChild).NotifyChildAdded (adapter);
			}
		}
		
		internal override void RemoveChild (Atk.Object childToRemove)
		{
			if (childToRemove == selectedItem)
				selectedItem = null;
			base.RemoveChild (childToRemove);
			
			for (int i = 0; i < NAccessibleChildren; i++) {
				TreeItem currentChild = RefAccessibleChild (i) as TreeItem;
				if (currentChild != null)
					currentChild.NotifySomeChildRemoved (childToRemove);
			}
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
			// Don't allow the name to be set
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.AddState (Atk.StateType.ManagesDescendants);
			
			if (hasFocus)
				states.AddState (Atk.StateType.Focused);
			else
				states.RemoveState (Atk.StateType.Focused);

			return states;
		}

		internal void HandleItemFocus (Adapter item, bool itemFocused)
		{
			bool listFocused = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
			if (hasFocus != listFocused) {
				NotifyStateChange (Atk.StateType.Focused, listFocused);
				if (listFocused)
					Atk.Focus.TrackerNotify (this);
			}
			if (itemFocused) {
				// Orca wants to get Selection events before
				// active-descendant-changed events; otherwise
				// it is excessively verbose.
				if (!CanSelectMultiple)
					NotifyItemSelected (item);
				GLib.Signal.Emit (this, "active-descendant-changed", item.Handle);
			}
			hasFocus = listFocused;
		}

#region Atk.SelectionImplementor

		public int SelectionCount
		{
			get { return selectionHelper.SelectionCount; }
		}
		public bool AddSelection (int i)
		{
			selectionHelper.AddSelection (i);
			//FIXME: currently unit-tests force this to always true, we may be interested in changing them when we report the gail bug about this (see ComboBox.cs)
			return true;
		}
		public bool ClearSelection ()
		{
			return selectionHelper.ClearSelection ();
		}
		public bool IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected (i);
		}
		public Atk.Object RefSelection (int i)
		{
			return selectionHelper.RefSelection (i);
		}
		public bool RemoveSelection (int i)
		{
			return selectionHelper.RemoveSelection (i);
		}
		public bool SelectAllSelection ()
		{
			return selectionHelper.SelectAllSelection ();
		}

#endregion

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AsyncContentLoadedEvent) {
				// TODO: Handle AsyncContentLoadedEvent
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == AutomationElementIdentifiers.ControlTypeProperty) {
				//We remove the current Adapter and add it again to reflect the ControlType change
				StructureChangedEventArgs args 
					= new StructureChangedEventArgs (StructureChangeType.ChildRemoved, 
					                                 new int[] { 0 }); //TODO: Fix ?
				AutomationInteropProvider.RaiseStructureChangedEvent (Provider,
				                                                      args);

				args = new StructureChangedEventArgs (StructureChangeType.ChildAdded,
				                                      new int[] { 0 }); //TODO: Fix ?
				AutomationInteropProvider.RaiseStructureChangedEvent (Provider,
				                                                      args);
			} else if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				//if it's a toggle, it should not be a basic Button class, but CheckBox or other
				Log.Error ("Tree: Toggle events should not land here (should not be reached)");
			} else if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty) {
				hasFocus = (bool)e.NewValue;
				base.RaiseAutomationPropertyChangedEvent (e);
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			//TODO
		}

		internal void NotifyItemSelected (Adapter item)
		{
			if (item == selectedItem)
				return;
			if (selectedItem != null)
				selectedItem.NotifyStateChange (Atk.StateType.Selected, false);
			item.NotifyStateChange (Atk.StateType.Selected, true);
			selectedItem = item;
		}

		internal void NotifyItemSelectionRemoved (Adapter item)
		{
			if (item != selectedItem)
				return;
			item.NotifyStateChange (Atk.StateType.Selected, false);
			selectedItem = null;
		}

		public Atk.Object RefAt (int row, int column)
		{
			if (column != 0)
				return null;
			IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
			int index = 0;
			return GetObjectAtRow (child, row, ref index);
		}

		private Atk.Object GetObjectAtRow (IRawElementProviderFragment provider, int target, ref int index)
		{
			if (provider == null)
				return null;
			if (!IsListItem (provider))
				return GetObjectAtRow (provider.Navigate (NavigateDirection.NextSibling), target, ref index);
			if (index == target)
				return AutomationBridge.GetAdapterForProviderLazy (provider);
			index++;
			if (IsExpanded (provider)) {
				Atk.Object result = GetObjectAtRow (provider.Navigate (NavigateDirection.FirstChild), target, ref index);
				if (index == target && result != null)
					return result;
			}
			return GetObjectAtRow (provider.Navigate (NavigateDirection.NextSibling), target, ref index);
		}

		public int GetIndexAt (int row, int column)
		{
			Atk.Object obj = RefAt (row, column);
			return obj.IndexInParent;
		}

		public int GetColumnAtIndex (int index)
		{
			return 0;
		}

		public int GetRowAtIndex (int index)
		{
			Adapter target = RefAccessibleChild (index) as Adapter;
			if (target == null)
				return -1;
			return GetRowForAdapter (target);
		}

		private int GetRowForAdapter (Adapter target)
		{
			IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
			int count = 0;
			return GetRowForProvider (child, (IRawElementProviderFragment)target.Provider, ref count);
		}

		private int GetRowForProvider (IRawElementProviderFragment provider, IRawElementProviderFragment target, ref int index)
		{
			if (provider != null && !IsListItem (provider))
				return GetRowForProvider (provider.Navigate (NavigateDirection.NextSibling), target, ref index);
			if (provider == null)
				return -1;
			if (provider == target)
				return index;
			index++;
			if (IsExpanded (provider)) {
				int result = GetRowForProvider (provider.Navigate (NavigateDirection.FirstChild), target, ref index);
				if (result != -1)
					return result;
			}
			return GetRowForProvider (provider.Navigate (NavigateDirection.NextSibling), target, ref index);
		}

		public int NColumns { 
			get { return 1; }
		}

		public int NRows {
			get {
				int count = 0;
				IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
				CalculateRowCount (child, ref count);
				return count;
			}
		}

		private void CalculateRowCount (IRawElementProviderFragment provider, ref int count)
		{
			if (provider == null)
				return;
			if (IsListItem (provider))
				count++;
			if (IsExpanded (provider))
				CalculateRowCount  (provider.Navigate (NavigateDirection.FirstChild), ref count);
			CalculateRowCount  (provider.Navigate (NavigateDirection.NextSibling), ref count);
		}

		private bool IsExpanded (IRawElementProviderFragment provider)
		{
			object obj = provider.GetPropertyValue (ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			if (obj == null)
				return false;
			return ((ExpandCollapseState)obj) == ExpandCollapseState.Expanded;
		}

		public void NotifyRowAdjusted (Adapter adapter, bool expanded)
		{
			int row = GetRowForAdapter (adapter);
			int rowCount = 0;
			IRawElementProviderFragment childProvider = (IRawElementProviderFragment)adapter.Provider;
			for (IRawElementProviderFragment child = childProvider.Navigate (NavigateDirection.FirstChild); child != null; child = child.Navigate (NavigateDirection.NextSibling))
				if (IsListItem (child))
					rowCount++;
			if (rowCount > 0) {
				GLib.Signal.Emit (this, (expanded? "row-inserted": "row-deleted"), row + 1, rowCount);
				EmitVisibleDataChanged ();
			}
		}

		public int GetColumnExtentAt (int row, int column)
		{
			return (column == 0 ? 1: -1);
		}

		public int GetRowExtentAt (int row, int column)
		{
			return (column == 0 ? 1: -1);
		}

		public Atk.Object Caption
		{
			get { return tableExpert.Caption; } set { tableExpert.Caption = value; }
		}

		public string GetColumnDescription (int column)
		{
			return string.Empty;
		}

		public Atk.Object GetColumnHeader (int column)
		{
			return null;
		}

		public string GetRowDescription (int row)
		{
			return String.Empty;
		}

		public Atk.Object GetRowHeader (int row)
		{
			return null;
		}

		public Atk.Object Summary
		{
			get { return tableExpert.Summary; } set { tableExpert.Summary = value; }
		}


		public void SetColumnDescription (int column, string description)
		{
			tableExpert.SetColumnDescription (column, description);
		}

		public void SetColumnHeader (int column, Atk.Object header)
		{
			tableExpert.SetColumnHeader (column, header);
		}

		public void SetRowDescription (int row, string description)
		{
			tableExpert.SetRowDescription (row, description);
		}

		public void SetRowHeader (int row, Atk.Object header)
		{
			tableExpert.SetRowHeader (row, header);
		}

		public int [] SelectedColumns {
			get { return tableExpert.SelectedColumns; }
		}

		public int [] SelectedRows {
			get { return tableExpert.SelectedRows; }
		}

		// TODO: Remove next methods when atk-sharp is fixed (BNC#512477)
		public int GetSelectedColumns (out int selected)
		{
			selected = 0;
			return 0;
		}

		public int GetSelectedColumns (out int [] selected)
		{
			selected = new int [0];
			return 0;
		}

		public int GetSelectedRows (out int selected)
		{
			selected = 0;
			return 0;
		}

		public int GetSelectedRows (out int [] selected)
		{
			selected = new int [0];
			return 0;
		}

		public bool IsColumnSelected (int column)
		{
			return false;
		}

		public bool IsRowSelected (int row)
		{
			// TODO: Implement
			return false;
		}

		public bool IsSelected (int row, int column)
		{
			// TODO: Implement
			return false;
		}

		public bool AddRowSelection (int row)
		{
			// TODO: Implement
			return false;
		}

		public bool RemoveRowSelection (int row)
		{
			// TODO: Implement
			return false;
		}

		public bool AddColumnSelection (int column)
		{
			return false;
		}

		public bool RemoveColumnSelection (int column)
		{
			return false;
		}

		private bool IsListItem (IRawElementProviderSimple provider)
		{
			int controlTypeId = (int)provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			return (controlTypeId == ControlType.TreeItem.Id || controlTypeId == ControlType.ListItem.Id);
		}
	}
}
