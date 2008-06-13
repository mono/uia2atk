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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms
{
	public class NumericUpDownProvider : SimpleControlProvider, IRangeValueProvider
	{
#region Private Fields
		
		private NumericUpDown upDown;
		
#endregion
		
#region Constructors
		
		public NumericUpDownProvider (NumericUpDown upDown) :
			base (upDown)
		{
			this.upDown = upDown;
			
			upDown.ValueChanged += OnValueChanged;
			//upDown.max
			
			// TODO: Use SetEventStrategy
			// TODO: Child InvokeProviders for up/down!
		}
		
#endregion
		
#region Public Methods
		
		public override void InitializeEvents ()
		{
			base.InitializeEvents ();

			SetEvent (ProviderEventType.InvokedEvent, 
			          new DefaultInvokedEvent (this));
		}
		
#endregion
		
#region IRawElementProviderSimple Members
		
		public override object GetPatternProvider (int patternId)
		{
			if (patternId == RangeValuePatternIdentifiers.Pattern.Id)
				return this;
			
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ClassNameProperty.Id)
				return "WindowsForms10.BUTTON.app.0.bf7d44";
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Spinner.Id;
			else if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id)
				return false; // TODO: ???
			else if (propertyId == RangeValuePatternIdentifiers.IsReadOnlyProperty.Id)
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
		
#region IRangeValueProvider Members
	
		public bool IsReadOnly {
			get { return upDown.ReadOnly; }
		}

		public double LargeChange {
			get { return double.NaN; }
		}

		public double Maximum {
			get { return (double) upDown.Maximum; }
		}

		public double Minimum {
			get { return (double) upDown.Minimum; }
		}

		public void SetValue (double value)
		{
			if (value < Minimum || value > Maximum)
				throw new ArgumentOutOfRangeException ();
			upDown.Value = (decimal) value;
		}

		public double SmallChange {
			get { return (double) upDown.Increment; }
		}

		public double Value {
			get { return (double) upDown.Value; }
		}

#endregion
		
#region Event Handlers
		
		private void OnValueChanged (object sender, EventArgs e)
		{
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationPropertyChangedEventArgs args =
					new AutomationPropertyChangedEventArgs (RangeValuePatternIdentifiers.ValueProperty,
					                                        null, // TODO: Test against MS
					                                        Value);
				AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (this, args);
			}
		}
		
#endregion
	}
}
