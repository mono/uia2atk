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
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace UiaAtkBridge
{
	public class ComboBox : ComponentParentAdapter, Atk.ISelectionImplementor
	{
		private ISelectionProvider 					selProvider;
		
		private SelectionProviderUserHelper			selectionHelper;
		
		public ComboBox (IRawElementProviderSimple provider) : base (provider)
		{
			if (!(provider is IRawElementProviderFragment))
				throw new ArgumentException ("Provider should be IRawElementProviderFragment");
			
			this.Role = Atk.Role.ComboBox;

			selProvider = (ISelectionProvider)provider.GetPatternProvider (SelectionPatternIdentifiers.Pattern.Id);
			
			if (selProvider == null)
				throw new ArgumentException ("ComboBoxProvider should always implement ISelectionProvider");
			
			selectionHelper = 
				new SelectionProviderUserHelper ((IRawElementProviderFragment)Provider, 
				                                 selProvider, true);
			
			var providersSelected = selProvider.GetSelection ();
			if (providersSelected != null && providersSelected.Length == 1)
				Name = (string)providersSelected [0].GetPropertyValue (AEIds.NameProperty.Id);
		}

		internal static bool IsSimple (IRawElementProviderSimple provider)
		{
			return provider.GetPatternProvider (ExpandCollapsePatternIdentifiers.Pattern.Id) == null;
		}

		internal bool IsSimple ()
		{
			return IsSimple (Provider);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			states.AddState (Atk.StateType.ManagesDescendants);
			return states;
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
			// We'll manage the name manually
		}

#region Atk.ISelectionImplementor

		public int SelectionCount
		{
			get { return selectionHelper.SelectionCount; }
		}
		public bool AddSelection (int i)
		{
			bool success = selectionHelper.AddSelection (i);
			if (success) {
				string propagateName = null;
				if (!selProvider.CanSelectMultiple) {
					Atk.Object selected = RefSelection (0);
					if (selected != null)
						propagateName = selected.Name;
				}
				RaiseSelectionChanged (propagateName);
			}

			// TODO: Report gail bug, and return 'success' instead
			return true;
		}
		
		public bool ClearSelection ()
		{
			bool success = selectionHelper.ClearSelection ();

			//will likely never happen because UIA throws IOE...
			if (success) {
				RaiseSelectionChanged (String.Empty);
			}
			
			return success;
		}
		
		public Atk.Object RefSelection (int i)
		{
			return selectionHelper.RefSelection (i);
		}
		
		public bool IsChildSelected (int i)
		{
			return selectionHelper.IsChildSelected (i);
		}
		
		public bool RemoveSelection (int i)
		{
			if (!selectionHelper.IsChildSelected (i))
				return true;
			
			bool success = selectionHelper.RemoveSelection (i);
			
			//will likely never happen because UIA throws IOE...
			if (success) {
				string propagateName = null;
				if (!selProvider.CanSelectMultiple)
					propagateName = String.Empty;
				RaiseSelectionChanged (propagateName);
			}
			
			return success;
		}
		
		public bool SelectAllSelection ()
		{
			bool success = selectionHelper.SelectAllSelection ();
			if (success)
				RaiseSelectionChanged (String.Empty);
			return success;
		}

#endregion

		internal void RaiseSelectionChanged (string name)
		{
			if (name == Name)
				return;
			
			if (name == null)
				name = String.Empty;
			
			Name = name;
			EmitSelectionChanged ();
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			base.RaiseAutomationEvent (eventId, e);
		}

		public override void RaiseAutomationPropertyChangedEvent (System.Windows.Automation.AutomationPropertyChangedEventArgs e)
		{
			base.RaiseAutomationPropertyChangedEvent (e);
		}

		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
			// TODO
		}
	}
}
