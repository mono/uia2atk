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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows;
using Mono.UIAutomation.Bridge;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.BalloonWindow
{
	internal class EmbeddedImageProviderBehavior
		: ProviderBehavior, IEmbeddedImageProvider
	{
		private SWF.NotifyIcon.BalloonWindow balloonWindow;

#region Constructors
		public EmbeddedImageProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}
#endregion
		
#region IProviderBehavior Interface
		public override AutomationPattern ProviderPattern { 
			get { return EmbeddedImagePatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
			balloonWindow = (SWF.NotifyIcon.BalloonWindow)Provider.Control;
		}
		
		public override void Disconnect ()
		{
		}
#endregion

#region IEmbeddedImageProvider Interface
		public System.Windows.Rect Bounds {
			get {
				// from ThemeWin32Classic.cs
				int borderSize = 8;
				int iconSize = (balloonWindow.Icon == SWF.ToolTipIcon.None) ? 0 : 16;
				if (iconSize == 0)
					return Rect.Empty;
				int imageX, imageY, imageWidth, imageHeight;

				Rect balloonRect 
					= (Rect) Provider.GetPropertyValue (AutomationElementIdentifiers.BoundingRectangleProperty.Id);
			
				imageX = (int) balloonRect.X + borderSize;
				imageY = (int) balloonRect.Y + borderSize;
				imageWidth = iconSize;
				imageHeight = iconSize;

				Rect imageRect = new Rect (imageX, imageY, imageWidth, imageHeight);
				balloonRect.Intersect (imageRect);
				return balloonRect;
			}
		}

		public string Description {
			get { return String.Empty; }
		}
#endregion
	}
}
