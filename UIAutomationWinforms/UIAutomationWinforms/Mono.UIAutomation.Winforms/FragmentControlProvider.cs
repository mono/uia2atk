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

using System.Threading;

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

		#endregion

		#region Protected Fields

		#endregion

		#region Constructor

		protected FragmentControlProvider (Component component)
			: base (component)
		{
			navigation = new Navigation.Navigation (this);

			if (Control != null) {
				Control.ContextMenuChanged += HandleContextMenuChanged;
				Control.ContextMenuStripChanged += HandleContextMenuStripChanged;

				HandleContextMenuChanged (null, EventArgs.Empty);
				HandleContextMenuStripChanged (null, EventArgs.Empty);
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

		// This method is aimed to move child provider from one parent provider to anothe one.
		// This is suitable to deal with branch of `FormProvider` rooted on `DesktopProvier`.
		public void SetAsOwnerFor (SWF.Form form)
		{
			var formProvider = (FormProvider) ProviderFactory.GetProvider (form);
			var oldOwnerProvider = (FragmentControlProvider) formProvider.Navigate (NavigateDirection.Parent);

			oldOwnerProvider?.HandleChildComponentRemoved (form);
			HandleChildComponentAdded (form);
		}

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

			if (index == FAKE_INDEX_TREATED_AS_END)
				Navigation.AppendChild (childProvider);
			else
				Navigation.InsertChild (index, childProvider);

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
				Control.ControlAdded += OnChildControlAdded;
				Control.ControlRemoved += OnChildControlRemoved;
				
				foreach (SWF.Control childControl in Control.Controls)
					HandleChildComponentAdded (childControl);
			}
		}
		
		public virtual void FinalizeChildControlStructure ()
		{
			if (Control != null) {
				Control.ControlAdded -= OnChildControlAdded;
				Control.ControlRemoved -= OnChildControlRemoved;
			}

			for (; Navigation.ChildrenCount > 0; ) {
				var ch = Navigation.GetChild (0);
				HandleChildComponentRemoved (ch.Component); 
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
	
		private void OnChildControlAdded (object sender, SWF.ControlEventArgs args)
		{
			HandleChildComponentAdded (args.Control);
		}
	
		private void OnChildControlRemoved (object sender, SWF.ControlEventArgs args)
		{
			HandleChildComponentRemoved (args.Control);
		}

		private void OnChildControlVisibleChanged (object sender, EventArgs args)
		{
			var control = (SWF.Control) sender;
			if (IsComponentVisible (control))
				HandleChildComponentAdded (control);
			else
				HandleChildComponentRemoved (control);
		}

		#endregion

		#region Protected Methods

		protected void HandleChildComponentAdded (Component childComponent)
		{
			if (Navigation.ChildrenContains (childComponent))
				return;

			/*
			  At some not totally explored condition a Provider object has not been removed from a hidden/deleted
			parent one properly. This not-removed Provider stais with a `Navigation` object storing uncleared
			refrrences to ancestor, siblings and descendants. As a consequence this not-removed Provider leads to 
			deeper refrrences breaking while it is an argument of the `HandleChildComponentAdded` method call.
			  To deal with this case let's check the `Parent` refence and use `HandleChildComponentRemoved`
			in case of any confusion.
			*/
			var provider = (FragmentControlProvider) ProviderFactory.FindProvider (childComponent);
			if (provider != null && provider.Navigation.Parent != null)
				provider.Navigation.Parent.HandleChildComponentRemoved (childComponent);

			if (childComponent is SWF.Control childCcontrol) {
				if (Mono.UIAutomation.Winforms.ErrorProvider.InstancesTracker.IsControlFromErrorProvider (childCcontrol))
					return;
				childCcontrol.VisibleChanged += OnChildControlVisibleChanged;
			}

			if (IsComponentVisible (childComponent)) {
				var childProvider = (FragmentControlProvider) ProviderFactory.GetProvider (childComponent);
				if (childProvider == null)
					return;
				InsertChildProvider (childProvider, FAKE_INDEX_TREATED_AS_END);
			}
		}

		protected void HandleChildComponentRemoved (Component childComponent)
		{
			var childProvider = Navigation.TryGetChild (childComponent);
			if (childProvider == null)
				return;

			// Event source:
			//       Parent of removed child runtimeId: The child that was
			//       removed. (pg 6 of fxref_uiautomationtypes_p2.pdf)
			RemoveChildProvider (childProvider);

			if (childComponent is SWF.Control childCcontrol)
				childCcontrol.VisibleChanged -= OnChildControlVisibleChanged;

			ProviderFactory.ReleaseProvider (childComponent);
		}

		protected virtual bool IsComponentVisible (Component component)
		{
			if (component is UserCustomComponent)
				return true;
			else if (component is SWF.Control control)
				return control.Visible;
			else // Component based providers will need to override this method
				return false;
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
			switch (direction)
			{
				case NavigateDirection.Parent:
					// TODO: Review this
					// "Fragment roots do not enable navigation to
					// a parent or siblings; navigation among fragment
					// roots is handled by the default window providers."
					// Source: http://msdn.microsoft.com/en-us/library/system.windows.automation.provider.irawelementproviderfragment.navigate.aspx
					return ((IRawElementProviderFragment) Navigation.Parent); // !!!:      ?? Provider.FragmentRoot;
				case NavigateDirection.FirstChild:
					return Navigation.FirstChildProvider;
				case NavigateDirection.LastChild:
					return Navigation.LastChildProvider;
				case NavigateDirection.NextSibling:
					return Navigation.NextProvider;
				case NavigateDirection.PreviousSibling:
					return Navigation.PreviousProvider;
				default:
					return null;
			}
		}
		
		public virtual void SetFocus ()
		{
			if (Control != null)
				Control.Focus ();
		}

		#endregion

		public override string ToString ()
		{
			return $"<{this.GetType ()}:{Component},{runtimeId}>";
		}
	}
}
