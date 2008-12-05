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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class ComboBoxDropDown : ComboBox, Atk.ActionImplementor
	{
		
		private string actionDescription = null;
		private string actionName = "press";
		
		private IRawElementProviderFragmentRoot 	provider;
		private IExpandCollapseProvider				expandColapseProvider;
		private MenuItem InnerMenu {
			get { return (MenuItem)RefAccessibleChild (0); }
		}
		
		public ComboBoxDropDown (IRawElementProviderSimple provider) : base (provider)
		{
			this.provider = provider as IRawElementProviderFragmentRoot;
			if (this.provider == null)
				throw new ArgumentException ("Provider should be IRawElementProviderFragmentRoot");
			
			expandColapseProvider = (IExpandCollapseProvider)provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id);
		}
		
		
		public int NActions {
			get { return 1; }
		}
		
		public bool DoAction (int i)
		{
			if (i != 0)
				return false;
			try {
				switch (expandColapseProvider.ExpandCollapseState) {
				case ExpandCollapseState.Collapsed:
					expandColapseProvider.Expand ();
					break;
				case ExpandCollapseState.Expanded:
					expandColapseProvider.Collapse ();
					break;
				default:
					throw new NotSupportedException ("A combobox should not have an ExpandCollapseState different than Collapsed/Expanded");
				}
				return true;
			} catch (ElementNotEnabledException) { }
			return false;
		}

		public string GetDescription (int i)
		{
			if (i != 0)
				return null;
			return actionDescription;
		}

		public string GetName (int i)
		{
			if (i != 0)
				return null;
			return actionName;
		}

		public string GetKeybinding (int i)
		{
			//TODO:
			return null;
		}

		public bool SetDescription (int i, string desc)
		{
			if (i != 0)
				return false;
			actionDescription = desc;
			return true;
		}

		public string GetLocalizedName (int i)
		{
			if (i != 0)
				return null;
			return actionName;
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			base.RaiseAutomationEvent (eventId, e);
		}

		private Window fakeWindow = null;
		
		public override void RaiseAutomationPropertyChangedEvent (System.Windows.Automation.AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id != ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty.Id) {
				base.RaiseAutomationPropertyChangedEvent (e);
				return;
			}

			ExpandCollapseState newState = (ExpandCollapseState)e.NewValue;
			if (newState == ExpandCollapseState.Expanded) {
				if (fakeWindow == null) {
					fakeWindow = new Window ();
					fakeWindow.AddOneChild ((Adapter)RefAccessibleChild (0));
				}
				TopLevelRootItem.Instance.AddOneChild (fakeWindow);
			} else if (newState == ExpandCollapseState.Collapsed) {
				TopLevelRootItem.Instance.RemoveChild (fakeWindow);
			}
			InnerMenu.RaiseExpandedCollapsed ();
		}


		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			// TODO
		}
	}
}
