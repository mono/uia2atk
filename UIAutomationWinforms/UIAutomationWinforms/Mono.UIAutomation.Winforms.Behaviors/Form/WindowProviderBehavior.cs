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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.Form;

namespace Mono.UIAutomation.Winforms.Behaviors.Form
{

	internal class WindowProviderBehavior 
		: ProviderBehavior, IWindowProvider
	{

		#region Constructors
		
		public WindowProviderBehavior (FormProvider provider)
			: base (provider) 
		{
			form = (SWF.Form) provider.Control;
			closing = false;
		}
		
		#endregion

		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern {
			get { return WindowPatternIdentifiers.Pattern; }
		}		
		
		public override void Connect () 
		{
			// NOTE: IsModal Property NEVER changes, because you need to call
			//       ShowDialog() to make it modal, and to change it to non-modal
			//       you need to close it and use Show()

			Provider.SetEvent (ProviderEventType.WindowPatternCanMaximizeProperty,
			                   new WindowPatternCanMaximizePropertyEvent (Provider));
			Provider.SetEvent (ProviderEventType.WindowPatternCanMinimizeProperty,
			                   new WindowPatternCanMinimizePropertyEvent (Provider));
			Provider.SetEvent (ProviderEventType.WindowPatternIsTopmostProperty,
			                   new WindowPatternIsTopmostPropertyEvent (Provider));
			Provider.SetEvent (ProviderEventType.WindowPatternWindowClosedEvent,
			                   new WindowPatternWindowClosedEvent (Provider));
			Provider.SetEvent (ProviderEventType.WindowPatternWindowInteractionStateProperty,
			                   new WindowPatternWindowInteractionStateProperty (Provider));
			Provider.SetEvent (ProviderEventType.WindowPatternWindowOpenedEvent,
			                   new WindowPatternWindowOpenedEvent (Provider));
			Provider.SetEvent (ProviderEventType.WindowPatternWindowVisualStateProperty,
			                   new WindowPatternWindowVisualStatePropertyEvent (Provider));

			form.Closed += OnClosed;
			form.Closing += OnClosing;
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.WindowPatternCanMaximizeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternCanMinimizeProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternIsModalProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternIsTopmostProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternWindowClosedEvent,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternWindowInteractionStateProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternWindowOpenedEvent,
			                   null);
			Provider.SetEvent (ProviderEventType.WindowPatternWindowVisualStateProperty,
			                   null);

			form.Closing -= OnClosing;
			form.Closed -= OnClosed;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == WindowPatternIdentifiers.CanMaximizeProperty.Id)
				return Maximizable;
			else if (propertyId == WindowPatternIdentifiers.CanMinimizeProperty.Id)
				return Minimizable;
			else if (propertyId == WindowPatternIdentifiers.IsModalProperty.Id)
				return IsModal;
			else if (propertyId == WindowPatternIdentifiers.IsTopmostProperty.Id)
				return IsTopmost;
			else if (propertyId == WindowPatternIdentifiers.WindowInteractionStateProperty.Id)
				return InteractionState;
			else if (propertyId == WindowPatternIdentifiers.WindowVisualStateProperty.Id)
				return VisualState;
			else
				return base.GetPropertyValue (propertyId);
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
			SWF.FormWindowState newState = form.WindowState;
			switch (state) {
			case WindowVisualState.Maximized:
				newState = SWF.FormWindowState.Maximized;
				break;
			case WindowVisualState.Minimized:
				newState = SWF.FormWindowState.Minimized;
				break;
			case WindowVisualState.Normal:
				newState = SWF.FormWindowState.Normal;
				break;
			}
			PerformSetVisualState (form, newState);
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
			get {
				return form.TopMost;
			}
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
				var activeForm = SWF.Form.ActiveForm;
				if (activeForm != null && activeForm != form && activeForm.Modal)
					return WindowInteractionState.BlockedByModalWindow;
				return WindowInteractionState.ReadyForUserInteraction;
				// TODO: I failed to find any example (on Win 7) to make WindowInteractionState
				// be NotResponding or Running,
				// Even if I set form.UseWaitCursor = true, the state is still ReadyForUserInteraction.
				// And if I sleep a long time in a button click handler to hang the form, then the UIA
				// client will also hang when trying to get the Form's UIA infomation, but not return
				// "NotResponding" as I expected.
			}
		}
		
		public WindowVisualState VisualState {
			get {
				switch (form.WindowState) {
				case SWF.FormWindowState.Maximized:
					return WindowVisualState.Maximized;
				case SWF.FormWindowState.Minimized:
					return WindowVisualState.Minimized;
				case SWF.FormWindowState.Normal:
				default:
					return WindowVisualState.Normal;
				}
			}
		}
		
		#endregion
		
		#region Event handlers
		
		private void OnClosed (object sender, EventArgs args)
		{
			closing = false;			
		}
		
		private void OnClosing (object sender, EventArgs args)
		{
			closing = true;
		}
		
		#endregion
		
		#region Private Methods
		
		private void PerformSetVisualState (SWF.Form form, SWF.FormWindowState state)
		{
			if (form.InvokeRequired == true) {
				form.BeginInvoke (new PerformSetVisualStateDelegate (PerformSetVisualState),
				                  new object [] { form, state });
				return;
			}
			form.WindowState = state;
		}
		
		#endregion
		
		#region Private Fields

		private bool closing;
		private SWF.Form form;

		#endregion
	}
	
	delegate void PerformSetVisualStateDelegate (SWF.Form form, SWF.FormWindowState state);
	
}
