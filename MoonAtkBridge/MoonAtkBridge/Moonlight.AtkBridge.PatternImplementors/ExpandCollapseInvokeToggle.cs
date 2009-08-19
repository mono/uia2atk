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
	[ImplementsPattern (PatternInterface.ExpandCollapse)]
	[ImplementsPattern (PatternInterface.Invoke)]
	[ImplementsPattern (PatternInterface.Toggle)]
	public class ExpandCollapseInvokeToggle : Atk.ActionImplementor
	{
#region Public Properties
		public int NActions {
			get { return actions.Count; }
		}

		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}
#endregion

#region Public Methods
		public ExpandCollapseInvokeToggle (AutomationPeer peer)
		{
			this.peer = peer;

			// TODO: also do this in response to patterns being
			// added/removed
			RefreshActions ();
		}

		public bool DoAction (int i)
		{
			if (i < 0 || i >= actions.Count)
				return false;

			ActionDescriptor action = actions [i];
			if (action.Pattern == null)
				return false;

			try {
				if (action.Pattern is IInvokeProvider)
					((IInvokeProvider) action.Pattern).Invoke ();
				else if (action.Pattern is IToggleProvider)
					((IToggleProvider) action.Pattern).Toggle ();
				else if (action.Pattern is IExpandCollapseProvider) {
					IExpandCollapseProvider ecProvider
						= (IExpandCollapseProvider) action.Pattern;

					if (ecProvider.ExpandCollapseState == ExpandCollapseState.Expanded
					    || ecProvider.ExpandCollapseState == ExpandCollapseState.PartiallyExpanded)
						ecProvider.Collapse ();
					else
						ecProvider.Expand ();
				}

				return true;
			} catch (ElementNotEnabledException) {
				return false;
			}
		}

		public string GetDescription (int i)
		{
			if (i < 0 || i >= actions.Count)
				return null;

			return actions [i].Description;
		}

		public string GetKeybinding (int i)
		{
			if (i < 0 || i >= actions.Count)
				return null;

			return actions [i].Keybinding;
		}

		public string GetName (int i)
		{
			if (i < 0 || i >= actions.Count)
				return null;

			return actions [i].Name;
		}

		public string GetLocalizedName (int i)
		{
			if (i < 0 || i >= actions.Count)
				return null;

			return actions [i].LocalizedName ?? actions [i].Name;
		}

		public bool SetDescription (int i, string desc)
		{
			if (i < 0 || i >= actions.Count)
				return false;

			// TODO: preserve these across refreshes
			actions [i].Description = desc;
			return true;
		}
#endregion

#region Private Methods
		private void RefreshActions ()
		{
			actions.Clear ();

			var toggle = peer.GetPattern (PatternInterface.Toggle);
			if (toggle != null) {
				actions.Add (new ActionDescriptor {
					Name = "click", Pattern = toggle
				});
			}

			// Only add Invoke if Toggle doesn't exist.
			var invoke = peer.GetPattern (PatternInterface.Invoke);
			if (toggle == null && invoke != null) {
				actions.Add (new ActionDescriptor {
					Name = "click", Pattern = invoke
				});
			}

			var expandCollapse
				= peer.GetPattern (PatternInterface.ExpandCollapse);
			if (expandCollapse != null) {
				actions.Add (new ActionDescriptor {
					Name = "expand or collapse", Pattern = expandCollapse
				});
			}
		}
#endregion

#region Private Fields
		private class ActionDescriptor {
			public string Description = null;
			public string Keybinding = null;
			public string Name = null;
			public string LocalizedName = null;
			public object Pattern = null;
		}

		private List<ActionDescriptor> actions
			= new List<ActionDescriptor> ();

		private AutomationPeer peer;
#endregion
	}
}
