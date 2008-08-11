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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class ListProvider : FragmentRootControlProvider
	{

		#region Constructors

		protected ListProvider (ListControl control) : base (control)
		{
			listControl = control;
			items = new List<ListItemProvider> ();
			
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             new ListSelectionProviderBehavior (this));
		}
		
		#endregion
		
		#region Public Properties

		public ListControl ListControl {
			get { return listControl; }
		}
	
		public abstract int ItemsCount { get; }
		
		public abstract int SelectedItemsCount { get; }
		
		public abstract bool SupportsMultipleSelection { get; }
		
		#endregion
		
		#region Public Methods
		
		public abstract bool IsItemSelected (ListItemProvider item);

		public virtual ListItemProvider GetItemProvider (int index)
		{
			ListItemProvider item;
			
			if (index < 0 || index >= ItemsCount)
				return null;
			else if (index >= items.Count) {
				for (int loop = items.Count - 1; loop < index; loop++) {
					item = new ListItemProvider (GetItemsListProvider (), 
					                             listControl);
					items.Add (item);
					item.InitializeEvents ();
				}
			}
			
			return items [index];
		}
		
		public virtual int IndexOfItem (ListItemProvider item)
		{
			return items.IndexOf (item);
		}
		
		public abstract string GetItemName (ListItemProvider item);
		
		public abstract ListItemProvider[] GetSelectedItemsProviders ();
		
		public abstract void SelectItem (ListItemProvider item);

		public abstract void UnselectItem (ListItemProvider item);
		
		public virtual ListItemProvider RemoveItemAt (int index)
		{
			ListItemProvider item = null;
			
			if (index < items.Count) {
				item = items [index];
				items.RemoveAt (index);
				item.Terminate ();
			}
			
			return item;
		}
		
		public abstract void ScrollItemIntoView (ListItemProvider item);

		#endregion
		
		#region FragmentControlProvider: Specializations

		public override IRawElementProviderFragment GetFocus ()
		{
			return GetItemProvider (listControl.SelectedIndex);
		}

		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			//Items
			try {
				Helper.AddPrivateEvent (GetTypeOfObjectCollection (), 
				                        GetInstanceOfObjectCollection (), 
				                        "ChildAdded",
				                        this, 
				                        "OnChildAdded");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ChildAdded not defined", GetType ());
			}
			
			try {
				Helper.AddPrivateEvent (GetTypeOfObjectCollection (), 
				                        GetInstanceOfObjectCollection (), 
				                        "ChildRemoved", 
				                        this, 
				                        "OnChildRemoved");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ChildRemoved not defined", GetType ());
			}
			
			try {
				Helper.AddPrivateEvent (GetTypeOfObjectCollection (), 
				                        GetInstanceOfObjectCollection (),
				                        "ChildrenClear", 
				                        this, 
				                        "OnChildrenClear");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ChildrenClear not defined", GetType ());
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			try {
				Helper.RemovePrivateEvent (GetTypeOfObjectCollection (), 
				                           GetInstanceOfObjectCollection (), 
				                           "ChildAdded",
				                           this, 
				                           "OnChildAdded");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ChildAdded not defined", GetType ());
			}
			
			try {
				Helper.RemovePrivateEvent (GetTypeOfObjectCollection (), 
				                           GetInstanceOfObjectCollection (), 
				                           "ChildRemoved", 
				                           this, 
				                           "OnChildRemoved");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ChildRemoved not defined", GetType ());
			}

			try {
				Helper.RemovePrivateEvent (GetTypeOfObjectCollection (), 
				                           GetInstanceOfObjectCollection (), 
				                           "ChildrenClear", 
				                           this, 
				                           "OnChildrenClear");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: ChildrenClear not defined", GetType ());
			}
			
			foreach (ListItemProvider item in items)
				OnNavigationChildRemoved (false, item);
			ClearItemsList ();
		}
		
		#endregion

		#region Protected Methods
		
		protected bool ContainsItem (ListItemProvider item)
		{
			return item == null ? false : items.Contains (item);
		}
		
		protected void ClearItemsList ()
		{
			while (items.Count > 0) {
				RemoveItemAt (items.Count - 1);
			}
		}

		protected abstract Type GetTypeOfObjectCollection ();
		
		protected abstract object GetInstanceOfObjectCollection ();		
		
		protected virtual ListProvider GetItemsListProvider ()
		{
			return this;
		}

		#endregion
		
		#region Private Methods: StructureChangedEvent
		
#pragma warning disable 169

		private void OnChildAdded (object sender, int index)
		{
			ListItemProvider item = GetItemProvider (index);
			OnNavigationChildAdded (true, item);
		}
		
		private void OnChildRemoved (object sender, int index)
		{
			ListItemProvider item = RemoveItemAt (index);
			OnNavigationChildRemoved (true, item);
		}
		
		private void OnChildrenClear (object sender, EventArgs args)
		{
			ClearItemsList ();
			OnNavigationChildrenClear (true);
		}
		
#pragma warning restore 169
		
		#endregion

		#region Private Fields
		
		private ListControl listControl;
		private List<ListItemProvider> items;
		
		#endregion

	}
}
