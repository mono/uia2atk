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
using SCG = System.Collections.Generic;

namespace UiaAtkBridge
{

	public class TreeItem : ComponentAdapter, Atk.ITextImplementor, Atk.IActionImplementor,
		Atk.IImageImplementor, Atk.IEditableTextImplementor
	{
		protected const string EXPAND_OR_CONTRACT_ACTION_NAME = "expand or contract";
		protected const string EXPAND_OR_CONTRACT_ACTION_DESC =
			"expands or contracts the row in the tree view containing this cell";

		protected IInvokeProvider			invokeProvider;
		private ISelectionItemProvider		selectionItemProvider;
		private IExpandCollapseProvider		expandCollapseProvider;

		private ITextImplementor textExpert = null;
		internal ActionImplementorHelper actionExpert = null;
		private ImageImplementorHelper imageExpert = null;
		private EditableTextImplementorHelper editableTextExpert = null;

		public TreeItem (IRawElementProviderSimple provider) : base (provider)
		{
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider(InvokePatternIdentifiers.Pattern.Id);
			selectionItemProvider = (ISelectionItemProvider)provider.GetPatternProvider(SelectionItemPatternIdentifiers.Pattern.Id);

			expandCollapseProvider = provider.GetPatternProvider (
				ExpandCollapsePatternIdentifiers.Pattern.Id) as IExpandCollapseProvider;

			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			actionExpert = new ActionImplementorHelper ();

			// TODO: Localize the name?s
			actionExpert.Add ("click", "click", null, DoClick);
			if (ToggleProvider != null)
				actionExpert.Add ("toggle", "toggle", null, DoToggle);
			if (invokeProvider != null)
				actionExpert.Add ("invoke", "invoke", null, DoInvoke);

			IRawElementProviderFragment fragment = Provider as IRawElementProviderFragment;
			if (fragment != null && fragment.Navigate (NavigateDirection.FirstChild) != null)
				AddExpandContractAction ();

			Role = (ToggleProvider != null? Atk.Role.CheckBox: Atk.Role.TableCell);

			imageExpert = new ImageImplementorHelper (this);
			editableTextExpert = new EditableTextImplementorHelper (this, this, textExpert);
		}
		
		protected IToggleProvider ToggleProvider {
			get {
				return (IToggleProvider) Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			}
		}

		protected IExpandCollapseProvider ExpandCollapseProvider {
			get { return expandCollapseProvider; }
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.AddState (Atk.StateType.Transient);
			states.AddState (Atk.StateType.SingleLine);

			if (selectionItemProvider != null) {
				states.AddState (Atk.StateType.Selectable);
				if (selectionItemProvider.IsSelected)
					states.AddState (Atk.StateType.Selected);
			}

			IToggleProvider toggleProvider = ToggleProvider;
			if (toggleProvider != null) {
				ToggleState state = toggleProvider.ToggleState;
				
				if (state == ToggleState.On)
					states.AddState (Atk.StateType.Checked);
				else
					states.RemoveState (Atk.StateType.Checked);
			}

			if (expandCollapseProvider != null) {
				ExpandCollapseState expandCollapseState
					= (ExpandCollapseState) Provider.GetPropertyValue (
					ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
				if (expandCollapseState != ExpandCollapseState.LeafNode)
					states.AddState (Atk.StateType.Expandable);
				if (expandCollapseState == ExpandCollapseState.Expanded)
					states.AddState (Atk.StateType.Expanded);
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
			if (editableTextExpert.RaiseAutomationEvent (eventId, e)
				|| textExpert.RaiseAutomationEvent (eventId, e))
				return;
			
			if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent) {
				Tree list = Parent as Tree;
				if (list != null)
					list.NotifyItemSelected (this);
				else
				NotifyStateChange (Atk.StateType.Selected, true);
			}
			else if (eventId == SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent)
				NotifyStateChange (Atk.StateType.Selected, true);
			else if (eventId == SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent) {
				NotifyStateChange (Atk.StateType.Selected, false);
				Tree list = Parent as Tree;
				if (list != null)
					list.NotifyItemSelectionRemoved (this);
			}
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
				return false;
			}
		}

		internal bool DoToggle ()
		{
			try {
				ToggleProvider.Toggle ();
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

		internal bool DoExpandCollapse ()
		{
			if (expandCollapseProvider == null)
				return false;

			ExpandCollapseState expandCollapseState
				= (ExpandCollapseState) Provider.GetPropertyValue (
					ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id);
			if (expandCollapseState == ExpandCollapseState.Expanded) {
				try {
					expandCollapseProvider.Collapse ();
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}
			} else {
				try {
					expandCollapseProvider.Expand ();
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}
			}

			return true;
		}

		internal void AddExpandContractAction ()
		{
			if (expandCollapseProvider == null) 
				return;

			actionExpert.Add (EXPAND_OR_CONTRACT_ACTION_NAME,
					  EXPAND_OR_CONTRACT_ACTION_NAME,
					  EXPAND_OR_CONTRACT_ACTION_DESC,
					  DoExpandCollapse);
		}

		internal void NotifyChildAdded (Atk.Object child)
		{
			AddExpandContractAction ();
		}
		
		internal void NotifySomeChildRemoved (Atk.Object childToRemove)
		{
			if (!VirtualChildren)
				actionExpert.Remove (EXPAND_OR_CONTRACT_ACTION_NAME);
		}

		public int NSelections {
			get {
				return -1;
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationPropertyChangedEvent (e)
				|| textExpert.RaiseAutomationPropertyChangedEvent (e))
				return;
			
			if (e.Property == AutomationElementIdentifiers.HasKeyboardFocusProperty) {
				bool focused = (bool) e.NewValue;
				Adapter parentAdapter = (Adapter) Parent;
				if (parentAdapter is Tree) {
					((Tree)parentAdapter).HandleItemFocus (this, focused);
					return;
					}
				if (parentAdapter is DataGrid) {
					((DataGrid)parentAdapter).HandleItemFocus (this, focused);
					return;
					}
				parentAdapter.NotifyStateChange (Atk.StateType.Focused, focused);
				if (focused)
					Atk.Focus.TrackerNotify (parentAdapter);
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

		//FIXME: this should be the real implementation of VirtualParent in the Adapter class, and later each class should
		//override the special cases, not the other way around (so the AutomationBridge.GetParentAdapter method gets OOP)
		internal override Adapter VirtualParent {
			get {
				Adapter parent = null;
				IRawElementProviderFragment frag = Provider as IRawElementProviderFragment;
				if (frag == null)
					return null;

				//we need this loop because in some ListViews there are parentTreeItems 
				//that have the same content as this TreeItem and don't have an Adapter
				while (parent == null) {
					frag = frag.Navigate (NavigateDirection.Parent);
					if (frag == null)
						break;
					parent = AutomationBridge.GetAdapterForProviderSemiLazy (frag);
				}
				return parent;
			}
		}


		#region Atk.IImageImplementor implementation
		
		public void GetImagePosition (out int x, out int y, Atk.CoordType coordType)
		{
			imageExpert.GetImagePosition (out x, out y, coordType);
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
			get { return imageExpert.ImageDescription; }
		}

		public string ImageLocale {
			get { return imageExpert.ImageLocale; }
		}

		#endregion

		#region Atk.EditableTextImplementor implementation 
		
		public bool SetRunAttributes (GLib.SList attribSet, int startOffset, int endOffset)
		{
			return editableTextExpert.SetRunAttributes (attribSet, 
			                                            startOffset,
			                                            endOffset);
		}
		
		public void InsertText (string str, ref int position)
		{
			editableTextExpert.InsertText (str, ref position);
		}
		
		public void CopyText (int startPos, int endPos)
		{
			editableTextExpert.CopyText (startPos, endPos);
		}
		
		public void CutText (int startPos, int endPos)
		{
			editableTextExpert.CutText (startPos, endPos);
		}
		
		public void DeleteText (int startPos, int endPos)
		{
			editableTextExpert.DeleteText  (startPos, endPos);
		}
		
		public void PasteText (int position)
		{
			editableTextExpert.PasteText (position);
		}
		
		public string TextContents {
			get { return editableTextExpert.TextContents; }
			set { editableTextExpert.TextContents = value; }
		}
		
		#endregion
	}
}
