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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	public class ThumbProvider : SimpleControlProvider
	{

#region Constructor

		public ThumbProvider () : base (null)
		{
			//TODO: How to implement this?
			//BoundingRectangleProperty property-changed event.
			//IsOffscreenProperty property-changed event.
			//IsEnabledProperty property-changed event.
			//AutomationFocusChangedEvent
			//StructureChangedEvent
		}

#endregion
		
#region Public Overrides

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
				return 1; //FIXME: This doesn't make sense.
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return null; //TODO: We may need to use Reflection to get the "real" value
			else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return null; //TODO: We may need to use Reflection to get the "real" value
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Thumb.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "thumb";
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return true;
			else
				return null;
		}
		
#endregion

	}

}
