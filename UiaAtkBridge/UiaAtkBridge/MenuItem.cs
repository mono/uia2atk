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
// Copyright (c) 2008, 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;

using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace UiaAtkBridge
{
	
	public class MenuItem : Menu, Atk.ActionImplementor, Atk.TextImplementor
	{
		ITextImplementor textExpert = null;
		IInvokeProvider invokeProvider = null;
		IToggleProvider toggleProvider = null;
		IExpandCollapseProvider expandCollapseProvider = null;
		ISelectionItemProvider selectionItemProvider = null;
		ActionImplementorHelper actionExpert = new ActionImplementorHelper ();
		
		public MenuItem (IRawElementProviderSimple provider) : base (provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			if ((provider as IRawElementProviderFragment) == null)
				throw new ArgumentException ("Provider for ParentMenu should be IRawElementProviderFragment");

			textExpert = TextImplementorFactory.GetImplementor (this, provider);

			string name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			if (!String.IsNullOrEmpty (name))
				Name = name;

			invokeProvider = (IInvokeProvider)
				provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			toggleProvider = (IToggleProvider)
				provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
			selectionItemProvider = (ISelectionItemProvider)
				provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
			expandCollapseProvider = (IExpandCollapseProvider)
				provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);

			actionExpert.Add ("click", "click", null, DoClick);
			
			Role = Atk.Role.MenuItem;
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			states.AddState (Atk.StateType.Selectable);

			if (Parent != null) {
				Atk.StateSet parentsStates = Parent.RefStateSet ();
				if (parentsStates.ContainsState (Atk.StateType.Visible))
					states.AddState (Atk.StateType.Visible);

				if ((Parent is MenuBar && parentsStates.ContainsState (Atk.StateType.Visible))
				    || parentsStates.ContainsState (Atk.StateType.Selected))
					states.AddState (Atk.StateType.Showing);
			}

			if (states.ContainsState (Atk.StateType.Showing)
			    && states.ContainsState (Atk.StateType.Focused)) {
				states.AddState (Atk.StateType.Selected);
			} else {
				states.RemoveState (Atk.StateType.Selected);
			}

			if (Checked || SelectionItemSelected)
				states.AddState (Atk.StateType.Checked);
			else
				states.RemoveState (Atk.StateType.Checked);

			return states;
		}

//TODO: here we should drop and create new Atk.Objects, because we cannot change their
//	implementation on the fly (menuitem elts don't have Atk.Selection, menu elts do)
		protected override void OnChildrenChanged (uint change_index, IntPtr changed_child) 
		{
		}

		internal override void RequestChildren ()
		{
			base.RequestChildren ();

			IRawElementProviderFragment fragmentProvider =
				Provider as IRawElementProviderFragment;
			var child = fragmentProvider.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				if (ControlType.Menu.Id.Equals (child.GetPropertyValue (AEIds.ControlTypeProperty.Id)))
					AutomationBridge.AddChildrenToParent (child);
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}

		public override Atk.Layer Layer {
			get { return IsToolBarItem? Atk.Layer.Widget: Atk.Layer.Popup; }
		}

		internal bool IsToolBarItem {
			get {
				Adapter adapter = Parent as Adapter;
				for (;;) {
					Atk.Object parent = adapter.Parent;
					if (adapter.Provider != null && (int)adapter.Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.ToolBar.Id)
						return true;
					if (parent == null || parent == adapter)
						return false;
					adapter = parent as Adapter;
				}
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				selected = (bool) e.NewValue ? false : selected;
				NotifyStateChange (Atk.StateType.Showing);
			} else if (e.Property.Id == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id) {
				NotifyStateChange (Atk.StateType.Selected, (bool) e.NewValue);
				base.RaiseAutomationPropertyChangedEvent (e);
			} else if (e.Property == AutomationElementIdentifiers.IsTogglePatternAvailableProperty) {
				toggleProvider = (IToggleProvider)
					Provider.GetPatternProvider (TogglePatternIdentifiers.Pattern.Id);
				NotifyStateChange (Atk.StateType.Checked, Checked);
			} else if (e.Property == AutomationElementIdentifiers.IsInvokePatternAvailableProperty) {
				invokeProvider = (IInvokeProvider)
					Provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			} else if (e.Property == AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty) {
				expandCollapseProvider = (IExpandCollapseProvider)
					Provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
			} else if (e.Property == AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty) {
				selectionItemProvider = (ISelectionItemProvider)
					Provider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id);
				NotifyStateChange (Atk.StateType.Checked, SelectionItemSelected);
			} else {
				base.RaiseAutomationPropertyChangedEvent (e);
			}
		}

		private bool Checked {
			get {
				return toggleProvider != null &&
					toggleProvider.ToggleState == ToggleState.On;
			}
		}

		private bool SelectionItemSelected {
			get {
				return selectionItemProvider != null &&
					selectionItemProvider.IsSelected;
			}
		}

		#region Action implementation

		private bool DoClick ()
		{
			if (invokeProvider != null) {
				try {
					invokeProvider.Invoke ();
					return true;
				} catch (ElementNotEnabledException) {}
			} else if (expandCollapseProvider != null) {
				try {
					switch (expandCollapseProvider.ExpandCollapseState) {
					case ExpandCollapseState.Collapsed:
						expandCollapseProvider.Expand ();
						return true;
					case ExpandCollapseState.Expanded:
						expandCollapseProvider.Collapse ();
						return true;
					default:
						// Should never happen
						break;
					}
				} catch (ElementNotEnabledException) { }
			}

			return false;
		}
		
		public bool DoAction (int i)
		{
			return actionExpert.DoAction (i);
		}
		
		public string GetName (int i)
		{
			return actionExpert.GetName (i);
		}
		
		public string GetKeybinding (int i)
		{
			return null;
		}
		
		public string GetLocalizedName (int i)
		{
			return actionExpert.GetLocalizedName (i);
		}
		
		public bool SetDescription (int i, string desc)
		{
			return actionExpert.SetDescription (i, desc);
		}
		
		public string GetDescription (int i)
		{
			return actionExpert.GetDescription (i);
		}

		
		public int NActions {
			get { return actionExpert.NActions; }
		}
		
		#endregion 
		

		#region TextImplementor implementation 
		
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
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
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
			throw new NotImplementedException ();
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
		}
		
		public bool AddSelection (int startOffset, int endOffset)
		{
			return textExpert.AddSelection (startOffset, endOffset);
		}

		bool Atk.TextImplementor.RemoveSelection (int i)
		{
			return textExpert.RemoveSelection (i);
		}
		
		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return textExpert.SetSelection (selectionNum, startOffset, endOffset);
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
			throw new NotImplementedException ();
		}
		
		public int CaretOffset {
			get { return 0; }
		}
		
		public int CharacterCount {
			get { return textExpert.Length; }
		}
		
		public int NSelections {
			get { return -1; }
		}
		
		#endregion

		//HACK: when I finished this refactoring I realized that MenuItem should not inherit from ComponentParentAdapter, so
		//FIXME: to overcome the need of multiple inheritance, use an ImplementorHelper for selection related features and
		//       remove this transformation (although it may be useful for the case in which menus are added/removed)
		internal override void AddOneChild (Atk.Object child)
		{
			AutomationBridge.PerformTransformation <ParentMenu> (this, new ParentMenu (Provider)).AddOneChild (child);
		}
		
		protected virtual void AddChildToParent (Atk.Object child)
		{
			base.AddOneChild (child);
		}
	}
}
