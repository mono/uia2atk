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
using System.ComponentModel;
using System.Collections.Generic;
using SD = System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Navigation;

using Mono.UIAutomation.Winforms.Behaviors.ListItem;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class ListProvider : FragmentRootControlProvider
	{

		#region Constructors

		protected ListProvider (Control control) : base (control)
		{
			items = new List<ListItemProvider> ();

			//According to: http://msdn.microsoft.com/en-us/library/ms742462.aspx
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             GetBehaviorRealization (SelectionPatternIdentifiers.Pattern));
			SetBehavior (ScrollPatternIdentifiers.Pattern,
			             GetBehaviorRealization (ScrollPatternIdentifiers.Pattern));
			SetBehavior (GridPatternIdentifiers.Pattern,
			             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			SetBehavior (MultipleViewPatternIdentifiers.Pattern,
			             GetBehaviorRealization (MultipleViewPatternIdentifiers.Pattern));
		}
		
		#endregion
		
		#region ListItem: Selection Methods and Properties
		
		public abstract int SelectedItemsCount { get; }
				
		public abstract ListItemProvider[] GetSelectedItems ();
		
		public abstract bool IsItemSelected (ListItemProvider item);
		
		public abstract void SelectItem (ListItemProvider item);
		
		public abstract void UnselectItem (ListItemProvider item);
		
		#endregion
		
		#region ListItem: Collection Methods and Properties
		
		public abstract int ItemsCount { get; }

		public virtual ListItemProvider GetItemProviderAt (FragmentRootControlProvider rootProvider,
		                                                   int index)
		{
			ListItemProvider item;
			
			if (index < 0 || index >= ItemsCount)
				return null;
			else if (index >= items.Count) {
				for (int loop = items.Count - 1; loop < index; loop++) {
					item = new ListItemProvider (rootProvider,
					                             GetItemsListProvider (), 
					                             Control);
					items.Add (item);
					item.Initialize ();
				}
			}
			
			return items [index];
		}
		
		public virtual int IndexOfItem (ListItemProvider item)
		{
			return items.IndexOf (item);
		}
		
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
		
		protected bool ContainsItem (ListItemProvider item)
		{
			return item == null ? false : items.Contains (item);
		}
		
		protected void ClearItemsList ()
		{
			while (items.Count > 0)
				RemoveItemAt (items.Count - 1);
		}		
		
		#endregion
		
		#region ListItem: Toggle Methods
		
		public virtual ToggleState GetItemToggleState (ListItemProvider item)
		{
			return ToggleState.Indeterminate;
		}
		
		public virtual void ToggleItem (ListItemProvider item)
		{
		}
		
		#endregion
		
		#region ListItem: Scroll Methods
		
		public abstract void ScrollItemIntoView (ListItemProvider item);		
		
		#endregion
		
		#region ListItem: Properties Methods
		
		public abstract object GetItemPropertyValue (ListItemProvider item,
		                                             int propertyId);

		public abstract IConnectable GetListItemHasKeyboardFocusEvent (ListItemProvider provider);		

		#endregion
		
		#region FragmentRootControlProvider: Specializations
		
		public override void InitializeChildControlStructure ()
		{
			try {
				Helper.AddPrivateEvent (GetTypeOfObjectCollection (), 
				                        GetInstanceOfObjectCollection (), 
				                        "UIACollectionChanged",
				                        this, 
				                        "OnCollectionChanged");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: CollectionChanged not defined", GetType ());
			}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			try {
				Helper.RemovePrivateEvent (GetTypeOfObjectCollection (), 
				                           GetInstanceOfObjectCollection (), 
				                           "UIACollectionChanged",
				                           this, 
				                           "OnCollectionChanged");
			} catch (NotSupportedException) {
				Console.WriteLine ("{0}: CollectionChanged not defined", GetType ());
			}
			
			foreach (ListItemProvider item in items)
				OnNavigationChildRemoved (false, item);
			ClearItemsList ();
		}
	
		#endregion
		
		#region Internal Methods: Get Behaviors

		internal abstract IProviderBehavior GetBehaviorRealization (AutomationPattern behavior);
		
		internal virtual IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
		                                                                   ListItemProvider listItem)
		{
			//According to: http://msdn.microsoft.com/en-us/library/ms744765.aspx
			if (behavior == ScrollItemPatternIdentifiers.Pattern) {
				//LAMESPEC: Supported only if the list item is contained within a container that is scrollable.
				if (IsBehaviorEnabled (ScrollPatternIdentifiers.Pattern) == true)
				    return new ScrollItemProviderBehavior (listItem);
				else
					return null;
			} else
				return null;
		}
		
		#endregion

		#region Protected Methods
		
		//NOTE: 
		//      The following methods WILL BE DELETED in Mono 2.0 (because of
		//      InternalsVisibleTo attribute)

		protected abstract Type GetTypeOfObjectCollection ();
		
		protected abstract object GetInstanceOfObjectCollection ();		

		protected virtual ListProvider GetItemsListProvider ()
		{
			return this;
		}
				
#pragma warning disable 169

		protected virtual void OnCollectionChanged (object sender, CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add) {
				ListItemProvider item = GetItemProviderAt (this, (int) args.Element);
				OnNavigationChildAdded (true, item);
			} else if (args.Action == CollectionChangeAction.Remove) {
				ListItemProvider item = RemoveItemAt ((int) args.Element);
				OnNavigationChildRemoved (true, item);
			} else if (args.Action == CollectionChangeAction.Refresh) {
				ClearItemsList ();
				OnNavigationChildrenCleared (true);
			}
		}
		
#pragma warning restore 169
		
		#endregion

		#region Private Fields
		
		private List<ListItemProvider> items;
		
		#endregion

	}
}
