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
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using RB = Mono.UIAutomation.Winforms.Behaviors.RadioButton;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class FragmentRootControlProvider 
		: FragmentControlProvider, IRawElementProviderFragmentRoot
	{
		#region Private Members

		private bool hasRadioButtonChild;

		#endregion

		#region Constructors
		
		protected FragmentRootControlProvider (Component component) :
			base (component)
		{
		}
		
		#endregion

		#region IRawElementProviderFragmentRoot Interface
		
		public virtual IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			if (!BoundingRectangle.Contains (x, y))
				return null;
			
			// TODO: Check for child fragments.  Can this logic be generalized?
			
			return this;
		}
		
		public virtual IRawElementProviderFragment GetFocus ()
		{
			// TODO: Check for child fragments.  Can this logic be generalized?
			return null;
		}
		
		#endregion
		
		#region FragmentControlProvider Overrides

		//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.fragmentroot.aspx
		public override IRawElementProviderFragmentRoot FragmentRoot
		{
			get
			{ 
				if (Container == null)
					return this;
				return (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (Container);
			}
		}

		protected override void InsertChildProviderBefore (FragmentControlProvider newChild, FragmentControlProvider baseChild, bool raiseEvent, bool recursive)
		{
			base.InsertChildProviderBefore (newChild, baseChild, raiseEvent, recursive);
			
			// TODO: Figure out exactly when to do this (talk to bridge guys)
			CheckForRadioButtonChild (newChild);
		}

		public override void RemoveChildProvider (FragmentControlProvider removedProvider, bool raiseEvent)
		{
			base.RemoveChildProvider (removedProvider, raiseEvent);

			if (hasRadioButtonChild) {
				bool radioButtonFound = false;
				foreach (FragmentControlProvider childProvider in Navigation.GetAllChildren ()) {
					if (childProvider != removedProvider &&
					    (int) childProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.RadioButton.Id)
					{
						radioButtonFound = true;
						break;
					}
				}
				if (!radioButtonFound) {
					SetBehavior (SelectionPatternIdentifiers.Pattern, null);
					hasRadioButtonChild = false;
				}
			}
		}

		#endregion
		
		#region Private Methods
		
		// If a child control is ControlType.RadioButton, this provider
		// needs to provide SelectionPattern behavior.
		private void CheckForRadioButtonChild (IRawElementProviderSimple childProvider)
		{
			if (GetBehavior (SelectionPatternIdentifiers.Pattern) == null &&
			    childProvider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) != null &&
			    (int) childProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.RadioButton.Id) {
				var selectionProvider = new RB.SelectionProviderBehavior (this);
				SetBehavior (SelectionPatternIdentifiers.Pattern, selectionProvider);
				hasRadioButtonChild = true;
			}
		}
		
		#endregion
	}
}
