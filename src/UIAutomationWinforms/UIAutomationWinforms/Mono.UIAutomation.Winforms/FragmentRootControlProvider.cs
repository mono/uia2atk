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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{
	public abstract class FragmentRootControlProvider :
		SimpleControlProvider, IRawElementProviderFragmentRoot
	{
#region Internal Data
		
		internal IDictionary<Control, IRawElementProviderSimple>
			controlProviders;  // TODO: Fix this...
		
#endregion

#region Constructors
		
		protected FragmentRootControlProvider (Control control) :
			base (control)
		{
			controlProviders =
				new Dictionary<Control, IRawElementProviderSimple> ();
			
			control.ControlAdded += OnControlAdded;
			control.ControlRemoved += OnControlRemoved;
			
			foreach (Control childControl in control.Controls) {
				IRawElementProviderSimple provider =
					CreateProvider (childControl);
				// TODO: Null check, compound, etc?
				controlProviders [childControl] = provider;
			}
		}
		
#endregion
		
#region Private Event Handlers
	
		private void OnControlAdded (object sender, ControlEventArgs args)
		{
			Console.WriteLine ("ControlAdded: " + args.Control.GetType ().ToString ());
		}
	
		private void OnControlRemoved (object sender, ControlEventArgs args)
		{
			Console.WriteLine ("ControlRemoved: " + args.Control.GetType ().ToString ());
		}

#endregion

#region Protected Methods
	
		protected IRawElementProviderSimple CreateProvider (Control control)
		{
			return ProviderFactory.GetProvider (control);
		}
		
		protected IRawElementProviderSimple GetProvider (Control control)
		{
			if (controlProviders.ContainsKey (control))
				return controlProviders [control];
			else
				return null;
		}
		
#endregion

#region IRawElementProviderFragmentRoot Overrides
	
		public virtual IRawElementProviderFragment ElementProviderFromPoint (double x, double y)
		{
			if (!BoundingRectangle.Contains (x, y))
				return null;
			
			// TODO: Check for child fragments.  Can this logic be generalized?
			
			return this;
		}
		
		public virtual IRawElementProviderFragment GetFocus ()
		{
			// TODO: Check for child fragments.  Can this logic be generalized?
			return null;
		}
		
		public virtual void SetFocus ()
		{
			control.Focus ();
		}
		
		public virtual IRawElementProviderFragment Navigate (NavigateDirection direction)
		{
			// TODO: Check for child fragments.  Can this logic be generalized?
			return null;
		}
		
		public abstract int [] GetRuntimeId ();
		
		public virtual IRawElementProviderSimple [] GetEmbeddedFragmentRoots ()
		{
			return null;
		}
		
		public virtual IRawElementProviderFragmentRoot FragmentRoot {
			get {
				return this; // Because we implement IRawElementProviderFragmentRoot
			}
		}
		
		public virtual Rect BoundingRectangle {
			get {
				return (Rect)
					GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			}
		}
#endregion
	}
}
