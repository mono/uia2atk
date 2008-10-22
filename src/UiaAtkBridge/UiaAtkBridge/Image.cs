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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	public class Image : ComponentAdapter, Atk.ImageImplementor
	{
		private IRawElementProviderSimple	provider;

		bool? hasImage = null;
		Mono.UIAutomation.Bridge.IEmbeddedImage embeddedImage = null;
		protected object imageProvider = null;

		public Image (IRawElementProviderSimple provider)
		{
			this.provider = provider;
			this.imageProvider = provider;
			
			// Perhaps we should use Atk.Role.Image instead,
			// but gail uses Atk.Role.Icon
			Role = Atk.Role.Icon;
			
			Name = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
		}
		
		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			return states;
		}

		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

#region ImageImplementor implementation 

		string imageDescription = null;

		private bool HasImage {
			get {
				if (hasImage == null) {
					//type only available in our Provider implementation
					embeddedImage = imageProvider as Mono.UIAutomation.Bridge.IEmbeddedImage;
					
					if (embeddedImage == null) {
						Console.WriteLine ("WARNING: your provider implementation doesn't have unofficial IEmbeddedImage support");
						hasImage = false;
					}
					else
						hasImage = !embeddedImage.BoundingRectangle.IsEmpty;
				}
				
				return hasImage.Value;
			}
		}
		
		public string ImageDescription
		{
			get { return imageDescription; }
		}
		
		public void GetImageSize (out int width, out int height)
		{
			width = -1;
			height = -1;
			if (HasImage) {
				width = (int)embeddedImage.BoundingRectangle.Width;
				height = (int)embeddedImage.BoundingRectangle.Height;
			}
		}
		
		public void GetImagePosition (out int x, out int y, Atk.CoordType coordType)
		{
			x = int.MinValue;
			y = int.MinValue;
			if (HasImage) {
				x = (int)embeddedImage.BoundingRectangle.X;
				y = (int)embeddedImage.BoundingRectangle.Y;
			}
		}
		
		public bool SetImageDescription (string description)
		{
			if (HasImage) {
				imageDescription = description;
				return true;
			}
			return false;
		}
		
		public string ImageLocale 
		{
			get { return imageDescription; /*TODO*/ }
		}
		
#endregion


	}
}
