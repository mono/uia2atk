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

using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors.NumericUpDown;


namespace Mono.UIAutomation.Winforms
{
	internal class NumericUpDownProvider : FragmentControlProvider
	{
#region Constructors
		
		public NumericUpDownProvider (NumericUpDown upDown) :
			base (upDown)
		{
			// TODO: Child InvokeProviders for up/down!
			SetBehavior (RangeValuePatternIdentifiers.Pattern,
			             new RangeValueProviderBehavior (this));
		}
		
#endregion
		
#region Public Methods
		
		public override void InitializeEvents ()
		{
			base.InitializeEvents ();

			// TODO: Review...do we need this?
//			SetEvent (ProviderEventType.InvokePatternInvokedEvent, 
//			          new InvokePatternInvokedEvent (this));
		}
		
#endregion
		
#region IRawElementProviderSimple Members
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Spinner.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "spinner";
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion
	}
}
