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
			int retOffset = startOffset;
			while (!CharEqualsAny (explored [retOffset], seps))
				retOffset++;
			stopEarlyOffset = retOffset;
			while (CharEqualsAny (explored [retOffset], seps))
				retOffset++;
			stopLateOffset = retOffset;
		}
		
		private int ForwardToNextSeparator (char[] seps, string explored, int startOffset, bool stopEarly)
		{
			int retOffset = startOffset;
			while (!CharEqualsAny (explored [retOffset], seps))
				retOffset++;
			if (stopEarly)
				return retOffset;
			while (CharEqualsAny (explored [retOffset], seps))
				retOffset++;
			return retOffset;
		}
		
		private int BackwardToNextSeparator (char[] seps, string explored, int startOffset, bool stopEarly)
		{
			int retOffset = startOffset;
			
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
		private static char [] sentenceSeparators = new char[] { '.', '\n', '\r' };
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){
			case Atk.TextBoundary.Char:
				startOffset = offset;
				endOffset = offset + 1;
				return new String (new char[] { GetCharacterAtOffset (offset) });
				
			case Atk.TextBoundary.LineEnd:
				ForwardToNextSeparator (newLineSeparators, Name, offset, out startOffset, out endOffset);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, endOffset, true);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.LineStart:
				startOffset = ForwardToNextSeparator (newLineSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, startOffset, false);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.WordEnd:
				ForwardToNextSeparator (wordSeparators, Name, offset, out startOffset, out endOffset);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, endOffset, true);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.WordStart:
				startOffset = ForwardToNextSeparator (wordSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, startOffset, false);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.SentenceEnd:
				ForwardToNextSeparator (sentenceSeparators, Name, offset, out startOffset, out endOffset);
				endOffset = ForwardToNextSeparator (sentenceSeparators, Name, endOffset, true);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.SentenceStart:
				//TODO: take in account other blanks, such as \r,\n,\t
				string afterOffset3 = Name.Substring (offset);
				endOffset = startOffset = offset;
				return afterOffset3;
				
			default:
				throw new NotSupportedException (
					String.Format ("The value {0} is not supported for Atk.TextBoundary type.",
						(long)boundaryType));
			}
		}

		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){
			case Atk.TextBoundary.WordEnd:
				startOffset = BackwardToNextSeparator (wordSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, offset, true);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.WordStart:
				startOffset = BackwardToNextSeparator (wordSeparators, Name, offset, true);
				endOffset = ForwardToNextSeparator (wordSeparators, Name, offset, false);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.LineEnd:
				startOffset = BackwardToNextSeparator (newLineSeparators, Name, offset, false);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, offset, true);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.LineStart:
				startOffset = BackwardToNextSeparator (newLineSeparators, Name, offset, true);
				endOffset = ForwardToNextSeparator (newLineSeparators, Name, offset, false);
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.SentenceEnd:
				endOffset = offset + Name.Substring (offset).IndexOf (".") + 1;
				startOffset = Name.Substring (0, offset).LastIndexOf (".");
				if (startOffset == -1)
					startOffset = 0;
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.SentenceStart:
				endOffset = offset + Name.Substring (offset).IndexOf (".") + 1;
				while ((Name [endOffset] == '\r') || (Name [endOffset] == '\n'))
					endOffset++;
				startOffset = Name.Substring (0, offset).LastIndexOf (".");
				if (startOffset == -1)
					startOffset = 0;
				return Name.Substring (startOffset, endOffset - startOffset);
				
			case Atk.TextBoundary.Char:
				startOffset = offset;
				if (offset >= Name.Length)
					endOffset = offset;
				else
					endOffset = offset + 1;
				return new String (new char[] { GetCharacterAtOffset (offset) });
				
			default:
				return GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			}
		}

		public char GetCharacterAtOffset (int offset)
		{
			if (offset >= Name.Length)
				return '\0';
			return Name.ToCharArray () [offset];
		}

		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
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
			startOffset = 0;
			endOffset = 0;
			return String.Empty;
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
