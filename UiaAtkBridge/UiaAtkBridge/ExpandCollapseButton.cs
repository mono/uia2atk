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
//      Brad Taylor <brad@getcoded.net>

using System;
using System.Windows;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class ExpandCollapseButton
		: ComponentParentAdapter, Atk.IActionImplementor,
		  Atk.ISelectionImplementor, ICanHaveSelection
	{
#region Public Methods
		public ExpandCollapseButton (IRawElementProviderSimple provider) : base (provider)
		{
			ec_prov = (IExpandCollapseProvider)provider.GetPatternProvider (
				ExpandCollapsePatternIdentifiers.Pattern.Id);

			Role = Atk.Role.ToggleButton;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Atk.StateType.Focusable);

			return states;
		}
		
#region Atk.IAction Implementation
		public int NActions {
			get { return 1; }
		}
		
		public virtual bool DoAction (int action)
		{
			if (ec_prov == null || action != 0) {
				return false;
			}

			OnPressed ();

			ExpandCollapseState state = ec_prov.ExpandCollapseState;
			if (state == ExpandCollapseState.Expanded
			    || state == ExpandCollapseState.PartiallyExpanded) {
				try {
					ec_prov.Collapse ();
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}
			} else {
				try {
					ec_prov.Expand ();
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}
			}

			OnReleased ();
			return true;
		}

		public string GetDescription (int action)
		{
			return action == 0 ? actionDescription : null;
		}

		public string GetKeybinding (int action)
		{
			string keyBinding = null;
			
			if (action != 0)
				return keyBinding;

			keyBinding = (string) Provider.GetPropertyValue (
				AutomationElementIdentifiers.AccessKeyProperty.Id);

			if (!String.IsNullOrEmpty (keyBinding))
				keyBinding = keyBinding.Replace ("Alt+", "<Alt>");
			
			return keyBinding;
		}

		public string GetLocalizedName (int action)
		{
			// TODO: Localize the name?
			return action == 0 ? ACTION_NAME : null;
		}
		
		public string GetName (int action)
		{
			return action == 0 ? ACTION_NAME : null;
		}
		
		public bool SetDescription (int action, string description)
		{
			if (action != 0)
				return false;
			
			actionDescription = description;
			return true;
		}
#endregion

		//NOTE: ToolStripSplitButton in UIA does not implement the selection pattern
#region SelectionImplementor implementation 
		public int SelectionCount {
			get {
				return 0;
			}
		}
		
		public bool AddSelection (int i)
		{
			if ((i < 0) || (i >= NAccessibleChildren))
				return false;
			
			Atk.Object child = RefAccessibleChild (i);
			if (child is Atk.IActionImplementor) {
				return ((Atk.IActionImplementor) child).DoAction (0);
			}

			return false;
		}
		
		public bool ClearSelection ()
		{
			return true;
		}
		
		public Atk.Object RefSelection (int i)
		{
			return null;
		}
		
		public bool IsChildSelected (int i)
		{
			return false;
		}
		
		public bool RemoveSelection (int i)
		{
			return true;
		}
		
		public bool SelectAllSelection ()
		{
			return false;
		}
#endregion 

#region ICanHaveSelection interface
		void ICanHaveSelection.RecursivelyDeselectAll (Adapter keepSelected)
		{
			((ICanHaveSelection) this).RecursivelyDeselect (keepSelected);
		}
		
		void ICanHaveSelection.RecursivelyDeselect (Adapter keepSelected)
		{
			lock (syncRoot) {
				for (int i = 0; i < NAccessibleChildren; i++) {
					Atk.Object child = RefAccessibleChild (i);
					if (child is ICanHaveSelection) {
						((ICanHaveSelection) child).RecursivelyDeselect (keepSelected);
					}
				}
			}
		}
#endregion

		public override void RaiseStructureChangedEvent (object provider, StructureChangedEventArgs e)
		{
		}
#endregion

#region Private Methods
		private void OnPressed ()
		{
			NotifyStateChange (Atk.StateType.Armed, true);
		}

		private void OnReleased ()
		{
			NotifyStateChange (Atk.StateType.Armed, false);
		}
#endregion

#region Private Fields
		private IExpandCollapseProvider ec_prov;
		private const string ACTION_NAME = "click";
		private static string actionDescription = String.Empty;
#endregion
	}
}
