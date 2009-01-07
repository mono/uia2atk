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
// 	Neville Gao <nevillegao@gmail.com>
// 

using System;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.Unix;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Behaviors.BaseColorControl;

namespace Mono.UIAutomation.Winforms
{
	internal class BaseColorControlProvider : FragmentRootControlProvider
	{
		#region Constructor

		public BaseColorControlProvider (ColorDialog.BaseColorControl baseColorControl)
			: base (baseColorControl)
		{
		}
		
		#endregion
		
		#region SimpleControlProvider: Specializations
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("pane");
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		#endregion

		#region Internal Class: SmallColorControl Provider

		internal class SmallColorControlProvider : FragmentControlProvider
		{
			#region Constructor
	
			public SmallColorControlProvider (ColorDialog.BaseColorControl.SmallColorControl smallColorControl)
				: base (smallColorControl)
			{
			}
			
			#endregion

			#region SimpleControlProvider: Specializations

			public override void Initialize()
			{
				base.Initialize ();
				
				SetBehavior (InvokePatternIdentifiers.Pattern,
				             new SmallColorControlInvokeProviderBehavior (this));
			}
			
			#endregion
			
			#region SimpleControlProvider: Specializations
			
			protected override object GetProviderPropertyValue (int propertyId)
			{
				if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
					return ControlType.Button.Id;
				else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
					return Catalog.GetString ("button");
				else
					return base.GetProviderPropertyValue (propertyId);
			}
			
			#endregion
		}

		#endregion
	}
}
