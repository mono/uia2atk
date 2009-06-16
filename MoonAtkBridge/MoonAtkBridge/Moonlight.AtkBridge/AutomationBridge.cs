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
//      Andres Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace Moonlight.AtkBridge
{
	public class AutomationBridge
	{
#region Public Methods
		public static AutomationBridge CreateAutomationBridge ()
		{
			return new AutomationBridge ();
		}

 		private AutomationBridge ()
 		{
			Log.Debug ("Moonlight Automation Bridge starting up...");

			AutomationSingleton.Instance.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);

			AutomationSingleton.Instance.AutomationEventRaised
				+= new EventHandler<AutomationEventEventArgs> (
					OnAutomationEventRaised);
		}

		public static bool IsAccessibilityEnabled ()
		{
			// Detect whether it is turned on at a platform level
			// TODO: Copy this code from UiaAtkBridge
			return true;
		}

		public IntPtr GetAccessibleHandle ()
		{
			Adapter root
				= DynamicAdapterFactory.Instance.RootVisualAdapter;
			return (root != null) ? root.Handle : IntPtr.Zero;
		}
#endregion

#region Private Methods
		private void OnAutomationPropertyChanged (
			object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Peer == null)
				return;

			Log.Info ("OnAutomationPropertyChanged: Peer = {0}, Property = {1}, Old = {2}, New = {3}",
			          args.Peer, args.Property, args.OldValue, args.NewValue);

			Adapter adapter
				= DynamicAdapterFactory.Instance.GetAdapter (args.Peer);
			if (adapter == null)
				return;

			adapter.HandleAutomationPropertyChanged (args);
		}

		private void OnAutomationEventRaised (
			object o, AutomationEventEventArgs args)
		{
			if (args.Peer == null)
				return;

			Log.Info ("OnAutomationEventRaised: Peer = {0}, Event = {1}",
			          args.Peer, args.Event);

			Adapter adapter
				= DynamicAdapterFactory.Instance.GetAdapter (args.Peer);
			if (adapter == null)
				return;

			adapter.HandleAutomationEventRaised (args);
		}
	}
#endregion
}
