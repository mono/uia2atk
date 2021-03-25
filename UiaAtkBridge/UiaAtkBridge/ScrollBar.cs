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

	public class ScrollBar : ComponentParentAdapter , Atk.IValueImplementor
	{
		private IRangeValueProvider rangeValueProvider;
		private IScrollProvider parentScrollProvider = null;
		private OrientationType orientation;
		
		public ScrollBar (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.ScrollBar;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			orientation = (OrientationType)provider.GetPropertyValue (AutomationElementIdentifiers.OrientationProperty.Id);
		}

		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Minimum: 0);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Maximum: 100);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.SmallChange: 1);
		}

		public void GetCurrentValue (ref GLib.Value value)
		{
			value = new GLib.Value (Value);
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			Value =  (double)value.Val;
			return SetCurrentValue (value);
		}

		private bool SetCurrentValue (double v)
		{
			if (rangeValueProvider != null) {
				if (v > rangeValueProvider.Maximum)
					return false;
				if (v < rangeValueProvider.Minimum)
					v = rangeValueProvider.Minimum;

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
			
			if (parentScrollProvider == null)
				GetScrollProviderFromParent ();
			
			//TODO: This double validation will be removed in future versions because
			//if your SWF.ScrollBar.Parent doesn't support IScrollProvider then your
			//ScrollBar is not ScrollBar is Pane!!
			if (parentScrollProvider == null)
				return false;

			if (orientation == OrientationType.Vertical) {
				try {
					parentScrollProvider.SetScrollPercent (parentScrollProvider.HorizontalScrollPercent, v);
				} catch (ArgumentOutOfRangeException e) {
					Log.Debug (e);
					return false;
				}
			} else {
				try {
					parentScrollProvider.SetScrollPercent (v, parentScrollProvider.VerticalScrollPercent);
				} catch (ArgumentOutOfRangeException e) {
					Log.Debug (e);
					return false;
				}
			}
			return true;
		}
		public void GetValueAndText (out double value, out string text)
		{
			value = Value;
			text = "";
		}

		public Atk.Range Range {
			get {
				double minimum = (rangeValueProvider != null? rangeValueProvider.Minimum: 0);
				double maximum = (rangeValueProvider != null? rangeValueProvider.Maximum: 0);
				return new Atk.Range (minimum, maximum, "");
			}
		}

		public double Increment {
			get {
			return (rangeValueProvider != null? rangeValueProvider.SmallChange: 1);
			}
		}

		public GLib.SList SubRanges {
			get {
				return null;
			}
		}

		public double Value {
			get {
				if (rangeValueProvider != null)
					return rangeValueProvider.Value;
			
				if (parentScrollProvider == null)
					GetScrollProviderFromParent ();
			
				//TODO: This double validation will be removed in future versions because
				//if your SWF.ScrollBar.Parent doesn't support IScrollProvider then your
				//ScrollBar is not ScrollBar is Pane!!
				if (parentScrollProvider == null) {
					Log.Warn ("ScrollBar: Provider does not implement IValueProvider.");
					return 0;
				}
				else
				{
					return (orientation == OrientationType.Vertical? parentScrollProvider.VerticalScrollPercent: parentScrollProvider.HorizontalScrollPercent);
				}
			}
			set {
				SetCurrentValue (value);
			}
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			// Selectable added by atk if parent has Atk.Selection
			states.RemoveState (Atk.StateType.Selectable);
			states.AddState (orientation == OrientationType.Vertical? Atk.StateType.Vertical: Atk.StateType.Horizontal);
			return states;
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
			Log.Warn ("ScrollBar: RaiseStructureChangedEvent not implemented.");
		}

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}
		
		private void GetScrollProviderFromParent ()
		{
			Adapter parentAdapter = Parent as Adapter;
			if (parentAdapter != null)
				parentScrollProvider = (IScrollProvider)parentAdapter.Provider.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == RangeValuePatternIdentifiers.ValueProperty) {
				Notify ("accessible-value");
			}
		}
	}
}
