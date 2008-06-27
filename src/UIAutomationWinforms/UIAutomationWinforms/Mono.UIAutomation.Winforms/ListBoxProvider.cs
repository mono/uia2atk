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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using System.Windows;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public class ListBoxProvider : FragmentRootControlProvider
	{
		
#region Constructor 

		public ListBoxProvider (ListBox listbox) : base (listbox)
		{
			this.listbox = listbox;
			items = new  Dictionary<int,ListBoxItemProvider> ();
			
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new ListBoxSelectionProviderBehavior (this));
			SetBehavior (ScrollPatternIdentifiers.Pattern,
			             new ListBoxScrollProviderBehavior (this));
			
			Navigation = new ListBoxNavigation (this);
		}

#endregion
		
#region FragmentControlProvider Overrides
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			throw new NotImplementedException ();
		}
		
		public override IRawElementProviderFragment GetFocus ()
		{
			return GetListBoxItem (listbox.SelectedIndex);
		}
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			if (direction == NavigateDirection.FirstChild)
				return GetListBoxItem (0);
			if (direction == NavigateDirection.LastChild)
				return GetListBoxItem (listbox.Items.Count - 1);
			else
				return base.Navigate (direction);
		}

#endregion
		
#region Public Methods
		
		public ListBoxItemProvider GetListBoxItem (int index)
		{
			ListBoxItemProvider item;
			
			if (listbox.Items.Count == 0 || index < 0 || index >= listbox.Items.Count)
				return null;
			else if (items.TryGetValue (index, out item) == false) {
				item = new ListBoxItemProvider (this, listbox, index);
				items [index] = item;
			}

			return item;
		}
		
		public ListBoxItemProvider[] GetSelectedListBoxItems ()
		{
			ListBoxItemProvider []items;
			
			if (listbox.SelectedIndices.Count == 0)
				return null;
			
			items = new ListBoxItemProvider [listbox.SelectedIndices.Count];
			
			for (int index = 0; index < items.Length; index++) 
				items [index] = GetListBoxItem (listbox.SelectedIndices [index]);
			
			return items;
		}
		
#endregion

#region Private fields

		private ListBox listbox;
		private Dictionary<int, ListBoxItemProvider> items;

#endregion

	}
}
