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
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class ListItem : ComponentAdapter, Atk.ITextImplementor,
	Atk.IActionImplementor, Atk.IEditableTextImplementor, Atk.IImageImplementor
	{
		private IInvokeProvider invokeProvider;
		private ISelectionItemProvider selectionItemProvider;
		private IToggleProvider toggleProvider;

		private ITextImplementor textExpert = null;
		private ActionImplementorHelper actionExpert = null;
		private ImageImplementorHelper imageExpert = null;
		private EditableTextImplementorHelper editableTextExpert;

		public ListItem (IRawElementProviderSimple provider) : base (provider)
		{
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
			selectionItemProvider = (ISelectionItemProvider)provider.GetPatternProvider(SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItemProvider == null)
				throw new ArgumentException ("ListItem should always implement ISelectionItemProvider");
			toggleProvider = (IToggleProvider) provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);

			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			imageExpert = new ImageImplementorHelper (this);
			actionExpert = new ActionImplementorHelper ();
			editableTextExpert = new EditableTextImplementorHelper (this, this, textExpert);

			// TODO: Localize the name?s
			actionExpert.Add ("click", "click", null, DoClick);
			if (toggleProvider != null)
				actionExpert.Add ("toggle", "toggle", null, DoToggle);
			if (invokeProvider != null)
				actionExpert.Add ("invoke", "invoke", null, DoInvoke);
			Role = (toggleProvider != null? Atk.Role.CheckBox: Atk.Role.ListItem);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.AddState (Atk.StateType.Transient);
			states.AddState (Atk.StateType.SingleLine);
			
			states.AddState (Atk.StateType.Selectable);
			if (selectionItemProvider.IsSelected)
				states.AddState (Atk.StateType.Selected);

			if (toggleProvider != null) {
				ToggleState state = toggleProvider.ToggleState;
				
				if (state == ToggleState.On)
					states.AddState (Atk.StateType.Checked);
				else
					states.RemoveState (Atk.StateType.Checked);
			}

			editableTextExpert.UpdateStates (states);

			return states;
		}

		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetStringAtOffset (int offset, Atk.TextGranularity granularity, out int startOffset, out int endOffset)
		{
			return textExpert.GetStringAtOffset (offset, granularity, out startOffset, out endOffset);
		}

		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
		}

		public Atk.Attribute [] DefaultAttributes {
			get { return textExpert.DefaultAttributes; }
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			return textExpert.GetOffsetAtPoint (x, y, coords);
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			return textExpert.AddSelection (startOffset, endOffset);
		}

		public bool RemoveSelection (int selectionNum)
		{
			return textExpert.RemoveSelection (selectionNum);
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return textExpert.SetSelection (selectionNum, startOffset, endOffset);
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			return textExpert.GetBoundedRanges (rect, coordType, xClipType, yClipType);
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationEvent (eventId, e))
				return;
			
			if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent) {
				List list = Parent as List;
				if (list != null)
					list.NotifyItemSelected (this);
				else
				NotifyStateChange (Atk.StateType.Selected, true);
			}
			else if (eventId == SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent)
				NotifyStateChange (Atk.StateType.Selected, true);
			else if (eventId == SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent)
				NotifyStateChange (Atk.StateType.Selected, false);
		}
		
		public int CaretOffset {
			get {
				return 0;
			}
		}

		public int CharacterCount {
			get {
				return textExpert.Length;
			}
		}

		// Return the number of actions (Read-Only)
		public int NActions
		{
			get {
				return actionExpert.NActions;
			}
		}
		
		// Get a localized name for the specified action
		public string GetLocalizedName (int action)
		{
			return actionExpert.GetLocalizedName (action);
		}
		
		// Sets a description of the specified action
		public bool SetDescription (int action, string description)
		{
			return actionExpert.SetDescription (action, description);
		}
		
		// Get the key bindings for the specified action
		public string GetKeybinding (int action)
		{
			return null;
		}

		// Get the name of the specified action
		public virtual string GetName (int action)
		{
			return actionExpert.GetName (action);
		}
		
		// Get the description of the specified action
		public string GetDescription (int action)
		{
			return actionExpert.GetDescription (action);
		}

		// Perform the action specified
		public virtual bool DoAction (int action)
		{
			return actionExpert.DoAction (action);
		}

		internal virtual bool DoClick ()
		{
			if (selectionItemProvider == null) 
				return false;

			try {
				selectionItemProvider.Select ();
				return true;
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
			}

			return false;
		}

		internal bool DoToggle ()
		{
			try {
				toggleProvider.Toggle ();
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
				return false;
			}
			return true;
		}

		internal bool DoInvoke ()
		{
			try {
				invokeProvider.Invoke ();
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
				return false;
			}
			return true;
		}
		public int NSelections {
			get {
				return -1;
			}
		}
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationPropertyChangedEvent (e))
				return;
			
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty) {
				bool focused = (bool) e.NewValue;
				Adapter parentAdapter = (Adapter) Parent;
				if (parentAdapter is List) {
					((List)parentAdapter).HandleItemFocus (this, focused);
					return;
					}
				parentAdapter.NotifyStateChange (Atk.StateType.Focused, focused);
			} else if (e.Property == TogglePatternIdentifiers.ToggleStateProperty)
				NotifyStateChange (Atk.StateType.Checked, IsChecked ((ToggleState)e.NewValue));
			else if (e.Property == AutomationElementIdentifiers.IsTogglePatternAvailableProperty) {
				if ((bool)e.NewValue == true) {
					toggleProvider = (IToggleProvider) Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
					actionExpert.Add ("toggle", "toggle", null, DoToggle);
				} else {
					toggleProvider = null;
					actionExpert.Remove ("toggle");
				}
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}

		private bool IsChecked (ToggleState state)
		{
			switch (state) {
			case ToggleState.On:
				return true;
			case ToggleState.Indeterminate:
			case ToggleState.Off:
				return false;
			default:
				throw new NotSupportedException ("Unknown toggleState " + state.ToString ());
			}
		}

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
			get { return editableTextExpert.TextContents; }
			set {
				editableTextExpert.TextContents = value;
			}
		}
		
		#endregion

		#region ImageImplementor implementation 
		
		public void GetImagePosition (out int x, out int y, Atk.CoordType coord_type)
		{
			imageExpert.GetImagePosition (out x, out y, coord_type);
		}
		
		public void GetImageSize (out int width, out int height)
		{
			imageExpert.GetImageSize (out width, out height);
		}
		
		public bool SetImageDescription (string description)
		{
			return imageExpert.SetImageDescription (description);
		}		
		
		public string ImageDescription {
			get {
				return imageExpert.ImageDescription;
			}
		}
		
		public string ImageLocale {
			get {
				return imageExpert.ImageLocale;
			}
		}
		
		#endregion 
	}
}
