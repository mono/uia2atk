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
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TextBox;

namespace Mono.UIAutomation.Winforms.Behaviors.TextBox
{
	internal class CaretProviderBehavior 
		: ProviderBehavior, ICaretProvider
	{
		#region Constructor
		
		public CaretProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}

		#endregion
		
		#region ProviderBehavior: Specialization
		
		public override AutomationPattern ProviderPattern { 
			get { return CaretPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.TextPatternCaretMovedEvent,
			                   new TextPatternCaretMovedEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TextPatternCaretMovedEvent,
			                   null);
		}
		
		#endregion

		#region ICaretProvider Implementation

		public int CaretOffset {
			get {
				// TODO: This won't scale; we really should
				// find a better way of doing it
				SWF.Document document = Document;
				if (document.caret.line.line_no > document.Lines)
					return Text.Length;
				return Document.LineTagToCharIndex
					(document.caret.line, document.CaretPosition);
			}
		}

		public bool SetCaretOffset (int offset)
		{
			if (offset < 0)
				return false;
			SWF.Document document = Document;
			int curPos = 0;
			SWF.Line line = null;
			for (int i = 1;i <= document.Lines;i++) {
				line = document.GetLine (i);
				int length = line.Text.ToString().Length;
				if (curPos + length >= offset) {
					document.PositionCaret (line, offset - curPos);
					return true;
				}
				curPos += length;
			}
			return false;
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = endOffset = CaretOffset;
			if (selectionNum != 0)
				return null;
			SWF.TextBoxBase textBoxBase = TextBoxBase;
			if (TextBoxBase.Document.SelectionVisible) {
				startOffset = textBoxBase.SelectionStart;
				endOffset = startOffset + textBoxBase.SelectionLength;
				return textBoxBase.Text.Substring (startOffset, endOffset - startOffset);
			}
			return null;
		}

		#endregion

		#region Private Fields

		private SWF.Document Document { 
			get { return TextBoxBase.Document; }
		}

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

		#endregion
	}
}
