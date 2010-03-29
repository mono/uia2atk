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
using Mono.UIAutomation.Services;
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
		
		// StructureChanged events are handled entirely in
		// AutomationBridge.cs, so this currently is unused.
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
				SetParent (child, this);
			EmitChildrenChanged (Atk.Object.ChildrenChangedDetail.Add, (uint)(children.Count - 1), child);
			Adapter adapter = child as Adapter;
			if (adapter != null)
				adapter.PostInit ();
		}
		
		internal virtual void RemoveChild (Atk.Object childToRemove)
		{
			RemoveChild (childToRemove, true);
		}

		internal virtual void RemoveChild (Atk.Object childToRemove, bool terminate)
		{
			if (childToRemove == null) {
				//FIXME: better throw an ArgumentNullException
				return;
			}

			//Console.WriteLine ("RemoveChild: {0}", childToRemove.GetType ());
			
			if (childToRemove is ParentAdapter) {
				((ParentAdapter)childToRemove).RemoveUnmanagedChildren ();
			}

			Adapter adapter = childToRemove as Adapter;
			if (adapter != null)
				adapter.RemoveFromParent (this, terminate);

			int childIndex;
			lock (syncRoot) {
				childIndex = children.IndexOf (childToRemove);
				children.Remove (childToRemove);
			}
			EmitChildrenChanged (Atk.Object.ChildrenChangedDetail.Remove, (uint)childIndex, childToRemove);
		}
		
		// Note: Only called if an object is explicitly removed, not
		// when removed as a result of recursion.  Useful if the atk
		// hierarchy needs to be adjusted (ie, for Splitter)
		internal virtual void PreRemoveChild (Atk.Object childToRemove)
		{
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
			
			AutomationBridge.AddChildrenToParent (Provider);
		}
		
		internal void UpdateChildren ()
		{
			if (requestedChildren) {
				// TODO: Figure out if this severely impacts performance
				requestedChildren = false;
				RequestChildren ();
			}
		}

		protected new void EmitChildrenChanged (ChildrenChangedDetail detail, uint child_index, Atk.Object child)
		{
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (delegate {
			base.EmitChildrenChanged (detail, child_index, child);
				return false;
			}));
		}
			
#endregion

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs args)
		{
			base.RaiseAutomationEvent (eventId, args);
		}
		
	}
}
