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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{
	public abstract class FragmentRootControlProvider :
		FragmentControlProvider, IRawElementProviderFragmentRoot
	{
#region Internal Data
		
		internal IDictionary<Control, IRawElementProviderSimple>
			controlProviders;  // TODO: Fix this...
		
#endregion

#region Constructors
		
		protected FragmentRootControlProvider (Control control) :
			base (control)
		{
			controlProviders =
				new Dictionary<Control, IRawElementProviderSimple> ();
			
			control.ControlAdded += OnControlAdded;
			control.ControlRemoved += OnControlRemoved;
			
			foreach (Control childControl in control.Controls) {
				IRawElementProviderSimple provider =
					CreateProvider (childControl);
				// TODO: Null check, compound, etc?
				controlProviders [childControl] = provider;
			}
		}
		
#endregion
		
#region Public Methods
	
		//TODO: Are the generated events duplicated? Because we're already
		//supporting StructureChangeType when Children are added to Controls.
		//See: SimpleControlProvider.InitializeEvents
		public void InitializeChildControlStructure ()
		{
			// HACK: This is just to make sure control providers
			//       aren't sent to bridge until the parent's already
			//       there.  There are about 100 ways to do this
			//       better.
			foreach (IRawElementProviderSimple childProvider in controlProviders.Values) {
				// TODO: Fill in rest of eventargs
				if (childProvider == null)
					break;
				
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenBulkAdded,
				                                   (IRawElementProviderFragment) childProvider);

				FragmentRootControlProvider rootProvider =
					childProvider as FragmentRootControlProvider;
				if (rootProvider != null)
					rootProvider.InitializeChildControlStructure ();
			}
		}
		
#endregion
		
#region Private Event Handlers
	
		private void OnControlAdded (object sender, ControlEventArgs args)
		{
			Console.WriteLine ("ControlAdded: " + args.Control.GetType ().ToString ());
			
			Control childControl = args.Control;
			IRawElementProviderSimple childProvider =
				CreateProvider (childControl);

			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenBulkAdded,
			                                   (IRawElementProviderFragment) childProvider);
			
			// TODO: Figure out exactly when to do this (talk to bridge guys)
			if (GetBehavior (SelectionPatternIdentifiers.Pattern) == null &&
			    childProvider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) != null &&
			    (int) childProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.RadioButton.Id) {
				RadioButtonSelectionProviderBehavior selectionProvider =
					new RadioButtonSelectionProviderBehavior ();
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             selectionProvider);
			}
			
			FragmentRootControlProvider rootProvider =
				childProvider as FragmentRootControlProvider;
			if (rootProvider != null)
				InitializeChildControlStructure ();
		}
	
		private void OnControlRemoved (object sender, ControlEventArgs args)
		{
			Console.WriteLine ("ControlRemoved: " + args.Control.GetType ().ToString ());
			
			bool radioButtonFound = false;
			IRawElementProviderSimple removedProvider = controlProviders [args.Control];
			foreach (IRawElementProviderSimple childProvider in controlProviders.Values) {
				if (childProvider != removedProvider &&
				    (int) childProvider.GetPatternProvider (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.RadioButton.Id) {
					radioButtonFound = true;
					break;
				}
			}
			if (!radioButtonFound)
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             null);
			
			// TODO: StructureChangedEvent
			
			// TODO: Some sort of disposal
			
			controlProviders.Remove (args.Control);
			
			ProviderFactory.ReleaseProvider (args.Control);
		}

#endregion

#region Protected Methods
	
		protected IRawElementProviderSimple CreateProvider (Control control)
		{
			return ProviderFactory.GetProvider (control);
		}
		
		protected IRawElementProviderSimple GetProvider (Control control)
		{
			if (controlProviders.ContainsKey (control))
				return controlProviders [control];
			else
				return null;
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

		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { return this; }
		}

#endregion
	}
}
