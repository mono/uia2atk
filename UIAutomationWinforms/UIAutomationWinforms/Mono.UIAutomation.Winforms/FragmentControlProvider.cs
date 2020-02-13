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
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Winforms.Navigation;
using Mono.UIAutomation.Winforms.UserCustom;
using SWF = System.Windows.Forms;

using System.Threading;

namespace Mono.UIAutomation.Winforms
{
	internal abstract partial class FragmentControlProvider
		: SimpleControlProvider, IRawElementProviderFragment
	{
		enum ReallyVisible
		{
			Yes,
			No,
			Unknown
		}

		#region Private Fields

		private readonly NavigationWinform navigation;
		private SWF.ContextMenu contextMenu;
		private bool previousVisible;  // Just to avoid event duplicates.
		private ReallyVisible isReallyVisible = ReallyVisible.Unknown;

		#endregion

		#region Protected Fields

		#endregion

		#region Constructor

		protected FragmentControlProvider (Component component)
			: base (component)
		{
			navigation = new NavigationWinform (this);
			previousVisible = false;

			if (Control != null) {
				Control.ContextMenuChanged += HandleContextMenuChanged;
				Control.ContextMenuStripChanged += HandleContextMenuStripChanged;
				Control.Click += TrackControlClick;

				HandleContextMenuChanged (null, EventArgs.Empty);
				HandleContextMenuStripChanged (null, EventArgs.Empty);
			}
		}
		
		void HandleContextMenuStripChanged (object sender, EventArgs e)
		{
			var contextMenuStrip = Control.ContextMenuStrip;

			if (contextMenuStrip != null) {
				contextMenuStrip.Opened -= ContextMenuStripProvider.HandleContextMenuStripOpened;
				contextMenuStrip.Opened += ContextMenuStripProvider.HandleContextMenuStripOpened;
				
				contextMenuStrip.Closed -= ContextMenuStripProvider.HandleContextMenuStripClosed;
				contextMenuStrip.Closed += ContextMenuStripProvider.HandleContextMenuStripClosed;
			}
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

		void TrackControlClick (object sender, EventArgs e)
		{
			
		}

		void HandleContextMenuPopup (object sender, EventArgs e)
		{
			var contextMenuProvider = (FragmentControlProvider) ProviderFactory.GetProvider (contextMenu);
			AddChildProvider (contextMenuProvider);
		}

		void HandleContextMenuCollapse (object sender, EventArgs e)
		{
			var contextMenuProvider = (FragmentControlProvider) ProviderFactory.FindProvider (contextMenu);
			if (contextMenuProvider == null)
				return;
			RemoveChildProvider (contextMenuProvider);
			contextMenuProvider.Terminate ();
			ProviderFactory.ReleaseProvider (contextMenu);
			// TODO: Need to handle disposal of some parent without close happening?
		}
		
		#endregion
		
		#region Public Events

		public event NavigationEventHandler NavigationUpdated;

		#endregion
		
		#region Public Properties

		public NavigationWinform Navigation => navigation;

		public int ChildrenCount => navigation.VisibleWinformChildrenCount;

		#endregion
		
		#region Public Methods

		public void AddChildProvider (FragmentControlProvider newChild)
		{
			AddChildProvider (newChild, true);
		}

		protected void AddChildProvider (FragmentControlProvider newChild, bool raiseEvent)
		{
			InsertChildProviderBefore (newChild, null, raiseEvent);
		}

		protected void InsertChildProviderBefore (FragmentControlProvider newChild, FragmentControlProvider baseChild)
		{
			InsertChildProviderBefore (newChild, baseChild, true);
		}

		protected void InsertChildProviderBefore (FragmentControlProvider newChild, FragmentControlProvider baseChild, bool raiseEvent)
		{
			InsertChildProviderBefore (newChild, baseChild, raiseEvent, true);
		}

		protected virtual void InsertChildProviderBefore (FragmentControlProvider newChild, FragmentControlProvider baseChild, bool raiseEvent, bool recursive)
		{
			if (navigation.ContainsAnyChild (newChild)) {
				Log.Error($"FragmentControlProvider.InsertChildProviderBefore: this={this} already contains newChild={newChild}");
				return;
			}

			if (baseChild == null) {
				navigation.AppendChild (newChild);
			}
			else {
				if (!navigation.ContainsAnyChild (baseChild)) {
					Log.Error($"FragmentControlProvider.InsertChildProviderBefore: this={this} does not contain baseChild={baseChild}");
					return;
				}
				navigation.InsertChildBefore (newChild, baseChild);
			}

			NotifyOnChildProviderAdded (newChild, raiseEvent);
			
			if (recursive)
				newChild.InitializeChildControlStructure ();
		}

		public void RemoveChildProvider (FragmentControlProvider child)
		{
			RemoveChildProvider (child, true);
		}

		public virtual void RemoveChildProvider (FragmentControlProvider child, bool raiseEvent)
		{
			if (!navigation.ContainsAnyChild (child)) {
				Log.Error($"FragmentControlProvider.RemoveChildProvider: this={this} does not contain child={child}");
				return;
			}
			
			child.FinalizeChildControlStructure ();  // Always recursive

			NotifyOnChildProviderRemoved (child, raiseEvent);

			navigation.RemoveChild (child);
		}

		public override void Initialize ()
		{
			base.Initialize ();
		}

		public override void Terminate ()
		{
			InvalidateReallyVisible ();
			FinalizeChildControlStructure ();
			// No navigation.Clear (): `DataGridProver`, `DataGridViewProvder` uses`Terminate()` on Refresh and some othe cases.
			base.Terminate ();
		}

		public void SetWfcUserCustomProviderWrapper (UserCustomFragmentProviderWrapper wrapper)
		{
			if (wrapper == null) {
				Log.Error($"[FragmentControlProvider.SetWfcUserCustomProviderWrapper] wrapper is null; this={this}");
				return;
			}
			navigation.SetWfcUserCustomProviderWrapper (wrapper);
		}

		//TODO: Are the generated events duplicated? Because we're already
		//supporting StructureChangeType when Children are added to Controls.
		//See: SimpleControlProvider.InitializeEvents
		protected virtual void InitializeChildControlStructure ()
		{
			if (Control != null) {
				Control.ControlAdded += OnChildControlAdded;
				Control.ControlRemoved += OnChildControlRemoved;
				
				foreach (SWF.Control childControl in Control.Controls)
					HandleChildComponentAdded (childControl);
			}
		}
		
		protected virtual void FinalizeChildControlStructure()
		{
			if (Control != null) {
				Control.ControlAdded -= OnChildControlAdded;
				Control.ControlRemoved -= OnChildControlRemoved;
			}

			foreach (var ch in navigation.GetAllChildren ())
				HandleChildComponentRemoved (ch.Component);

			if (navigation.UserCustomProvider != null)
				navigation.UserCustomProvider.Terminate ();
		}
		
		#endregion
		
		#region Protected Methods

		private void NotifyOnChildProviderAdded (FragmentControlProvider child, bool raiseEvent)
		{
			child.InvalidateReallyVisible ();
			var visible = child.IsReallyVisible ();
			var re = raiseEvent && visible && (child.previousVisible != visible);
			OnNavigationUpdated (this, StructureChangeType.ChildRemoved, re);
			RaiseStructureChangedEvents (child, StructureChangeType.ChildAdded, re);
			var ucp = child.navigation.UserCustomProvider; 
			if (re && ucp != null) {
				RaiseStructureChangedEvents (ucp, StructureChangeType.ChildAdded, true);
				ucp.OnAdded ();
			}
			child.previousVisible = visible;
		}
		
		private void NotifyOnChildProviderRemoved (FragmentControlProvider child, bool raiseEvent)
		{
			child.InvalidateReallyVisible ();
			var re = raiseEvent && child.previousVisible;
			var ucp = child.navigation.UserCustomProvider;
			if (re && ucp != null) {
				ucp.OnRemoved ();
				RaiseStructureChangedEvents (ucp, StructureChangeType.ChildRemoved, true);
			}
			OnNavigationUpdated (this, StructureChangeType.ChildRemoved, re);
			RaiseStructureChangedEvents (child, StructureChangeType.ChildRemoved, re);
			child.previousVisible = false;
		}

		protected virtual void OnNavigationChildrenCleared ()
		{
			//TODO: Is this the event to generate?
			OnNavigationUpdated (this, StructureChangeType.ChildrenBulkRemoved, true);
			RaiseStructureChangedEvents (this, StructureChangeType.ChildrenBulkRemoved, true);
		}

		private void OnNavigationUpdated (FragmentControlProvider subjectProvider, StructureChangeType structureChangeType, bool raiseEvent)
		{
			if (NavigationUpdated != null) {
				var args = new NavigationEventArgs (raiseEvent, structureChangeType, subjectProvider);
				NavigationUpdated (this, args);
			}
		}

		private void RaiseStructureChangedEvents (IRawElementProviderFragment subjectProvider, StructureChangeType structureChangeType, bool emitUiaEvents)
		{
			if (emitUiaEvents) {
				Helper.RaiseStructureChangedEvent (structureChangeType, subjectProvider);
				Helper.RaiseStructureChangedEvent (StructureChangeType.ChildrenInvalidated, this);
			}
		}

		#endregion

		// This method is aimed to move child provider from one parent provider to another one.
		// This is suitable to deal with branch of `FormProvider` rooted on `DesktopProvier`.
		protected static void BecomeParent (FragmentControlProvider newParent, FormProvider child)
		{
			var oldParent = (FragmentControlProvider) child.Navigate (NavigateDirection.Parent);
			if (oldParent != null)
				oldParent.HandleChildComponentRemoved (child.Component);
			newParent.HandleChildComponentAdded (child.Component);

			child.Form.FormClosed -= form_FormClosed;
			child.Form.FormClosed += form_FormClosed;
		}

		private static void form_FormClosed (object sender, SWF.FormClosedEventArgs args)
		{
			var form = (SWF.Form) sender;
			var formProvider = (FragmentControlProvider) ProviderFactory.FindProvider (form);
			var parentProvider = (FragmentControlProvider) formProvider.navigation.Parent;
			if (parentProvider != null) {
				parentProvider.HandleChildComponentRemoved (form);
			}
			else {
				formProvider.Terminate ();
				ProviderFactory.ReleaseProvider (form);
			}
			
		}
		
		#region Private Methods: Event Handlers
	
		private void OnChildControlAdded (object sender, SWF.ControlEventArgs args)
		{
			HandleChildComponentAdded (args.Control);
		}
	
		private void OnChildControlRemoved (object sender, SWF.ControlEventArgs args)
		{
			HandleChildComponentRemoved (args.Control);
		}

		private static void OnControlVisibleChanged (object sender, EventArgs args)
		{
			var control = (SWF.Control) sender;
			var provider = (FragmentControlProvider) ProviderFactory.FindProvider (control);
			provider?.UpdateVisibilityRecursive ();
		}
		
		private void UpdateVisibilityRecursive ()
		{
			InvalidateReallyVisible ();
			if (IsReallyVisible ()) {
				navigation.Parent.NotifyOnChildProviderAdded (this, true);
				foreach (var p in navigation.GetAllChildren ())
					p.UpdateVisibilityRecursive ();
			}
			else {
				foreach (var p in navigation.GetAllChildren ())
					p.UpdateVisibilityRecursive ();
				navigation.Parent.NotifyOnChildProviderRemoved (this, true);
			}
		}

		protected void HandleChildComponentAdded (Component childComponent)
		{
			if (childComponent is SWF.Control childCcontrol) {
				if (Mono.UIAutomation.Winforms.ErrorProvider.InstancesTracker.IsControlFromErrorProvider (childCcontrol))
					return;
				childCcontrol.VisibleChanged += OnControlVisibleChanged;
			}

			var childProvider = (FragmentControlProvider) ProviderFactory.GetProvider (childComponent);
			AddChildProvider (childProvider);
		}

		protected void HandleChildComponentRemoved (Component childComponent)
		{
			if (childComponent is SWF.Control childCcontrol)
				childCcontrol.VisibleChanged -= OnControlVisibleChanged;

			var childProvider = (FragmentControlProvider) ProviderFactory.FindProvider (childComponent);
			if (childProvider == null) {
				Log.Error($"[FragmentControlProvider.HandleChildComponentRemoved] no Provider for <{childComponent}>");
				return;
			}

			// Event source:
			//       Parent of removed child runtimeId: The child that was
			//       removed. (pg 6 of fxref_uiautomationtypes_p2.pdf)
			RemoveChildProvider (childProvider);

			ProviderFactory.ReleaseProvider (childComponent);
		}

		#endregion

		private void InvalidateReallyVisible ()
		{
			isReallyVisible = ReallyVisible.Unknown;
		}

		internal virtual bool IsReallyVisible ()
		{
			isReallyVisible = ReallyVisible.Unknown;
			if (isReallyVisible == ReallyVisible.Unknown) {
				var parent = navigation.Parent;
				var visible = (parent == null)
					? false
					: IsComponentVisible () && parent.IsReallyVisible ();
				isReallyVisible = visible ? ReallyVisible.Yes : ReallyVisible.No;
			}
			return (isReallyVisible == ReallyVisible.Yes);
		}

		private bool IsComponentVisible ()
		{
			if (navigation.Parent != null && navigation.Parent.GuideChildComponentVisible (this, out bool isVisible))
				return isVisible;
			if (Component is SWF.Control control)
				return control.Visible;
			return true;  // Component based providers will need to override this method
		}

		protected virtual bool GuideChildComponentVisible (FragmentControlProvider childProvider, out bool isVisible)
		{
			isVisible = false;
			return false;
		}

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
					return navigation.Parent; // !!!:      ?? Provider.FragmentRoot;
				case NavigateDirection.FirstChild:
					return navigation.FirstVisibleChildProvider;
				case NavigateDirection.LastChild:
					return navigation.LastVisibleChildProvider;
				case NavigateDirection.NextSibling:
					return navigation.NextVisibleProvider;
				case NavigateDirection.PreviousSibling:
					return navigation.PreviousVisibleProvider;
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
			var comp = (Component?.ToString () ?? "[null]").Replace (Environment.NewLine, " ");
			return $"<{this.GetType ()}:{comp},{runtimeId}>";
		}
	}
}
