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
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.ComboBox;

namespace Mono.UIAutomation.Winforms.Behaviors.ComboBox
{
	//NOTE: This class is used by both ComboBox and ComboBox.ListBox providers.
	internal class SelectionProviderBehavior 
		: ProviderBehavior, ISelectionProvider
	{
		
		#region Constructors

		public SelectionProviderBehavior (ListProvider provider)
			: base (provider)
		{
		}
		
		#endregion

		#region IProviderBehavior Interface		
		
		public override AutomationPattern ProviderPattern { 
			get { return SelectionPatternIdentifiers.Pattern; }
		}

		public override void Connect (SWF.Control control)
		{
			//NOTE: CanSelectMultiple Property NEVER changes, so we aren't generating it.
			Provider.SetEvent (ProviderEventType.SelectionPatternInvalidatedEvent,
			                   new SelectionPatternInvalidatedEvent ((ListProvider) Provider));
			Provider.SetEvent (ProviderEventType.SelectionPatternIsSelectionRequiredProperty,
			                   new SelectionPatternIsSelectionRequiredEvent ((ListProvider) Provider));
			Provider.SetEvent (ProviderEventType.SelectionPatternSelectionProperty,
			                   new SelectionPatternSelectionEvent ((ListProvider) Provider));
		}
		
		public override void Disconnect (SWF.Control control)
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
			if (propertyId == SelectionPatternIdentifiers.CanSelectMultipleProperty.Id)
				return CanSelectMultiple;
			else if (propertyId == SelectionPatternIdentifiers.IsSelectionRequiredProperty.Id)
				return IsSelectionRequired;
			else if (propertyId == SelectionPatternIdentifiers.SelectionProperty.Id)
				return GetSelection ();
			else
				return null;
		}
		
		#endregion
		
		#region ISelectionProvider Members

		public bool CanSelectMultiple {
			get { return false; }
		}

		public bool IsSelectionRequired {
			get { return ((SWF.ComboBox) Provider.Control).SelectedIndex != -1; }
		}
		
		public IRawElementProviderSimple[] GetSelection ()
		{
			return ((ListProvider) Provider).GetSelectedItemsProviders ();
		}

		#endregion
	}
}
