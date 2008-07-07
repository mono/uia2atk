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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Behaviors;

namespace Mono.UIAutomation.Winforms
{

	internal class ScrollBarButtonProvider : FragmentControlProvider
	{

#region Constructors
		
		public ScrollBarButtonProvider (ScrollBar scrollbar_container,
		                                ScrollBarButtonOrientation orientation)
			: base (scrollbar_container) 
		{
			this.orientation = orientation;
			this.scrollbar_container = scrollbar_container;
			
			SetBehavior (InvokePatternIdentifiers.Pattern, 
			             new ScrollBarButtonInvokeProviderBehavior (this));
		}

#endregion
		
#region Public Properties

		public ScrollBarButtonOrientation Orientation {
			get { return orientation; }
		}
		
		public ScrollBar ScrollBarContainer {
			get { return scrollbar_container; }
		}

#endregion
		
#region Public Methods
		
		public override object GetPropertyValue (int propertyId)
		{
			//TODO: We may need to get VALID information using Reflection from 
			//ScrollBarContainer and return those values, I'm thiking in the
			//following propierties: BoundingRectangleProperty and ClickablePointProperty
			if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return GetNameFromOrientation ();
			else
				return base.GetPropertyValue (propertyId);
		}
		
#endregion
		
#region Private Methods
		
		private string GetNameFromOrientation ()
		{
			//TODO: Should we use generalization?
			//TODO: Should this be translatable?
			if (orientation == ScrollBarButtonOrientation.LargeBack)
				return "Back by large amount";
			else if (orientation == ScrollBarButtonOrientation.LargeForward)
				return "Forward by large amount";
			else if (orientation == ScrollBarButtonOrientation.SmallBack)
				return "Back by small amount";
			else //Should be ScrollBarButtonOrientation.SmallForward
				return "Forward by small amount";
		}
		
#endregion
		
#region Private Fields
		
		private ScrollBarButtonOrientation orientation;
		private ScrollBar scrollbar_container;

#endregion

	}
}
