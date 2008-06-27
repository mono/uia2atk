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
using Mono.UIAutomation.Winforms.Behaviors;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{

	public class ScrollBarProvider : FragmentControlProvider
	{
		
#region Constructor

		public ScrollBarProvider (ScrollBar scrollbar) : base (scrollbar)
		{
			scrollbar.ParentChanged += new EventHandler (OnParentChanged);
			
			Navigation = new ScrollBarNavigation (this);
		}

#endregion

#region Public Methods
		
		public override void InitializeEvents ()
		{
			base.InitializeEvents (); 

			SetEvent (ProviderEventType.FocusChangedEvent,
			          new AutomationFocusChangedEvent (this));
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.ScrollBar.Id;
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "scroll bar";
			else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
				return Single.NaN;
			else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
				return null;			
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return false;
			else if (propertyId == AutomationElementIdentifiers.OrientationProperty.Id)
				return true;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return null;
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion

#region Private Methods
		
		private void OnParentChanged (object sender, EventArgs args)
		{	
			IRawElementProviderFragment container 
				= ProviderFactory.FindProvider (((Control) sender).Parent);

			if (container != null) {
				IScrollProvider provider 
					= (IScrollProvider) container.GetPatternProvider (ScrollPatternIdentifiers.Pattern.Id);
				if (provider == null)
					SetBehavior (RangeValuePatternIdentifiers.Pattern,
					             new ScrollBarRangeValueBehavior (this));
				else
					SetBehavior (RangeValuePatternIdentifiers.Pattern, null);
			} else
				//TODO: Is this default behavior OK?
				SetBehavior (RangeValuePatternIdentifiers.Pattern,
				             new ScrollBarRangeValueBehavior (this));
		}

#endregion
		
	}
}
