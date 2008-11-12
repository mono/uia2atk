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
using SD = System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	// NOTE: 
	//	When calling MWF.ErrorProvider.SetError(control) an instance of 
	//	UserControl is added to control.Parent and control.ControlAdded is 
	//	generated. This new UserControl is used to show the "Exclamation 
	//	Mark Image". We are skipping all those UserControl instances by 
	//	doing the following:
	//
	//	All SWF.Control subclasses support MWF.ErrorProvider.SetError, so
	//	Control-based providers use the following internal events
	//	to keep track of those instances:
	//	- SWF.Control.ErrorProviderHookup: event generated when 
	//	  MWF.ErrorProvider.SetError(control, "error message") or 
	//	  UserControl.Visible = true
	//	- SWF.Control.ErrorProviderUnhookup: event generated when
	//	  MWF.ErrorProvider.SetError(control, "") or 
	//	  UserControl.Visible = false
	//
	internal class ErrorProvider : PaneProvider
	{
		
		#region Constructor
		
		public ErrorProvider (SWF.ErrorProvider errorProvider) : base (errorProvider)
		{	
			this.errorProvider = errorProvider;
			controls = new List<SWF.Control> ();
		}

		#endregion
		
		#region Public Properties
		
		public SWF.ErrorProvider SWFErrorProvider {
			get { return errorProvider; }
		}
	
		public SWF.Control Parent {
			get { return parent; }
		}

		#endregion
		
		#region Public Methods
		
		public void AddControl (SWF.Control control)
		{
			if (parent == null)
				parent = InstancesTracker.GetParentFromControl (control);

			if (controls.Contains (control) == false) {
				controls.Add (control);
				if (controls.Count == 1 && control.Visible == true) {
					FragmentRootControlProvider root 
						= (FragmentRootControlProvider) ProviderFactory.GetProvider (Parent);
					root.AddChildProvider (true, this);
				}
				control.VisibleChanged += new EventHandler (OnControlVisibleChanged);
			}
		}
		
		public void DeleteControl (SWF.Control control)
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
		
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return GetBoundingRectangle ();
			else
				return base.GetProviderPropertyValue (propertyId);
		}

		#endregion

		#region Private Methods
		
		private Rect GetBoundingRectangle ()
		{
			List<SWF.Control> controls 
				= InstancesTracker.GetControlsFromProvider (this);
			
			if (controls == null)
				return Rect.Empty;
			
			SD.Rectangle bounding = controls [0].Bounds;
			for (int i = 1; i < controls.Count; i++)
				bounding = SD.Rectangle.Union (bounding, controls [i].Bounds);

			return Helper.RectangleToRect (bounding);
		}
		
		private void OnControlVisibleChanged (object sender, EventArgs args)
		{
			SWF.Control control = (SWF.Control) sender;

			if (control.Visible == true) {
				FragmentRootControlProvider root 
					= (FragmentRootControlProvider) ProviderFactory.GetProvider (Parent);
				root.AddChildProvider (true, this);
			} else
				DeleteControl (control);
		}		
		
		#endregion
		
		#region Private Fields
		
		private List<SWF.Control> controls;
		private SWF.Control parent;
		private SWF.ErrorProvider errorProvider;
		
		#endregion
		
		#region Internal Class: ToolTipProvider
		
		internal class ErrorProviderToolTipProvider : ToolTipBaseProvider
		{
			public ErrorProviderToolTipProvider (SWF.ErrorProvider provider) 
				: base (provider)
			{
				errorProvider = provider;
			}

			protected override Rect GetBoundingRectangle ()
			{
				return Helper.RectangleToRect (Helper.GetPrivateProperty<SWF.ErrorProvider, SD.Rectangle> (typeof (SWF.ErrorProvider),
				                                                                                          errorProvider,
				                                                                                          "UIAToolTipRectangle"));
			}

			protected override string GetTextFromControl (SWF.Control control)
			{
				return errorProvider.GetError (control);
			}
		
			private SWF.ErrorProvider errorProvider;
		}
		
		#endregion
		
		#region Internal Class: InstancesTracker
		
		// NOTE: 
		//      Class used to keep track of UserControl instances added by 
		//      SWF.ErrorProvider.
		internal static class InstancesTracker
		{		
			public static SWF.Control GetParentFromControl (SWF.Control control)
			{
				if (dictionary == null)
					return null;
				
				if (control.Parent == null) {
					foreach (KeyValuePair<SWF.Control, ErrorProviderControlList>
					         value in parentDictionary) {
						if (value.Value.Contains (control) == true)
							return value.Key;
					}
				} 
				
				return control.Parent;
			}
			
			public static void AddControl (SWF.Control control,
			                               SWF.Control parent,
			                               SWF.ErrorProvider provider)
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
				if (parentControls.Contains (control) == false) {
					parentControls.Add (control);
					ErrorProvider errorProvider 
						= (ErrorProvider) ProviderFactory.FindProvider (provider);
					if (errorProvider == null)
						errorProvider = (ErrorProvider) ProviderFactory.GetProvider (provider);
					errorProvider.AddControl (control);
				}
			}
			
			public static void RemoveControl (SWF.Control control, 
			                                  SWF.Control parent, 
			                                  SWF.ErrorProvider provider)
			{
				if (dictionary == null 
				    || dictionary.ContainsKey (control) == false)
					return;

				parentDictionary [parent].Remove (control);
				dictionary.Remove (control);	
			}			
			
			public static SWF.ErrorProvider GetErrorProviderFromControl (SWF.Control control)
			{
				if (dictionary == null)
					return null;
				
				SWF.ErrorProvider provider = null;
				dictionary.TryGetValue (control, out provider);
				return provider;
			}
			
			public static List<SWF.Control> GetControlsFromProvider (ErrorProvider errorProvider)
			{
				FragmentRootControlProvider parent 
					= (FragmentRootControlProvider) errorProvider.Navigate (NavigateDirection.Parent);
				if (parent == null)
					return null;

				if (parentDictionary == null
				    || parentDictionary.ContainsKey (parent.Control) == false)
					return null;
				
				ErrorProviderControlList parentControls = parentDictionary [parent.Control];
				
				List<SWF.Control> controls =
					(from c in parentControls
					where dictionary [c] == errorProvider.SWFErrorProvider
					select c).ToList ();
				
				return controls;
			}

			public static bool IsControlFromErrorProvider (SWF.Control control) 
			{
				if (dictionary == null)
					return false;
				else
					return dictionary.ContainsKey (control);
			}
			
			public static bool IsFirstControlFromErrorProvider (SWF.Control control)
			{
				if (IsControlFromErrorProvider (control) == false)
					return false;
				
				IEnumerable<SWF.Control> controls =
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
		
		class ErrorProviderControlList : List<SWF.Control>
		{
		}	
		
		class ParentDictionary : Dictionary<SWF.Control, ErrorProviderControlList>
		{
		}
		
		class ErrorProviderControlDictionary : Dictionary<SWF.Control, SWF.ErrorProvider>
		{
		}

		#endregion

	}
}
