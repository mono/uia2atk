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
using System.Collections.Generic;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using Mono.Unix;

namespace Mono.UIAutomation.Winforms
{
	internal class MenuStripProvider : ToolStripProvider
	{
		private MenuStrip menu;
		
		public MenuStripProvider (MenuStrip menu) : base (menu)
		{
			this.menu = menu;
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.MenuBar.Id;
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("menu bar");
			else if (propertyId == AEIds.OrientationProperty.Id) {
				switch (menu.Orientation) {
				case Orientation.Vertical:
					return OrientationType.Vertical;
				case Orientation.Horizontal:
				default:
					return OrientationType.Horizontal;
				}
			} else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return true;
			else if (propertyId == AEIds.AccessKeyProperty.Id)
				return "ALT";
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
	}
}
