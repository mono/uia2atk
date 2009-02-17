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

using System;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class ProgressBar : ComponentAdapter, Atk.ValueImplementor
	{
		private IRangeValueProvider rangeValueProvider;
		
		public ProgressBar (IRawElementProviderSimple provider) : base (provider)
		{
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Role = Atk.Role.ProgressBar;
		}
		
		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Minimum: 0);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Maximum: 0);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value ((double)0);
		}

		public void GetCurrentValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Value: 0);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			Log.Warn ("ProgressBar: SetCurrentValue not implemented");
			return false;
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == RangeValuePatternIdentifiers.ValueProperty)
				Notify ("accessible-value");
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
	}
}
