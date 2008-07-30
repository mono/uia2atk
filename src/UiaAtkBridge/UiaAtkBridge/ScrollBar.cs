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
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class ScrollBar : ComponentParentAdapter , Atk.ValueImplementor
	{
		private IRawElementProviderSimple provider;
		private IRangeValueProvider rangeValueProvider;
		private IScrollProvider parentScrollProvider = null;
		private OrientationType orientation;
		
		public ScrollBar (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			Role = Atk.Role.ScrollBar;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Adapter parentAdapter = Parent as Adapter;
			if (parentAdapter != null)
				parentScrollProvider = (IScrollProvider)parentAdapter.Provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
			orientation = (OrientationType)provider.GetPropertyValue (AutomationElementIdentifiers.OrientationProperty.Id);
		}
		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public GLib.Value MinimumValue
		{
			get {
				return new GLib.Value (rangeValueProvider.Minimum);
			}
		}

		public GLib.Value MaximumValue
		{
			get {
				return new GLib.Value(rangeValueProvider.Maximum);
			}
		}

		public GLib.Value MinimumIncrement
		{
			get {
				return new GLib.Value (rangeValueProvider.SmallChange);
			}
		}

		public GLib.Value CurrentValue
		{
			get {
				if (rangeValueProvider != null)
					return new GLib.Value (rangeValueProvider.Value);
				else
				{
					return new GLib.Value (orientation == OrientationType.Horizontal? parentScrollProvider.VerticalScrollPercent: parentScrollProvider.HorizontalScrollPercent);
				}
			}
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			double v = (double)value.Val;
			if (v < 0 || v > 100) return false;
			rangeValueProvider.SetValue (v);
			return true;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (orientation == OrientationType.Vertical? Atk.StateType.Vertical: Atk.StateType.Horizontal);
			return states;
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
			Console.WriteLine ("Received StructureChangedEvent in Scrollbar--todo");
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			// TODO
		}
		}
}
