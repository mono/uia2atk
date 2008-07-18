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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	internal class ErrorProviderProvider : PaneProvider
	{
		
		#region Constructor
		
		public ErrorProviderProvider (ErrorProvider errorProvider) : base (null)
		{
			this.error_provider = errorProvider;
			controls = new List<Control> ();
		}
		
		#endregion
		
		#region PaneProvider: Specializations
		
		public override void InitializeEvents ()
		{
			base.InitializeEvents ();
			
			//TODO: Add delegates whe SetError and ClearError
		}
		
		public override void Terminate ()
		{
			base.Terminate ();

			controls.Clear ();
		}
		
		#endregion
		
		#region Private Methods
		
		private void OnErrorSet (object sender, Control control)
		{
			if (controls.Contains (control) == false) {
				controls.Add (control);
				if (controls.Count == 1)
					Helper.RaiseStructureChangedEvent (StructureChangeType.ChildAdded,
					                                   this);
			}

		}
		
		private void OnErrorClear (object sender, Control control)
		{
			controls.Clear ();
		}
		
		#endregion
		
		#region Private Fields
		
		private List<Control> controls;
		private ErrorProvider error_provider;
		
		#endregion

	}
}
