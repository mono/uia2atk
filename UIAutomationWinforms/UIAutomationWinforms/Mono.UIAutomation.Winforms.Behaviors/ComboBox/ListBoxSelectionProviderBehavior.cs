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
	internal class ListBoxSelectionProviderBehavior 
		: SelectionProviderBehavior
	{
		
		#region Constructors

		public ListBoxSelectionProviderBehavior (ComboBoxProvider.ComboBoxListBoxProvider listBoxProvider,
		                                         ComboBoxProvider provider)
			: base (listBoxProvider)
		{
			this.provider = provider;
		}
		
		#endregion

		#region IProviderBehavior Interface		
		
		public override AutomationPattern ProviderPattern { 
			get { return SelectionPatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
			//NOTE: CanSelectMultiple Property NEVER changes, so we aren't generating it.
			Provider.SetEvent (ProviderEventType.SelectionPatternInvalidatedEvent,
			                   new SelectionPatternInvalidatedEvent (provider));
			Provider.SetEvent (ProviderEventType.SelectionPatternIsSelectionRequiredProperty,
			                   new SelectionPatternIsSelectionRequiredEvent (provider));
			Provider.SetEvent (ProviderEventType.SelectionPatternSelectionProperty,
			                   new SelectionPatternSelectionEvent (provider));
		}
		
		
		#endregion
		
		#region ISelectionProvider Members

		public override bool IsSelectionRequired {
			get { return ((SWF.ComboBox) provider.Control).SelectedIndex != -1; }
		}
		
		public override IRawElementProviderSimple[] GetSelection ()
		{
			return ((ListProvider) Provider).GetSelectedItems ();
		}

		#endregion

		#region Private Fields
		
		private ComboBoxProvider provider;
		
		#endregion
	}
}
