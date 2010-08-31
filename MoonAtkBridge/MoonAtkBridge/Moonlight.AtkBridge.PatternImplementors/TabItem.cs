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
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
//
// Authors:
//      Mario Carrion <mcarrion@novell.com>
//

using Atk;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (AutomationControlType.TabItem,
	                    Provides=PatternInterface.SelectionItem)]
	public class TabItem : BasePatternImplementor
	{
#region Public Methods
		public TabItem (Adapter adapter, AutomationPeer peer) : base (adapter, peer)
		{
			adapter.AutomationPropertyChanged += (o, args) => {
				if (args.Property == SelectionItemPatternIdentifiers.IsSelectedProperty) {
					isSelectedChanged = true;
					newValue = (bool) args.NewValue;
				}
			};

			// XXX: Warning! Hack alert!
			//
			// TabItem's children are avaliable *only* when the TabItem is selected,
			// (its IsSelectedProperty is true). We are updating children here instead for
			// two reasons:
			//
			// 1. We can't modify the sources in System.Windows.Controls.dll because this
			//    dll is bundled in the packaged XAP, VS does this automatically. If we
			//    fix the sources in SWC.dll all applications built using VS *will fail*
			//    anyway.
			// 2. We are using both the AutomationPropertyEvent (IsSelected) and the event
			//    LayoutUpdated because the TabControl has *one* SelectedContent area, and
			//    this area is updated *after* raising the event. If we only use
			//    IsSelectedPropertyEvent we will always get the previous/unselected
			//    TabItem's children instead of the current/selected's.
			//
			FrameworkElement tabItem
				= ((FrameworkElementAutomationPeer) peer).Owner as FrameworkElement;
			if (tabItem != null) {
				tabItem.LayoutUpdated += (o, e) => {
					if (isSelectedChanged) {
						Adapter parentTabControl = ParentTabControl;

						// We don't have a parent let's update our children anyway.
						if (parentTabControl == null)
							adapter.HandleStructureChanged ();
						// We have a parent, let's update our children "the right way",
						// first the unselected TabItem then the selected TabItem.
						else {
							Adapter cachedTabItem = null;
							if (CachedTabItems.TryGetValue (parentTabControl,
							    out cachedTabItem)) {
								if (newValue) {
									cachedTabItem.HandleStructureChanged ();
									cachedTabItem.NotifyStateChange (Atk.StateType.Selected);

									adapter.HandleStructureChanged ();
									adapter.NotifyStateChange (Atk.StateType.Selected);
								} else {
									adapter.HandleStructureChanged ();
									adapter.NotifyStateChange (Atk.StateType.Selected);

									cachedTabItem.HandleStructureChanged ();
									cachedTabItem.NotifyStateChange (Atk.StateType.Selected);
								}
								CachedTabItems.Remove (parentTabControl);
							} else
								CachedTabItems.Add (parentTabControl, adapter);
						}
						isSelectedChanged = false;
						newValue = false;
					}
				};
			}
		}

		public override void OnRefStateSet (ref Atk.StateSet states)
		{
			states.AddState (Atk.StateType.Selectable);

			ISelectionItemProvider selectionItem
				= peer.GetPattern (PatternInterface.SelectionItem) as ISelectionItemProvider;
			if (selectionItem.IsSelected)
				states.AddState (Atk.StateType.Selected);
			else
				states.RemoveState (Atk.StateType.Selected);
		}

		public override bool OverridesGetChildren {
			get { return true; }
		}

		public override List<AutomationPeer> GetChildren ()
		{
			List<AutomationPeer> children = peer.GetChildren ();
			if (children == null)
				return null;

			// XXX: Warning! Hack alert!
			//
			// All our SL2 tests pass on ML2 and SL2, however it seems there's
			// something else happening when returning children using UIAClient API.
			// Using SL2's tabItemPeer.GetChildren() returns 2 children (header and content),
			// however when using the client API (or UISPy) it returns one child (content).
			// The following hack does two things:
			// - Removes the TextBlock when its name is the same name as the TabItem.Header.
			// - Removes the spurious TextBlock added when switching between pages.
			// This keeps SL2 compatibility and UIAutomationClient compatibility.
			// Notice the first 2 children must match this behavior
			// - If the first child is Text and its GetName() is empty, then second child
			//   would be removed
			// - If the first child is Text and its GetName() is the same as the TabItem,
			//   is removed
			if (children.Count > 0
			    && children [0].GetAutomationControlType () == AutomationControlType.Text) {
				if (children [0].GetName () == peer.GetName ()) {
					children.RemoveAt (0);
					if (children.Count > 0
					   && children [0].GetAutomationControlType () == AutomationControlType.Text
					   && string.IsNullOrEmpty (children [0].GetName ()))
						children.RemoveAt (0);
				} else if (string.IsNullOrEmpty (children [0].GetName ())) {
					children.RemoveAt (0);
					if (children.Count > 0
					   && children [0].GetAutomationControlType () == AutomationControlType.Text
					   && children [0].GetName () == peer.GetName ())
						children.RemoveAt (0);
				}
			}

			return children;
		}
#endregion
	
#region Private Members
		private bool isSelectedChanged;
		private bool newValue;

		// TabControl changes TabItem.IsSelected properties sequentially:
		//     It loops  over *each one* of its Children and changes IsSelected,
		//     *one by one*.
		// In other words, it doesn't unselect the previous (selected) item first
		// and then selects the newly (unselected) item.
		// We use this dictionary to cache the previous changed item because we
		// need to update the children depending on the selection, and the right
		// and valid way is:
		// 1. Unselected TabItem.
		// 2. Selected TabItem.
		private static Dictionary<Adapter, Adapter> CachedTabItems
			= new Dictionary<Adapter, Adapter> ();

		private Adapter ParentTabControl {
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
	}

}
