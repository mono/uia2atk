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
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using SWFErrorProvider = System.Windows.Forms.ErrorProvider;

namespace Mono.UIAutomation.Winforms
{
	// NOTE: 
	//      Calling MWF.ErrorProvider.SetError(control) will add one UserControl to 
	//control.Parent to show the "Image Exclamation Mark". 
	//We are doing the following to keep track of those UserControls to ignore 
	//them when control.Parent.ControlAdded event is generated:
	//1. Use internal event MWF.Control.ErrorProviderHookup, this is generated 
	//   in control when MWF.ErrorProvider.SetError(control,"error") is called.
	//   We keep track of added UserControl instance using InstancesTracker.
	//2. Use internal event MWF.Control.ErrorProviderUnhookup, this is generated 
	//   in control when MWF.ErrorProvider.SetError(control,"") is called.
	//   We remove track of added UserControl instance using InstancesTracker.
	internal class ErrorProvider : PaneProvider
	{
		
		#region Constructor
		
		public ErrorProvider (Control ctrl,
		                      SWFErrorProvider errorProvider) : base (errorProvider )
		{	
			this.errorProvider = errorProvider;

			parent = ErrorProvider.InstancesTracker.GetParentFromControl (ctrl);
			controls = new List<Control> ();
			AddControl (ctrl);
		}

		#endregion
		
		#region Public Properties
		
		public SWFErrorProvider SWFErrorProvider {
			get { return errorProvider; }
		}
	
		public Control Parent {
			get { return parent; }
		}

		#endregion
		
		#region Public Methods
		
		public void AddControl (Control control)
		{
			if (controls.Contains (control) == false) {
				controls.Add (control);
				control.VisibleChanged += new EventHandler (OnControlVisibleChanged);
				if (controls.Count == 1) {
					FragmentRootControlProvider root 
						= (FragmentRootControlProvider) ProviderFactory.GetProvider (Parent);
					root.AddChildProvider (true, this);
				}
			}
		}
		
		public void DeleteControl (Control control)
		{
			if (controls.Contains (control) == true) {
				if (controls.Count == 1) {
					FragmentRootControlProvider root 
						= (FragmentRootControlProvider) ProviderFactory.GetProvider (Parent);
					root.RemoveChildProvider (true, this);
				}
				controls.Remove (control);
				control.VisibleChanged -= new EventHandler (OnControlVisibleChanged);
			}
		}
		
		#endregion

		#region SimpleControlProvider: Specializations
		
		public override Component Container {
			get { return Parent; }
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
		
		private Rect GetBoundingRectangle ()
		{
			List<Control> controls 
				= ErrorProvider.InstancesTracker.GetControlsFromProvider (this);
			
			Rectangle bounding = controls [0].Bounds;
			for (int i = 1; i < controls.Count; i++)
				bounding = Rectangle.Union (bounding, controls [i].Bounds);

			return Helper.RectangleToRect (bounding);
		}
		
		private void OnControlVisibleChanged (object sender, EventArgs args)
		{
			Control control = (Control) sender;

			if (control.Visible == true) {
				FragmentRootControlProvider root 
					= (FragmentRootControlProvider) ProviderFactory.GetProvider (Parent);
				root.AddChildProvider (true, this);
			} else
				DeleteControl (control);
		}		
		
		#endregion
		
		#region Private Fields
		
		private List<Control> tooltips;
		private List<Control> controls;
		private Control parent;
		private SWFErrorProvider errorProvider;
		
		#endregion
		
		#region Internal Class: InstancesTracker
		
		//NOTE:
		//     This class tracks added Icon-like Controls when SWF.ErrorProvider.SetError 
		//     is called. We are tracking this instances because there's one
		//     provider for all Icon-like controls added and then we need to get 
		//     Bounds information from each Icon-like control.
		internal sealed class InstancesTracker 
		{
			private InstancesTracker ()
			{
			}
			
			public static Control GetParentFromControl (Control control)
			{
				if (dictionary == null)
					return null;
				
				if (control.Parent == null) {
					foreach (KeyValuePair<Control, ErrorProviderControlList> 
					         value in parentDictionary) {
						if (value.Value.Contains (control) == true)
							return value.Key;
					}
				} 
				
				return control.Parent;
			}
			
			public static void AddControl (Control control,
			                               Control parent,
			                               SWFErrorProvider provider)
			{
				
				if (dictionary == null) {
					dictionary = new ErrorProviderControlDictionary ();
					parentDictionary = new ParentDictionary ();
				}
				dictionary [control] = provider;
				
				ErrorProviderControlList parentControls;
				if (parentDictionary.TryGetValue (parent, 
				                                  out parentControls) == false) {
					parentControls = new ErrorProviderControlList ();
					parentDictionary [parent] = parentControls;
				}
				parentControls.Add (control);
				
				ErrorProvider errorProvider = (ErrorProvider) ProviderFactory.GetProvider (control);
			}
			
			public static void RemoveControl (Control control, 
			                                  Control parent, 
			                                  SWFErrorProvider provider)
			{
				if (dictionary == null 
				    || dictionary.ContainsKey (control) == false)
					return;

				parentDictionary [parent].Remove (control);
				dictionary.Remove (control);	
			}			
			
			public static SWFErrorProvider GetErrorProviderFromControl (Control control)
			{
				return dictionary == null ? null : dictionary [control];
			}
			
			public static List<Control> GetControlsFromProvider (ErrorProvider errorProvider)
			{
				FragmentRootControlProvider parent 
					= (FragmentRootControlProvider) errorProvider.Navigation.Navigate (NavigateDirection.Parent);
				if (parent == null)
					return null;

				if (parentDictionary == null || parentDictionary.ContainsKey (parent.Control) == false)
					return null;
				
				ErrorProviderControlList parentControls = parentDictionary [parent.Control];
				
				List<Control> controls =
					(from c
					in parentControls
					where dictionary [c] == errorProvider.SWFErrorProvider
					select c).ToList ();
				
				return controls;
			}

			public static bool IsControlFromErrorProvider (Control control) 
			{
				if (dictionary == null)
					return false;
				else
					return dictionary.ContainsKey (control);
			}
			
			public static bool IsFirstControlFromErrorProvider (Control control)
			{
				if (IsControlFromErrorProvider (control) == false)
					return false;
				
				IEnumerable<Control> controls =
					from c
					in parentDictionary [GetParentFromControl (control)]
					where dictionary [c] == dictionary [control]
					select c;
				
				return controls.Count () == 1;
			}
			
			private static ErrorProviderControlDictionary dictionary;
			private static ParentDictionary parentDictionary;
		}
		
		#endregion
		
		#region Internal Classes: Dictionaries and Lists
		
		class ErrorProviderControlList : List<Control>
		{
		}	
		
		class ParentDictionary : Dictionary<Control, ErrorProviderControlList>
		{
		}
		
		class ErrorProviderControlDictionary : Dictionary<Control, SWFErrorProvider>
		{
		}

		#endregion

	}
}
