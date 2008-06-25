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
	public class SelectionProviderImplementorHelper
	{
		private ISelectionProvider					selectionProvider;
		private IRawElementProviderFragmentRoot		provider;

		
		public SelectionProviderImplementorHelper(IRawElementProviderFragmentRoot provider,
		                                          ISelectionProvider selectionProvider)
		{
			this.selectionProvider = selectionProvider;
			this.provider = provider;
		}

#region Atk.SelectionImplementor

		public int SelectionCount
		{
			get {
				return selectionProvider.GetSelection().GetLength(0);
			}
		}

		public bool AddSelection (int i)
		{
			ISelectionItemProvider childItem = ChildItemAtIndex(i);
				
			if(childItem != null) {
				if(selectionProvider.CanSelectMultiple)
					childItem.AddToSelection();
				else
					childItem.Select();
				return true;
			}
			return false;
		}

		public bool ClearSelection ()
		{
			IRawElementProviderSimple[] selectedElements = 
				selectionProvider.GetSelection();
				
			for(int i=0; i < selectedElements.GetLength(0); i++) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)selectedElements[i].GetPatternProvider
						(SelectionItemPatternIdentifiers.Pattern.Id);
				
				if(selectionItemProvider != null) {
					selectionItemProvider.RemoveFromSelection();
				}
			}	

			return true;
		}

		public bool IsChildSelected (int i)
		{
			ISelectionItemProvider childItem = ChildItemAtIndex(i);

			if(childItem != null) {
				return childItem.IsSelected;
			}
			return false;
		}
		
		public Atk.Object RefSelection (int i)		
		{
			//TODO: Implement
			throw new NotImplementedException ();
		}
		
		public bool RemoveSelection (int i)
		{
			ISelectionItemProvider childItem = ChildItemAtIndex(i);

			if(childItem != null) {
				childItem.RemoveFromSelection();
				return true;
			}
			return false;
		}

		public bool SelectAllSelection ()
		{
			if(!selectionProvider.CanSelectMultiple)
				return false;

			IRawElementProviderSimple[] selectedElements = 
				selectionProvider.GetSelection();
				
			for(int i=0; i < selectedElements.GetLength(0); i++) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)selectedElements[i].GetPatternProvider
						(SelectionItemPatternIdentifiers.Pattern.Id);
				
				if(selectionItemProvider != null) {
					selectionItemProvider.AddToSelection();
				} else
					return false;
			}	
			return true;
		}

#endregion

		
		private ISelectionItemProvider ChildItemAtIndex(int i)
		{
			IRawElementProviderFragment child = 
				provider.Navigate(NavigateDirection.FirstChild);
			int childCount = 0;
			
			while( (child != null) && (childCount != i) ) {
				child = child.Navigate(NavigateDirection.NextSibling);
			}
			
			if(child != null) {
				ISelectionItemProvider selectionItemProvider = 
					(ISelectionItemProvider)provider.GetPatternProvider
						(SelectionItemPatternIdentifiers.Pattern.Id);
				
				if(selectionItemProvider != null) {
					return selectionItemProvider;
				}
			}
			return null;
		}
		
	}
}
