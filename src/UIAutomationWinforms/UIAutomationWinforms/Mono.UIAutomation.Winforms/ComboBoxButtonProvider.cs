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
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	public class ComboBoxButtonProvider : FragmentControlProvider
	{
#region Constructors

		public ComboBoxButtonProvider ()
			: base (null)
		{
//			combobox_container = provider;
			
			//TODO: Add "InvokePatternIdentifiers.Pattern" Behavior
		}
		
#endregion

//#region Public Properties
//
//		public ComboBox ComboBoxContainer {
//			get { return combobox_container; }
//		}
//
//#endregion
		
#region Public Methods
		
		public override void InitializeEvents ()
		{
			//We don't to support any event associated to this.Control.
			//However we need to defined the following events:
			//
			//AutomationFocusChangedEvent
			//BoundingRectangleProperty property-changed event.
			//IsOffscreenProperty property-changed event.
			//IsEnabledProperty property-changed event.
			//NameProperty property-changed event.
			//StructureChangedEvent
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			//TODO: We may need to get VALID information using Reflection from 
			//ScrollBarContainer and return those values, I'm thiking in the
			//following propierties: BoundingRectangleProperty and ClickablePointProperty
			if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
				return 1; //FIXME: Get a valid value
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return "Drop Down Button"; //TODO: i18n?
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion

//#region Private Fields
//
//		private ComboBox combobox_container;
//		
//#endregion
		
	}
}
