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
		private IRawElementProviderFragment ChildrenHolder {
			get {
				if (childrenHolder == null) {
					IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
					while (child != null) {
						if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) 
						  == ControlType.List.Id) 
							break;
						child = child.Navigate (NavigateDirection.NextSibling);
					}
					childrenHolder = child;
				}
				return childrenHolder;
			}
		}

		private IRawElementProviderFragment TextBoxHolder {
			get {
				if (textboxHolder == null) {
					IRawElementProviderFragment child = provider.Navigate (NavigateDirection.FirstChild);
					while (child != null) {
						if ((int) child.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) 
						  == ControlType.Edit.Id) 
							break;
						child = child.Navigate (NavigateDirection.NextSibling);
					}
					textboxHolder = child;
				}
				return textboxHolder;
			}
		}

		private IRawElementProviderFragment textboxHolder = null;
		private IRawElementProviderFragment childrenHolder = null;
		
		private string actionDescription = null;
		private string actionName = "press";
		private ISelectionProvider 					selProvider;
		
		//this one, when not null, indicates that the combobox is editable (like a gtkcomboboxentry vs normal gtkcombobox)
		private IValueProvider						valProvider;
		private IRawElementProviderFragmentRoot 	provider;
		private SelectionProviderUserHelper			selectionHelper;
		
		
		public ComboBox (IRawElementProviderFragmentRoot provider)
		{
			this.provider = provider;
			this.Role = Atk.Role.ComboBox;

			selProvider = (ISelectionProvider)provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			valProvider = (IValueProvider)provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			
			if (selProvider == null)
				throw new ArgumentException ("ComboBoxProvider should always implement ISelectionProvider");

			MenuItem menu = new MenuItem (ChildrenHolder);
			children.Add (menu); //yes, even if combobox is empty
			AutomationBridge.ProviderAdapterMapping [ChildrenHolder] = menu;
			
			if (valProvider != null)
				children.Add (new TextBoxEntryView (TextBoxHolder));
			
			selectionHelper = new SelectionProviderUserHelper(provider, selProvider, ChildrenHolder);
		}

		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			//FIXME: figure out why Gail comboboxes don't like this state
			states.RemoveState (Atk.StateType.Focusable);
			return states;
		}
		
		int Atk.ActionImplementor.NActions {
			get { return 1; }
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

#region Atk.SelectionImplementor

		int Atk.SelectionImplementor.SelectionCount
		{
			get { return selectionHelper.SelectionCount; }
		}
		bool Atk.SelectionImplementor.AddSelection (int i)
		{
			return selectionHelper.AddSelection(i);
		}
		bool Atk.SelectionImplementor.ClearSelection ()
		{
			return selectionHelper.ClearSelection();
		}
		Atk.Object Atk.SelectionImplementor.RefSelection (int i)
		{
			return selectionHelper.RefSelection(i);
		}
		bool Atk.SelectionImplementor.IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected(i);
		}
		bool Atk.SelectionImplementor.RemoveSelection (int i)
		{
			return selectionHelper.RemoveSelection(i);
		}
		bool Atk.SelectionImplementor.SelectAllSelection ()
		{
			return selectionHelper.SelectAllSelection();
		}

#endregion

		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}

		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			// TODO
		}
	}
}
