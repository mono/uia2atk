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
using Mono.Unix;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class ToolTipBaseProvider : FragmentRootControlProvider 
	{
		
		#region Constructors 

		protected ToolTipBaseProvider (Component component) : base (component)
		{
			message = string.Empty;
		}
		
		#endregion 

		#region SimpleControlProvider: Specializations
		
		public override Component Container {
			get { return null; }
		}

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return this; }
		}
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ToolTip.Id;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return message;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				//When the tooltip is offscren is hidden, so is removed from tree.
				return false;
			else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return true;
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		#endregion
		
		#region Protected Methods

		protected abstract string GetTextFromControl (Control control);

		protected string Message {
			set { message = value; }
		}
		
		#endregion

		#region Public Methods
		
		public void Show (Control control) 
		{
			if (AutomationInteropProvider.ClientsAreListening == true) {					
				message = GetTextFromControl (control);
				
				//TODO: We need deeper tests in Vista because MS is generating both events
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
				                                   this);

				AutomationEventArgs eventArgs 
					= new AutomationEventArgs (AutomationElementIdentifiers.ToolTipOpenedEvent);
				AutomationInteropProvider.RaiseAutomationEvent (this, eventArgs);
			}
		}
		
		public void Hide (Control control)
		{
			if (AutomationInteropProvider.ClientsAreListening == true) {
				message = GetTextFromControl (control);
				
				//TODO: We need deeper tests in Vista because MS is generating both events
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
				                                   this);
				
				AutomationEventArgs eventArgs 
					= new AutomationEventArgs (AutomationElementIdentifiers.ToolTipClosedEvent);
				AutomationInteropProvider.RaiseAutomationEvent (this, eventArgs);
			}
		}

		#endregion
		
		#region Private Fields
		
		private string message;
		
		#endregion
	}
}

