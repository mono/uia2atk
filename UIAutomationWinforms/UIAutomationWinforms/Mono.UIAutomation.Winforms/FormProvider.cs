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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//      Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Services;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.Form;
using Mono.UIAutomation.Winforms.Behaviors.Form;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (Form))]
	internal class FormProvider : ContainerControlProvider
	{
		#region Private Data
		
		private Form form;
		private Form owner;
		private bool alreadyClosed;
		private MainMenuProvider mainMenuProvider;
		public event EventHandler ProviderClosed;
		
		#endregion
		
		#region Constructors

		public FormProvider (Form form) : base (form)
		{
			this.form = form;
			
			// We keep a copy because we can't reference "form" after 
			// Disposed (used in Close()) called by WindowPatternWindowClosedEvent.
			owner = form.Owner;
			alreadyClosed = false;
		}
		
		#endregion

		public bool AlreadyClosed {
			get { return alreadyClosed; }
		}

		public override void Initialize ()
		{
			base.Initialize ();

			// Behaviors
			
			SetBehavior (WindowPatternIdentifiers.Pattern,
			             new WindowProviderBehavior (this));
			SetBehavior (TransformPatternIdentifiers.Pattern,
			             new TransformProviderBehavior (this));

			// Events
			SetEvent (ProviderEventType.AutomationFocusChangedEvent,
			          new FormAutomationFocusChangedEvent (this));

			// Internal Event
			SetEvent (ProviderEventType.WindowDeactivatedEvent,
			          new WindowDeactivatedEvent (this));
		}

		public override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			form.UIAMenuChanged += OnUIAMenuChanged;
			SetupMainMenuProvider ();
		}

		public override void FinalizeChildControlStructure ()
		{
			form.UIAMenuChanged -= OnUIAMenuChanged;
			
			base.FinalizeChildControlStructure ();
		}

		public override void Terminate ()
		{
			// We are trying to Terminate our events, however the instance
			// is already disposed so can't remove the delegates
			if (!AlreadyClosed)
				base.Terminate ();
		}
		
		#region Private Event Handlers

		private void OnUIAMenuChanged (object sender, EventArgs args)
		{
			if (mainMenuProvider != null) {
				mainMenuProvider.Terminate ();
				RemoveChildProvider (mainMenuProvider);
			}
			SetupMainMenuProvider ();
		}
		
		#endregion

		#region IRawElementProviderFragmentRoot Members

		public override IRawElementProviderSimple HostRawElementProvider {
			get {
				return this;
			}
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Window.Id;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return Control.Text;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return false;
			else
				return base.GetProviderPropertyValue (propertyId);
		}
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			if (x > form.Width || y > form.Height)
				return null;
			
			Control child = form.GetChildAtPoint (new Point ((int)x, (int)y));
			
			if (child != null) {
				Log.Debug (child.ToString ());
				FragmentControlProvider childFragmentProvider = Navigation.TryGetChild (child);
				if (childFragmentProvider != null)
					return childFragmentProvider;
			} else
				Log.Debug ("ElementProviderFromPoint: Child is null");
			
			return this;
		}
		
		public override IRawElementProviderFragment GetFocus ()
		{
			foreach (Control control in form.Controls) {
				if (control.Focused) {
					// TODO: Necessary to delve into child control
					// for focused element?
					FragmentControlProvider childFragmentProvider = Navigation.TryGetChild (control);
					if (childFragmentProvider != null)
						return childFragmentProvider;
				}
			}
			return null;
		}

		#endregion

		#region Public Methods

		public void Close ()
		{
			if (AutomationInteropProvider.ClientsAreListening && !AlreadyClosed) {
				alreadyClosed = true;

				if (owner == null)
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved, this);
				else {
					var ownerProvider = ProviderFactory.FindProvider (owner) as FormProvider;
					if (ownerProvider != null)
						ownerProvider.RemoveChildProvider (this);
				}

				EmitProviderClosed ();
			}
		}

		private void SetupMainMenuProvider ()
		{
			if (form.Menu != null) {
				mainMenuProvider = (MainMenuProvider) ProviderFactory.GetProvider (form.Menu);
				if (mainMenuProvider != null)
					AddChildProvider (mainMenuProvider);
			}
		}

		#endregion

		private void EmitProviderClosed ()
		{
			if (ProviderClosed != null) {
				ProviderClosed (this, EventArgs.Empty);
			}
		}
	}
}
