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

namespace Mono.UIAutomation.Winforms.Events
{

	internal abstract class BaseAutomationPropertyEvent : ProviderEvent
	{
		#region Constructors
		
		protected BaseAutomationPropertyEvent (IRawElementProviderSimple provider,
		                                       AutomationProperty property)
			: base (provider)
		{
			this.property = property;
			OldValue = Provider.GetPropertyValue (Property.Id);
		}
		
		#endregion
		
		#region Public Properties
		
		public AutomationProperty Property {
			get { return property; }
		}
		
		public object OldValue {
			get { return oldValue; }
			private set { oldValue = value; }
		}
		
		#endregion
		
		#region Protected Methods
		
		protected void RaiseAutomationPropertyChangedEvent ()
		{
			if (AutomationInteropProvider.ClientsAreListening == true) {
				object newValue = Provider.GetPropertyValue (Property.Id);
				
				if (object.Equals (OldValue, newValue) == false) {
					
					AutomationPropertyChangedEventArgs args =					
						new AutomationPropertyChangedEventArgs (Property,
						                                        OldValue,
						                                        newValue);
					AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (Provider, 
					                                                               args);
					OldValue = newValue;
				}
			}
		}
		
		#endregion
		
		#region Private fields
		
		private object oldValue;
		private AutomationProperty property;
		
		#endregion
	}
}
