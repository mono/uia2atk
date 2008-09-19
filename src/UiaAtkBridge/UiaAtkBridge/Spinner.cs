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
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors:
//      Mike Gorse <mgorse@novell.com>
// 
// Note: This class handles spinners that set a numeric value.
// Spinners that select items instead are treated as lists.

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class Spinner : ComponentAdapter, Atk.ValueImplementor
	{
		private IRawElementProviderSimple provider;
		private IRangeValueProvider rangeValueProvider;

		public Spinner (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			string text = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = text;
			Role = Atk.Role.SpinButton;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
		}

		public override IRawElementProviderSimple Provider {
			get { return provider; }
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
			value = new GLib.Value(rangeValueProvider.Value);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			double v = (double)value.Val;
			if (v < rangeValueProvider.Minimum || v > rangeValueProvider.Maximum)
				return false;
			rangeValueProvider.SetValue (v);
			return true;
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}
	}
}
