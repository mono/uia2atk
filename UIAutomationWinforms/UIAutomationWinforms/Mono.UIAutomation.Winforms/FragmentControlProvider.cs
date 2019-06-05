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
using Mono.UIAutomation.Winforms.Navigation;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	internal abstract class FragmentControlProvider 
		: SimpleControlProvider, IRawElementProviderFragment, IEnumerable<FragmentControlProvider>
	{
		#region Private Fields

		private const int FAKE_INDEX_TREATED_AS_END = -1;

		private readonly Navigation.Navigation navigation;
		private SWF.ContextMenu contextMenu;
		private SWF.ContextMenuStrip contextMenuStrip;
		private bool visible;

		#endregion

		#region Protected Fields

		#endregion

		#region Constructor
		
		protected FragmentControlProvider (Component component)
			: base (component)
		{
			navigation = MakeSuitableNavigation ();

			if (Control != null) {
				Control.ContextMenuChanged += HandleContextMenuChanged;
				Control.ContextMenuStripChanged += HandleContextMenuStripChanged;

				HandleContextMenuChanged (null, EventArgs.Empty);
				HandleContextMenuStripChanged (null, EventArgs.Empty);

				visible = Control.Visible;
			}
		}
		
		protected virtual Navigation.Navigation MakeSuitableNavigation ()
		{
			return new Navigation.Navigation (this);
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
			var children = Navigation.GetChildren ();
			foreach (FragmentControlProvider childProvider in children)
				yield return childProvider;
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		
		#endregion

		#region Public Events

		public event NavigationEventHandler NavigationUpdated;

		#endregion
		
		#region Public Properties

		public Navigation.Navigation Navigation
		{
			get { return navigation; }
		}

		public int ChildrenCount {
			get { return Navigation.ChildrenCount; }
		}

		#endregion
		
		#region Public Methods

		public FragmentControlProvider GetChildProviderAt (int index)
		{
			return Navigation.GetChild (index);
		}
		
		public void AddChildProvider (FragmentControlProvider childProvider)
		{
			AddChildProvider (true, childProvider);
		}

		protected void AddChildProvider (bool raiseEvent, FragmentControlProvider childProvider)
		{
			InsertChildProvider (raiseEvent, childProvider, FAKE_INDEX_TREATED_AS_END);
		}

		protected void InsertChildProvider (FragmentControlProvider childProvider, int index)
		{
			InsertChildProvider (true, childProvider, index);
		}

		protected virtual void InsertChildProvider (bool raiseEvent, FragmentControlProvider childProvider, int index)
		{
			InsertChildProvider (raiseEvent, childProvider, index, true);
		}

		protected void InsertChildProvider (bool raiseEvent, FragmentControlProvider childProvider, int index, bool recursive)
		{
			if (Navigation.ChildrenContains (childProvider))
				return;

			if (index == FAKE_INDEX_TREATED_AS_END || !Navigation.IsIndexValid (index))
			{
				Navigation.AppendChild (childProvider);
			}
			else
			{
				Navigation.InsertChild (index, childProvider);
			}

			NotifyOnChildStructureChanged (childProvider, StructureChangeType.ChildAdded, raiseEvent);

			if (recursive)
				childProvider.InitializeChildControlStructure ();
		}

		public void RemoveChildProvider (FragmentControlProvider childProvider)
		{
			RemoveChildProvider (true, childProvider);
		}

		public virtual void RemoveChildProvider (bool raiseEvent, FragmentControlProvider childProvider)
		{
			if (!Navigation.ChildrenContains (childProvider))
				return;

			childProvider.Terminate ();  // Finalize Child Control Structure at first ...
			NotifyOnChildStructureChanged (childProvider, StructureChangeType.ChildRemoved, raiseEvent);  // ... Then raise events, remove UIA-element in bridge ...
			Navigation.RemoveChild (childProvider);  // ... And destroy object links to free memory.
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
				Control.VisibleChanged += OnControlVisibleChanged;
				
				foreach (SWF.Control childControl in Control.Controls)
					HandleComponentAdded (childControl);
			}
		}
		
		public virtual void FinalizeChildControlStructure ()
		{
			if (Control != null) {				
				Control.ControlAdded -= OnControlAdded;
				Control.ControlRemoved -= OnControlRemoved;
				Control.VisibleChanged -= OnControlVisibleChanged;
			}

			for (; Navigation.ChildrenCount > 0; ) {
				HandleComponentRemoved (Navigation.GetChild (0).Component); 
			}
		}
		
		#endregion
		
		#region Protected Methods

		protected virtual void OnNavigationChildrenCleared ()
		{
			//TODO: Is this the event to generate?
			NotifyOnChildStructureChanged (this, StructureChangeType.ChildrenBulkRemoved, true);
		}
		
		protected void NotifyOnChildStructureChanged (
			FragmentControlProvider subjectProvider, StructureChangeType structureChangeType, bool emitUiaEvents)
		{
			if (NavigationUpdated != null) {
				var args = new NavigationEventArgs (emitUiaEvents, structureChangeType, subjectProvider);
				NavigationUpdated (this, args);
			}

			if (emitUiaEvents) {
				Helper.RaiseStructureChangedEvent (structureChangeType, subjectProvider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated, this);
			}
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
			var control = (SWF.Control) sender;
			bool controlExists = Navigation.ChildrenContains (control);
			bool controlVisible = IsComponentVisible (control);

			if (controlVisible && !controlExists)
				InitializeComponentProvider (control);
			else if (!controlVisible && controlExists)
				TerminateComponentProvider (control);
		}

		#endregion

		#region Protected Methods

		protected virtual FragmentControlProvider CreateProvider (Component component)
		{
			if (component is SWF.Control control)
			{
				if (Mono.UIAutomation.Winforms.ErrorProvider.InstancesTracker.IsControlFromErrorProvider (control))
					return null;
			}
			return (FragmentControlProvider) ProviderFactory.GetProvider (component);
		}

		protected virtual void DestroyProvider (Component component)
		{
			ProviderFactory.ReleaseProvider (component);
		}
		
		protected FragmentControlProvider GetProvider (SWF.Control control)
		{
			return Navigation.TryGetChild (control);
		}

		protected void InitializeComponentProvider (Component childComponent)
		{
			if (Navigation.ChildrenContains (childComponent))
				return;

			FragmentControlProvider childProvider = CreateProvider (childComponent);
			if (childProvider == null)
				return;

			InsertChildProvider (childProvider, FAKE_INDEX_TREATED_AS_END);
		}

		protected void TerminateComponentProvider (Component childComponent)
		{
			FragmentControlProvider removedProvider = Navigation.TryGetChild (childComponent);
			if (removedProvider == null)
				return;

			// Event source:
			//       Parent of removed child runtimeId: The child that was
			//       removed. (pg 6 of fxref_uiautomationtypes_p2.pdf)
			RemoveChildProvider (removedProvider);
			DestroyProvider (childComponent);
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
			if (component is UserCustomComponent)
				return true;

			SWF.Control control = null;
			if ((control = component as SWF.Control) != null)
				return control.Visible;
			else // Component based providers will need to override this method
				return false;
		}

		protected void HandleComponentAdded (Component childComponent)
		{
			if (Navigation.ChildrenContains (childComponent))
				return;

			// We don't add an invisible component but we keep track of its 
			// Visible event to add it when Visible changes
			AddVisibleEvent (childComponent);
			if (IsComponentVisible (childComponent))
				InitializeComponentProvider (childComponent);
		}

		protected void HandleComponentRemoved (Component childComponent)
		{
			if (!Navigation.ChildrenContains (childComponent))
				return;

			TerminateComponentProvider (childComponent);
			RemoveVisibleEvent (childComponent);
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
		
		public virtual int[] GetRuntimeId ()
		{
			return new int[] { AutomationInteropProvider.AppendRuntimeId, runtimeId };
		}

		public virtual IRawElementProviderFragment Navigate (NavigateDirection direction) 
		{
			return Navigation.Navigate (direction);
		}
		
		public virtual void SetFocus ()
		{
			if (Control != null)
				Control.Focus ();
		}

		#endregion

		public override string ToString ()
		{
			string name = SafeGetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			string locCtrlType = SafeGetPropertyValue (AutomationElementIdentifiers.LocalizedControlTypeProperty.Id);
			return string.Format ("<{0}:'{1}';'{2}'>", this.GetType (), name, locCtrlType);
		}

		private string SafeGetPropertyValue (int propertyId)
		{
			const string errValueDummy = "[err]";
			try {
				return (this.GetPropertyValue (propertyId) ?? errValueDummy).ToString ();
			}
			catch {
				return errValueDummy;
			}
		}
	}
}
