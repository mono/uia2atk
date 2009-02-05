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
// 

using System;
using System.Windows;
using Mono.UIAutomation.Bridge;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.ToolBar
{
	internal class ToolBarButtonEmbeddedImageProviderBehavior
		: ProviderBehavior, IEmbeddedImageProvider
	{
#region Constructors
		public ToolBarButtonEmbeddedImageProviderBehavior (FragmentControlProvider provider)
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
		}
		
		public override void Disconnect ()
		{
		}
#endregion

#region IEmbeddedImageProvider Interface
		public System.Windows.Rect Bounds {
			get {
//				SWF.ToolBarItem item = null;
//				
//				SWF.ToolBarButton butt = ((SWF.ToolBarButton)Provider.Component);
//				if (butt.ImageIndex < 0)
//					return Rect.Empty;
//				
//				foreach (SWF.ToolBarItem it in butt.Parent.items)
//					if (it != null && it.Button == butt)
//						item = it;
//				
//				return Helper.RectangleToRect (item.ImageRectangle);
				return Helper.GetToolBarButtonImageBounds (
					Provider, ((SWF.ToolBarButton)Provider.Component));
			}
		}

		public string Description {
			get { return String.Empty; }
		}
#endregion
	}
}
