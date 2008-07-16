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
#region Delegates
	
	public delegate void StructureChangeEventHandler (object sender, 
	                                                  ListItemProvider item, 
	                                                  int index);
	
	public delegate void ScrollbarNavigableEventHandler (object container,
	                                                     ScrollBar scrollbar,
	                                                     bool navigable);

#endregion
	
	public abstract class ListProvider : FragmentRootControlProvider
	{

#region Constructors

		protected ListProvider (ListControl control) : base (control)
		{
			list_control = control;
			items = new List<ListItemProvider> ();
			
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
		
		public abstract bool SupportsMultipleSelection { get; }
		
#endregion
		
#region Public Events

		public event StructureChangeEventHandler ChildAdded;
		
		public event StructureChangeEventHandler ChildRemoved;
		
#endregion
		
#region Public Methods
		
		public abstract bool IsItemSelected (ListItemProvider item);

		public ListItemProvider GetItemProvider (int index)
		{
			ListItemProvider item;
			
			if (index < 0 || index >= ItemsCount)
				return null;
			else if (index >= items.Count) {
				for (int loop = items.Count - 1; loop < index; loop++) {
					item = new ListItemProvider (this, list_control);
					items.Add (item);
					item.InitializeEvents ();
				}
			}
			
			return items [index];
		}
		
		public int IndexOfItem (ListItemProvider item)
		{
			return items.IndexOf (item);
		}
		
		public abstract string GetItemName (ListItemProvider item);
		
		public abstract ListItemProvider[] GetSelectedItemsProviders ();
		
		public abstract void SelectItem (ListItemProvider item);

		public abstract void UnselectItem (ListItemProvider item);
		
		public ListItemProvider RemoveItemAt (int index)
		{
			ListItemProvider item = null;
			
			if (index < items.Count) {
				item = items [index];
				items.RemoveAt (index);
				item.Terminate ();
			}
			
			return item;
		}

#endregion
		
#region SimpleControlProvider: Specializations
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.IsSelectionPatternAvailableProperty.Id)
				return true;
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
			
#region FragmentControlProvider: Specializations

		public override IRawElementProviderFragment GetFocus ()
		{
			return GetItemProvider (list_control.SelectedIndex);
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
			} catch (NotSupportedException) {}
			
			try {
			Helper.AddPrivateEvent (GetTypeOfObjectCollection (), 
			                        GetInstanceOfObjectCollection (), 
			                        "ChildRemoved", 
			                        this, 
			                        "OnChildRemoved");
			} catch (NotSupportedException) {}
		}
		
		public override void FinalizeChildControlStructure ()
		{
			try {
			Helper.RemovePrivateEvent (GetTypeOfObjectCollection (), 
			                           GetInstanceOfObjectCollection (), 
			                           "ChildAdded",
			                           this, 
			                           "OnChildAdded");
			} catch (NotSupportedException) {}
			
			try {
			Helper.RemovePrivateEvent (GetTypeOfObjectCollection (), 
			                           GetInstanceOfObjectCollection (), 
			                           "ChildRemoved", 
			                           this, 
			                           "OnChildRemoved");
			} catch (NotSupportedException) {}
			
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
		
		protected void GenerateChildAddedEvent (int index)
		{
			ListItemProvider item = GetItemProvider (index);
			
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
			                                   item);
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
			                                   this);
			
			if (ChildAdded != null)
				ChildAdded (this, item, index);
		}
		
		protected void GenerateChildRemovedEvent (int index)
		{
			ListItemProvider item = RemoveItemAt (index);
				
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
			                                   item);
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
			                                   this);
			
			if (ChildRemoved != null)
				ChildRemoved (this, item, index);
		}

		protected abstract Type GetTypeOfObjectCollection ();
		
		protected abstract object GetInstanceOfObjectCollection ();		

#endregion
		
#region Private Methods: StructureChangedEvent
		
		private void OnChildAdded (object sender, int index)
		{
			GenerateChildAddedEvent (index);
		}
		
		private void OnChildRemoved (object sender, int index)
		{
			GenerateChildRemovedEvent (index);
		}
		
#endregion

#region Private Fields
		
		private ListControl list_control;
		private List<ListItemProvider> items;
		
#endregion

	}
}
