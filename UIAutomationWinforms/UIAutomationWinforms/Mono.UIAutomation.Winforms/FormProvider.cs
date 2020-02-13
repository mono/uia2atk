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
using System.Linq;
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
		private MainMenuProvider mainMenuProvider;
		#endregion

		#region Public Data
		public readonly Form Form;
		#endregion
		
		#region Constructors

		public FormProvider (Form form) : base (form)
		{
			Form = form;
			AlreadyClosed = false;
		}
		
		#endregion

		public bool AlreadyClosed { get; private set; }

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

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			Form.UIAMenuChanged += OnFormUIAMenuChanged;
			Form.UIAOwnerChanged += OnFromUIAOwnerChanged;
			
			SetupMainMenuProvider ();
		}

		protected override void FinalizeChildControlStructure ()
		{
			Form.UIAMenuChanged -= OnFormUIAMenuChanged;
			Form.UIAOwnerChanged -= OnFromUIAOwnerChanged;
			
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

		private void OnFormUIAMenuChanged (object sender, EventArgs args)
		{
			if (mainMenuProvider != null) {
				mainMenuProvider.Terminate ();
				RemoveChildProvider (mainMenuProvider);
			}
			SetupMainMenuProvider ();
		}

		private static void OnFromUIAOwnerChanged (object sender, EventArgs args)
		{
			var form = (Form) sender;
			UpdateOwnerProviderOfForm (form);
		}

		// This method is aimed to move child provider from one parent provider to another one.
		// This is suitable to deal with branch of `FormProvider` rooted on `DesktopProvier`.		
		internal static void UpdateOwnerProviderOfForm (Form form)
		{
			var ownerProvider = (form.Owner != null)
				? (FragmentControlProvider) ProviderFactory.GetProvider (form.Owner)
				: ProviderFactory.DesktopProvider;

			var formProvider = (FormProvider) ProviderFactory.GetProvider (form);

			BecomeParent (ownerProvider, formProvider);
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
			if (x > Form.Width || y > Form.Height)
				return null;
			
			Control child = Form.GetChildAtPoint (new Point ((int)x, (int)y));
			
			if (child != null) {
				Log.Debug (child.ToString ());
				FragmentControlProvider childFragmentProvider = Navigation.GetVisibleChildByComponent (child);
				if (childFragmentProvider != null)
					return childFragmentProvider;
			} else
				Log.Debug ("ElementProviderFromPoint: Child is null");
			
			return this;
		}
		
		public override IRawElementProviderFragment GetFocus ()
		{
			foreach (Control control in Form.Controls) {
				if (control.Focused) {
					// TODO: Necessary to delve into child control
					// for focused element?
					FragmentControlProvider childFragmentProvider = Navigation.GetVisibleChildByComponent (control);
					if (childFragmentProvider != null)
						return childFragmentProvider;
				}
			}
			return null;
		}

		#endregion

		#region Public Methods

		// Is called from the `WindowPatternWindowClosedEvent` class.
		public void Close ()
		{
			if (!AlreadyClosed)
				AlreadyClosed = true;
		}

		private void SetupMainMenuProvider ()
		{
			if (Form.Menu != null) {
				mainMenuProvider = (MainMenuProvider) ProviderFactory.GetProvider (Form.Menu);
				if (mainMenuProvider != null)
					AddChildProvider (mainMenuProvider);
			}
		}

		#endregion
	}
}
