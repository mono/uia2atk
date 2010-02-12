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
	[ImplementsPattern (PatternInterface.SelectionItem)]
	public sealed class SelectionItem : BasePatternImplementor
	{
#region Public Methods
		public SelectionItem (Adapter adapter, AutomationPeer peer) : base (adapter, peer)
		{
			bool? isSelected = IsSelected;
			if (isSelected.HasValue)
				WasSelected = isSelected.Value;

			adapter.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
				OnAutomationPropertyChanged);

			adapter.AutomationEventRaised
				+= new EventHandler<AutomationEventEventArgs> (
				OnAutomationEventRaised);
		}

		public override void OnRefStateSet (ref Atk.StateSet states)
		{
			bool? isSelected = IsSelected;
			if (!isSelected.HasValue)
				return;

			states.AddState (Atk.StateType.Selectable);

			if (isSelected.Value)
				states.AddState (Atk.StateType.Selected);
			else
				states.RemoveState (Atk.StateType.Selected);
		}

		public bool WasSelected {
			get;
			private set;
		}

		public Adapter ParentSelectionContainer {
			get {
				ISelectionItemProvider provider
					= peer.GetPattern (PatternInterface.SelectionItem)
						as ISelectionItemProvider;
				if (provider.SelectionContainer == null)
					return null;

				return DynamicAdapterFactory.Instance.GetAdapter (provider.SelectionContainer.AutomationPeer,
				                                                  false);
			}
		}

#endregion

#region Private Methods
		private void OnAutomationPropertyChanged (object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Property == SelectionItemPatternIdentifiers.IsSelectedProperty) {
				if (WasSelected != (bool) args.NewValue) {
					WasSelected = !WasSelected;
					adapter.NotifyStateChange (Atk.StateType.Selected);
				}
			}
		}

		private void OnAutomationEventRaised (object o, AutomationEventEventArgs args)
		{
			bool notifyStateChanged = false;

			switch (args.Event) {
			case AutomationEvents.SelectionItemPatternOnElementSelected:
				// We have to notify that we are the only selected item,
				// we do that by unselecting previous selected items.
				SelectionItem.ClearSelectedItems (this);
				if (!WasSelected) {
					notifyStateChanged = true;
					SelectionItem.AddSelectedItem (this);
				}
				break;
			case AutomationEvents.SelectionItemPatternOnElementAddedToSelection:
				if (!WasSelected) {
					notifyStateChanged = true;
					SelectionItem.AddSelectedItem (this);
				}
				break;
			case AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection:
				if (WasSelected) {
					notifyStateChanged = true;
					SelectionItem.RemoveSelectedItem (this);
				}
				break;
			}

			if (notifyStateChanged) {
				WasSelected = !WasSelected;
				adapter.NotifyStateChange (Atk.StateType.Selected);
			}
		}

		private bool? IsSelected {
			get {
				ISelectionItemProvider selectionItem
					= peer.GetPattern (PatternInterface.SelectionItem)
						as ISelectionItemProvider;
				if (selectionItem == null)
					return null;

				return selectionItem.IsSelected;
			}
		}
#endregion

#region Static Members

		private static void ClearSelectedItems (SelectionItem exceptSelectionItem)
		{
			Adapter parent = exceptSelectionItem.ParentSelectionContainer;
			if (parent == null)
				return;

			List<SelectionItem> selectedItems = null;
			if (SelectionItem.CachedSelectionItems.TryGetValue (parent,
			                                                    out selectedItems)) {
				foreach (SelectionItem selectionItem in selectedItems) {
					if (selectionItem != exceptSelectionItem) {
						selectionItem.WasSelected = false;
						selectionItem.Adapter.NotifyStateChange (Atk.StateType.Selected);
					}
				}
				selectedItems.Clear ();
				selectedItems.Add (exceptSelectionItem);
			}
		}

		private static void AddSelectedItem (SelectionItem selectionItem)
		{
			Adapter parent = selectionItem.ParentSelectionContainer;
			if (parent == null)
				return;

			List<SelectionItem> selectedItems = null;
			if (!SelectionItem.CachedSelectionItems.TryGetValue (parent,
			                                                     out selectedItems)) {
				selectedItems = new List<SelectionItem> ();
				SelectionItem.CachedSelectionItems [parent] = selectedItems;
			}

			selectedItems.Add (selectionItem);
		}

		private static void RemoveSelectedItem (SelectionItem selectionItem)
		{
			Adapter parent = selectionItem.ParentSelectionContainer;
			if (parent == null)
				return;

			List<SelectionItem> selectedItems = null;
			if (SelectionItem.CachedSelectionItems.TryGetValue (parent,
			                                                     out selectedItems))
				selectedItems.Remove (selectionItem);
		}

		private static Dictionary<Adapter, List<SelectionItem>> CachedSelectionItems
			= new Dictionary<Adapter, List<SelectionItem>> ();
#endregion
	}
}
