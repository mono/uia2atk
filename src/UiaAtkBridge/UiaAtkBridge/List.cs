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
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class List : ComponentParentAdapter, Atk.SelectionImplementor
	{
		private IRawElementProviderFragmentRoot		provider;
		private ISelectionProvider					selectionProvider;
		private SelectionProviderUserHelper	selectionHelper;
		private Adapter selectedItem;
		
/*
AtkObject,
?AtkAction,
?AtkSelection,
?AtkRelation (to associate a text label with the control),
?AtkRelationSet,
?AtkStateSet
*/


#region UI Automation Properties supported


		// AutomationElementIdentifiers.BoundingRectangleProperty.Id

		// AutomationElementIdentifiers.AutomationIdProperty.Id
		public string AutomationId
		{
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.AutomationIdProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id
		public bool IsKeyboardFocusable
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.NameProperty.Id
		// already handled by the Atk object

		// AutomationElementIdentifiers.ClickablePointProperty.Id
		public System.Windows.Point ClickablePoint
		{ 
			get {
				return (System.Windows.Point) provider.GetPropertyValue (AutomationElementIdentifiers.ClickablePointProperty.Id);
			}
		}

		// AutomationElementIdentifiers.ControlTypeProperty.Id
		public int ControlType
		{ 
			get {
				return (int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			}
		}

		// AutomationElementIdentifiers.LocalizedControlTypeProperty.Id
		public string LocalizedControlType
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id);
			}
		}
		
		// AutomationElementIdentifiers.IsContentElementProperty.Id
		public bool IsContentElement
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsContentElementProperty.Id);
			}
		}

		// AutomationElementIdentifiers.IsControlElementProperty.Id
		public bool IsControlElement
		{ 
			get {
				return (bool) provider.GetPropertyValue (AutomationElementIdentifiers.IsControlElementProperty.Id);
			}
		}

		// AutomationElementIdentifiers.HelpTextProperty.Id
		public string HelpText
		{ 
			get {
				return (string) provider.GetPropertyValue (AutomationElementIdentifiers.HelpTextProperty.Id);
			}
		}

		// SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id
		public bool IsSelectionRequired
		{ 
			get {
				return (bool) provider.GetPropertyValue (SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id);
			}
		}

		// SelectionPatternIdentifiers.CanSelectMultipleProperty.Id
		public bool CanSelectMultiple
		{ 
			get {
				return (bool) provider.GetPropertyValue (SelectionPatternIdentifiers.CanSelectMultipleProperty.Id);
			}
		}


#endregion


		public List (IRawElementProviderFragmentRoot provider)
		{
			this.provider = provider;
			
			selectionProvider = (ISelectionProvider)provider.GetPatternProvider(SelectionPatternIdentifiers.Pattern.Id);
			if (selectionProvider == null)
				throw new NotImplementedException ("List should always implement ISelectionProvider");

			Role = Atk.Role.List;
			
			string componentName = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = componentName;
			
			selectionHelper = new SelectionProviderUserHelper (provider, selectionProvider);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			states.AddState (StateType.ManagesDescendants);
			
			return states;
		}

		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public GLib.SList DefaultAttributes {
			get {
				throw new NotImplementedException();
			}
		}


#region Atk.SelectionImplementor

		public int SelectionCount
		{
			get { return selectionHelper.SelectionCount; }
		}
		public bool AddSelection (int i)
		{
			return selectionHelper.AddSelection (i);
		}
		public bool ClearSelection ()
		{
			return selectionHelper.ClearSelection ();
		}
		public bool IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected (i);
		}
		public Atk.Object RefSelection (int i)
		{
			return selectionHelper.RefSelection (i);
		}
		public bool RemoveSelection (int i)
		{
			return selectionHelper.RemoveSelection (i);
		}
		public bool SelectAllSelection ()
		{
			return selectionHelper.SelectAllSelection ();
		}

#endregion


		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == AutomationElementIdentifiers.AsyncContentLoadedEvent) {
				// TODO: Handle AsyncContentLoadedEvent
			} else if (eventId == AutomationElementIdentifiers.AutomationFocusChangedEvent) {
				// TODO: Handle AutomationFocusChangedEvent
			} else if (eventId == AutomationElementIdentifiers.StructureChangedEvent) {
				// TODO: Handle StructureChangedEvent
			}
		}


		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
				//if it's a toggle, it should not be a basic Button class, but CheckBox or other
				throw new NotSupportedException ("Toggle events should not land here (should not be reached)");
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			//TODO
		}

		internal void NotifyItemSelected (Adapter item)
		{
			if (item == selectedItem)
				return;
			if (selectedItem != null)
				selectedItem.NotifyStateChange ((ulong) Atk.StateType.Selected, false);
			item.NotifyStateChange ((ulong) Atk.StateType.Selected, true);
			selectedItem = item;
		}
	}
}
