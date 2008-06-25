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
	public class ComboBox : ComponentParentAdapter, Atk.ActionImplementor, Atk.SelectionImplementor
	{
		private string[] ChildrenItems {
			get {
				List<string> children = new List<string> ();
				
				IRawElementProviderFragmentRoot rootProvider =
					(IRawElementProviderFragmentRoot)provider;
				IRawElementProviderFragment child = rootProvider.Navigate (NavigateDirection.FirstChild);
	
				do {
					children.Add ((string) child.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id));
					child.Navigate (NavigateDirection.NextSibling);
				} while (child != null);
				
				return children.ToArray ();
			}
		}
		
		private string actionDescription = null;
		private string actionName = "press";
		private ISelectionProvider selProvider = null;
		
		public ComboBox (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			this.Role = Atk.Role.ComboBox;
			children.Add (new Menu (ChildrenItems));
			selProvider = (ISelectionProvider)provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			if (selProvider == null)
				throw new NotImplementedException ("ComboBoxProvider should always implement ISelectionProvider");
		}

		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}
		
		private IRawElementProviderSimple provider = null;
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			bool canFocus = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			if (canFocus)
				states.AddState (Atk.StateType.Selectable);
			else
				states.RemoveState (Atk.StateType.Selectable);
			
			bool enabled = (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsEnabledProperty.Id);
			if (enabled)
				states.AddState (Atk.StateType.Enabled);
			else
				states.RemoveState (Atk.StateType.Enabled);
			
			return states;
		}

		
		int Atk.ActionImplementor.NActions {
			get {
				return 1;
			}
		}
		
		bool pressed = false;
		
		bool Atk.ActionImplementor.DoAction (int i)
		{
			if (i != 0)
				return false;
			pressed = !pressed;
			return pressed;
		}

		string Atk.ActionImplementor.GetDescription (int i)
		{
			if (i != 0)
				return null;
			return actionDescription;
		}

		string Atk.ActionImplementor.GetName (int i)
		{
			if (i != 0)
				return null;
			return actionName;
		}

		string Atk.ActionImplementor.GetKeybinding (int i)
		{
			//TODO:
			return null;
		}

		bool Atk.ActionImplementor.SetDescription (int i, string desc)
		{
			if (i != 0)
				return false;
			actionDescription = desc;
			return true;
		}

		string Atk.ActionImplementor.GetLocalizedName (int i)
		{
			if (i != 0)
				return null;
			return actionName;
		}

		// not multi-selection for now
		int Atk.SelectionImplementor.SelectionCount {
			get {
				return (childSelected != null)? 1 : 0;
			}
		}
		
		bool Atk.SelectionImplementor.AddSelection (int i)
		{
			if (i >= 0) {
				IRawElementProviderFragmentRoot rootProvider =
					(IRawElementProviderFragmentRoot)provider;
				IRawElementProviderFragment child = rootProvider.Navigate (NavigateDirection.FirstChild);
	
				if (i > 0) {
					int current = 0;
					while (current < i) {
						child = child.Navigate (NavigateDirection.NextSibling);
						if (child == null)
							//selection out of bounds, return true anyway (according to unit tests)
							return true;
						current++;
					}
				}
				childSelected = i;
				Name = (string) child.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			}
			return true;
		}

		bool Atk.SelectionImplementor.ClearSelection ()
		{
			childSelected = null;
			Name = null;
			return true;
		}

		Atk.Object Atk.SelectionImplementor.RefSelection (int i)
		{
			throw new NotImplementedException ();
		}

		private int? childSelected = null;
		
		bool Atk.SelectionImplementor.IsChildSelected (int i)
		{
			return (!((childSelected == null) || (childSelected != i)));
		}

		bool Atk.SelectionImplementor.RemoveSelection (int i)
		{
			throw new NotImplementedException ();
		}

		bool Atk.SelectionImplementor.SelectAllSelection ()
		{
			throw new NotImplementedException ();
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			throw new NotImplementedException ();
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			throw new NotImplementedException ();
		}
		
		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			throw new NotImplementedException ();
		}
	}
}
