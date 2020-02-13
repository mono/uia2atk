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
using Mono.Unix;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.ListItem;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class ListProvider
		: FragmentRootControlProvider, IListProvider, IScrollBehaviorSubject
	{

		#region Constructors

		protected ListProvider (Control control) : base (control)
		{ 
			items = new Dictionary<object, ListItemProvider> ();
		}
		
		#endregion

		#region IScrollBehaviorSubject specialization
		
		public IScrollBehaviorObserver ScrollBehaviorObserver { 
			get { return observer; }
		}
		
		public FragmentControlProvider GetScrollbarProvider (ScrollBar scrollbar)
		{
			return new ListScrollBarProvider (scrollbar, this);
		}
		
		#endregion
	
		#region ListItem: Selection Methods and Properties
		
		public abstract int SelectedItemsCount { get; }
				
		public abstract IRawElementProviderSimple[] GetSelectedItems ();
		
		public abstract bool IsItemSelected (ListItemProvider item);
		
		public abstract void SelectItem (ListItemProvider item);
		
		public abstract void UnselectItem (ListItemProvider item);

		#endregion
		
		#region ListItem: Collection Methods and Properties

		public IEnumerable<ListItemProvider> Items {
			get {
				foreach (ListItemProvider listItem in items.Values)
					yield return listItem;
			}
		}
		
		public abstract int ItemsCount { get; }

		public abstract int IndexOfObjectItem (object objectItem);

		public virtual void FocusItem (object objectItem)
		{
			Log.Warn ("{0}: FocusItem unimplemented", this.GetType ());
		}

		public virtual ListItemProvider GetItemProviderFrom (FragmentRootControlProvider rootProvider,
		                                                     object objectItem)
		{
			return GetItemProviderFrom (rootProvider, objectItem, true);
		}

		public virtual ListItemProvider GetItemProviderFrom (FragmentRootControlProvider rootProvider,
		                                                     object objectItem,
		                                                     bool create)
		{
			ListItemProvider item = null;
			
			if (items.TryGetValue (objectItem, out item) == false) {
				if (!create)
					return null;
				item = GetNewItemProvider (rootProvider,
				                           this,
				                           Control,
				                           objectItem);
				items [objectItem] = item;
				item.Initialize ();
			}
			
			return item;
		}

		public ListItemProvider RemoveItemFrom (object objectItem)
		{
			ListItemProvider item = null;

			if (items.TryGetValue (objectItem, out item) == true) {
				items.Remove (objectItem);
				item.Terminate ();
			}
			
			return item;
		} 

		protected bool ContainsObject (object objectItem)
		{
			return objectItem == null ? false : items.ContainsKey (objectItem);
		}
		
		protected bool ContainsItem (ListItemProvider item)
		{
			return item == null ? false : items.ContainsKey (item.ObjectItem);
		}
		
		protected void ClearItemsList ()
		{
			foreach (ListItemProvider provider in Items)
				provider.Terminate ();

			items.Clear ();
		}

		protected virtual ListItemProvider GetNewItemProvider (FragmentRootControlProvider rootProvider,
		                                                       ListProvider provider,
		                                                       Control control,
		                                                       object objectItem)
		{
			return new ListItemProvider (rootProvider,
			                             provider, 
			                             control,
			                             objectItem);
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

		#endregion
		
		#region FragmentRootControlProvider: Specializations

		public override void Initialize ()
		{
			base.Initialize ();

			//According to: http://msdn.microsoft.com/en-us/library/ms742462.aspx
			SetBehavior (SelectionPatternIdentifiers.Pattern,
			             GetBehaviorRealization (SelectionPatternIdentifiers.Pattern));
			SetBehavior (GridPatternIdentifiers.Pattern,
			             GetBehaviorRealization (GridPatternIdentifiers.Pattern));
			SetBehavior (MultipleViewPatternIdentifiers.Pattern,
			             GetBehaviorRealization (MultipleViewPatternIdentifiers.Pattern));
			SetBehavior (TablePatternIdentifiers.Pattern,
			             GetBehaviorRealization (TablePatternIdentifiers.Pattern));
		}

		protected override void InitializeChildControlStructure ()
		{
			InitializeScrollBehaviorObserver ();
		}
	
		protected override void FinalizeChildControlStructure ()
		{
			foreach (ListItemProvider item in Items)
				RemoveChildProvider (item);

			ClearItemsList ();
		}
	
		#endregion
		
		#region Internal Methods: Get Behaviors

		internal abstract IProviderBehavior GetBehaviorRealization (AutomationPattern behavior);

		#endregion
		
		#region IListProvider implementation
		public virtual IProviderBehavior GetListItemBehaviorRealization (AutomationPattern behavior,
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

		public virtual IConnectable GetListItemEventRealization (ProviderEventType eventType, 
		                                                         ListItemProvider prov)
		{
			return null;
		}

		public event Action ScrollPatternSupportChanged;
		#endregion

		#region Scroll Methods and Properties

		protected abstract ScrollBar HorizontalScrollBar { get; }

		protected abstract ScrollBar VerticalScrollBar { get; }

		protected virtual void InitializeScrollBehaviorObserver ()
		{
			// Updates Navigation and sets/unsets Scroll Pattern
			observer = new ScrollBehaviorObserver (this,
			                                       HorizontalScrollBar, 
			                                       VerticalScrollBar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			observer.Initialize ();
			UpdateScrollBehavior ();
		}

		
		protected void OnScrollPatternSupportChanged (object sender, EventArgs args)
		{
			UpdateScrollBehavior ();
		}

		private void UpdateScrollBehavior ()
		{
			UpdateScrollBehavior (ScrollBehaviorObserver);
		}

		protected void UpdateScrollBehavior (IScrollBehaviorObserver observer)
		{
			if (observer != null && observer.SupportsScrollPattern)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             GetBehaviorRealization (ScrollPatternIdentifiers.Pattern));
			else
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
			if (ScrollPatternSupportChanged != null)
				ScrollPatternSupportChanged ();
		}
		
		#endregion

		#region Protected Methods

		protected virtual void OnCollectionChanged (object sender, CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add) {
				ListItemProvider item = GetItemProviderFrom (this, args.Element);
				AddChildProvider (item);
			} else if (args.Action == CollectionChangeAction.Remove) {
				ListItemProvider item = RemoveItemFrom (args.Element);
				RemoveChildProvider (item);
			} else if (args.Action == CollectionChangeAction.Refresh) {
				ClearItemsList ();
				OnNavigationChildrenCleared ();
			}
		}
		
		#endregion

		#region Private Fields
		
		private Dictionary<object, ListItemProvider> items;
		private ScrollBehaviorObserver observer;
		
		#endregion

		#region Internal Class: ScrollBar provider

		internal class ListScrollBarProvider : ScrollBarProvider
		{
			public ListScrollBarProvider (ScrollBar scrollbar, ListProvider provider)
				: base (scrollbar)
			{
				this.provider = provider;
				name = scrollbar is HScrollBar ? Catalog.GetString ("Horizontal Scroll Bar")
					: Catalog.GetString ("Vertical Scroll Bar");
			}
			
			public override IRawElementProviderFragmentRoot FragmentRoot {
				get { return provider; }
			}			
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
					return name;
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			private ListProvider provider;
			private string name;
		}
		
		#endregion

	}
}
