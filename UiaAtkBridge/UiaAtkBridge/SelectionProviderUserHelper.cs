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
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;


namespace UiaAtkBridge
{
	internal class SelectionProviderUserHelper
	{
		private IRawElementProviderFragment		provider;
		private ISelectionProvider				selectionProvider;
		private IRawElementProviderFragment		childrenHolder;

		public SelectionProviderUserHelper (IRawElementProviderFragment provider,
		                                    ISelectionProvider selectionProvider) :
		  this (provider, selectionProvider, null)
		{
		}
		
		public SelectionProviderUserHelper (IRawElementProviderFragment provider,
		                                    ISelectionProvider selectionProvider,
		                                    IRawElementProviderFragment childrenHolder)
		{
			this.provider = provider;
			this.selectionProvider = selectionProvider;
			this.childrenHolder = (childrenHolder != null) ? childrenHolder : provider;
		}

#region Atk.SelectionImplementor

		public int SelectionCount
		{
			get {
				var selectedItems = GetSelection ();
				return (selectedItems == null) ? 0 : selectedItems.Length;
			}
		}

		public bool AddSelection (int i)
		{
			ISelectionItemProvider childItem;
			try {
				childItem = ChildItemAtIndex (i);
			} catch (System.IndexOutOfRangeException) {
				// This is arguably not right, but it is what gail does
				return true;
			}
			
			if (childItem != null) {
				if (selectionProvider.CanSelectMultiple)
					childItem.AddToSelection();
				else
					childItem.Select();
				return true;
			}
			return false;
		}

		public bool ClearSelection ()
		{
			var selectedElements = GetSelection ();
			bool result = true;
			
			if (selectedElements == null)
				return true;
			for (int i=0; i < selectedElements.Length; i++) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)selectedElements[i].GetPatternProvider
						(SelectionItemPatternIdentifiers.Pattern.Id);
				
				if (selectionItemProvider != null) {
					try {
						selectionItemProvider.RemoveFromSelection ();
					} catch (System.InvalidOperationException) {
						result = false;
					}
				}
			}	

			return result;
		}

		public bool IsChildSelected (int i)
		{
			ISelectionItemProvider childItem;
			try {
				childItem = ChildItemAtIndex (i);
			} catch (System.IndexOutOfRangeException) {
				return false;
			}

			if (childItem != null) {
				return childItem.IsSelected;
			}
			return false;
		}
		
		public Atk.Object RefSelection (int i)
		{
			IRawElementProviderSimple[] selectedElements = 
				GetSelection ();
			if (selectedElements == null || (i < 0 || i >= selectedElements.Length))
				return null;
			return AutomationBridge.GetAdapterForProviderLazy (selectedElements[i]);
		}
		
		public bool RemoveSelection (int i)
		{
			ISelectionItemProvider childItem;
			try {
				childItem = ChildItemAtIndex (i);
			} catch (System.IndexOutOfRangeException) {
				return false;
			}

			if(childItem != null) {
				try {
					childItem.RemoveFromSelection();
				} catch (System.InvalidOperationException) {
					// May happen, ie, if a ComboBox requires a selection
					return false;
				}
				return true;
			}
			return false;
		}

		public bool SelectAllSelection ()
		{
			if(!selectionProvider.CanSelectMultiple)
				return false;

			IRawElementProviderFragment child = 
				childrenHolder.Navigate(NavigateDirection.FirstChild);
			while (child != null) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)child.GetPatternProvider
						(SelectionItemPatternIdentifiers.Pattern.Id);
				
				if(selectionItemProvider != null) {
					selectionItemProvider.AddToSelection();
				} else
					return false;
				child.Navigate (NavigateDirection.NextSibling);
			}	
			return true;
		}

#endregion

		
		private ISelectionItemProvider ChildItemAtIndex (int i)
		{
			IRawElementProviderFragment child = 
				childrenHolder.Navigate(NavigateDirection.FirstChild);
			int childCount = 0;
			
			while( (child != null) && (childCount != i) ) {
				child = child.Navigate(NavigateDirection.NextSibling);
				childCount++;
			}
			
			if (child == null) {
				throw new System.IndexOutOfRangeException();
			}
			ISelectionItemProvider selectionItemProvider = 
				(ISelectionItemProvider)child.GetPatternProvider
					(SelectionItemPatternIdentifiers.Pattern.Id);
				
			if (selectionItemProvider != null)
				return selectionItemProvider;
			return null;
		}
		
		IRawElementProviderSimple [] GetSelection ()
		{
			IRawElementProviderSimple [] elements = selectionProvider.GetSelection ();
			int controlTypeId = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (elements == null || controlTypeId != ControlType.Group.Id)
				return elements;
			IRawElementProviderSimple [] ret = new IRawElementProviderSimple [elements.Length];
			int count = 0;
			for (int i = ret.Length - 1; i >= 0; i--) {
				IRawElementProviderFragment parent = ((IRawElementProviderFragment)ret [i]).Navigate (NavigateDirection.Parent);
				if (parent == childrenHolder)
					ret [count++] = elements [i];
			}
			Array.Resize<IRawElementProviderSimple> (ref ret, count);
			return ret;
		}
	}
}
