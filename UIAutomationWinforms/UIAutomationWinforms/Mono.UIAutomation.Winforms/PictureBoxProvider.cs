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
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Mono.UIAutomation.Bridge;

namespace Mono.UIAutomation.Winforms
{

	internal class PictureBoxProvider : PaneProvider, IEmbeddedImage
	{
		
		#region Constructors

		private PictureBox control;

		public PictureBoxProvider (PictureBox pictureBox) : base (pictureBox)
		{
			control = pictureBox;
		}
		
		#endregion
		
		#region SimpleControlProvider: Specialization
		
		public override Component Container  {
			get { return Control.Parent; }
		}

		#endregion

		#region IEmbeddedImage Members
		public Rect Bounds {
			get {
				if (control.Image == null)
					return Rect.Empty;
				GraphicsUnit unit = GraphicsUnit.Pixel;
				RectangleF rectF = control.Image.GetBounds (ref unit);
				if (unit != GraphicsUnit.Pixel)
					return Rect.Empty;
				Rect ret = BoundingRectangle;
				ret.X += rectF.X;
				ret.Y += rectF.Y;
				ret.Width = rectF.Width;
				ret.Height = rectF.Height;
				return ret;
			}
		}

		public bool HasImage {
			get { return !Bounds.Equals (System.Windows.Rect.Empty); }
		}

		public string Description {
			get { return string.Empty; }
		}
		#endregion
		
		#region SimpleControlProvider specialization

		// TODO: In Vista the PictureBox is defined as Pane Provider
		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Image.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "image";
			else
				return base.GetProviderPropertyValue (propertyId);
		}		
		#endregion
	}
}
