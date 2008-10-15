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
//      Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;

namespace Mono.UIAutomation.Winforms.Behaviors.Form
{

	internal class TransformProviderBehavior 
		: ProviderBehavior, ITransformProvider
	{
		#region Constructors
		
		public TransformProviderBehavior (FormProvider provider)
			: base (provider) 
		{
			form = (SWF.Form) provider.Control;
		}
		
		#endregion

		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern {
			get { return TransformPatternIdentifiers.Pattern; }
		}		
		
		public override void Connect () 
		{
			//FIXME: Automation Events not generated
		}
		
		public override void Disconnect ()
		{
			//FIXME: Automation Events not generated
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == TransformPatternIdentifiers.CanMoveProperty.Id)
				return CanMove;
			else if (propertyId == TransformPatternIdentifiers.CanResizeProperty.Id)
				return CanResize;
			else if (propertyId == TransformPatternIdentifiers.CanRotateProperty.Id)
				return CanRotate;
			else
				return base.GetPropertyValue (propertyId);
		}

		#endregion	
		
		#region ITransformProvider Members
	
		public bool CanMove {
			get {
				return form.WindowState == SWF.FormWindowState.Normal;
			}
		}

		public bool CanResize {
			get {
				switch (form.FormBorderStyle) {
				case SWF.FormBorderStyle.Fixed3D:
				case SWF.FormBorderStyle.FixedDialog:
				case SWF.FormBorderStyle.FixedSingle:
				case SWF.FormBorderStyle.FixedToolWindow:
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
			// TODO: Test Vista behavior to see if any
			//       exceptions are thrown with bad input
			if (!CanMove)
				throw new InvalidOperationException ("CanMove is false");
			
			if (form.InvokeRequired == true) {
				form.BeginInvoke (new PerformTransformDelegate (Move),
				                  new object [] { x, y});
				return;
			}
			
			form.Location = new Point ((int) x, (int) y);
		}

		public void Resize (double width, double height)
		{
			// TODO: Test Vista behavior to see if any
			//       exceptions are thrown with bad input
			if (!CanResize)
				throw new InvalidOperationException ("CanResize is false");
			
			if (form.InvokeRequired == true) {
				form.BeginInvoke (new PerformTransformDelegate (Resize),
				                  new object [] { width, height});
				return;
			}
			
			form.Size = new Size ((int) width, (int) height);
		}

		public void Rotate (double degrees)
		{
			throw new InvalidOperationException ("Cannot rotate a Form");
		}

		#endregion
		
		#region Private Fields
		
		private SWF.Form form;
		
		#endregion
		
	}
	
	delegate void PerformTransformDelegate (double width, double height);

}
