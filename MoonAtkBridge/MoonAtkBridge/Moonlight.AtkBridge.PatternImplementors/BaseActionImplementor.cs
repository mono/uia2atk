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
	public abstract class BaseActionImplementor : BasePatternImplementor
	{
#region Public Properties
		public int NActions {
			get { return actions.Count; }
		}
#endregion

#region Public Methods
		public BaseActionImplementor (Adapter adapter, AutomationPeer peer)
			: base (adapter, peer)
		{
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
				action.Delegate (action.Pattern);
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

			// In UIA all controls support only one KeyBinding (aka AccessKey)
			return Keybinding;
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

#region Protected Methods
		protected abstract void RefreshActions ();
#endregion

#region Protected Fields
		protected class ActionDescriptor {
			public string Description = null;
			public string Name = null;
			public string LocalizedName = null;
			public object Pattern = null;
			public Action<object> Delegate = null;
		}

		protected List<ActionDescriptor> actions
			= new List<ActionDescriptor> ();

		protected string Keybinding {
			get {
				string keybinding = peer.GetAccessKey ();
				if (!string.IsNullOrEmpty (keybinding))
					return keybinding.ToUpper ().Replace ("ALT+", "<Alt>");
				return null;
			}
		}
#endregion
	}
}
