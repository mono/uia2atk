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
//	Neville Gao <nevillegao@gmail.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors
{
	internal class ProgressBarRangeValueProviderBehavior
		: ProviderBehavior, IRangeValueProvider
	{
		
		#region Constructors

		public ProgressBarRangeValueProviderBehavior (ProgressBarProvider provider)
			: base (provider)
		{
			progressBar = (ProgressBar) provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface		
		
		public override AutomationPattern ProviderPattern { 
			get { return RangeValuePatternIdentifiers.Pattern; }
		}

		public override void Connect (Control control)
		{
			Provider.SetEvent (ProviderEventType.RangeValuePatternValueProperty,
			                   new ProgressBarRangeValuePatternValueEvent (Provider));
		}
		
		public override void Disconnect (Control control)
		{
			Provider.SetEvent (ProviderEventType.RangeValuePatternValueProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == RangeValuePatternIdentifiers.ValueProperty.Id)
				return Value;
			else if (propertyId == RangeValuePatternIdentifiers.IsReadOnlyProperty.Id)
				return IsReadOnly;
			else if (propertyId == RangeValuePatternIdentifiers.MinimumProperty.Id)
				return Minimum;
			else if (propertyId == RangeValuePatternIdentifiers.MaximumProperty.Id)
				return Maximum;
			else if (propertyId == RangeValuePatternIdentifiers.LargeChangeProperty.Id)
				return LargeChange;
			else if (propertyId == RangeValuePatternIdentifiers.SmallChangeProperty.Id)
				return SmallChange;
			
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
		
		#region IRangeValueProvider Members
		
		public double Value {
			get { return (double) progressBar.Value; }
		}
		
		public bool IsReadOnly {
			get { return true; }
		}
		
		public double Minimum {
			get { return 0.0; }
		}
		
		public double Maximum {
			get { return 100.0; }
		}
		
		public double LargeChange {
			get { return Double.NaN; }
		}
		
		public double SmallChange {
			get { return Double.NaN; }
		}
		
		public void SetValue (double value)
		{
			if (value < Minimum || value > Maximum)
				throw new ArgumentOutOfRangeException ();
			
			progressBar.Value = (int) value;
		}
		
		#endregion

		#region Private Fields
		
		private ProgressBar progressBar;
		
		#endregion
		
	}
}
