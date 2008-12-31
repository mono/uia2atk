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

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class MenuItem : ComboBoxOptions,
	                        Atk.ActionImplementor
	{
		IInvokeProvider invokeProvider = null;
		
		public MenuItem (IRawElementProviderSimple provider) : base (provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			if ((provider as IRawElementProviderFragment) == null)
				throw new ArgumentException ("Provider for ParentMenu should be IRawElementProviderFragment");

			string name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			if (!String.IsNullOrEmpty (name))
				Name = name;

			int controlType = (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			invokeProvider = (IInvokeProvider)provider.GetPatternProvider (InvokePatternIdentifiers.Pattern.Id);
			
			OnChildrenChanged ();
		}

		private bool selected = false;
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			states.AddState (Atk.StateType.Selectable);

			if (selected)
				states.AddState (Atk.StateType.Showing);
			else
				states.RemoveState (Atk.StateType.Showing);

			if (Parent.RefStateSet ().ContainsState (Atk.StateType.Visible)) {
				states.AddState (Atk.StateType.Visible);
				states.AddState (Atk.StateType.Showing);
			}

			if (selected) {
				states.AddState (Atk.StateType.Selected);

				bool canFocus = (bool) Provider.GetPropertyValue (
				     AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
				if (canFocus) {
					states.AddState (Atk.StateType.Focused);
				}
			} else {
				states.RemoveState (Atk.StateType.Selected);
			}

			return states;
		}

		private void OnChildrenChanged () 
		{
			IRawElementProviderFragment child = ((IRawElementProviderFragment)Provider).Navigate (NavigateDirection.FirstChild);
			
			if (child != null)
				Role = Atk.Role.Menu;
			else
				Role = Atk.Role.MenuItem;
		}
		
		protected override void OnChildrenChanged (uint change_index, IntPtr changed_child) 
		{
			OnChildrenChanged ();
		}

		public override Atk.Layer Layer {
			get { return IsToolBarItem? Atk.Layer.Widget: Atk.Layer.Popup; }
		}

		internal bool IsToolBarItem {
			get {
				Atk.Object testObj = Parent;
				if (testObj != null)
					testObj = testObj.Parent;
				if (testObj != null)
					testObj = testObj.Parent;
				Adapter parentAdapter = testObj as Adapter;
				return (parentAdapter != null && (int)parentAdapter.Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.ToolBar.Id);
			}
		}

		internal void Deselect ()
		{
			selected = false;
			NotifyStateChange (Atk.StateType.Selected, false);
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == InvokePatternIdentifiers.InvokedEvent) {
				selected = !selected;
				NotifyStateChange (Atk.StateType.Selected, selected);
				NotifyStateChange (Atk.StateType.Focused, selected);
				if (Parent is MenuItem)
					((MenuItem)Parent).RecursiveDeselect (this);
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				if (Parent is MenuItem)
					((MenuItem)Parent).RecursiveDeselect (this);
			} else if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent) {
				selected = true;
				NotifyStateChange (Atk.StateType.Selected, selected);
				if ((Parent is MenuItem) || (Parent is ComboBoxOptions))
					//FIXME: when decoupling ToolStripMenuItem from ComboBoxOptions, change this assumption as well:
					((ComboBoxOptions)Parent).RecursiveDeselect (this);
			} else {
				Console.WriteLine ("WARNING: RaiseAutomationEvent({0},...) not handled yet", eventId.ProgrammaticName);
				base.RaiseAutomationEvent (eventId, e);
			}
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id == SelectionItemPatternIdentifiers.IsSelectedProperty.Id) {
				selected = (bool)e.NewValue;
				NotifyStateChange (Atk.StateType.Selected, selected);
			} else if (e.Property.Id == AutomationElementIdentifiers.IsOffscreenProperty.Id) {
				NotifyStateChange (Atk.StateType.Showing);
			} else {
				base.RaiseAutomationPropertyChangedEvent (e);
			}
		}

		#region Action implementation 

		private string actionDescription = null;
		
		public bool DoAction (int i)
		{
			if (i == 0) {
				if (invokeProvider != null) {
					try {
						invokeProvider.Invoke ();
						return true;
					} catch (ElementNotEnabledException) { }
				}
			}
			return false;
		}
		
		public string GetName (int i)
		{
			if (i == 0 && invokeProvider != null)
				return "click";
			return null;
		}
		
		public string GetKeybinding (int i)
		{
			return null;
		}
		
		public string GetLocalizedName (int i)
		{
			return null;
		}
		
		public bool SetDescription (int i, string desc)
		{
			if (i == 0) {
				actionDescription = desc;
				return true;
			}
			return false;
		}
		
		public string GetDescription (int i)
		{
			if (i == 0)
				return actionDescription;
			return null;
		}

		
		public int NActions {
			get { return 1; }
		}
		
		#endregion 

		public override bool AddSelection (int i)
		{
			if ((i < 0) || (i >= NAccessibleChildren))
				return false;
			return ((MenuItem)RefAccessibleChild (i)).DoAction (0);
		}

		public override bool RemoveSelection (int i)
		{
			if (i == 0) {
				selectedChild = -1;
				return true;
			}
			return false;
		}

		public override bool ClearSelection ()
		{
			selectedChild = -1;
			return true;
		}
		
	}
}
