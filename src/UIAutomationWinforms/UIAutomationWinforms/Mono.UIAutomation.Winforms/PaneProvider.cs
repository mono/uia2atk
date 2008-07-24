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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{

	internal class PaneProvider : FragmentControlProvider
	{

		#region Constructor
		
		public PaneProvider (Component component) : base (component)
		{
			this.component = component;
		}
		
		#endregion
		
		#region SimpleControlProvider: Specialization
		
		public override Component Container  {
			get { return component.Container as Component; }
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return string.Empty; //FIXME: "must always be a clear, concise and meaningful title."
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "pane";
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return false;
			else
				return base.GetPropertyValue (propertyId);
		}
		
		//LabeledByProperty
		//HelpTextProperty
		//AccessKeyProperty
		//NameProperty
		#endregion
		
		#region Private Fields
		
		private Component component;
		
		#endregion

	}
}
