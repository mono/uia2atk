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
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms.Events
{

	//TODO: Should we define IsXXXPatternAvailableProperty classes instead?
	internal class AutomationIsPatternAvailablePropertyEvent : ProviderEvent
	{
		
		#region Constructors

		public AutomationIsPatternAvailablePropertyEvent (SimpleControlProvider provider) 
			: base (provider)
		{
		}
		
		#endregion

		#region IConnectable Overrides		
		
		public override void Connect ()
		{
			Provider.ProviderBehaviorSet += OnProviderBehaviorSet;
		}

		public override void Disconnect ()
		{
			Provider.ProviderBehaviorSet -= OnProviderBehaviorSet;
		}

		#endregion
		
		#region Private Methods
		
		private void OnProviderBehaviorSet (object sender, ProviderBehaviorEventArgs eventArgs)
		{
			if (AutomationInteropProvider.ClientsAreListening == false)
				return;
			else if ((eventArgs.Replaced == true && eventArgs.Behavior == null)
			    || (eventArgs.Replaced == false && eventArgs.Behavior != null)) {

				AutomationProperty property = null;
	
				if (eventArgs.Pattern == DockPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsDockPatternAvailableProperty;
				else if (eventArgs.Pattern == ExpandCollapsePatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty;
				else if (eventArgs.Pattern == GridItemPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsGridItemPatternAvailableProperty;
				else if (eventArgs.Pattern == GridPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsGridPatternAvailableProperty;
				else if (eventArgs.Pattern == InvokePatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsInvokePatternAvailableProperty;
				else if (eventArgs.Pattern == MultipleViewPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsMultipleViewPatternAvailableProperty;
				else if (eventArgs.Pattern == RangeValuePatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsRangeValuePatternAvailableProperty;
				else if (eventArgs.Pattern == ScrollItemPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsScrollItemPatternAvailableProperty;
				else if (eventArgs.Pattern == ScrollPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsScrollPatternAvailableProperty;
				else if (eventArgs.Pattern == SelectionItemPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsSelectionItemPatternAvailableProperty;
				else if (eventArgs.Pattern == SelectionPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsSelectionPatternAvailableProperty;
				else if (eventArgs.Pattern == TableItemPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsTableItemPatternAvailableProperty;
				else if (eventArgs.Pattern == TablePatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsTablePatternAvailableProperty;
				else if (eventArgs.Pattern == TextPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsTextPatternAvailableProperty;
				else if (eventArgs.Pattern == TogglePatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsTogglePatternAvailableProperty;
				else if (eventArgs.Pattern == TransformPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsTransformPatternAvailableProperty;
				else if (eventArgs.Pattern == ValuePatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsValuePatternAvailableProperty;
				else if (eventArgs.Pattern == WindowPatternIdentifiers.Pattern)
					property = AutomationElementIdentifiers.IsWindowPatternAvailableProperty;
				
				if (property == null) //This never should happen, theoretically
					return;
				
				bool? val = Provider.GetPropertyValue (property.Id) as bool?;
	
				// This should never happen.
				if (val == null)
					return;
				
				bool newValue = val ?? false;
				
				AutomationPropertyChangedEventArgs args 
					= new AutomationPropertyChangedEventArgs (property,
					                                          !newValue,
					                                          newValue);
				AutomationInteropProvider.RaiseAutomationPropertyChangedEvent (Provider, 
				                                                               args);
			}
		}
		
		#endregion

	}
}
