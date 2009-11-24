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
using ECPIs = System.Windows.Automation.ExpandCollapsePatternIdentifiers;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (PatternInterface.ExpandCollapse)]
	[ImplementsPattern (PatternInterface.Invoke)]
	[ImplementsPattern (PatternInterface.Toggle)]
	public class ExpandCollapseInvokeToggle
		: BaseActionImplementor, Atk.ActionImplementor
	{
#region Public Properties
		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}
#endregion

#region Public Methods
		public ExpandCollapseInvokeToggle (Adapter adapter, AutomationPeer peer)
			: base (adapter, peer)
		{
			adapter.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);
		}

		public override void OnRefStateSet (ref Atk.StateSet states)
		{
			IExpandCollapseProvider expandCollapse
				= peer.GetPattern (PatternInterface.ExpandCollapse)
					as IExpandCollapseProvider;
			if (expandCollapse != null) {
				var state = expandCollapse.ExpandCollapseState;

				// If it's not a leaf, it can be expanded
				if (state != ExpandCollapseState.LeafNode)
					states.AddState (Atk.StateType.Expandable);

				if (state == ExpandCollapseState.Expanded
				    || state == ExpandCollapseState.PartiallyExpanded)
					states.AddState (Atk.StateType.Expanded);
				else
					states.RemoveState (Atk.StateType.Expanded);
			}

			IToggleProvider toggle
				= peer.GetPattern (PatternInterface.Toggle)
					as IToggleProvider;
			if (toggle != null) {
				var state = toggle.ToggleState;
				if (state == ToggleState.On)
					states.AddState (Atk.StateType.Checked);
				else
					states.RemoveState (Atk.StateType.Checked);
			}
		}
#endregion

#region Protected Methods
		protected override void RefreshActions ()
		{
			actions.Clear ();

			// TODO: Localize actions

			var toggle = peer.GetPattern (PatternInterface.Toggle);
			if (toggle != null) {
				actions.Add (new ActionDescriptor {
					Name = "click", Pattern = toggle,
					Delegate = p => ((IToggleProvider) p).Toggle ()
				});
			}

			// Only add Invoke if Toggle doesn't exist.
			var invoke = peer.GetPattern (PatternInterface.Invoke);
			if (toggle == null && invoke != null) {
				actions.Add (new ActionDescriptor {
					Name = "click", Pattern = invoke,
					Delegate = p => ((IInvokeProvider) p).Invoke ()
				});
			}

			var expandCollapse
				= peer.GetPattern (PatternInterface.ExpandCollapse);
			if (expandCollapse != null) {
				actions.Add (new ActionDescriptor {
					Name = "expand or collapse", Pattern = expandCollapse,
					Delegate = p => {
						var e = (IExpandCollapseProvider) p;

						if (e.ExpandCollapseState == ExpandCollapseState.Expanded
						    || e.ExpandCollapseState == ExpandCollapseState.PartiallyExpanded)
							e.Collapse ();
						else
							e.Expand ();
					}
				});
			}
		}
#endregion

#region Private Methods
		private void OnAutomationPropertyChanged (object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Property == ECPIs.ExpandCollapseStateProperty) {
				adapter.NotifyStateChange (Atk.StateType.Expanded);
				adapter.EmitSignal ("visible_data_changed");
			} else if (args.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				adapter.NotifyStateChange (Atk.StateType.Checked);
			}
		}
#endregion
	}
}
