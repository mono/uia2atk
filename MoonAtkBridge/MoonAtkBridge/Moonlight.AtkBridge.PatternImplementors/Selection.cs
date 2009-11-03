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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Authors:
//      Calvin Gaisford <cgaisford@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (PatternInterface.Selection)]
	public sealed class Selection : BasePatternImplementor, Atk.SelectionImplementor
	{
#region Public Properties
		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}

		public int SelectionCount {
			get { 
				IRawElementProviderSimple[] selection
					= selectionProvider.GetSelection ();
				return selection != null ? selection.Length : 0;
			}
		}
#endregion

#region Public Methods
		public Selection (Adapter adapter, AutomationPeer peer) : base (adapter, peer)
		{
			this.selectionProvider = (ISelectionProvider) peer.GetPattern (
				PatternInterface.Selection);

			adapter.AutomationPropertyChanged += (o, args) => {
				if (args.Property == SelectionPatternIdentifiers.SelectionProperty)
					adapter.EmitSignal ("selection_changed");
			};
		}

		public bool AddSelection (int i)
		{
			AutomationPeer childPeer = GetChildAt (i);
			if (childPeer == null)
				return false;

			ISelectionItemProvider childItem = childPeer.GetPattern (
				PatternInterface.SelectionItem) as ISelectionItemProvider;
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
				} catch (InvalidOperationException e) {
					Log.Debug (e);
					return false;
				}
			}

			return true;
		}

		public bool ClearSelection ()
		{
			IRawElementProviderSimple[] selection
				= selectionProvider.GetSelection ();

			bool result = true;
			foreach (IRawElementProviderSimple item in selection) {
				AutomationPeer itemPeer = item.AutomationPeer;
				ISelectionItemProvider selItem = itemPeer.GetPattern (
					PatternInterface.SelectionItem) as ISelectionItemProvider;
				if (selItem == null)
					continue;

				try {
					if (selItem.IsSelected)
						selItem.RemoveFromSelection ();
				} catch (InvalidOperationException e) {
					Log.Debug (e);
					result = false;
				}
			}

			return result;
		}

		public bool IsChildSelected (int i)
		{
			AutomationPeer childPeer = GetChildAt (i);
			if (childPeer == null)
				return false;

			ISelectionItemProvider childItem = childPeer.GetPattern (
				PatternInterface.SelectionItem) as ISelectionItemProvider;
			if (childItem == null)
				return false;

			return childItem.IsSelected;
		}

		public Atk.Object RefSelection (int i)
		{
			IRawElementProviderSimple[] selection
				= selectionProvider.GetSelection ();
			if (i < 0 || i > selection.Length - 1)
				return null;

			return DynamicAdapterFactory.Instance.GetAdapter (
				selection[i].AutomationPeer);
		}

		public bool RemoveSelection (int i)
		{
			IRawElementProviderSimple[] selection
				= selectionProvider.GetSelection ();
			if (i < 0 || i > selection.Length - 1)
				return false;

			AutomationPeer childPeer = selection[i].AutomationPeer;
			if (childPeer == null)
				return false;

			ISelectionItemProvider childItem = childPeer.GetPattern (
				PatternInterface.SelectionItem) as ISelectionItemProvider;
			if (childItem == null)
				return false;

			try {
				if (childItem.IsSelected)
					childItem.RemoveFromSelection ();
				else
					return false;
			} catch (InvalidOperationException e) {
				// May happen, ie, if a ComboBox requires a selection
				Log.Debug (e);
				return false;
			}

			return true;
		}

		public bool SelectAllSelection ()
		{
			List<AutomationPeer> children = peer.GetChildren ();
			foreach (AutomationPeer child in children) {
				ISelectionItemProvider childItem = child.GetPattern (
					PatternInterface.SelectionItem) as ISelectionItemProvider;
				if (childItem == null) {
					// Ignore this, as there could be
					// non-SelectionItem children
					continue;
				}

				try {
					if (!childItem.IsSelected)
						childItem.AddToSelection ();
				} catch (InvalidOperationException e) {
					Log.Debug (e);
					return false;
				}
			}

			return true;
		}
#endregion

#region Private Fields
		private ISelectionProvider selectionProvider;
#endregion

#region Private Methods
		private AutomationPeer GetChildAt (int i)
		{
			List<AutomationPeer> children = peer.GetChildren ();
			if (children == null || i < 0 || i > children.Count - 1) 
				return null;

			return children[i];
		}
#endregion
	}
}
