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
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Navigation
{
	
	internal class ParentNavigation : SimpleNavigation
	{

		#region Constructor
		
		public ParentNavigation (FragmentControlProvider provider,
		                         ParentNavigation parentNavigation)
			: base (provider)
		{
			chain = new NavigationChain ();
			this.parentNavigation = parentNavigation;
		}
		
		public ParentNavigation (FragmentControlProvider provider)
			: this (provider, null)
		{
		}
		
		#endregion
		
		#region INavigable Interface
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			if (direction == NavigateDirection.Parent) {
				// TODO: Review this
				// "Fragment roots do not enable navigation to
				// a parent or siblings; navigation among fragment
				// roots is handled by the default window providers."
				// Source: http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.navigate.aspx
				if (Provider is FragmentRootControlProvider)
					return Provider.FragmentRoot;
				else
					return parentNavigation.Provider;
			}
			else if (direction == NavigateDirection.FirstChild)
				return chain.First == null ? null : chain.First.Value.Provider;
			else if (direction == NavigateDirection.LastChild)
				return chain.Last == null ? null : chain.Last.Value.Provider;
			else if (direction == NavigateDirection.NextSibling && parentNavigation != null)
				return parentNavigation.GetNextExplicitSiblingProvider (this);
			else if (direction == NavigateDirection.PreviousSibling && parentNavigation != null) 
				return parentNavigation.GetPreviousExplicitSiblingProvider (this);			
			else
				return null;
		}

		public override void Initialize ()
		{
			foreach (FragmentControlProvider child in Provider)
				AddLast (child.Navigation);
			
			Provider.NavigationUpdated += new NavigationEventHandler (OnNavigationChildrenUpdated);
		}

		public override void Terminate ()
		{		
			Provider.NavigationUpdated -= new NavigationEventHandler (OnNavigationChildrenUpdated);
		}
		
		#endregion

		#region Public Methods
		
		public IRawElementProviderFragment GetNextExplicitSiblingProvider (INavigation navigation)
		{
			if (chain.Contains (navigation) == true) {
				LinkedListNode<INavigation> nextNode = chain.Find (navigation).Next;
				if (nextNode == null)
					return null;
				else
					return nextNode.Value.Provider;
			} else
				return null;
		}
		
		public IRawElementProviderFragment GetPreviousExplicitSiblingProvider (INavigation navigation)
		{
			if (chain.Contains (navigation) == true) {
				LinkedListNode<INavigation> previousNode = chain.Find (navigation).Previous;
				if (previousNode == null)
					return null;
				else
					return previousNode.Value.Provider;
			} else
				return null;
		}
		
		public void AddAfter (INavigation after, INavigation navigation)
		{
			chain.AddAfter (chain.Find (after), navigation);
		}
		
		public void AddLast (INavigation navigation)
		{
			chain.AddLast (navigation);
		}
		
		public void AddFirst (INavigation navigation)
		{
			chain.AddFirst (navigation);
		}
		
		public void Remove (INavigation navigation)
		{
			chain.Remove (navigation);
		}

		#endregion
		
		#region Private Fields

		private void OnNavigationChildrenUpdated (object sender,
		                                          NavigationEventArgs args)
		{
			if (args.ChangeType == StructureChangeType.ChildAdded) {
				AddLast (args.ChildProvider.Navigation);
		
				if (args.RaiseEvent == true) {
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded, 
					                                   args.ChildProvider);
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
					                                   (FragmentControlProvider) sender);
				}
			} else if (args.ChangeType == StructureChangeType.ChildRemoved) {
				Remove (args.ChildProvider.Navigation);
	
				if (args.RaiseEvent == true) {
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved, 
					                                   args.ChildProvider);
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
					                                   (FragmentControlProvider) sender);
				}
			} else if (args.ChangeType == StructureChangeType.ChildrenReordered) {
				chain.Clear ();
				
				//TODO: Is this the event to generate?
				if (args.RaiseEvent == true) {
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenBulkRemoved, 
					                                   (FragmentControlProvider) sender);
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated,
					                                   (FragmentControlProvider) sender);
				}
			}
		}
		
		#endregion
		
		#region Private Fields
		
		private NavigationChain chain;
		private ParentNavigation parentNavigation;
		
		#endregion
	}
}
