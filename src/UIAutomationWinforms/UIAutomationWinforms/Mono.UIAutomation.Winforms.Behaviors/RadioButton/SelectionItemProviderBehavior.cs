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
using System.Linq;
using System.Collections.Generic;
using SWF = System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.RadioButton
{
	internal class SelectionItemProviderBehavior :
		ProviderBehavior, ISelectionItemProvider
	{
#region Private Members
		
		private SWF.RadioButton radioButton;
		
#endregion
		
#region Constructor
		
		public SelectionItemProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
			radioButton = (SWF.RadioButton) provider.Control;
		}
		
#endregion
		
#region IProviderBehavior Interface

		public override void Connect ()
		{
			//TODO: Use SetEventStrategy
			radioButton.CheckedChanged += OnCheckedChanged;
		}
		
		public override void Disconnect ()
		{
			//TODO: Use SetEventStrategy
			radioButton.CheckedChanged -= OnCheckedChanged;
		}
		
		public override AutomationPattern ProviderPattern { 
			get { return SelectionItemPatternIdentifiers.Pattern; }
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
				return IsSelected;
			else if (propertyId == SelectionItemPatternIdentifiers.SelectionContainerProperty.Id)
				return SelectionContainer;
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
		
#region ISelectionItem Members
	
		public void AddToSelection ()
		{
			IEnumerable<SWF.RadioButton> otherButtons =
				from SWF.Control c in radioButton.Parent.Controls
					where c is SWF.RadioButton && c != radioButton
					select (SWF.RadioButton)c;
			
			foreach (SWF.RadioButton button in otherButtons)
				if (button.Checked)
					// Assuming CanSelectMultiple==false...
					throw new InvalidOperationException ("RadioButton");
			Select ();
		}

		public bool IsSelected {
			get { return radioButton.Checked; }
		}

		public void RemoveFromSelection ()
		{
			// Assuming IsSelectionRequired==true and CanSelectMultiple==false...
			throw new InvalidOperationException ("RadioButton");
		}

		public void Select ()
		{
			PerformSelectDelegate (radioButton);
		}

		public IRawElementProviderSimple SelectionContainer {
			get {
				IRawElementProviderSimple parentProvider =
					ProviderFactory.GetProvider (radioButton.Parent);
				if (parentProvider != null && parentProvider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id) != null)
					return parentProvider;
				return null;
			}
		}

#endregion
		
#region Event Handlers
		
		private void OnCheckedChanged (object sender, EventArgs e)
		{
			if (AutomationInteropProvider.ClientsAreListening) {
				AutomationEventArgs args =
					new AutomationEventArgs (SelectionItemPatternIdentifiers.ElementSelectedEvent);
				AutomationInteropProvider.RaiseAutomationEvent (SelectionItemPatternIdentifiers.ElementSelectedEvent,
				                                                Provider,
				                                                args);
				// TODO: Many other events to fire, including
				//       property change events!  This is also
				//       true for other providers, methinks.
			}
		}
		
#endregion
		
		#region Private Methods
		
		private void PerformSelectDelegate (SWF.RadioButton radioButton)
		{
			if (radioButton.InvokeRequired == true) {
				radioButton.BeginInvoke (new PerformSelectDelegate (PerformSelectDelegate),
				                         new object [] { radioButton });
				return;
			}
			radioButton.Checked = true;
		}
		
		#endregion 
	}
	
	delegate void PerformSelectDelegate (SWF.RadioButton radioButton);
}
