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
	public class MenuBar : ComponentParentAdapter, Atk.SelectionImplementor, ICanHaveSelection
	{
		public MenuBar (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.MenuBar;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			//FIXME: figure out why MenuItem elements in Gail don't like this state
			states.RemoveState (Atk.StateType.Focusable);
			return states;
		}
		
		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			//TODO
			return;
		}

		#region ICanHaveSelection implementation

		void ICanHaveSelection.RecursivelyDeselectAll (Adapter keepSelected)
		{
			((ICanHaveSelection) this).RecursivelyDeselect (keepSelected);
		}

		void ICanHaveSelection.RecursivelyDeselect (Adapter keepSelected)
		{
			lock (syncRoot) {
				for (int i = 0; i < NAccessibleChildren; i++) {
					Atk.Object child = RefAccessibleChild (i);
					if (child is ICanHaveSelection) {
						((ICanHaveSelection) child).RecursivelyDeselect (keepSelected);
					}
				}
			}
		}

		#endregion

		#region SelectionImplementor implementation 
		//NOTE: MenuBar in UIA does not implement the selection pattern

		private bool selected = false;
		private int selectedChild = -1;
		
		public bool AddSelection (int i)
		{
			if ((i < 0) || (i >= NAccessibleChildren))
				return false;
			return ((MenuItem)RefAccessibleChild (i)).DoAction (0);
		}
		
		public bool ClearSelection ()
		{
			//TODO
			return true;
		}
		
		public Atk.Object RefSelection (int i)
		{
			//TODO
			return null;
		}
		
		public bool IsChildSelected (int i)
		{
			//TODO
			return false;
		}
		
		public bool RemoveSelection (int i)
		{
			//TODO
			return true;
		}
		
		public bool SelectAllSelection ()
		{
			//TODO
			return false;
		}
		
		
		public int SelectionCount {
			get {
				//TODO
				return 0;
			}
		}
		
		#endregion 
		
	}
}
