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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Navigation;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	internal abstract class FragmentControlProvider 
		: SimpleControlProvider, IRawElementProviderFragment, IEnumerable<FragmentControlProvider>
	{
		#region Private Fields
		
		private List<FragmentControlProvider> children;
		private List<Component> componentChildren;
		private INavigation navigation;
		private SWF.ContextMenu contextMenu;
		private SWF.ContextMenuStrip contextMenuStrip;
		
		#endregion
		
		#region Internal Data
		
		protected IDictionary<Component, FragmentControlProvider>
			componentProviders;
		
		#endregion

		#region Constructor	
		
		protected FragmentControlProvider (Component component) : base (component)
		{
			componentProviders =
				new Dictionary<Component, FragmentControlProvider> ();
			children = new List<FragmentControlProvider> ();
			componentChildren = new List<Component> ();

			if (Control != null) {
				Control.ContextMenuChanged += HandleContextMenuChanged;
				Control.ContextMenuStripChanged += HandleContextMenuStripChanged;

				HandleContextMenuChanged (null, EventArgs.Empty);
				HandleContextMenuStripChanged (null, EventArgs.Empty);

				visible = Control.Visible;
			}
		}

		void HandleContextMenuStripChanged (object sender, EventArgs e)
		{
			if (contextMenuStrip != null) {
				contextMenuStrip.Opened -= HandleContextMenuStripOpened;
				contextMenuStrip.Closed -= HandleContextMenuStripClosed;
			}
			contextMenuStrip = Control.ContextMenuStrip;

			if (contextMenuStrip != null) {
				contextMenuStrip.Opened += HandleContextMenuStripOpened;
				contextMenuStrip.Closed += HandleContextMenuStripClosed;
			}
		}

		void HandleContextMenuStripOpened (object sender, EventArgs e)
		{
			FragmentControlProvider contextMenuStripProvider = (FragmentControlProvider)
				ProviderFactory.GetProvider (contextMenuStrip);
			AddChildProvider (contextMenuStripProvider);
		}

		void HandleContextMenuStripClosed (object sender, EventArgs e)
		{
			FragmentControlProvider contextMenuStripProvider = (FragmentControlProvider)
				ProviderFactory.FindProvider (contextMenuStrip);
			if (contextMenuStripProvider == null)
				return;
			contextMenuStripProvider.Terminate ();
			RemoveChildProvider (contextMenuStripProvider);
			ProviderFactory.ReleaseProvider (contextMenuStrip);
			// TODO: Need to handle disposal of some parent without close happening?
		}

		void HandleContextMenuChanged (object sender, EventArgs e)
		{
			if (contextMenu != null) {
				contextMenu.Popup -= HandleContextMenuPopup;
				contextMenu.Collapse -= HandleContextMenuCollapse;
			}
			contextMenu = Control.ContextMenu;

			if (contextMenu != null) {
				contextMenu.Popup += HandleContextMenuPopup;
				contextMenu.Collapse += HandleContextMenuCollapse;
			}
		}

		void HandleContextMenuPopup (object sender, EventArgs e)
		{
			FragmentControlProvider contextMenuProvider = (FragmentControlProvider)
				ProviderFactory.GetProvider (contextMenu);
			AddChildProvider (contextMenuProvider);
		}

		void HandleContextMenuCollapse (object sender, EventArgs e)
		{
			FragmentControlProvider contextMenuProvider = (FragmentControlProvider)
				ProviderFactory.FindProvider (contextMenu);
			if (contextMenuProvider == null)
				return;
			contextMenuProvider.Terminate ();
			RemoveChildProvider (contextMenuProvider);
			ProviderFactory.ReleaseProvider (contextMenu);
			// TODO: Need to handle disposal of some parent without close happening?
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
		
		public NavigationEventHandler NavigationUpdated;
		
		#endregion

		#region Public Properties
		
		public INavigation Navigation {
			get { return navigation; }
			set { 
				if (value != navigation && navigation != null)
					navigation.Terminate ();

				navigation = value; 
			}
		}

		public int ChildrenCount {
			get { return children.Count; }
		}

		#endregion
		
		#region Public Methods

		public int GetChildProviderIndexOf (FragmentControlProvider provider)
		{
			return provider == null ? -1 : children.IndexOf (provider);
		}

		public FragmentControlProvider GetChildProviderAt (int index)
		{
			if (index < 0 || index >= children.Count)
				return null;
			else
				return children [index];
		}
		
		public void AddChildProvider (bool raiseEvent,
		                              FragmentControlProvider childProvider)
		{
			InsertChildProvider (raiseEvent, childProvider, -1);
		}
		
		public virtual void InsertChildProvider (bool raiseEvent,
		                                         FragmentControlProvider childProvider,
		                                         int index)
		{
			if (children.Contains (childProvider) == true)
				return;
			
			childProvider.Navigation = NavigationFactory.CreateNavigation (childProvider, this);
			childProvider.Navigation.Initialize ();

			if (index < 0 || index >= children.Count) {
				children.Add (childProvider);
			} else {
				children.Insert (index, childProvider);
			}

			OnNavigationUpdated (new NavigationEventArgs (raiseEvent, 
			                                              StructureChangeType.ChildAdded, 
			                                              childProvider, index));

			childProvider.InitializeChildControlStructure ();
		}

		public virtual void RemoveChildProvider (bool raiseEvent,
		                                         FragmentControlProvider childProvider)
		{
			if (children.Contains (childProvider) == false)
				return;
			
			OnNavigationUpdated (new NavigationEventArgs (raiseEvent, 
			                                              StructureChangeType.ChildRemoved, 
			                                              childProvider));

			childProvider.Navigation.Terminate ();
			childProvider.Navigation = null;
			
			children.Remove (childProvider);
		}
		
		public void AddChildProvider (FragmentControlProvider childProvider) 
		{
			AddChildProvider (true, childProvider);
		}
		
		public void InsertChildProvider (FragmentControlProvider childProvider,
		                                 int index)
		{
			InsertChildProvider (true, childProvider, index);
		}

		public void RemoveChildProvider (FragmentControlProvider childProvider) 
		{
			RemoveChildProvider (true, childProvider);
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
			if (Control != null) {				
				Control.ControlAdded += OnControlAdded;
				Control.ControlRemoved += OnControlRemoved;
				
				foreach (SWF.Control childControl in Control.Controls)
					HandleComponentAdded (childControl);

				Control.VisibleChanged += OnControlVisibleChanged;
			}
		}
		
		public virtual void FinalizeChildControlStructure ()
		{
			if (Control != null) {				
				Control.ControlAdded -= OnControlAdded;
				Control.ControlRemoved -= OnControlRemoved;
				Control.VisibleChanged -= OnControlVisibleChanged;
			}

			for (; componentChildren.Count > 0; ) {
				HandleComponentRemoved (componentChildren [0]);
			}

			children.Clear ();
			componentProviders.Clear ();
		}
		
		#endregion
		
		#region Protected Methods
		
		protected virtual void OnNavigationUpdated (NavigationEventArgs args)
		{
			if (NavigationUpdated != null)
				NavigationUpdated (this, args);
		}
			
		protected virtual void OnNavigationChildrenCleared ()
		{
			OnNavigationUpdated (new NavigationEventArgs (true,
			                                              StructureChangeType.ChildrenReordered, 
			                                              null));
		}

		#endregion
		
		#region Private Methods: Event Handlers
	
		private void OnControlAdded (object sender, SWF.ControlEventArgs args)
		{
			HandleComponentAdded (args.Control);
		}
	
		private void OnControlRemoved (object sender, SWF.ControlEventArgs args)
		{
			HandleComponentRemoved (args.Control);
		}

		private bool visible = false;
		
		private void OnControlVisibleChanged (object sender, EventArgs args)
		{
			if (visible == Control.Visible)
				return;

			if (Control.Visible)
				InitializeChildControlStructure ();
			else
				FinalizeChildControlStructure ();

			visible = Control.Visible;
		}

		private void OnChildControlVisibleChanged (object sender, EventArgs args)
		{
			SWF.Control control = (SWF.Control) sender;

			bool controlExists = componentProviders.ContainsKey (control);
			if (IsComponentVisible (control)) {
				if (!controlExists)
					InitializeComponentProvider (control);
			} else {
				if (controlExists)
					TerminateComponentProvider (control);
			}
		}

		#endregion

		#region Protected Methods
	
		protected FragmentControlProvider CreateProvider (SWF.Control control)
		{
			if (Mono.UIAutomation.Winforms.ErrorProvider.InstancesTracker.IsControlFromErrorProvider (control))
				return null;

			return (FragmentControlProvider) ProviderFactory.GetProvider (control);
		}

		protected virtual FragmentControlProvider CreateProvider (Component component)
		{
			return (FragmentControlProvider) ProviderFactory.GetProvider (component);
		}

		protected virtual void DestroyProvider (Component component)
		{
			ProviderFactory.ReleaseProvider (component);
		}
		
		protected FragmentControlProvider GetProvider (SWF.Control control)
		{
			if (componentProviders.ContainsKey (control))
				return componentProviders [control];
			else
				return null;
		}

		protected void InitializeComponentProvider (Component childComponent)
		{
			SWF.Control control = null;
			FragmentControlProvider childProvider = null;

			if (componentProviders.ContainsKey (childComponent))
				return;
			
			if ((control = childComponent as SWF.Control) != null)
				childProvider = CreateProvider (control);
			else
				childProvider = CreateProvider (childComponent);
			
			if (childProvider == null)
				return;

			componentProviders [childComponent] = childProvider;
			AddChildProvider (childProvider);
		}

		protected void TerminateComponentProvider (Component childComponent)
		{			
			FragmentControlProvider removedProvider;
			
			if (componentProviders.TryGetValue (childComponent, 
			                                    out removedProvider)) {
				componentProviders.Remove (childComponent);
				// Event source:
				//       Parent of removed child runtimeId: The child that was
				//       removed. (pg 6 of fxref_uiautomationtypes_p2.pdf)
				RemoveChildProvider (removedProvider);
				DestroyProvider (childComponent);
			}
		}

		protected void AddChildComponent (Component childComponent)
		{
			if (componentChildren.Contains (childComponent))
				return;

			componentChildren.Add (childComponent);
			AddVisibleEvent (childComponent);
		}

		protected void RemoveChildComponent (Component childComponent)
		{
			if (!componentChildren.Contains (childComponent))
				return;
			
			componentChildren.Remove (childComponent);
			RemoveVisibleEvent (childComponent);
		}

		protected virtual void AddVisibleEvent (Component component)
		{
			SWF.Control control = null;

			if ((control = component as SWF.Control) != null)
				control.VisibleChanged += OnChildControlVisibleChanged;
		}		

		protected virtual void RemoveVisibleEvent (Component component)
		{
			SWF.Control control = null;

			if ((control = component as SWF.Control) != null)
				control.VisibleChanged -= OnChildControlVisibleChanged;			
		}

		protected virtual bool IsComponentVisible (Component component)
		{
			SWF.Control control = null;
			if ((control = component as SWF.Control) != null)
				return control.Visible;
			else // Component based providers will need to override this method
				return false;
		}

		protected void HandleComponentAdded (Component component)
		{
			AddChildComponent (component);

			// We don't add an invisible component but we keep track of its 
			// Visible event to add it when Visible changes
			if (!IsComponentVisible (component))
				return;

			InitializeComponentProvider (component);
		}

		protected void HandleComponentRemoved (Component component)
		{
			TerminateComponentProvider (component);

			RemoveChildComponent (component);
		}

		#endregion

		#region SimpleControlProvider Overrides

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.RuntimeIdProperty.Id)
				return GetRuntimeId ();
			return base.GetProviderPropertyValue (propertyId);
		}

		#endregion

		#region IRawElementProviderFragment Interface

		public virtual System.Windows.Rect BoundingRectangle {
			get {
				return (Rect)
					GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			}
		}
		
		public virtual IRawElementProviderFragmentRoot FragmentRoot {
			get { 
				return (IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (Container);
			}
		}
		
		public virtual IRawElementProviderSimple[] GetEmbeddedFragmentRoots () 
		{
			return null;
		}
		
		public int[] GetRuntimeId ()
		{
			return new int [] { AutomationInteropProvider.AppendRuntimeId,
				System.Diagnostics.Process.GetCurrentProcess ().Id,
				(int) GetPropertyValue (AutomationElementIdentifiers.AutomationIdProperty.Id) };
		}
		
		public IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			return Navigation == null ? null : Navigation.Navigate (direction);
		}
		
		public virtual void SetFocus ()
		{
			if (Control != null)
				Control.Focus ();
		}

		#endregion
		
	}
}
