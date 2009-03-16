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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class ComboBoxOptions : ComponentParentAdapter, Atk.SelectionImplementor
	{
		ISelectionProvider selectionProvider = null;
		SelectionProviderUserHelper selectionHelper = null;
		
		public ComboBoxOptions (IRawElementProviderSimple provider) : base (provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			if ((provider as IRawElementProviderFragment) == null)
				throw new ArgumentException ("Provider for ParentMenu should be IRawElementProviderFragment");

			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new ArgumentException ("The List inside the ComboBox should always implement ISelectionProvider");

			selectionHelper = new SelectionProviderUserHelper (provider as IRawElementProviderFragment, selectionProvider);

			Role = Atk.Role.Menu;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			ComboBoxDropDown dropdown = Parent as ComboBoxDropDown;
			if (dropdown != null) {
				states.RemoveState (Atk.StateType.Focusable);
				
				if (dropdown.ExpandCollapseState == ExpandCollapseState.Collapsed ||
				    !dropdown.IsEditable)
					states.RemoveState (Atk.StateType.Focused);
			}
			return states;
		}

		#region SelectionImplementor implementation //FIXME: consider making ComboBoxOptions inherit from List
		
		public int SelectionCount {
			get { return selectionHelper.SelectionCount; }
		}
		
		public virtual bool AddSelection (int i)
		{
			return selectionHelper.AddSelection (i);
			//return ((ComboBoxItem)RefAccessibleChild (i)).DoAction (0);
		}

		public virtual bool ClearSelection ()
		{
			return selectionHelper.ClearSelection ();
			//in the past, we did this because ComboBox doesn't support this: return (SelectionCount == 0);
		}

		public Atk.Object RefSelection (int i)
		{
			return selectionHelper.RefSelection (i);
		}

		public bool IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected (i);
		}

		public bool RemoveSelection (int i)
		{
			return selectionHelper.RemoveSelection (i);
			//in the past, we did this because ComboBox doesn't support this: return false;
		}

		public bool SelectAllSelection ()
		{
			return selectionHelper.SelectAllSelection ();
			//in the past, we did this because ComboBox doesn't support this: return false;
		}

		#endregion
		
		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			//TODO
		}

		internal void RecursiveDeselect (Adapter keepSelected)
		{
			int nChildren = 0;
			lock (syncRoot) {
				nChildren = NAccessibleChildren;
			}

			for (int i = 0; i < nChildren; i++) {
				Atk.Object child = RefAccessibleChild (i);

				if (child == null || ((Adapter)child) == keepSelected) {
					continue;
				}
				
				ComboBoxItem item = child as ComboBoxItem;
				if (item != null)
					item.Deselect ();
			}

			if (Parent is ComboBoxOptions)
				((ComboBoxOptions)Parent).RecursiveDeselect (keepSelected);
			else if (Parent is ComboBox)
				((ComboBox)Parent).RaiseSelectionChanged (keepSelected.Name);
		}

		internal void RaiseExpandedCollapsed ()
		{
			NotifyStateChange (Atk.StateType.Showing);
			NotifyStateChange (Atk.StateType.Visible);
		}

		internal override void RemoveChild (Atk.Object childToRemove)
		{
			int childIndex = children.IndexOf (childToRemove);
			bool cancelSelection = false;
			if (IsChildSelected (childIndex))
				cancelSelection = true;
			base.RemoveChild (childToRemove);
			if (children.Count <= 0 || cancelSelection)
				((ComboBox)Parent).RaiseSelectionChanged (null);
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			base.RaiseAutomationPropertyChangedEvent (e);
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
		}
	}
}
