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
	public class WindowProvider : IWindowProvider, IRawElementProviderFragmentRoot
	{
#region Private Data
		
		private Form form;
		private IDictionary<Control, IRawElementProviderSimple>
			controlProviders;
		
#endregion
		
#region Constructors

		public WindowProvider (Form form)
		{
			this.form = form;
			
			controlProviders =
				new Dictionary<Control, IRawElementProviderSimple> ();
			
			form.ControlAdded += OnControlAdded;
			form.ControlRemoved += OnControlRemoved;
			form.Closed += OnClosed;
			
			Console.WriteLine ("WindowProvider created");
			
			foreach (Control control in form.Controls) {
				IRawElementProviderSimple provider =
					CreateProvider (control);
				// TODO: Null check, compound, etc?
				controlProviders [control] = provider;
			}
		}
		
#endregion
		
#region Private Methods
	
		private IRawElementProviderSimple CreateProvider (Control control)
		{
			Button b = control as Button;
			if (b != null)
				return new ButtonProvider (b);
			
			return null;
		}
		
		private IRawElementProviderSimple GetProvider (Control control)
		{
			if (controlProviders.ContainsKey (control))
				return controlProviders [control];
			else
				return null;
		}
		
#endregion
		
#region Private Event Handlers
	
		private void OnControlAdded (object sender, EventArgs args)
		{
			Console.WriteLine ("ControlAdded: " + sender.GetType ().ToString ());
		}
	
		private void OnControlRemoved (object sender, EventArgs args)
		{
			Console.WriteLine ("ControlRemoved: " + sender.GetType ().ToString ());
		}
		
		private void OnClosed (object sender, EventArgs args)
		{
			// TODO: Fill in rest of eventargs
			AutomationInteropProvider.RaiseStructureChangedEvent (
			  this,
			  new StructureChangedEventArgs (StructureChangeType.ChildrenBulkRemoved,
			                                 new int [] {0}));
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
				throw new NotImplementedException ();
			}
		}
		
		public bool Maximizable {
			get {
				throw new NotImplementedException ();
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
				throw new NotImplementedException ();
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
		
#region IRawElementProviderFragmentRoot Members

		public IRawElementProviderSimple HostRawElementProvider {
			get {
				return AutomationInteropProvider.HostProviderFromHandle (form.Handle);
			}
		}
		
		public ProviderOptions ProviderOptions {
			get {
				return ProviderOptions.ServerSideProvider;
			}
		}

		public object GetPatternProvider (int patternId)
		{
			if (patternId == WindowPatternIdentifiers.Pattern.Id)
				return this;
			return null;
		}
		
		public object GetPropertyValue (int propertyId)
		{
			// TODO: Complete...figure out by testing Windows implementation (UISpy is helpful)
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Window.Id;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return form.Text;
			else
				return null;
			
		}
		
		public System.Windows.Rect BoundingRectangle {
			get {
				System.Windows.Rect boundingRect =
					new System.Windows.Rect (form.Location.X,
					                         form.Location.Y,
					                         form.Width,
					                         form.Height);
				return boundingRect;
			}
		}
		
		public IRawElementProviderFragmentRoot FragmentRoot {
			get {
				return this;
			}
		}

		public IRawElementProviderSimple[] GetEmbeddedFragmentRoots ()
		{
			/* "This method returns an array of fragments only if the current
			 * element is hosting another UI Automation framework. Most
			 * providers return null reference" */
			return null;
		}
		
		public int [] GetRuntimeId ()
		{
			return null;
		}
		
		public IRawElementProviderSimple Navigate (NavigateDirection direction)
		{
			// TODO: Consider what exactly "first" and "last" are
			//       supposed to mean.  Consider maintaining separate
			//       list just containing all of the providers and
			//       indexing off of that.
			
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
			
			return null;
			
//			"Fragment roots do not enable navigation to a parent or siblings;
//			navigation among fragment roots is handled by the default
//			window providers. Elements in fragments must navigate only 
//			to other elements within that fragment."
		}
		
		public void SetFocus ()
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
		
		public IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			if (x > form.Width || y > form.Height)
				return null;
			
			Control child = form.GetChildAtPoint (new Point ((int)x, (int)y));
			
			if (child != null) {
				Console.WriteLine (child);
				
				if (controlProviders.ContainsKey (child)) {
					IRawElementProviderSimple provider =
						controlProviders [child];
					IRawElementProviderFragment providerFragment =
						provider as IRawElementProviderFragment;
					if (providerFragment != null)
						return providerFragment;
				}
			} else
				Console.WriteLine ("ElementProviderFromPoint: Child is null");
			
			return this;
		}
		
		public IRawElementProviderFragment GetFocus ()
		{
			foreach (Control control in form.Controls) {
				if (control.Focused) {
					// TODO: Necessary to delve into child control
					// for focused element?
					
					if (controlProviders.ContainsKey (control)) {
						IRawElementProviderSimple provider =
							controlProviders [control];
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
