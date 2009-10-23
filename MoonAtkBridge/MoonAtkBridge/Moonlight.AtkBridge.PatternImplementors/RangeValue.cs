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
//      Mike Gorse <mgorse@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

using Atk;

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.PatternImplementors
{
	[ImplementsPattern (PatternInterface.RangeValue)]
	public sealed class RangeValue : BasePatternImplementor, Atk.ValueImplementor
	{
#region Public Properties
		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}
#endregion

#region Public Methods
		public RangeValue (Adapter adapter, AutomationPeer peer)
			: base (adapter, peer)
		{
			this.rangeValueProvider = (IRangeValueProvider) peer.GetPattern (
				PatternInterface.RangeValue);

			adapter.AutomationPropertyChanged
				+= new EventHandler<AutomationPropertyChangedEventArgs> (
					OnAutomationPropertyChanged);
		}

		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.Minimum);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.Maximum);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.SmallChange);
		}

		public void GetCurrentValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider.Value);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			double v = (double)value.Val;
			if (v < rangeValueProvider.Minimum || v > rangeValueProvider.Maximum)
				return false;

			try {
				rangeValueProvider.SetValue (v);
			} catch (ArgumentOutOfRangeException e) {
				Log.Debug (e);
				return false;
			} catch (ElementNotEnabledException e) {
				Log.Debug (e);
				return false;
			}

			return true;
		}
#endregion

#region Private Fields
		private IRangeValueProvider rangeValueProvider;
#endregion

#region Private Methods
		private void OnAutomationPropertyChanged (object o, AutomationPropertyChangedEventArgs args)
		{
			if (args.Property == RangeValuePatternIdentifiers.ValueProperty)
				adapter.NotifyPropertyChanged ("accessible-value");
		}
#endregion
	}
}
