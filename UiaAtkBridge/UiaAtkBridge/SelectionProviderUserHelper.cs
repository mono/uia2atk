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
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;


namespace UiaAtkBridge
{
	internal class SelectionProviderUserHelper
	{
		private IRawElementProviderFragment		provider;
		private ISelectionProvider			selectionProvider;
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
			get { return GetSelection ().Length; }
		}

		public bool AddSelection (int i)
		{
			ISelectionItemProvider childItem;
			childItem = ChildItemAtIndex (i);
			if (childItem == null)
				return false;
			
			if (selectionProvider.CanSelectMultiple) {
				try { 
					childItem.AddToSelection ();
				} catch (InvalidOperationException e) {
					Log.Debug (e);
					return false;
				}
			} else {
				try {
					childItem.Select ();
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}
			}
			return true;
		}

		public bool ClearSelection ()
		{
			var selectedElements = GetSelection ();
			bool result = true;
			
			if (selectedElements.Length == 0)
				return true;
			for (int i = 0; i < selectedElements.Length; i++) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)selectedElements[i].GetPatternProvider
						(SelectionItemPatternIdentifiers.Pattern.Id);
				
				if (selectionItemProvider != null) {
					try {
						selectionItemProvider.RemoveFromSelection ();
					} catch (InvalidOperationException e) {
						Log.Debug (e);
						result = false;
					}
				}
			}	

			return result;
		}

		public bool IsChildSelected (int i)
		{
			ISelectionItemProvider childItem;
			childItem = ChildItemAtIndex (i);
			if (childItem == null)
				return false;

			return childItem.IsSelected;
		}
		
		public Atk.Object RefSelection (int i)
		{
			IRawElementProviderSimple[] selectedElements = 
				GetSelection ();
			if (selectedElements.Length == 0 || (i < 0 || i >= selectedElements.Length))
				return null;
			IRawElementProviderSimple provider = selectedElements [i];
			if (provider is IRawElementProviderFragment && (int)provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.DataItem.Id)
				provider = ((IRawElementProviderFragment)provider).Navigate (NavigateDirection.FirstChild);
			return AutomationBridge.GetAdapterForProviderSemiLazy (provider);
		}
		
		public bool RemoveSelection (int i)
		{
			ISelectionItemProvider childItem;
			childItem = ChildItemAtIndex (i);

			if (childItem != null) {
				try {
					childItem.RemoveFromSelection ();
				} catch (InvalidOperationException e) {
					// May happen, ie, if a ComboBox requires a selection
					Log.Debug (e);
					return false;
				}
				return true;
			}
			return false;
		}

		public bool SelectAllSelection ()
		{
			if (!selectionProvider.CanSelectMultiple)
				return false;

			var child = childrenHolder.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)child.GetPatternProvider (
					  SelectionItemPatternIdentifiers.Pattern.Id);
				
				if (selectionItemProvider != null) {
					try {
						selectionItemProvider.AddToSelection ();
					} catch (InvalidOperationException e) {
						Log.Debug (e);
						return false;
					}
				} else
					return false;
				child = child.Navigate (NavigateDirection.NextSibling);
			}	
			return true;
		}

#endregion

		
		private ISelectionItemProvider ChildItemAtIndex (int i)
		{
			Adapter adapter = AutomationBridge.GetAdapterForProviderSemiLazy (childrenHolder).RefAccessibleChild (i) as Adapter;
			if (adapter == null || adapter.Provider == null)
				return null;
			ISelectionItemProvider ret = (ISelectionItemProvider)adapter.Provider.GetPatternProvider
				(SelectionItemPatternIdentifiers.Pattern.Id);
			if (ret != null)
				return ret;
			IRawElementProviderFragment fragment = adapter.Provider as IRawElementProviderFragment;
			if (fragment != null)
				fragment = fragment.Navigate (NavigateDirection.Parent);
			if (fragment != null && (int)fragment.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.DataItem.Id)
				ret = (ISelectionItemProvider)fragment.GetPatternProvider
					(SelectionItemPatternIdentifiers.Pattern.Id);
			return ret;
		}
		
		IRawElementProviderSimple [] GetSelection ()
		{
			var elements = selectionProvider.GetSelection ();
			int controlTypeId = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (elements.Length == 0 || controlTypeId != ControlType.Group.Id)
				return elements;
			IRawElementProviderSimple [] ret = new IRawElementProviderSimple [elements.Length];
			int count = 0;
			for (int i = 0; i < elements.Length; i++) {
				var parent = ((IRawElementProviderFragment)elements [i]).Navigate (NavigateDirection.Parent);
				if (parent == childrenHolder)
					ret [count++] = elements [i];
			}
			Array.Resize<IRawElementProviderSimple> (ref ret, count);
			return ret;
		}
	}
}
