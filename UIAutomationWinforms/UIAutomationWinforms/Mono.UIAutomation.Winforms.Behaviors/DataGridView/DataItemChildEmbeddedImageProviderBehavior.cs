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
//	Mario Carrion <mcarrion@novell.com>
// 
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGridView
{
	internal class DataItemChildEmbeddedImageProviderBehavior
		: ProviderBehavior, IEmbeddedImageProvider
	{
		
		#region Constructors

		public DataItemChildEmbeddedImageProviderBehavior (DataGridViewProvider.DataGridViewDataItemImageProvider provider)
			: base (provider)
		{
			this.provider = provider;
		}

		#endregion
		

		#region IProviderBehavior Interface

		public override AutomationPattern ProviderPattern { 
			get { return EmbeddedImagePatternIdentifiers.Pattern; }
		}

		public override void Disconnect ()
		{
		}

		public override void Connect ()
		{

		}

		#endregion

		#region IEmbeddedImageProvider Interface

		public Rect Bounds {
			get {
				Rect imageRect = Helper.RectangleToRect (provider.ImageCell.ContentBounds);
				imageRect.X += provider.ItemProvider.BoundingRectangle.X;
				imageRect.Y += provider.ItemProvider.BoundingRectangle.Y;
				
				return imageRect;
			}
		}

		public string Description {
			get { return string.Empty; }
		}
		
		#endregion

		#region Private Fields

		private DataGridViewProvider.DataGridViewDataItemImageProvider provider;

		#endregion

	}
}
