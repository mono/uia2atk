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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{
	
	public abstract class ListProvider : FragmentRootControlProvider
	{

#region Constructors

		protected ListProvider (ListControl control) : base (control)
		{
			list_control = control;
			items = new Dictionary<int, ListItemProvider> ();
			
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new ListSelectionProviderBehavior (this));
		}
		
#endregion
		
#region Public Properties

		public ListControl ListControl {
			get { return list_control; }
		}
	
		public abstract int ItemsCount { get; }
		
		public abstract int SelectedItemsCount { get; }
		
		public abstract bool SupportsMulipleSelection { get; }
		
#endregion
		
#region Public Methods

		public ListItemProvider GetItemProvider (int index)
		{
			ListItemProvider item;
			
			if (index < 0 || index >= ItemsCount)
				return null;
			else if (items.TryGetValue (index, out item) == false) {
				item = new ListItemProvider (this, list_control, index);
				items [index] = item;
			}

			return item;
		}
		
		public abstract string GetItemName (ListItemProvider item);
		
		public abstract ListItemProvider[] GetSelectedItemsProviders ();
		
		public abstract void SelectItem (ListItemProvider item);

		public abstract void UnselectItem (ListItemProvider item);
		
		public abstract bool IsItemSelected (ListItemProvider item);

#endregion
			
#region FragmentControlProvider Overrides

		public override IRawElementProviderFragment GetFocus ()
		{
			return GetItemProvider (list_control.SelectedIndex);
		}
			
#endregion

#region Private Fields
		
		private ListControl list_control;
		private Dictionary<int, ListItemProvider> items;
		
#endregion

	}
}
