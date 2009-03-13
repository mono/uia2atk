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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows.Automation;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Winforms.Events;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms.Behaviors.MaskedTextBox
{
	internal class InsertDeleteTextProviderBehavior
		: ProviderBehavior, IInsertDeleteTextProvider
	{
#region Constructor
		public InsertDeleteTextProviderBehavior (TextBoxProvider provider)
			: base (provider)
		{
			this.maskedTextBox = (SWF.MaskedTextBox) Provider.Control;
		}
#endregion
		
#region ProviderBehavior: Specialization
		public override AutomationPattern ProviderPattern { 
			get { return InsertDeleteTextPatternIdentifiers.Pattern; }
		}

		public override void Connect ()
		{
		}

		public override void Disconnect ()
		{
		}
#endregion

#region IInsertDeleteTextProvider: Specialization
		// This is kind of crack.  Basically, in order to emulate what
		// happens in the GUI, InsertText will actually overwrite
		// characters in the DisplayString as long as they're
		// placeholders.  If we just inserted as usual, it would be
		// rejected as it contains too many characters.
		public void InsertText (string str, ref int position)
		{
			if (maskedTextBox.ReadOnly)
				throw new ElementNotEnabledException ();

			if (Provider.Control.InvokeRequired) {
				object[] args = new object[] { str, position };
				Provider.Control.BeginInvoke (new InsertTextDelegate (InsertText), args);
				position = (int) args[1];
				return;
			}

			System.ComponentModel.MaskedTextProvider prov
				= maskedTextBox.MaskedTextProvider;

			string text = prov.ToDisplayString ();

			int offset = position;
			for (int i = 0; i < str.Length; i++) {
				int localOffset = offset;
				
				// skip over an unavailable slot if we see one
				bool reachedBounds = false;
				while (!prov.IsEditPosition (i + localOffset)) {
					localOffset++;
					if (i + localOffset >= prov.Length) {
						reachedBounds = true;
						break;
					}
				}

				if (reachedBounds)
					break;

				// There is already a character here, so while
				// the GUI will actually move this character
				// over, we cannot as there are _many, many_
				// edge cases that we cannot handle.
				// Instead, we skip over the character.
				if (!prov.IsAvailablePosition (i + localOffset)) {
					// Make sure offset is correct for the
					// next iteration
					offset = localOffset;
					continue;
				}

				// The correct value of position is equal to
				// the localOffset of the first character.
				//
				// This check needs to be done after
				// IsAvailablePosition in case we are trying to
				// insert into a place that already has a
				// character.
				if (offset != localOffset && i == 0)
					position = localOffset;
				
				offset = localOffset;

				text = text.Substring (0, i + offset)
					+ str[i] + text.Substring (i + offset + 1);
			}

			maskedTextBox.Text = text;
		}

		public void DeleteText (int start, int end)
		{
			if (maskedTextBox.ReadOnly)
				throw new ElementNotEnabledException ();

			if (Provider.Control.InvokeRequired) {
				Provider.Control.BeginInvoke (new DeleteTextDelegate (DeleteText),
				                              new object [] {start, end});
				return;
			}

			// MaskedTextProvider.RemoveAt does not work for this.
			System.ComponentModel.MaskedTextProvider prov
				= maskedTextBox.MaskedTextProvider;

			string text = prov.ToDisplayString ();
			for (int i = start; i < end; i++) {
				if (!prov.IsEditPosition (i)
				    || prov.IsAvailablePosition (i))
					continue;

				text = text.Substring (0, i) + prov.PromptChar
					+ text.Substring (i + 1);
			}

			maskedTextBox.Text = text;
		}
#endregion

#region Private Delegates
		private delegate void InsertTextDelegate (string str, ref int position);
		private delegate void DeleteTextDelegate (int start, int end);
#endregion

#region Private Fields
		private SWF.MaskedTextBox maskedTextBox;
#endregion
	}
}
