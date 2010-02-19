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
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (AutomationControlType.Calendar)]
	public sealed class Calendar
		: BasePatternImplementor
	{
#region Public Methods
		public Calendar (Adapter adapter, AutomationPeer peer)
			: base (adapter, peer)
		{
			FrameworkElementAutomationPeer feap = peer as FrameworkElementAutomationPeer;
			if (feap == null)
				return;

			// XXX: We can't link to System.Windows.Controls, so
			// we'll use reflection instead.

			UIElement calendar = feap.Owner;

			Type t = calendar.GetType ();
			if (t.Namespace != "System.Windows.Controls" || t.Name != "Calendar")
				return;

			// XXX SUCH A HACK! XXX
			// Workaround a bug in CalendarAutomationPeer.
			// Even though its selection and NameProperty
			// change, it does not send the proper change
			// notifications via the peer.  So we fake them.
			EventInfo ei = t.GetEvent ("SelectedDatesChanged");
			ei.AddEventHandler (calendar,
			                    new EventHandler<SelectionChangedEventArgs> (OnSelectedDatesChanged));
		}
#endregion

#region Private Methods
		private void OnSelectedDatesChanged (object o, SelectionChangedEventArgs args)
		{
			adapter.NotifyPropertyChanged ("accessible-name");
		}
#endregion
	}
}
