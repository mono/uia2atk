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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;

namespace UiaAtkBridge
{
	public class List : ComponentParentAdapter, Atk.ISelectionImplementor
	{
		private IRawElementProviderFragmentRoot		provider;
		private ISelectionProvider					selectionProvider;
		private SelectionProviderUserHelper	selectionHelper;
		private Adapter selectedItem;
		private bool hasFocus = false;
		
/*
AtkObject,
?AtkAction,
?AtkSelection,
?AtkRelation (to associate a text label with the control),
?AtkRelationSet,
?AtkStateSet
*/


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


		public List (IRawElementProviderFragmentRoot provider) : base (provider)
		{
			this.provider = provider;
			
			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new ArgumentException ("List should always implement ISelectionProvider");

			int controlTypeId = (int) Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId == ControlType.Spinner.Id)
				Role = Atk.Role.SpinButton;
			else
				Role = Atk.Role.List;
			
			selectionHelper = new SelectionProviderUserHelper (provider, selectionProvider);
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
			// ControlType.List returns Name from one static label, Atk returns NULL
			int controlTypeId = (int) Provider.GetPropertyValue (
				AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId != ControlType.List.Id && controlTypeId != ControlType.Spinner.Id)
				base.UpdateNameProperty (newName, fromCtor);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.AddState (Atk.StateType.ManagesDescendants);

			return states;
		}
		
		internal void HandleItemFocus (Adapter item, bool itemFocused)
		{
			bool listFocused = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
			if (hasFocus != listFocused) {
				NotifyStateChange (Atk.StateType.Focused, listFocused);
			}
			if (itemFocused)
				EmitSignal ("active-descendant-changed", item.Handle);
			hasFocus = listFocused;
		}

#region Atk.ISelectionImplementor

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
		public virtual bool RemoveSelection (int i)
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
			} if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				//if it's a toggle, it should not be a basic Button class, but CheckBox or other
				throw new NotSupportedException ("Toggle events should not land here (should not be reached)");
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
			Name = selectedItem.Name;
		}
	}

	public class ListWithEditableText
		: List, Atk.ITextImplementor, Atk.IEditableTextImplementor
	{
		private ITextImplementor text_helper;
		private EditableTextImplementorHelper editableTextExpert;

		public ListWithEditableText (IRawElementProviderFragmentRoot provider)
			: base (provider)
		{
			IValueProvider value_prov
				= provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id) as IValueProvider;
			if (value_prov == null) {
				throw new ArgumentException ("Provider does not implement IValue");
			}

			text_helper = TextImplementorFactory.GetImplementor (this, provider);
			editableTextExpert = new EditableTextImplementorHelper (this, this, text_helper);
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			editableTextExpert.UpdateStates (states);

			states.AddState (Atk.StateType.SingleLine);
			return states;
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationPropertyChangedEvent (e))
				return;

			base.RaiseAutomationPropertyChangedEvent (e);
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationEvent (eventId, e))
				return;
			
			base.RaiseAutomationEvent (eventId, e);
		}

#region TextImplementor Implementation 
		public string GetText (int startOffset, int endOffset)
		{
			return text_helper.GetText (startOffset, endOffset);
		}
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return text_helper.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return text_helper.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return text_helper.GetCharacterAtOffset (offset);
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return text_helper.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetStringAtOffset (int offset, Atk.TextGranularity granularity, out int startOffset, out int endOffset)
		{
			return text_helper.GetStringAtOffset (offset, granularity, out startOffset, out endOffset);
		}

		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return text_helper.GetRunAttributes (offset, out startOffset, out endOffset);
		}

		public Atk.Attribute [] DefaultAttributes {
			get { return text_helper.DefaultAttributes; }
		}
		
		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			text_helper.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			return text_helper.GetOffsetAtPoint (x, y, coords);
		}
		
		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return text_helper.GetSelection (selectionNum, out startOffset, out endOffset);
		}
		
		public bool AddSelection (int startOffset, int endOffset)
		{
			return text_helper.AddSelection ( startOffset, endOffset);
		}
		
		public override bool RemoveSelection (int selectionNum)
		{
			return text_helper.RemoveSelection (selectionNum);
		}
		
		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return text_helper.SetSelection (selectionNum, startOffset, endOffset);
		}

		public bool SetCaretOffset (int offset)
		{
			return text_helper.SetCaretOffSet (offset);
		}
		
		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			text_helper.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}
		
		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			return text_helper.GetBoundedRanges (rect, coordType, xClipType, yClipType);
		}
		
		public int CaretOffset {
			get { return text_helper.CaretOffset; }
		}
		
		public int CharacterCount {
			get { return text_helper.Length; }
		}
		
		public int NSelections {
			get { return text_helper.NSelections; }
		}
#endregion 

#region EditableTextImplementor implementation 
		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return editableTextExpert.SetRunAttributes (attrib_set, start_offset, end_offset);
		}
		
		public void InsertText (string str, ref int position)
		{
			editableTextExpert.InsertText (str, ref position);
		}
		
		public void CopyText (int start_pos, int end_pos)
		{
			editableTextExpert.CopyText (start_pos, end_pos);
		}
		
		public void CutText (int start_pos, int end_pos)
		{
			editableTextExpert.CutText (start_pos, end_pos);
		}
		
		public void DeleteText (int start_pos, int end_pos)
		{
			editableTextExpert.DeleteText (start_pos, end_pos);
		}
		
		public void PasteText (int position)
		{
			editableTextExpert.PasteText (position);
		}
		
		public string TextContents {
			get {
				return editableTextExpert.TextContents;
			}
			set {
				editableTextExpert.TextContents = value;
			}
		}
#endregion 
	}

	public class ListWithGrid : List, Atk.ITableImplementor
	{
		private TableImplementorHelper tableExpert = null;
		
		public ListWithGrid (IRawElementProviderFragmentRoot provider) : base (provider)
		{
			tableExpert = new TableImplementorHelper (this);
		}
		
		public Atk.Object RefAt (int row, int column)
		{
			return tableExpert.RefAt (row, column);
		}

		public int GetIndexAt (int row, int column)
		{
			return tableExpert.GetIndexAt (row, column);
		}

		public int GetColumnAtIndex (int index)
		{
			return tableExpert.GetColumnAtIndex (index);
		}

		public int GetRowAtIndex (int index)
		{
			return tableExpert.GetRowAtIndex (index);
		}

		public int NColumns { get { return tableExpert.NColumns; } }

		public int NRows { get { return tableExpert.NRows; } }
			
		public int GetColumnExtentAt (int row, int column)
		{
			return tableExpert.GetColumnExtentAt (row, column);
		}

		public int GetRowExtentAt (int row, int column)
		{
			return tableExpert.GetRowExtentAt (row, column);
		}

		public Atk.Object Caption
		{
			get { return tableExpert.Caption; } set { tableExpert.Caption = value; }
		}

		public string GetColumnDescription (int column)
		{
			return tableExpert.GetColumnDescription (column);
		}

		public Atk.Object GetColumnHeader (int column)
		{
			return tableExpert.GetColumnHeader (column);
		}

		public string GetRowDescription (int row)
		{
			return tableExpert.GetRowDescription (row);
		}

		public Atk.Object GetRowHeader (int row)
		{
			return tableExpert.GetRowHeader (row);
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
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedColumns (out int [] selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedRows (out int selected)
		{
			return tableExpert.GetSelectedRows (out selected);
		}

		public int GetSelectedRows (out int [] selected)
		{
			return tableExpert.GetSelectedRows (out selected);
		}

		public bool IsColumnSelected (int column)
		{
			return tableExpert.IsColumnSelected (column);
		}

		public bool IsRowSelected (int row)
		{
			return tableExpert.IsRowSelected (row);
		}

		public bool IsSelected (int row, int column)
		{
			return tableExpert.IsSelected (row, column);
		}

		public bool AddRowSelection (int row)
		{
			return tableExpert.AddRowSelection (row);
		}

		public bool RemoveRowSelection (int row)
		{
			return tableExpert.RemoveRowSelection (row);
		}

		public bool AddColumnSelection (int column)
		{
			return tableExpert.AddColumnSelection (column);
		}

		public bool RemoveColumnSelection (int column)
		{
			return tableExpert.RemoveColumnSelection (column);
		}
	}
}
