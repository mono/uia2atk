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
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Events.TabControl;

namespace Mono.UIAutomation.Winforms.Behaviors.TabControl
{
	internal class SelectionProviderBehavior
		: ProviderBehavior, ISelectionProvider
	{
#region Public Methods
		public SelectionProviderBehavior (TabControlProvider provider)
			: base (provider)
		{
		}
#endregion
		
#region IProviderBehavior Interface		
		public override AutomationPattern ProviderPattern { 
			get { return SelectionPatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
			//NOTE: IsSelectionRequired Property NEVER changes, so we aren't generating it.
			//NOTE: CanSelectMultiple Property NEVER changes, so we aren't generating it.

			Provider.SetEvent (ProviderEventType.SelectionPatternInvalidatedEvent,
			                   new SelectionPatternInvalidatedEvent ((TabControlProvider) Provider));
			Provider.SetEvent (ProviderEventType.SelectionPatternSelectionProperty,
			                   new SelectionPatternSelectionEvent ((TabControlProvider) Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.SelectionPatternCanSelectMultipleProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionPatternInvalidatedEvent,
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionPatternIsSelectionRequiredProperty,
			                   null);
			Provider.SetEvent (ProviderEventType.SelectionPatternSelectionProperty,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == SelectionPatternIdentifiers.CanSelectMultipleProperty.Id) {
				return CanSelectMultiple;
			} else if (propertyId == SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id) {
				return IsSelectionRequired;
			} else if (propertyId == SelectionPatternIdentifiers.SelectionProperty.Id) {
				return GetSelection ();
			}
			return null;
		}
#endregion
		
#region ISelectionProvider Implementation

		public bool CanSelectMultiple {
			get { return false; }
		}

		public bool IsSelectionRequired {
			get { return true; }
		}

		public IRawElementProviderSimple[] GetSelection ()
		{
			TabPageProvider prov
				= ((TabControlProvider) Provider).GetSelectedTab ();
			if (prov == null) {
				return new IRawElementProviderSimple[0];
			}
			
			return new IRawElementProviderSimple[] { prov };
		}

#endregion
	}
}
