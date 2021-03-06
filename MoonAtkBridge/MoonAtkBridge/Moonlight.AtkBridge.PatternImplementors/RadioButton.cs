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
	[ImplementsPattern (AutomationControlType.RadioButton)]
	public sealed class RadioButton
		: BaseActionImplementor, Atk.ActionImplementor
	{
#region Public Properties
		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}
#endregion

#region Public Methods
		public RadioButton (Adapter adapter, AutomationPeer peer)
			: base (adapter, peer)
		{
			adapter.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);
		}

		public override void OnRefStateSet (ref Atk.StateSet states)
		{
			var selectionItem = peer.GetPattern (
				PatternInterface.SelectionItem)
					as ISelectionItemProvider;
			if (selectionItem == null)
				return;

			if (selectionItem.IsSelected)
				states.AddState (Atk.StateType.Checked);
			else
				states.RemoveState (Atk.StateType.Checked);
		}
#endregion

#region Protected Methods
		protected override void RefreshActions ()
		{
			actions.Clear ();

			var selectionItem = peer.GetPattern (
				PatternInterface.SelectionItem);
			if (selectionItem == null)
				return;

			// TODO: Localize actions
			actions.Add (new ActionDescriptor {
				Name = "click", Pattern = selectionItem,
				Delegate = p => ((ISelectionItemProvider) p).Select ()
			});
		}
#endregion

#region Private Methods
		private void OnAutomationPropertyChanged (object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Property == SelectionItemPatternIdentifiers.IsSelectedProperty)
				adapter.NotifyStateChange (Atk.StateType.Checked);
		}
#endregion
	}
}
