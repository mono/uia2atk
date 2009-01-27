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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Drawing;
using Mono.UIAutomation.Bridge;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.PictureBox
{
	internal class EmbeddedImageProviderBehavior
		: ProviderBehavior, IEmbeddedImageProvider
	{
#region Constructors
		public EmbeddedImageProviderBehavior (PictureBoxProvider provider)
			: base (provider)
		{
			control = (SWF.PictureBox)provider.Control;
		}
#endregion
		
#region IProviderBehavior Interface
		public override AutomationPattern ProviderPattern { 
			get { return EmbeddedImagePatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
		}

		public override void Disconnect ()
		{
		}
#endregion

#region IEmbeddedImageProvider Interface
		public Rect Bounds {
			get {
				if (control.Image == null)
					return Rect.Empty;
				GraphicsUnit unit = GraphicsUnit.Pixel;
				RectangleF rectF = control.Image.GetBounds (ref unit);
				if (unit != GraphicsUnit.Pixel)
					return Rect.Empty;

				System.Windows.Rect ret
					= (System.Windows.Rect) Provider.GetPropertyValue (
						AutomationElementIdentifiers.BoundingRectangleProperty.Id);
				ret.X += rectF.X;
				ret.Y += rectF.Y;
				ret.Width = rectF.Width;
				ret.Height = rectF.Height;
				return ret;
			}
		}

		public string Description {
			get { return String.Empty; }
		}
#endregion

#region Private Fields
		private SWF.PictureBox control;
#endregion
	}
}
