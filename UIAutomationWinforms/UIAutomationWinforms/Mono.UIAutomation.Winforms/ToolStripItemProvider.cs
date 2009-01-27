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

using Mono.Unix;

using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors.ToolStripItem;
using ETSI = Mono.UIAutomation.Winforms.Events.ToolStripItem;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class ToolStripItemProvider : FragmentControlProvider
	{
		private ToolStripItem item;

		public ToolStripItemProvider (ToolStripItem item) : base (item)
		{
			this.item = item;
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {		
			get {
				//if (item.OwnerItem != null)
				//	return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (item.OwnerItem);
				//else
					return (IRawElementProviderFragmentRoot) ProviderFactory.FindProvider (item.Owner);
			}
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AEIds.ControlTypeProperty.Id)
				return ControlType.MenuItem.Id; // TODO: Verify this default
			else if (propertyId == AEIds.LocalizedControlTypeProperty.Id)
				return Catalog.GetString ("menu item");
			else if (propertyId == AEIds.LabeledByProperty.Id)
				return null;
			else if (propertyId == AEIds.NameProperty.Id)
				return item.Text;
			else if (propertyId == AEIds.IsOffscreenProperty.Id) {
				return Helper.ToolStripItemIsOffScreen (item);
			} else if (propertyId == AEIds.IsEnabledProperty.Id)
				return item.Enabled;
			else if (propertyId == AEIds.HasKeyboardFocusProperty.Id)
				return item.Selected;
			else if (propertyId == AEIds.IsKeyboardFocusableProperty.Id)
				return item.OwnerItem != null &&
					item.CanSelect &&
					Navigate (NavigateDirection.FirstChild) == null;
			else if (propertyId == AEIds.BoundingRectangleProperty.Id)
				return Helper.GetToolStripItemScreenBounds (item);
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		public override void Initialize()
		{
			base.Initialize ();

			SetBehavior (EmbeddedImagePatternIdentifiers.Pattern, 
			             new EmbeddedImageProviderBehavior (this));
			
			SetEvent (ProviderEventType.AutomationElementIsOffscreenProperty,
			          new ETSI.AutomationIsOffscreenPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsEnabledProperty, 
			          new ETSI.AutomationIsEnabledPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementHasKeyboardFocusProperty,
			          new ETSI.AutomationHasKeyboardFocusPropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementBoundingRectangleProperty,
			          new ETSI.AutomationBoundingRectanglePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationFocusChangedEvent,
			          new ETSI.AutomationFocusChangedEvent (this));
			SetEvent (ProviderEventType.AutomationElementIsKeyboardFocusableProperty,
			          new ETSI.AutomationIsKeyboardFocusablePropertyEvent (this));
			SetEvent (ProviderEventType.AutomationElementNameProperty,
			          new ETSI.AutomationNamePropertyEvent (this));
		}
	}
}
