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
using Mono.Unix;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.CheckBox;
using ButtonBehaviors = Mono.UIAutomation.Winforms.Behaviors.Button;

namespace Mono.UIAutomation.Winforms
{
	
	[MapsComponent (typeof (CheckBox))]
	internal class CheckBoxProvider : FragmentControlProvider
	{
	
#region Constructors
		
		public CheckBoxProvider (CheckBox checkbox) : base (checkbox)
		{
		}
		
#endregion
		
#region Public Methods

		public override void Initialize ()
		{
			base.Initialize ();
			
			SetBehavior (TogglePatternIdentifiers.Pattern,
			             new ToggleProviderBehavior (this));
			SetBehavior (EmbeddedImagePatternIdentifiers.Pattern, 
			             new ButtonBehaviors.EmbeddedImageProviderBehavior (this));
		}
		
#endregion
		
#region IRawElementProviderSimple Members
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.CheckBox.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("check box");
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id) 	 
				return null; 	 
			else
				return base.GetProviderPropertyValue (propertyId);
		}

#endregion

	}
}
