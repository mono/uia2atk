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
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	internal class WindowProvider : FragmentRootControlProvider, IWindowProvider, ITransformProvider
	{
#region Private Data
		
		private Form form;
		private bool closing;
		
#endregion
		
#region Constructors

		public WindowProvider (Form form) : base (form)
		{
			this.form = form;
			closing = false;
			
			form.Closed += OnClosed;
			form.Shown += OnShown;
			form.Closing += OnClosing;
			
			Console.WriteLine ("WindowProvider created");
		}
		
#endregion
		
#region Private Event Handlers
		
		private void OnClosed (object sender, EventArgs args)
		{
			closing = false;
			
			if (!AutomationInteropProvider.ClientsAreListening)
				return;
			
			AutomationEventArgs eventArgs =
				new AutomationEventArgs (WindowPatternIdentifiers.WindowClosedEvent);
			AutomationInteropProvider.RaiseAutomationEvent (WindowPatternIdentifiers.WindowClosedEvent,
			                                                this,
			                                                eventArgs);
			// TODO: Fill in rest of eventargs
			
			FinalizeChildControlStructure ();
			
			Helper.RaiseStructureChangedEvent (StructureChangeType.ChildRemoved,
			                                   this);
		}
		
		private void OnShown (object sender, EventArgs args)
		{
			if (!AutomationInteropProvider.ClientsAreListening)
				return;
			
			AutomationEventArgs eventArgs =
				new AutomationEventArgs (WindowPatternIdentifiers.WindowOpenedEvent);
			AutomationInteropProvider.RaiseAutomationEvent (WindowPatternIdentifiers.WindowOpenedEvent,
			                                                this,
			                                                eventArgs);
		}
		
		private void OnClosing (object sender, EventArgs args)
		{
			closing = true;
		}
		
#endregion
		
#region IWindowProvider Members
		
		public bool WaitForInputIdle (int milliseconds)
		{
			if (milliseconds <= 0 || milliseconds > int.MaxValue)
				throw new ArgumentOutOfRangeException ("milliseconds");
			
			// TODO: How...?
			// return true iff window enters idle state before timeout occurs...
			
			System.Threading.Thread.Sleep (milliseconds);
			
			return false;
		}
		
		public void SetVisualState (WindowVisualState state)
		{
			switch (state) {
			case WindowVisualState.Maximized:
				form.WindowState = FormWindowState.Maximized;
				break;
			case WindowVisualState.Minimized:
				form.WindowState = FormWindowState.Minimized;
				break;
			case WindowVisualState.Normal:
				form.WindowState = FormWindowState.Normal;
				break;
			}
		}
		
		public void Close ()
		{
			form.Close ();
		}
		
		public bool Minimizable {
			get {
				return form.MinimizeBox;
			}
		}
		
		public bool Maximizable {
			get {
				return form.MaximizeBox;
			}
		}
		
		public bool IsTopmost {
			get { return form.TopMost; }
		}
		
		public bool IsModal {
			get {
				return form.Modal;
			}
		}
		
		public WindowInteractionState InteractionState {
			get {
				if (closing)
					return WindowInteractionState.Closing;
				return WindowInteractionState.Running;
				// TODO: How to check for things like
				//       NotResponding and BlockedByModalWindow?
			}
		}
		
		public WindowVisualState VisualState {
			get {
				switch (form.WindowState) {
				case FormWindowState.Maximized:
					return WindowVisualState.Maximized;
				case FormWindowState.Minimized:
					return WindowVisualState.Minimized;
				case FormWindowState.Normal:
				default:
					return WindowVisualState.Normal;
				}
			}
		}
		
#endregion
		
#region ITransformProvider Members
	
		public bool CanMove {
			get {
				return form.WindowState == FormWindowState.Normal;
			}
		}

		public bool CanResize {
			get {
				switch (form.FormBorderStyle) {
				case FormBorderStyle.Fixed3D:
				case FormBorderStyle.FixedDialog:
				case FormBorderStyle.FixedSingle:
				case FormBorderStyle.FixedToolWindow:
					return false;
				default:
					return true;
				}
			}
		}

		public bool CanRotate {
			get { return false; }
		}

		public void Move (double x, double y)
		{
			form.Location = new Point ((int) x, (int) y);
		}

		public void Resize (double width, double height)
		{
			form.Size = new Size ((int) width, (int) height);
		}

		public void Rotate (double degrees)
		{
			return;
		}

#endregion
		
#region IRawElementProviderFragmentRoot Members

		public override IRawElementProviderSimple HostRawElementProvider {
			get {
				return this;
			}
		}

		public override object GetPatternProvider (int patternId)
		{
			if (patternId == WindowPatternIdentifiers.Pattern.Id ||
			    patternId == TransformPatternIdentifiers.Pattern.Id)
				return this;
			
			return base.GetPatternProvider (patternId);
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			// TODO: Complete...figure out by testing Windows implementation (UISpy is helpful)
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Window.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "window";
			else if (propertyId == AutomationElementIdentifiers.NativeWindowHandleProperty.Id)
				return form.Handle; // TODO: Should be int, maybe?
			else
				return base.GetPropertyValue (propertyId);
		}
		
		public override int [] GetRuntimeId ()
		{
			return null;
		}
		
		public override IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			// TODO: Consider what exactly "first" and "last" are
			//       supposed to mean.  Consider maintaining separate
			//       list just containing all of the providers and
			//       indexing off of that.
			/*
			switch (direction) {
			case NavigateDirection.FirstChild:
				if (form.Controls.Count > 0)
					return GetProvider (form.Controls [0]);
				break;
			case NavigateDirection.LastChild:
				for (int i = form.Controls.Count - 1; i >= 0; i--) {
					IRawElementProviderSimple provider =
						GetProvider (form.Controls [i]);
					if (provider != null)
						return provider;
				}
				break;
			default:
				break;
			}
			*/
			return null;
			
//			"Fragment roots do not enable navigation to a parent or siblings;
//			navigation among fragment roots is handled by the default
//			window providers. Elements in fragments must navigate only 
//			to other elements within that fragment."
		}
		
		public override void SetFocus ()
		{
//			"The UI Automation framework will ensure that the part of
//			the interface that hosts this fragment is already focused
//			before calling this method. Your implementation should
//			update only its internal focus state; for example, by 
//			repainting a list item to show that it has the focus.
//			If you prefer that UI Automation not focus the parent window,
//			set the ProviderOwnsSetFocus option in ProviderOptions for the fragment root."
			form.Focus ();
		}
		
		public override IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			if (x > form.Width || y > form.Height)
				return null;
			
			Control child = form.GetChildAtPoint (new Point ((int)x, (int)y));
			
			if (child != null) {
				Console.WriteLine (child);
				
				if (component_providers.ContainsKey (child)) {
					IRawElementProviderSimple provider =
						component_providers [child];
					IRawElementProviderFragment providerFragment =
						provider as IRawElementProviderFragment;
					if (providerFragment != null)
						return providerFragment;
				}
			} else
				Console.WriteLine ("ElementProviderFromPoint: Child is null");
			
			return this;
		}
		
		public override IRawElementProviderFragment GetFocus ()
		{
			foreach (Control control in form.Controls) {
				if (control.Focused) {
					// TODO: Necessary to delve into child control
					// for focused element?
					
					if (component_providers.ContainsKey (control)) {
						IRawElementProviderSimple provider =
							component_providers [control];
						IRawElementProviderFragment providerFragment =
							provider as IRawElementProviderFragment;
						if (providerFragment != null)
							return providerFragment;
					}
				}
			}
			
			return null;
		}
		
#endregion
	}
}
