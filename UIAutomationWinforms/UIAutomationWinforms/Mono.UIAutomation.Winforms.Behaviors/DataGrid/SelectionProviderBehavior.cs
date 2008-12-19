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
using Mono.UIAutomation.Winforms.Events.DataGrid;

namespace Mono.UIAutomation.Winforms.Behaviors.DataGrid
{

	internal class SelectionProviderBehavior 
		: ProviderBehavior, ISelectionProvider
	{
		
		#region Constructors

		public SelectionProviderBehavior (DataGridProvider provider)
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
			//NOTE: CanSelect Property NEVER changes, so we aren't generating it.
			Provider.SetEvent (ProviderEventType.SelectionPatternInvalidatedEvent,
			                   new SelectionPatternInvalidatedEvent ((DataGridProvider) Provider));
			Provider.SetEvent (ProviderEventType.SelectionPatternSelectionProperty,
			                   new SelectionPatternSelectionEvent ((DataGridProvider) Provider));
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
			get { return false; }
		}
		
		public IRawElementProviderSimple[] GetSelection ()
		{
			return ((DataGridProvider) Provider).GetSelectedItems ();
		}

		#endregion
	}
}
