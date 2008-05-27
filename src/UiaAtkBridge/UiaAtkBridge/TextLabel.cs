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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class TextLabel : Adapter , Atk.TextImplementor 
	{
		private IRawElementProviderSimple provider;
		
		private int cursorPosition = 0;
		
		public TextLabel (IRawElementProviderSimple provider)
		{
			Role = Atk.Role.Label;
			
			string text = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			Name = text;
		}
		
		public override IRawElementProviderSimple Provider {
			get { return provider; }
		}

		public int CaretOffset {
			get {
				return cursorPosition;
			}
		}

		public GLib.SList DefaultAttributes {
			get {
				//TODO:
				GLib.SList attribs = new GLib.SList(typeof(Atk.TextAttribute));
				return attribs;
			}
		}

		public int CharacterCount {
			get {
				return Name.Length;
			}
		}

		public int NSelections {
			get {
				return 0;
			}
		}
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			// TODO
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}

		public string GetText (int startOffset, int endOffset)
		{
			return Name.Substring (startOffset, endOffset);
		}
		
		private void ForwardToNextSeparator (char[] seps, string explored, int startOffset, out int stopEarlyOffset, out int stopLateOffset)
		{
			ForwardToNextSeparator (seps, explored, startOffset, out stopEarlyOffset, out stopLateOffset, false);
		}
		
		private void ForwardToNextSeparator (char[] seps, string explored, int startOffset, 
		                                      out int stopEarlyOffset, out int stopLateOffset, 
		                                      bool findNonSeparators)
		{
			int retOffset = startOffset;
			bool anyNonSeparator = false;
			while (true) {
				bool isSep = CharEqualsAny (explored [retOffset], seps);
				if (!isSep) {
					anyNonSeparator = true;
					retOffset++;
				}
				else
				{
					if (findNonSeparators) {
						if (anyNonSeparator)
							break;
						else
							retOffset++;
					}
					else {
						break;
					}
				}
			}

			stopEarlyOffset = retOffset;
			while (CharEqualsAny (explored [retOffset], seps))
				retOffset++;
			stopLateOffset = retOffset;
		}
		
		private int ForwardToNextSeparator (char[] seps, string explored, int startOffset, bool stopEarly)
		{
			int retOffset = startOffset;
			if (retOffset >= Name.Length)
				return -1;
			
			while (!CharEqualsAny (explored [retOffset], seps))
			{
				if (retOffset == Name.Length - 1)
					return -1;
				retOffset++;
			}
			if (stopEarly || (retOffset == Name.Length - 1))
				return retOffset;
			
			while (CharEqualsAny (explored [retOffset], seps) && (retOffset != Name.Length - 1))
				retOffset++;
			return retOffset;
		}

		private void BackwardToNextSeparator (char[] seps, string explored, int startOffset, out int stopEarlyOffset, out int stopLateOffset)
		{
			if (startOffset <= 1){
				stopEarlyOffset = 0;
				stopLateOffset = 0;
				return;
			}
			
			int retOffset = startOffset - 1;
			
			while (!CharEqualsAny (explored [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			stopEarlyOffset = retOffset + 1;
			if (retOffset < 0) {
				stopLateOffset = 0;
				return;
			}
			
			while (CharEqualsAny (explored [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}
			
			stopLateOffset = retOffset + 1;
			return;
		}
		
		private int BackwardToNextSeparator (char[] seps, string explored, int startOffset, bool stopEarly)
		{
			if (startOffset <= 1)
				return 0;
			
			int retOffset = startOffset - 1;
			
			while (!CharEqualsAny (explored [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			if (stopEarly)
				return retOffset + 1;
			else if (retOffset < 0)
				return 0;
			
			while (CharEqualsAny (explored [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}
			
			return retOffset + 1;
		}
		
		private bool CharEqualsAny (char boilerPlate, char[] candidates)
		{
			foreach(char candidate in candidates)
				if (boilerPlate == candidate)
					return true;
			return false;
		}
		
		//TODO: use regexp?
		private static char [] wordSeparators = new char[] { ' ', '\n', '\r', '.', '\t' };
		private static char [] newLineSeparators = new char[] { '\n', '\r' };
		private static char [] sentenceSeparators = new char[] { '\n', '\r', '.' };
		private static char [] softSentenceSeparators = new char[] { '.', ':'};

		private int selectionStartOffset = 0, selectionEndOffset = 0;
		
		private string ReturnTextWrtOffset (int startOffset, int endOffset)
		{
			selectionStartOffset = startOffset;
			selectionEndOffset = endOffset;
			
			//TODO: optimize?
			return Name.Substring (startOffset, endOffset - startOffset);
		}
		
		// endOffset == startOffset + 1
		private string ReturnTextWrtOffset (int startOffset)
		{
			selectionStartOffset = startOffset;
			selectionEndOffset = startOffset + 1;
				
			//TODO: optimize?
			return new String (new char[] { GetCharacterAtOffset (startOffset) });
		}
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){
			case Atk.TextBoundary.Char:
				
				endOffset = startOffset = offset + 1;
				if (startOffset > Name.Length)
					endOffset = startOffset = Name.Length;
				else if (endOffset + 1 <= Name.Length)
					endOffset++;

				return ReturnTextWrtOffset (startOffset);
				
			case Atk.TextBoundary.LineEnd:
				ForwardToNextSeparator (newLineSeparators, Name, offset, out startOffset, out endOffset);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, endOffset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.LineStart:
				startOffset = ForwardToNextSeparator (newLineSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, startOffset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.WordEnd:
				ForwardToNextSeparator (wordSeparators, Name, offset, out startOffset, out endOffset);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, endOffset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.WordStart:
				startOffset = ForwardToNextSeparator (wordSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, startOffset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.SentenceEnd:
				ForwardToNextSeparator (sentenceSeparators, Name, offset, out startOffset, out endOffset, true);
				endOffset = ForwardToNextSeparator (sentenceSeparators, Name, endOffset, true);
				int testStartOffset, nextStartOffset, testEndOffset, nextEndOffset;
				ForwardToNextSeparator(softSentenceSeparators, Name, startOffset, out testStartOffset, out nextStartOffset);
				if (testStartOffset == startOffset)
					startOffset = nextStartOffset;
				ForwardToNextSeparator(softSentenceSeparators, Name, startOffset, out testEndOffset, out nextEndOffset);
				if (testEndOffset == endOffset)
					endOffset = nextEndOffset;
				
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.SentenceStart:
				startOffset = ForwardToNextSeparator (sentenceSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (sentenceSeparators, Name, startOffset, false);
				
				endOffset = ForwardToNextSeparator (wordSeparators, Name, endOffset, false);
				
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			default:
				throw GetNotSupportedBoundary (boundaryType);
			}
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){

			case Atk.TextBoundary.WordEnd:
				startOffset = BackwardToNextSeparator (wordSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, offset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.WordStart:
				startOffset = BackwardToNextSeparator (wordSeparators, Name, offset, true);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, offset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.LineEnd:
				startOffset = BackwardToNextSeparator (newLineSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, offset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.LineStart:
				startOffset = BackwardToNextSeparator (newLineSeparators, Name, offset, true);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, offset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.SentenceEnd:
				startOffset = BackwardToNextSeparator (sentenceSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (sentenceSeparators, Name, offset, true);

				int testEndOffset, nextEndOffset;
				ForwardToNextSeparator(softSentenceSeparators, Name, startOffset, out testEndOffset, out nextEndOffset);
				if (testEndOffset == endOffset)
					endOffset = nextEndOffset;
				
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.SentenceStart:
				startOffset = BackwardToNextSeparator (sentenceSeparators, Name, offset, true);
				endOffset = ForwardToNextSeparator (sentenceSeparators, Name, offset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.Char:
				startOffset = offset;
				if (offset >= Name.Length)
					endOffset = offset;
				else
					endOffset = offset + 1;
				return ReturnTextWrtOffset (startOffset);
				
			default:
				throw GetNotSupportedBoundary (boundaryType);
			}
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){

			case Atk.TextBoundary.WordEnd:
				endOffset = BackwardToNextSeparator (wordSeparators, Name, offset, false);
				startOffset = BackwardToNextSeparator (wordSeparators, Name, endOffset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.WordStart:
				BackwardToNextSeparator (wordSeparators, Name, offset, out endOffset, out startOffset);
				startOffset = BackwardToNextSeparator (wordSeparators, Name, startOffset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.LineEnd:
				endOffset = BackwardToNextSeparator (newLineSeparators, Name, offset, false);
				startOffset = BackwardToNextSeparator (newLineSeparators, Name, endOffset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.LineStart:
				BackwardToNextSeparator (newLineSeparators, Name, offset, out endOffset, out startOffset);
				startOffset = BackwardToNextSeparator (newLineSeparators, Name, startOffset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.Char:
				startOffset = offset - 1;
				endOffset = offset;
				return ReturnTextWrtOffset (startOffset);

			case Atk.TextBoundary.SentenceEnd:
				endOffset = BackwardToNextSeparator (sentenceSeparators, Name, offset, false);
				startOffset = BackwardToNextSeparator (sentenceSeparators, Name, endOffset, false);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			case Atk.TextBoundary.SentenceStart:
				BackwardToNextSeparator (sentenceSeparators, Name, offset, out endOffset, out startOffset);
				startOffset = BackwardToNextSeparator (sentenceSeparators, Name, startOffset, true);
				return ReturnTextWrtOffset (startOffset, endOffset);
				
			default:
				throw GetNotSupportedBoundary (boundaryType);
			}
		}

		private NotSupportedException GetNotSupportedBoundary (Atk.TextBoundary bType)
		{
			return new NotSupportedException (
				String.Format ("The value {0} is not supported as a Atk.TextBoundary type.",
					bType));
		}
		
		public GLib.SList GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			// don't ask me why, this is what gail does 
			// (instead of throwing or returning null):
			if (offset > Name.Length)
				offset = Name.Length;
			else if (offset < 0)
				offset = 0;
			
			//just test values for now:
			endOffset = Name.Length;
			startOffset = offset;
				
			//TODO:
			GLib.SList attribs = new GLib.SList(typeof(Atk.TextAttribute));
			return attribs;
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			throw new NotImplementedException();
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException();
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
			return null;
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			return false;
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			if (offset >= Name.Length)
				return '\0';
			return Name.ToCharArray () [offset];
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, Atk.TextRectangle rect)
		{
			throw new NotImplementedException();
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException();
		}
		
	}
}
