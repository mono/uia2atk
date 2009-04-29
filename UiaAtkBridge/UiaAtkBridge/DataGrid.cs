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

	public class DataGrid : Table, Atk.SelectionImplementor
	{
		private ISelectionProvider		selectionProvider;
		private SelectionProviderUserHelper	selectionHelper;
		private bool hasFocus = false;
		
		public DataGrid (IRawElementProviderFragment provider): base (provider)
		{
			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new ArgumentException ("DataGrid should always implement ISelectionProvider");
			selectionHelper = new SelectionProviderUserHelper (provider, selectionProvider);
		}
		
		internal void HandleItemFocus (Adapter item, bool itemFocused)
		{
			bool tableFocused = (bool) Provider.GetPropertyValue (AutomationElementIdentifiers.HasKeyboardFocusProperty.Id);
			if (hasFocus != tableFocused) {
				NotifyStateChange (Atk.StateType.Focused, tableFocused);
				if (tableFocused)
					Atk.Focus.TrackerNotify (this);
			}
			if (itemFocused)
				GLib.Signal.Emit (this, "active-descendant-changed", item.Handle);
			hasFocus = tableFocused;
		}

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
	}
}
