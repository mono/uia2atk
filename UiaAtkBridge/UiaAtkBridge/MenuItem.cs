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
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class MenuItem : ComponentParentAdapter, 
	                        Atk.SelectionImplementor, Atk.TextImplementor,
	                        Atk.ActionImplementor, ICanHaveSelection
	{
		ITextImplementor textExpert = null;
		IInvokeProvider invokeProvider = null;
		
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

			int controlType = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			
			OnChildrenChanged ();
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			states.AddState (Atk.StateType.Selectable);

			if (selected)
				states.AddState (Atk.StateType.Showing);
			else
				states.RemoveState (Atk.StateType.Showing);

			if (Parent != null && Parent.RefStateSet ().ContainsState (Atk.StateType.Visible)) {
				states.AddState (Atk.StateType.Visible);
				states.AddState (Atk.StateType.Showing);
			}

			bool canFocus = (bool) Provider.GetPropertyValue (
			     AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			if (canFocus)
				states.AddState (Atk.StateType.Focusable);

			if (selected) {
				states.AddState (Atk.StateType.Selected);

				if (canFocus) {
					states.AddState (Atk.StateType.Focused);
				}
			} else {
				states.RemoveState (Atk.StateType.Selected);
			}

			return states;
		}

		private void OnChildrenChanged () 
		{
			IRawElementProviderFragment child = ((IRawElementProviderFragment)Provider).Navigate (NavigateDirection.FirstChild);
			
			if (child != null)
				Role = Atk.Role.Menu;
			else
				Role = Atk.Role.MenuItem;
		}
		
		protected override void OnChildrenChanged (uint change_index, IntPtr changed_child) 
		{
			OnChildrenChanged ();
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

		internal void Deselect ()
		{
			if (!selected)
				return;

			selected = false;
			NotifyStateChange (Atk.StateType.Selected, false);
		}

		#region ICanHaveSelection implementation

		void ICanHaveSelection.RecursivelyDeselectAll (Adapter keepSelected)
		{
			if (Parent is ICanHaveSelection) {
				((ICanHaveSelection) Parent).RecursivelyDeselectAll (
					keepSelected);
				return;
			}

			((ICanHaveSelection) this).RecursivelyDeselect (keepSelected);
		}

		void ICanHaveSelection.RecursivelyDeselect (Adapter keepSelected)
		{
			if (this != keepSelected)
				Deselect ();

			lock (syncRoot) {
				bool changed_selected_child = false;
				bool any_child_was_selected = (selectedChild >= 0);
				for (int i = 0; i < NAccessibleChildren; i++) {
					Atk.Object child = RefAccessibleChild (i);
					if (child == null)
						continue;

					if (((Adapter) child) == keepSelected) {
						if (selectedChild != i) {
							selectedChild = i;
							changed_selected_child = true;
						}
						continue;
					}
					
					if (child is ICanHaveSelection) {
						((ICanHaveSelection) child)
							.RecursivelyDeselect (keepSelected);
					}
				}
				if (changed_selected_child) {
					var sel_adapter = new Atk.SelectionAdapter (this);
					if (any_child_was_selected)
						//2 times: because we deselect a child and select another one
						sel_adapter.EmitSelectionChanged ();
					sel_adapter.EmitSelectionChanged ();
				}
			}
		}
		
		#endregion
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == InvokePatternIdentifiers.InvokedEvent ||
			    eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent) {
				if (!selected) {
					selected = true;
					NotifyStateChange (Atk.StateType.Selected, selected);
				}
				((ICanHaveSelection) this).RecursivelyDeselectAll (this);
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				selected = !selected;
				NotifyStateChange (Atk.StateType.Selected, selected);
				if (selected)
					//this causes the following in accerciser: focus:(0, 0, None)
					Atk.Focus.TrackerNotify (this);
				((ICanHaveSelection) this).RecursivelyDeselectAll (selected ? this : null);
			} else {
				Console.WriteLine ("WARNING: RaiseAutomationEvent({0},...) not handled yet", eventId.ProgrammaticName);
				base.RaiseAutomationEvent (eventId, e);
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id == SelectionItemPatternIdentifiers.IsSelectedProperty.Id) {
				selected = (bool)e.NewValue;
				NotifyStateChange (Atk.StateType.Selected, selected);
			} else if (e.Property.Id == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				NotifyStateChange (Atk.StateType.Showing);
			} else {
				base.RaiseAutomationPropertyChangedEvent (e);
			}
		}

		#region Action implementation 

		private string actionDescription = null;
		
		public bool DoAction (int i)
		{
			if (i == 0) {
				if (invokeProvider != null) {
					try {
						invokeProvider.Invoke ();
						return true;
					} catch (ElementNotEnabledException) { }
				}
			}
			return false;
		}
		
		public string GetName (int i)
		{
			if (i == 0 && invokeProvider != null)
				return "click";
			return null;
		}
		
		public string GetKeybinding (int i)
		{
			return null;
		}
		
		public string GetLocalizedName (int i)
		{
			return null;
		}
		
		public bool SetDescription (int i, string desc)
		{
			if (i == 0) {
				actionDescription = desc;
				return true;
			}
			return false;
		}
		
		public string GetDescription (int i)
		{
			if (i == 0)
				return actionDescription;
			return null;
		}

		
		public int NActions {
			get { return 1; }
		}
		
		#endregion 

		#region SelectionImplementor implementation
		//cannot use SelectionProviderHelper because it doesn't implement ISelectionProvider
		//(consider inheriting from MenuBar in the future)

		private bool selected = false;
		private int selectedChild = -1;
		
		public bool AddSelection (int i)
		{
			if ((i < 0) || (i >= NAccessibleChildren))
				return false;
			return ((MenuItem)RefAccessibleChild (i)).DoAction (0);
		}

		public bool SelectAllSelection ()
		{
			return false;
		}

		public bool IsChildSelected (int i)
		{
			return selectedChild == i;
		}
		
		bool Atk.SelectionImplementor.RemoveSelection (int i)
		{
			if (i == 0) {
				selectedChild = -1;
				((ICanHaveSelection) this).RecursivelyDeselect (null);
				return true;
			}
			return false;
		}

		public bool ClearSelection ()
		{
			selectedChild = -1;
			((ICanHaveSelection) this).RecursivelyDeselect (null);
			return true;
		}

		public int SelectionCount {
			get { return (selectedChild < 0 ? 0 : 1); }
		}

		public Atk.Object RefSelection (int i)
		{
			if (selectedChild < 0 || i != 0)
				return null;
			return RefAccessibleChild (selectedChild);
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
			return false;
		}

		bool Atk.TextImplementor.RemoveSelection (int i)
		{
			return false;
		}
		
		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
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

		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			//TODO
		}

	}
}
