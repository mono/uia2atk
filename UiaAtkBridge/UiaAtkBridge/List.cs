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

namespace UiaAtkBridge
{
	public class List : ComponentParentAdapter, Atk.SelectionImplementor
	{
		private IRawElementProviderFragmentRoot		provider;
		private ISelectionProvider					selectionProvider;
		private SelectionProviderUserHelper	selectionHelper;
		private Adapter selectedItem;
		
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

			//ControlTypeList returns Name from one static label, Atk returns ""
			string componentName = string.Empty;
			if (controlTypeId != ControlType.List.Id)
				componentName = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = componentName;
			
			selectionHelper = new SelectionProviderUserHelper (provider, selectionProvider);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Atk.StateType.ManagesDescendants);
			states.AddState (Atk.StateType.Focusable);
			return states;
		}
		
		public GLib.SList DefaultAttributes {
			get { throw new NotImplementedException (); }
		}


#region Atk.SelectionImplementor

		public int SelectionCount
		{
			get { return selectionHelper.SelectionCount; }
		}
		public bool AddSelection (int i)
		{
			return selectionHelper.AddSelection (i);
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
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
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
		}
	}

	public class ListWithEditableText
		: List, Atk.TextImplementor, Atk.EditableTextImplementor
	{
		private TextImplementorHelper text_helper;

		public ListWithEditableText (IRawElementProviderFragmentRoot provider)
			: base (provider)
		{
			IValueProvider value_prov
				= (IValueProvider) provider.GetPatternProvider (
					ValuePatternIdentifiers.Pattern.Id);
			if (value_prov == null) {
				throw new ArgumentException ("Provider does not implement IValue");
			}

			text_helper = new TextImplementorHelper (TextContents, this);
			caretOffset = text_helper.Length;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Atk.StateType.Editable);
			states.AddState (Atk.StateType.SingleLine);
			return states;
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property != ValuePatternIdentifiers.ValueProperty) {
				base.RaiseAutomationPropertyChangedEvent (e);
				return;
			}

			string new_text = TextContents;
			
			if (text_helper.HandleSimpleChange (new_text, ref caretOffset))
				return;

			Atk.TextAdapter adapter = new Atk.TextAdapter (this);

			// First delete all text, then insert the new text
			adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, text_helper.Length);

			text_helper = new TextImplementorHelper (new_text, this);
			adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
						 new_text == null ? 0 : new_text.Length);
			// TODO: The below line isn't quite right
			GLib.Signal.Emit (this, "text_caret_moved", new_text.Length);

			EmitVisibleDataChanged ();
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
		
		public GLib.SList GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return text_helper.GetRunAttributes (offset, out startOffset, out endOffset);
		}
		
		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			text_helper.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = caretOffset;
			endOffset = caretOffset;
			return null;
		}
		
		public bool AddSelection (int startOffset, int endOffset)
		{
			throw new NotImplementedException ();
		}
		
		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}
		
		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
		}

		int caretOffset = -1;
		
		public bool SetCaretOffset (int offset)
		{
			//TODO: internal interface
			caretOffset = offset;
			return true;
		}
		
		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			text_helper.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}
		
		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException ();
		}
		
		public int CaretOffset {
			get { return text_helper.Length; }
		}
		
		public GLib.SList DefaultAttributes {
			get { throw new NotImplementedException (); }
		}
		
		public int CharacterCount {
			get { return text_helper.Length; }
		}
		
		public int NSelections {
			get { return 0; }
		}
#endregion 

#region EditableTextImplementor implementation 
		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return false;
		}
		
		public void InsertText (string str, ref int position)
		{
			if (position < 0 || position > text_helper.Length)
				position = text_helper.Length;	// gail
			TextContents = text_helper.Text.Substring (0, position)
				+ str
				+ text_helper.Text.Substring (position);
			position += str.Length;
		}
		
		public void CopyText (int start_pos, int end_pos)
		{
			Console.WriteLine ("UiaAtkBridge (ListWithEditableText): CopyText unimplemented");
		}
		
		public void CutText (int start_pos, int end_pos)
		{
			Console.WriteLine ("UiaAtkBridge (ListWithEditableText): CutText unimplemented");
		}
		
		public void DeleteText (int start_pos, int end_pos)
		{
			if (start_pos < 0)
				start_pos = 0;
			if (end_pos < 0 || end_pos > text_helper.Length)
				end_pos = text_helper.Length;
			if (start_pos > end_pos)
				start_pos = end_pos;

			TextContents = TextContents.Remove (start_pos, end_pos - start_pos);
		}
		
		public void PasteText (int position)
		{
			Console.WriteLine ("UiaAtkBridge (ListWithEditableText): PasteText unimplemented");
		}
		
		public string TextContents {
			get {
				IValueProvider value_prov
					= (IValueProvider) Provider.GetPatternProvider (
						ValuePatternIdentifiers.Pattern.Id);
				return value_prov.Value.ToString ();
			}
			set {
				IValueProvider value_prov
					= (IValueProvider) Provider.GetPatternProvider (
						ValuePatternIdentifiers.Pattern.Id);
				value_prov.SetValue (value);
			}
		}
#endregion 
	}

	public class ListWithGrid : List, Atk.TableImplementor
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
			get { return tableExpert.Caption; } set { tableExpert.Caption = value; }
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

		// TODO: Remove next two methods when atk-sharp is fixed
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
