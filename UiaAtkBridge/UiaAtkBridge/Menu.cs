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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;

namespace UiaAtkBridge
{
	public abstract class Menu : ComponentParentAdapter, ICanHaveSelection
		//although we're implementing selection methods, don't do this (ParentMenu will use them):
		//,Atk.SelectionImplementor
	{
		public Menu (IRawElementProviderSimple provider) : base (provider)
		{
		}
		
		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			//TODO
			return;
		}

		//cannot use SelectionProviderHelper because it doesn't implement ISelectionProvider:
		#region SelectionImplementor implementation

		//cannot use prov.GetPropertyValue(IsSelectedProperty.Id) because returns null
		protected bool selected = false;
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
		
		public bool RemoveSelection (int i)
		{
			if (i == 0) {
				selectedChild = -1;
				((ICanHaveSelection)this).RecursivelyDeselect (null);
				return true;
			}
			return false;
		}

		public bool ClearSelection ()
		{
			selectedChild = -1;
			((ICanHaveSelection)this).RecursivelyDeselect (null);
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

		#region ICanHaveSelection implementation

		void ICanHaveSelection.RecursivelyDeselectAll (Adapter keepSelected)
		{
			if (Parent is ICanHaveSelection) {
				((ICanHaveSelection) Parent).RecursivelyDeselectAll (keepSelected);
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
					Atk.SelectionImplementor selImplementor = this as Atk.SelectionImplementor;
					if (selImplementor != null) {
						var sel_adapter = new Atk.SelectionAdapter (selImplementor);
						if (any_child_was_selected)
							//2 times: because we deselect a child and select another one
							sel_adapter.EmitSelectionChanged ();
						sel_adapter.EmitSelectionChanged ();
					}
				}
			}
		}
		
		#endregion

		internal void Deselect ()
		{
			if (!selected)
				return;

			selected = false;
			NotifyStateChange (Atk.StateType.Selected, false);
		}
		
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
				Log.Warn ("MenuItem: RaiseAutomationEvent({0},...) not implemented", eventId.ProgrammaticName);
				base.RaiseAutomationEvent (eventId, e);
			}
		}
	}
}
