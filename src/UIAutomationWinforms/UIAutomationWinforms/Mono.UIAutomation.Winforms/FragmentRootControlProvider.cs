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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class FragmentRootControlProvider 
		: FragmentControlProvider, IRawElementProviderFragmentRoot, IEnumerable<FragmentControlProvider>
	{
		#region Internal Data
		
		internal IDictionary<Component, FragmentControlProvider>
			componentProviders;  // TODO: Fix this...
		
		
		#endregion

		#region Constructors
		
		protected FragmentRootControlProvider (Component component) :
			base (component)
		{
			componentProviders =
				new Dictionary<Component, FragmentControlProvider> ();
			children = new List<FragmentControlProvider> ();
		}
		
		#endregion
		
		#region IEnumerable Specializations
		
		public IEnumerator<FragmentControlProvider> GetEnumerator ()
		{
			foreach (FragmentControlProvider childProvider in children)
				yield return childProvider;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		#endregion
		
		#region Public Events
		
		public NavigationEventHandler NavigationChildAdded;
    
		public NavigationEventHandler NavigationChildRemoved;
        
		public NavigationEventHandler NavigationChildrenCleared;
		
		#endregion
		
		#region Public Methods
		
		public void AddChildProvider (bool raiseEvent, 
		                              FragmentControlProvider provider) 
		{
			OnNavigationChildAdded (raiseEvent, provider);
		}

		public void RemoveChildProvider (bool raiseEvent, 
		                                 FragmentControlProvider provider) 
		{
			OnNavigationChildRemoved (raiseEvent, provider);
		}
		
		public override void Terminate ()
		{
			base.Terminate ();
			
			FinalizeChildControlStructure ();
		}
	
		//TODO: Are the generated events duplicated? Because we're already
		//supporting StructureChangeType when Children are added to Controls.
		//See: SimpleControlProvider.InitializeEvents
		public virtual void InitializeChildControlStructure ()
		{
			// HACK: This is just to make sure control providers
			//       aren't sent to bridge until the parent's already
			//       there.  There are about 100 ways to do this
			//       better.
			if (Control != null) {				
				Control.ControlAdded += OnControlAdded;
				Control.ControlRemoved += OnControlRemoved;
				
				foreach (Control childControl in Control.Controls) {
					FragmentControlProvider childProvider =
						CreateProvider (childControl);
					
					if (childControl == null)
						continue;
					// TODO: Null check, compound, etc?

					componentProviders [childControl] = childProvider;
					OnNavigationChildAdded (false,
					                        (FragmentControlProvider) childProvider);

					// TODO: Figure out exactly when to do this (talk to bridge guys)
					CheckForRadioButtonChild (childProvider);
				}
			}
		}
		
		public virtual void FinalizeChildControlStructure ()
		{
			for (; children.Count > 0; )
				OnNavigationChildRemoved (false, 
				                          (FragmentControlProvider) children [0]);

			children.Clear ();
			componentProviders.Clear ();
		}
		
		#endregion
		
		#region Protected Methods

		protected virtual void OnNavigationChildAdded (bool raiseEvent, 
		                                               FragmentControlProvider childProvider)
		{
			if (children.Contains (childProvider) || childProvider == null)
				return;
			
			childProvider.Navigation = NavigationFactory.CreateNavigation (childProvider, this);
			childProvider.Navigation.Initialize ();

			//WE MUST GENERATE THIS EVENT before initializing its children
			if (NavigationChildAdded != null)
				NavigationChildAdded (this, 
				                      new NavigationEventArgs (raiseEvent, 
				                                               StructureChangeType.ChildAdded, 
				                                               childProvider));			
			if (childProvider is FragmentRootControlProvider)
				((FragmentRootControlProvider) childProvider).InitializeChildControlStructure ();			

			children.Add (childProvider);
		}
		
		protected virtual void OnNavigationChildRemoved (bool raiseEvent, 
		                                                 FragmentControlProvider childProvider)
		{
			if (children.Contains (childProvider) == false)
				return;
			
			if (NavigationChildRemoved != null)
				NavigationChildRemoved (this,
				                        new NavigationEventArgs (raiseEvent, 
				                                                 StructureChangeType.ChildRemoved, 
				                                                 childProvider));

			childProvider.Navigation.Terminate ();
			childProvider.Navigation = null;
			
			children.Remove (childProvider);
		}
			
		protected virtual void OnNavigationChildrenCleared (bool raiseEvent)
		{
			if (NavigationChildrenCleared != null)
				NavigationChildrenCleared (this,
				                           new NavigationEventArgs (raiseEvent, 
				                                                    StructureChangeType.ChildrenReordered, 
				                                                    null));
		}
               
		#endregion
		
		#region Private Methods: Event Handlers
	
		private void OnControlAdded (object sender, ControlEventArgs args)
		{
			Console.WriteLine ("ControlAdded: " + args.Control.GetType ().ToString ());
			
			Control childControl = args.Control;
			FragmentControlProvider childProvider = CreateProvider (childControl);
			if (childProvider == null)
				return;

			componentProviders [childControl] = childProvider;	
			OnNavigationChildAdded (true, 
			                        (FragmentControlProvider) childProvider);
			
			// TODO: Figure out exactly when to do this (talk to bridge guys)
			CheckForRadioButtonChild (childProvider);
		}
	
		private void OnControlRemoved (object sender, ControlEventArgs args)
		{
			Console.WriteLine ("ControlRemoved: " + args.Control.GetType ().ToString ());
			
			bool radioButtonFound = false;
			FragmentControlProvider removedProvider;
			
			if (componentProviders.TryGetValue (args.Control, out removedProvider) == true) {
				foreach (FragmentControlProvider childProvider in componentProviders.Values) {
					if (childProvider != removedProvider &&
					    (int) childProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.RadioButton.Id) {
						radioButtonFound = true;
						break;
					}
				}
				if (!radioButtonFound)
					SetBehavior (SelectionPatternIdentifiers.Pattern,
					             null);
				
				// TODO: Some sort of disposal
				
				componentProviders.Remove (args.Control);
				// StructureChangedEvent
				// TODO: Use correct arguments, and fix bridge
				//       to handle them!!!
				//       Event source: Parent of removed child
				//       runtimeId: The child that was removed.
				//       (pg 6 of fxref_uiautomationtypes_p2.pdf)
				OnNavigationChildRemoved (true, 
				                          (FragmentControlProvider) removedProvider);
				ProviderFactory.ReleaseProvider (args.Control);
			}
		}

		#endregion
		
		#region Private Methods
		
		// If a child control is ControlType.RadioButton, this provider
		// needs to provide SelectionPattern behavior.
		private void CheckForRadioButtonChild (IRawElementProviderSimple childProvider)
		{
			if (childProvider == null)
				return;
			if (GetBehavior (SelectionPatternIdentifiers.Pattern) == null &&
			    childProvider.GetPatternProvider (SelectionItemPatternIdentifiers.Pattern.Id) != null &&
			    (int) childProvider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) == ControlType.RadioButton.Id) {
				RadioButtonSelectionProviderBehavior selectionProvider =
					new RadioButtonSelectionProviderBehavior ();
				SetBehavior (SelectionPatternIdentifiers.Pattern,
				             selectionProvider);
			}
		}
		
		#endregion

		#region Protected Methods
	
		protected FragmentControlProvider CreateProvider (Control control)
		{
			if (ErrorProvider.InstancesTracker.IsControlFromErrorProvider (control))
				return null;
//			
			return (FragmentControlProvider) ProviderFactory.GetProvider (control);
		}
		
		protected FragmentControlProvider GetProvider (Control control)
		{
			if (componentProviders.ContainsKey (control))
				return componentProviders [control];
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

		//http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.fragmentroot.aspx
		public override IRawElementProviderFragmentRoot FragmentRoot {
			get { 
				if (Container == null)
					return this; 
				else
					return (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (Container);
			}
		}

		#endregion
		
		#region Private Fields
		
		private List<FragmentControlProvider> children;
		
		#endregion
	}
}
