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
		
#endregion
		
#region Constructors

		public WindowProvider (Form form)
		{
			this.form = form;
			form.ControlAdded += OnControlAdded;
			form.ControlRemoved += OnControlRemoved;
			
			Console.WriteLine ("WindowProvider created");
		}
		
#endregion
		
#region Private Event Handlers
	
		private void OnControlAdded (object sender, EventArgs args)
		{
			
		}
	
		private void OnControlRemoved (object sender, EventArgs args)
		{
			
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
				return null; // TODO: What?
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
			return null;// TODO: Verify
		}
		
		public IRawElementProviderSimple Navigate (NavigateDirection direction)
		{
			throw new NotImplementedException ();
		}
		
		public void SetFocus ()
		{
			form.Focus ();
		}
		
		public IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			Control child = form.GetChildAtPoint (new Point ((int)x, (int)y));
			
			if (child == null) {
				Console.WriteLine ("Child is null");
				return null;
			} else {
				Console.WriteLine (child);
				// TODO: Logic to get provider for child
				return null;
			}
		}
		
		public IRawElementProviderFragment GetFocus ()
		{
			foreach (Control control in form.Controls) {
				if (control.Focused) {
					// TODO: Necessary to delve into child control
					// for focused element?
					// TODO: Logic to get provider for child
				}
			}
			
			return null;
		}
		
#endregion
	}
}
