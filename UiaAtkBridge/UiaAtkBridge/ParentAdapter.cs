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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public abstract class ParentAdapter : Adapter
	{
		internal ParentAdapter (IRawElementProviderSimple provider) : base (provider)
		{
		}
		
#region Private 
		
		private bool requestedChildren = false;
		
#endregion
		
#region Protected Fields
		
		protected static object syncRoot = new object ();
		
		protected List<Atk.Object> children = new List<Atk.Object> ();
		
		public abstract void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e);

		// Here, unmanaged => children with ManagesRemoval = true
		protected virtual void RemoveUnmanagedChildren ()
		{
		}
		
#endregion
		
#region Atk.Object Overrides
		
		protected override int OnGetNChildren ()
		{
			if (requestedChildren == false)
				RequestChildren ();
			lock (syncRoot)
				return children.Count;
		}
		
		protected override Atk.Object OnRefChild (int i)
		{
			if (requestedChildren == false)
				RequestChildren ();
				
			lock (syncRoot) {
				if (i < 0 || i >= children.Count)
					return null;
				
				return children [i];
			}
		}
		
#endregion
		
		
#region Public Methods
		
		internal virtual void AddOneChild (Atk.Object child)
		{
			//Console.WriteLine ("AddOneChild: " + Role + " -> " + child.Role);
			lock (syncRoot) {
				children.Add (child);
			}
			//we may want that a child has 2 parents
			if (child.Parent == null)
				child.Parent = this;
			EmitChildrenChanged (Atk.Object.ChildrenChangedDetail.Add, (uint)(children.Count - 1), child);
		}
		
		internal virtual void RemoveChild (Atk.Object childToRemove)
		{
			if (childToRemove == null) {
				//FIXME: better throw an ArgumentNullException
				return;
			}

			Console.WriteLine ("RemoveChild: {0}", childToRemove.GetType ());
			
			if (childToRemove is ParentAdapter) {
				((ParentAdapter)childToRemove).RemoveUnmanagedChildren ();
			}

			int childIndex;
			lock (syncRoot) {
				Adapter adapter = childToRemove as Adapter;
				if (adapter != null)
					adapter.RemoveFromParent (this);
				childIndex = children.IndexOf (childToRemove);
				children.Remove (childToRemove);
			}
			EmitChildrenChanged (Atk.Object.ChildrenChangedDetail.Remove, (uint)childIndex, childToRemove);
		}
		
		public virtual int GetIndexOfChild (Atk.Object child)
		{
			return children.IndexOf (child);
		}
#endregion
		
		
#region Private Methods
		
		internal virtual void RequestChildren ()
		{
			if (requestedChildren == true)
				return;

			requestedChildren = true;
			
			IRawElementProviderFragment fragment;
			if ((fragment = Provider as IRawElementProviderFragment) == null)
				return;
			
			IRawElementProviderFragment child 
				= fragment.Navigate (NavigateDirection.FirstChild);
			while (child != null) {
				AutomationInteropProvider.RaiseStructureChangedEvent (
				  child,
				  new StructureChangedEventArgs (StructureChangeType.ChildAdded,
				                                 child.GetRuntimeId ()));
				child = child.Navigate (NavigateDirection.NextSibling);
			}
		}
		
		internal void UpdateChildren ()
		{
			if (requestedChildren) {
				// TODO: Figure out if this severely impacts performance
				requestedChildren = false;
				RequestChildren ();
			}
		}
#endregion

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs args)
		{
			base.RaiseAutomationEvent (eventId, args);
		}
		
	}
}
