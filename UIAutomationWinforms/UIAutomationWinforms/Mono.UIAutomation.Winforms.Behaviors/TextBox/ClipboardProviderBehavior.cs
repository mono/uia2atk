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
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Behaviors.TextBox
{
	internal class ClipboardProviderBehavior 
		: ProviderBehavior, IClipboardProvider
	{
		#region Constructor
		
		public ClipboardProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}

		#endregion
		
		#region ProviderBehavior: Specialization
		
		public override AutomationPattern ProviderPattern { 
			get { return ClipboardPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
		}
		
		public override void Disconnect ()
		{
		}
		
		#endregion

		#region IClipboardSupport Implementation	

		public void Copy (int start, int end)
		{
			string text = Text;
			start = (int) System.Math.Max (start, 0);
			end = (int) System.Math.Min (end, text.Length);
			SWF.Clipboard.SetText (text.Substring (start, end - start));
		}
		
		public void Paste (int position)
		{
			string text = Text;
			position = (int) System.Math.Max (position, 0);
			position = (int) System.Math.Min (position, text.Length);

			// If you were to paste using the GUI, it would only
			// paste enough until the control was full, so emulate
			// that behavior.
			int maxLength = 0;
			if (Provider is TextBoxProvider)
				maxLength = ((TextBoxProvider) Provider).MaxLength;

			string clipboardText = SWF.Clipboard.GetText ();
			if (maxLength > 0 && clipboardText.Length > (maxLength - position))
				clipboardText = clipboardText.Substring (0, maxLength - position);

			IInsertDeleteTextProvider insertDeleteProv
				= Provider.GetPatternProvider (InsertDeleteTextPatternIdentifiers.Pattern.Id)
					as IInsertDeleteTextProvider;
			if (insertDeleteProv != null)
				insertDeleteProv.InsertText (clipboardText, ref position);
			else
				TextBoxBase.Text = text.Insert (position, clipboardText);
		}

		#endregion

		private SWF.TextBoxBase TextBoxBase {
			get {
				if (Provider.Control is SWF.TextBoxBase)
					return (SWF.TextBoxBase)Provider.Control;
				else if (Provider.Control is SWF.UpDownBase)
					return ((SWF.UpDownBase)Provider.Control).txtView;
				else
					throw new Exception ("TextBoxBase: Unknown type: " + Provider.Control);
			}
		}

		private string Text {
			get {
				if (TextBoxBase is SWF.MaskedTextBox)
					return ((SWF.MaskedTextBox) TextBoxBase).MaskedTextProvider.ToDisplayString ();
				else
					return TextBoxBase.Text;
			}
			set { TextBoxBase.Text = value; }
		}

	}
}
