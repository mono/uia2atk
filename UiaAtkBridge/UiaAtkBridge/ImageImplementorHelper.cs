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
//	Andrés G. Aragoneses <aaragoneses@novell.com>
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows;
using Mono.UIAutomation.Bridge;

namespace UiaAtkBridge
{
	
	public class ImageImplementorHelper
	{
		
		public ImageImplementorHelper (Adapter adapter, object implementerProvider)
		{
			this.adapter = adapter;
			this.implementerProvider = implementerProvider;
		}

		public bool HasImage {
			get {
				if (hasImage == null) {
					//type only available in our Provider implementation
					embeddedImage = implementerProvider as IEmbeddedImage;
					
					if (embeddedImage == null) {
						Console.WriteLine ("WARNING: your provider implementation doesn't have unofficial IEmbeddedImage support");
						hasImage = false;
					} else
						hasImage = !embeddedImage.Bounds.IsEmpty;
				}
				
				return hasImage.Value;
			}
		}
		
		public string ImageDescription {
			get { return imageDescription; }
		}

		/* FIXME: Returned value is OK? */ 
		public string ImageLocale {
			get { return null; }
		}

		public void GetImagePosition (out int x, out int y, Atk.CoordType coordType)
		{
			if (HasImage) {
				x = (int) embeddedImage.Bounds.X;
				y = (int) embeddedImage.Bounds.Y;
				if (coordType == Atk.CoordType.Window)
					adapter.ConvertCoords (ref x, ref y, false);
			} else {
				x = int.MinValue;
				y = int.MinValue;
			}
		}
		
		public void GetImageSize (out int width, out int height)
		{
			if (HasImage) {
				width = (int) embeddedImage.Bounds.Width;
				height = (int) embeddedImage.Bounds.Height;
			} else
				width = height = -1;
		}
		
		public bool SetImageDescription (string description)
		{
			if (HasImage) {
				imageDescription = description;
				return true;
			}

			return false;
		}

		private Adapter adapter;
		private bool? hasImage;		
		private string imageDescription;
		private object implementerProvider;
		private IEmbeddedImage embeddedImage;
	}
}

