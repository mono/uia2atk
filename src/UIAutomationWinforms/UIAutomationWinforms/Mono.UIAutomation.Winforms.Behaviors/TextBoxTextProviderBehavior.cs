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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Behaviors
{

	internal class TextBoxTextProviderBehavior 
		: ProviderBehavior, ITextProvider
	{
		
		#region Constructor
		
		public TextBoxTextProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}

		#endregion
		
		#region ProviderBehavior: Specialization
		
		public override AutomationPattern ProviderPattern { 
			get { return TextPatternIdentifiers.Pattern; }
		}
		
		public override void Connect (Control control)
		{
		}
		
		public override void Disconnect (Control control)
		{
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id) {
				TextBox textbox = Provider.Control as TextBox;
				if (textbox != null)
					return (textbox.UseSystemPasswordChar || (int) textbox.PasswordChar != 0);
				else
					return null;
			} else
					return base.GetPropertyValue (propertyId);
		}
		
		
		#endregion

		#region ITextProvider Members
		
		//TODO: We should connect the events to update this.text_range_provider?
		public ITextRangeProvider DocumentRange {
			get { 
				if (textRangeProvider == null)
					textRangeProvider = new TextRangeProvider (this, 
					                                             (TextBoxBase) Provider.Control); 
				return textRangeProvider;
			}
		}
		
		public SupportedTextSelection SupportedTextSelection {
			get { return SupportedTextSelection.Single; }
		}

		public ITextRangeProvider[] GetSelection ()
		{
			if (SupportedTextSelection == SupportedTextSelection.None)
				throw new InvalidOperationException ();
				
			//TODO: Return null when system cursor is not present, how to?

			return new ITextRangeProvider [] { 
				new TextRangeProvider (this, (TextBoxBase) Provider.Control) };
		}
		
		public ITextRangeProvider[] GetVisibleRanges ()
		{
			throw new NotImplementedException ();
		}
		
		public ITextRangeProvider RangeFromChild (IRawElementProviderSimple childElement) 
		{
			throw new NotImplementedException ();
		}
		
		public ITextRangeProvider RangeFromPoint (Point screenLocation)
		{
			throw new NotImplementedException ();
		}
		
		#endregion
		
		
		#region Private section
		
		private ITextRangeProvider textRangeProvider;
		
		#endregion		
		
	}
}
