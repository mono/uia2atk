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
//	Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TrackBar;

namespace Mono.UIAutomation.Winforms.Behaviors.TrackBar
{

	internal class RangeValueProviderBehavior 
		: ProviderBehavior, IRangeValueProvider
	{

		#region Constructors
		
		public RangeValueProviderBehavior (TrackBarProvider provider)
			: base (provider) 
		{
			trackbar = (SWF.TrackBar) provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern {
			get { return RangeValuePatternIdentifiers.Pattern; }
		}		
		
		public override void Connect () 
		{
			Provider.SetEvent (ProviderEventType.RangeValuePatternIsReadOnlyProperty,
			                   new RangeValuePatternIsReadOnlyEvent (Provider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternLargeChangeProperty,
			                   new RangeValuePatternLargeChangeEvent (Provider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternMaximumProperty,
			                   new RangeValuePatternMaximumEvent (Provider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternMinimumProperty,
			                   new RangeValuePatternMinimumEvent (Provider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternSmallChangeProperty,
			                   new RangeValuePatternSmallChangeEvent (Provider));
			Provider.SetEvent (ProviderEventType.RangeValuePatternValueProperty,
			                   new RangeValuePatternValueEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.RangeValuePatternIsReadOnlyProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternLargeChangeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternMaximumProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternMinimumProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternSmallChangeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.RangeValuePatternValueProperty, 
			                   null);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == RangeValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else if (propertyId == RangeValuePatternIdentifiers.LargeChangeProperty.Id)
				return LargeChange;
			else if (propertyId == RangeValuePatternIdentifiers.MaximumProperty.Id)
				return Maximum;
			else if (propertyId == RangeValuePatternIdentifiers.MinimumProperty.Id)
				return Minimum;
			else if (propertyId == RangeValuePatternIdentifiers.SmallChangeProperty.Id)
				return SmallChange;
			else if (propertyId == RangeValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion	
			
		#region IRangeValueProvider implementation 
		
		public void SetValue (double value)
		{
			if (value < Minimum || value > Maximum)
				throw new ArgumentOutOfRangeException ();
			
			PerformSetValue ((int) value);
		}
		
		public bool IsReadOnly {
			get { return trackbar.Enabled == false || trackbar.Visible == false; }
		}
		
		public double LargeChange {
			get { return trackbar.LargeChange; }
		}
		
		public double Maximum {
			get { return trackbar.Maximum; }
		}
		
		public double Minimum {
			get { return trackbar.Minimum; }
		}
		
		public double SmallChange {
			get { return trackbar.SmallChange; }
		}
		
		public double Value {
			get { return trackbar.Value; }
		}
		
		#endregion 
		
		#region Private Methods
		
		private void PerformSetValue (int value)
		{
			if (trackbar.InvokeRequired == true) {
				trackbar.BeginInvoke (new PerformSetValueDelegate (PerformSetValue),
				                       new object [] { value });
				return;
			}
			trackbar.Value = value;
		}
		
		#endregion

		#region Private Fields

		private SWF.TrackBar trackbar;

		#endregion
	}
	
	delegate void PerformSetValueDelegate (int value);
}
