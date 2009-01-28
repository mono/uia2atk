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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Automation.Provider;
using SWF = System.Windows.Forms;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Winforms.Events;
using Mono.UIAutomation.Winforms.Events.TextBox;

namespace Mono.UIAutomation.Winforms.Behaviors.TextBox
{
	// NOTE: This class also supports RichTextBox as they share pretty much
	// everything
	internal class TextProviderBehavior
		: ProviderBehavior, ITextProvider, IText, IClipboardSupport
	{
		#region Constructor
		
		public TextProviderBehavior (FragmentControlProvider provider)
			: base (provider)
		{
		}

		#endregion
		
		#region ProviderBehavior: Specialization
		
		public override AutomationPattern ProviderPattern { 
			get { return TextPatternIdentifiers.Pattern; }
		}
		
		public override void Connect ()
		{
			Provider.SetEvent (ProviderEventType.TextPatternCaretMovedEvent,
			                   new TextPatternCaretMovedEvent (Provider));
			Provider.SetEvent (ProviderEventType.TextPatternTextSelectionChangedEvent,
			                   new TextPatternTextSelectionChangedEvent (Provider));
		}
		
		public override void Disconnect ()
		{
			Provider.SetEvent (ProviderEventType.TextPatternTextSelectionChangedEvent,
			                   null);
			Provider.SetEvent (ProviderEventType.TextPatternCaretMovedEvent,
			                   null);
		}

		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.IsPasswordProperty.Id) {
				if (Provider.Control is SWF.TextBox) {
					SWF.TextBox textbox = TextBox;
					return (textbox.UseSystemPasswordChar || (int) textbox.PasswordChar != 0);
				} else if (Provider.Control is SWF.RichTextBox) {
					return false;
				}
			}
			return base.GetPropertyValue (propertyId);
		}
		
		#endregion

		#region ITextProvider Members
		
		//TODO: We should connect the events to update this.text_range_provider?
		public ITextRangeProvider DocumentRange {
			get { 
				return new TextRangeProvider (this, TextBoxBase);
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

			if (!TextBoxBase.Document.SelectionVisible)
				return new ITextRangeProvider [0];

			return new ITextRangeProvider [] { 
				new TextRangeProvider (this, TextBoxBase, TextBoxBase.SelectionStart, TextBoxBase.SelectionStart + TextBoxBase.SelectionLength) };
		}
		
		public ITextRangeProvider[] GetVisibleRanges ()
		{
			int start_line = -1, end_line = -1;
			Document.GetVisibleLineIndexes (TextBoxBase.Bounds, out start_line, out end_line);

			ITextRangeProvider range = DocumentRange.Clone ();
			range.MoveEndpointByUnit (TextPatternRangeEndpoint.Start, TextUnit.Line, start_line);
			range.MoveEndpointByUnit (TextPatternRangeEndpoint.End, TextUnit.Line, end_line - start_line);

			return new ITextRangeProvider[] { range };
		}
		
		public ITextRangeProvider RangeFromChild (IRawElementProviderSimple childElement) 
		{
			if (childElement == null) {
				throw new ArgumentNullException ("childElement");
			}
			
			if (Provider.Control is SWF.TextBox) {
				// TextBox can't have children
				throw new InvalidOperationException ();
			}
			
			// TODO: RichTextBox code path
			return null;
		}
		
		public ITextRangeProvider RangeFromPoint (Point screenLocation)
		{
			int index = -1;

			Document.FindCursor ((int)screenLocation.X, (int)screenLocation.Y, out index);

			// Return the degenerate range
			return (ITextRangeProvider) new TextRangeProvider (
				this, TextBoxBase, index, index);
		}
		
		#region IClipboardSupport Implementation	

		public void Copy (int start, int end)
		{
			string text = TextBoxBase.Text;
			start = (int) System.Math.Max (start, 0);
			end = (int) System.Math.Min (end, text.Length);
			SWF.Clipboard.SetText (text.Substring (start, end - start));
		}
		
		public void Paste (int position)
		{
			string text = TextBoxBase.Text;
			position = (int) System.Math.Max (position, 0);
			position = (int) System.Math.Min (position, text.Length);
			TextBoxBase.Text = text.Insert (position, SWF.Clipboard.GetText ());
		}

		#endregion

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

		// NOTE: If you use this, you need to check if it returns null,
		// as this class is reused for RichTextBox
		private SWF.TextBox TextBox {
			get { return Provider.Control as SWF.TextBox; }
		}
		
		internal object GetDocumentFromTextBoxBase (SWF.TextBoxBase textbox)
		{
			// Copied from TextRangeProvider.cs
			// I couldn't think of a good way to share this without
			// introducing a cyclical dependency -- Brad
			// %-< -------------------------------------------------
			Type textbox_type = textbox.GetType ();
			FieldInfo textbox_fi = textbox_type.GetField ("document", BindingFlags.NonPublic | BindingFlags.Instance);
			if (textbox_fi == null) {
				throw new Exception ("document field not found in TextBoxBase");
			}

			return textbox_fi.GetValue (textbox);
			// ------------------------------------------------- >-%
		}

		internal SWF.Document Document { 
			get { return TextBoxBase.Document; }
		}

		public int CaretOffset {
			get {
				// TODO: This won't scale; we really should
				// find a better way of doing it
				SWF.Document document = Document;
				if (document.caret.line.line_no > document.Lines)
					return TextBoxBase.Text.Length;
				return Document.LineTagToCharIndex
					(document.caret.line, document.CaretPosition);
			}
		}

		bool IText.SetCaretOffset (int offset)
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

		string IText.GetSelection (int selectionNum, out int startOffset, out int endOffset)
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
	}
}
