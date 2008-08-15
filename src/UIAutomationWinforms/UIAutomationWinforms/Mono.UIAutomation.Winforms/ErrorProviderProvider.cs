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
using Mono.UIAutomation.Winforms.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	//Calling MWF.ErrorProvider.SetError(control) will add one UserControl to 
	//control.Parent to show the "Image Exclamation Mark". 
	//We are doing the following to keep track of those UserControls to ignore 
	//them when control.Parent.ControlAdded event is generated:
	//1. Use internal event MWF.Control.ErrorProviderHookup, this is generated 
	//   in control when MWF.ErrorProvider.SetError(control,"error") is called.
	//   We keep track of added UserControl instance using InstancesTracker.
	//2. Use internal event MWF.Control.ErrorProviderUnhookup, this is generated 
	//   in control when MWF.ErrorProvider.SetError(control,"") is called.
	//   We remove track of added UserControl instance using InstancesTracker.
	internal class ErrorProviderProvider : PaneProvider
	{
		
		#region Constructor
		
		public ErrorProviderProvider (ErrorProvider errorProvider) : base (errorProvider)
		{
			this.errorProvider = errorProvider;
		}

		#endregion
		
		public ErrorProvider ErrorProvider {
			get { return errorProvider; }
		}
		
		#region SimpleControlProvider: Specializations
		
		public override Component Container {
			get { return errorProvider.ContainerControl; }
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return GetBoundingRectangle ();
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
		
		#region Private Methods
		
		private Rectangle GetBoundingRectangle ()
		{
			List<Control> controls = ErrorProviderProvider.InstancesTracker.GetControlsFromParent (errorProvider.ContainerControl);
			
			Rectangle bounding = controls [0].Bounds;
			for (int index = 1; index < controls.Count; index++)
				bounding = Rectangle.Union (bounding, controls [index].Bounds);
			return bounding;
		}
		
		#endregion
		
		#region Private Fields
		
		private List<Control> controls;
		private List<Control> tooltips;
		private ErrorProvider errorProvider;
		
		#endregion
		
		#region Internal Class: InstancesTracker
		
		//NOTE:
		//     This class tracks added Icon-like Controls when SWF.ErrorProvider.SetError 
		//     is called. We are tracking this instances because there's one
		//     provider for all Icon-like controls added and then we need to get 
		//     Bounds information from each Icon-like control.
		//
		//     The dictionary tree follows this structure:
		//
		//     ParentControl (dictionary.key)
		//          \________ InstancesDictionary0 (dictionary.value)
		//          |                \______________ ErrorProviderInstance0 (key)
		//          |                                         \_________________ List<Control> (value)
		//          |                                                              |
		//          |                                                              |___ Control0 added by ErrorProvider
		//          |                                                              |___ Control1 added by ErrorProvider
		//          |                                                              |___ ...
		//          |                                                              |___ Controln added by ErrorProvider
		//          |
		//          \________ InstancesDictionary1 (dictionary.value)
		//                          \______________ ErrorProviderInstance1 (key)
		//                                                    \_________________ List<Control> (value)
		//                                                                         |
		//                                                                         |___ Control0 added by ErrorProvider
		//                                                                         |___ Control1 added by ErrorProvider
		//                                                                         |___ ...
		//                                                                         |___ Controln added by ErrorProvider
		// 
		//
		internal sealed class InstancesTracker 
		{
			private InstancesTracker ()
			{
			}
			
			public static ErrorProviderProvider GetProviderFromControl (Control control)
			{
				if (dictionary == null)
					return null;
				else
					return dictionary.GetProviderFromControl (control);
			}
			
			public static List<Control> GetControlsFromParent (Control parent)
			{
				if (dictionary == null)
					return null;
				
				return dictionary.GetControls (parent);
			}
			
			public static void RemoveUserControlsFromParent (Control parent)
			{
				if (dictionary == null)
					return;
				
				dictionary.RemoveUserControlsFromParent (parent);
			}
			
			public static bool IsControlFromErrorProvider (Control control) 
			{
				if (dictionary == null)
					return false;
				else
					return dictionary.IsControlFromErrorProvider (control);
			}
			
			public static bool IsFirstControlFromErrorProvider (Control control)
			{
				if (dictionary == null)
					return false;
				else
					return dictionary.IsFirstControlFromErrorProvider (control);
			}
			
			public static void AddControl (Control control, 
			                               Control parent, 
			                               ErrorProviderProvider provider)
			{
				if (dictionary == null)
					dictionary = new InstancesDictionary ();

				dictionary.AddControl (control, parent, provider);
			}
			
			public static void RemoveControl (Control control, Control parent)
			{
				if (dictionary == null)
					return;
				
				dictionary.RemoveControl (control, parent);
			}
			
			private static InstancesDictionary dictionary;
		}
		
		#endregion
		
		#region Internal Class: InstancesDictionary
		
		class InstancesDictionary 
		{
			public InstancesDictionary () 
			{
				controlDictionary = new ErrorProviderControlDictionary ();
				parentDictionary = new ParentDictionary ();
				providerDictionary = new ErrorProviderDictionary ();
			}
			
			public void AddControl (Control control, 
			                        Control parent, 
			                        ErrorProviderProvider provider)
			{			
				if (controlDictionary.ContainsKey (control) == true)
					return;
				
				//UserControl <-> ErrorProvider 
				controlDictionary.Add (control, provider);

				//ErrorProvider <-> UserControl's
				ErrorProviderControlList errorControls;
				if (providerDictionary.TryGetValue (provider, 
				                                    out errorControls) == false) {
					errorControls = new ErrorProviderControlList ();
					providerDictionary [provider] = errorControls;
				}
				errorControls.Add (control);

				//Parent <-> UserControl's
				ErrorProviderControlList parentControls;
				if (parentDictionary.TryGetValue (parent, 
				                                  out parentControls) == false) {
					parentControls = new ErrorProviderControlList ();
					parentDictionary [parent] = parentControls;
				}
				parentControls.Add (control);
				
				//TODO: FIXME
				if (parentControls.Count == 1) {
					FragmentRootControlProvider root = (FragmentRootControlProvider) ProviderFactory.GetProvider (parent);
					root.AddChildProvider (true, provider);		
				}
				
				//To update Navigation
//				control.VisibleChanged += new EventHandler (OnErrorProviderControlVisibleChanged);
			}
			
//			private void OnErrorProviderControlVisibleChanged (object sender, EventArgs args)
//			{
//				if (((Control) sender).Visible == true) {
//					if (InstancesTracker.IsFirstControlFromErrorProvider ((Control) sender) == true) {
//						FragmentRootControlProvider root = (FragmentRootControlProvider) ProviderFactory.GetProvider (((Control) sender).Parent);
//						root.AddChildProvider (true, InstancesTracker.GetProviderFromControl ((Control) sender));
//					}
//				} else if (((Control) sender).Visible == false) {
//					if (InstancesTracker.IsFirstControlFromErrorProvider ((Control) sender) == true) {
//						FragmentRootControlProvider root = (FragmentRootControlProvider) ProviderFactory.GetProvider (((Control) sender).Parent);
//						root.RemoveChildProvider (true, InstancesTracker.GetProviderFromControl ((Control) sender));
//					}
//				}
//			}
			
			public ErrorProviderProvider GetProviderFromControl (Control control)
			{
				return controlDictionary [control];
			}
			
			public ErrorProviderControlList GetControls (Control parent)
			{
				ErrorProviderControlList parentControls;
				parentDictionary.TryGetValue (parent, out parentControls);
				return parentControls;
			}
			
			public bool IsControlFromErrorProvider (Control control) 
			{
				return controlDictionary.ContainsKey (control);
			}
			
			public bool IsFirstControlFromErrorProvider (Control control)
			{
				if (IsControlFromErrorProvider (control) == false)
					return false;
				else {
					ErrorProviderProvider provider = controlDictionary [control];
					ErrorProviderControlList errorControls = providerDictionary [provider];
					return errorControls [0] == control;
				}
			}
			
			public void RemoveControl (Control control, Control parent)
			{
				if (controlDictionary.ContainsKey (control) == false)
					return;
				
				Console.WriteLine ("Calling: RemoveControl");
				
				ErrorProviderProvider provider = controlDictionary [control];

//				control.VisibleChanged -= new EventHandler (OnErrorProviderControlVisibleChanged);
//				if (IsFirstControlFromErrorProvider (control) == true) {
//					FragmentRootControlProvider root = (FragmentRootControlProvider) ProviderFactory.GetProvider (parent);
//					root.RemoveChildProvider (false, provider);
//				}


//				//ErrorProvider <-> UserControl's
//				ErrorProviderControlList errorControls;
//				if (providerDictionary.TryGetValue (provider, 
//				                                    out errorControls) == true) {
//					errorControls.Remove (control);
//					if (errorControls.Count == 0)
//						providerDictionary.Remove (provider);
//				}

				//Parent <-> UserControl's
				ErrorProviderControlList parentControls;
				if (parentDictionary.TryGetValue (parent, 
				                                  out parentControls) == true) {
					parentControls.Remove (control);
					Console.WriteLine ("Size: {0}", parentControls.Count);
					if (parentControls.Count == 0) {
						parentDictionary.Remove (parent);
						//FIXME:
						FragmentRootControlProvider root = (FragmentRootControlProvider) ProviderFactory.GetProvider (parent);
						root.RemoveChildProvider (true, provider);	
					}
				}
				
				//ErrorProvider <-> UserControl's
				ErrorProviderControlList errorControls;
				if (providerDictionary.TryGetValue (provider, 
				                                    out errorControls) == true) {
					errorControls.Remove (control);
					if (errorControls.Count == 0)
						providerDictionary.Remove (provider);
				}
				
				//UserControl <-> ErrorProvider 
				controlDictionary.Remove (control);
			}
			
			public void RemoveUserControlsFromParent (Control parent)
			{
				Console.WriteLine ("Calling: RemoveUserControlsFromParent");
				ErrorProviderControlList parentControls;
				if (parentDictionary.TryGetValue (parent, 
				                                  out parentControls) == true) {
					for (; parentControls.Count > 0; )
						RemoveControl (parentControls [0], parent);
				}
			}

			private ErrorProviderControlDictionary controlDictionary;
			private ParentDictionary parentDictionary;	
			private ErrorProviderDictionary providerDictionary;
		}
		
		#endregion
		
		#region Internal Classes: Dictionaries and Lists
		
		class ErrorProviderControlList : List<Control>
		{
		}	
		
		class ParentDictionary : Dictionary<Control, ErrorProviderControlList>
		{
		}
		
		class ErrorProviderControlDictionary : Dictionary<Control, ErrorProviderProvider>
		{
		}
		
		class ErrorProviderDictionary : Dictionary<ErrorProviderProvider, ErrorProviderControlList>
		{
		}

		#endregion

	}
}
