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

namespace UiaAtkBridge
{

	public class TreeItem : ComponentAdapter, Atk.TextImplementor, Atk.ActionImplementor
	{
		private IInvokeProvider				invokeProvider;
		private ISelectionItemProvider		selectionItemProvider;

		private ITextImplementor textExpert = null;
		private ActionImplementorHelper actionExpert = null;

		public TreeItem (IRawElementProviderSimple provider) : base (provider)
		{
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
			selectionItemProvider = (ISelectionItemProvider)provider.GetPatternProvider(SelectionItemPatternIdentifiers.Pattern.Id);
			if (selectionItemProvider == null)
				throw new ArgumentException ("TreeItem should always implement ISelectionItemProvider");

			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			actionExpert = new ActionImplementorHelper ();

			// TODO: Localize the name?s
			actionExpert.Add ("click", "click", null, DoClick);
			if (ToggleProvider != null)
				actionExpert.Add ("toggle", "toggle", null, DoToggle);
			if (invokeProvider != null)
				actionExpert.Add ("invoke", "invoke", null, DoInvoke);
			Name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Role = (ToggleProvider != null? Atk.Role.CheckBox: Atk.Role.TableCell);
		}
		
		protected IToggleProvider ToggleProvider {
			get {
				return (IToggleProvider) Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			}
		}

		protected IExpandCollapseProvider ExpandCollapseProvider {
			get {
				return (IExpandCollapseProvider) Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			}
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			states.AddState (Atk.StateType.Transient);
			states.AddState (Atk.StateType.SingleLine);
			
			states.AddState (Atk.StateType.Selectable);
			if (selectionItemProvider.IsSelected)
				states.AddState (Atk.StateType.Selected);

			IToggleProvider toggleProvider = ToggleProvider;
			if (toggleProvider != null) {
				ToggleState state = toggleProvider.ToggleState;
				
				if (state == ToggleState.On)
					states.AddState (Atk.StateType.Checked);
				else
					states.RemoveState (Atk.StateType.Checked);
			}

			IExpandCollapseProvider expandCollapseProvider = ExpandCollapseProvider;
			if (expandCollapseProvider != null) {
				ExpandCollapseState expandCollapseState = (ExpandCollapseState)Provider.GetPropertyValue (ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
				if (expandCollapseState != ExpandCollapseState.LeafNode)
					states.AddState (Atk.StateType.Expandable);
				if (expandCollapseState == ExpandCollapseState.Expanded)
					states.AddState (Atk.StateType.Expanded);
			}

			return states;
		}

		protected override Atk.RelationSet OnRefRelationSet ()
		{
			Atk.RelationSet relSet = base.OnRefRelationSet ();
			IRawElementProviderFragment fragment = Provider as IRawElementProviderFragment;
			IRawElementProviderFragment parentProvider = fragment.Navigate (NavigateDirection.Parent);
			if (parentProvider != null) {
				Atk.Object parent = AutomationBridge.GetAdapterForProviderLazy (parentProvider);
				if (parent != null)
					relSet.AddRelationByType (Atk.RelationType.NodeChildOf, parent);
			}
			return relSet;
		}

		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		private int selectionStartOffset = 0, selectionEndOffset = 0;
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			return ret;
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
			throw new NotImplementedException();
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
			return null;
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			return false;
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
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
			throw new NotImplementedException();
		}
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent) {
				Tree list = Parent as Tree;
				if (list != null)
					list.NotifyItemSelected (this);
				else
				NotifyStateChange ((ulong) Atk.StateType.Selected, true);
			}
			else if (eventId == SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent)
				NotifyStateChange ((ulong) Atk.StateType.Selected, true);
			else if (eventId == SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent)
				NotifyStateChange ((ulong) Atk.StateType.Selected, false);
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
			Atk.SelectionImplementor sel = Parent as Atk.SelectionImplementor;
			if (sel == null)
				return false;
			return sel.AddSelection (IndexInParent);
		}

		internal bool DoToggle ()
		{
			try {
				ToggleProvider.Toggle();
			} catch (ElementNotEnabledException) {
				// TODO: handle this exception?
				return false;
			}
			return true;
		}

		internal bool DoInvoke ()
		{
			try {
				invokeProvider.Invoke ();
			} catch (ElementNotEnabledException) {
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
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty) {
				bool focused = (bool) e.NewValue;
				Adapter parentAdapter = (Adapter) Parent;
				if (parentAdapter is Tree) {
					((Tree)parentAdapter).HandleItemFocus (this, focused);
					return;
					}
				parentAdapter.NotifyStateChange (Atk.StateType.Focused, focused);
				if (focused)
					Atk.Focus.TrackerNotify (parentAdapter);
			} else if (e.Property == ValuePatternIdentifiers.ValueProperty) {
				String stringValue = (String)e.NewValue;
				
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.Text == stringValue)
					return;

				Atk.TextAdapter adapter = new Atk.TextAdapter (this);

				// First delete all text, then insert the new text
				adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

				adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                         stringValue == null ? 0 : stringValue.Length);

				// Accessible name and label text are one and
				// the same, so update accessible name
				Name = stringValue;

				EmitVisibleDataChanged ();
			} else if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				NotifyStateChange (Atk.StateType.Checked, IsChecked ((ToggleState)e.NewValue));
			} else if (e.Property == AutomationElementIdentifiers.IsTogglePatternAvailableProperty) {
				if ((bool)e.NewValue == true) {
					actionExpert.Add ("toggle", "toggle", null, DoToggle);
				} else {
					actionExpert.Remove ("toggle");
				}
			} else if (e.Property == ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty) {
				ExpandCollapseState oldValue = (ExpandCollapseState)e.OldValue;
				ExpandCollapseState newValue = (ExpandCollapseState)e.NewValue;
				if (oldValue == ExpandCollapseState.LeafNode || newValue == ExpandCollapseState.LeafNode)
					NotifyStateChange (Atk.StateType.Expandable, (newValue != ExpandCollapseState.LeafNode));
				if (oldValue == ExpandCollapseState.Expanded || newValue == ExpandCollapseState.Expanded) {
					bool expanded = (newValue == ExpandCollapseState.Expanded);
					NotifyStateChange (Atk.StateType.Expanded, expanded);
					((Tree)Parent).NotifyRowAdjusted (this, expanded);
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
	}
}
