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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	//TODO: Handle ToolTip
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

		#endregion
		
		#region PaneProvider: Specializations
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return GetBoundingRectangle ();
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion
		
		#region Private Methods
		
		private object GetBoundingRectangle ()
		{
			List<Control> controls = ErrorProviderProvider.InstancesTracker.GetControls (errorProvider.ContainerControl);
			
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
		internal sealed class InstancesTracker 
		{
			private InstancesTracker ()
			{
			}
			
			public static List<Control> GetControls (Control parent)
			{
				List<Control> list;
				controls.TryGetValue (parent, out list);
				
				return list;
			}
			
			public static void RemoveUserControlsFromParent (Control parent)
			{
				if (controls == null)
					return;

				controls.Remove (parent);
			}
			
			public static bool IsControlFromErrorProvider (Control control) 
			{
				if (controls == null || controls.ContainsKey (control.Parent) == false)
					return false;
				else
					return true;
			}
			
			public static void AddControl (Control control, Control parent)
			{
				if (controls == null)
					controls = new Dictionary<Control, List<Control>> ();
				
				List<Control> list;
				if (controls.TryGetValue (parent, out list) == false) {
					list = new List<Control> ();
					controls [parent] = list;
				}

				if (list.Contains (control) == false)
					list.Add (control);
			}
			
			public static void RemoveControl (Control control)
			{
				if (controls == null)
					return;
				
				List<Control> list;
				if (controls.TryGetValue (control.Parent, out list) == false)
					return;
				
				list.Remove (control);
				if (list.Count == 0)
					controls.Remove (control.Parent);
			}
			
			private static Dictionary<Control, List<Control>> controls;
		}
		
		#endregion

	}
}
