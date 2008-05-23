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

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){
			case Atk.TextBoundary.Char:
				startOffset = offset;
				endOffset = offset + 1;
				return new String (new char[] { GetCharacterAtOffset (offset) });
			case Atk.TextBoundary.LineEnd:
				string afterOffset1 = Name.Substring (offset);
				startOffset = afterOffset1.IndexOf (Environment.NewLine);
				string afterStart1 = afterOffset1.Substring (startOffset);
				endOffset = afterStart1.IndexOf (Environment.NewLine);
				return afterStart1.Substring (0, endOffset);
			case Atk.TextBoundary.LineStart:
				//TODO: optimize this (when we have unit tests):
				string afterOffset2 = Name.Substring (offset);
				startOffset = afterOffset2.IndexOf (Environment.NewLine) + Environment.NewLine.Length;
				string afterStart2 = afterOffset2.Substring (startOffset);
				endOffset = afterStart2.IndexOf (Environment.NewLine);
				return afterStart2.Substring (0, endOffset);
			case Atk.TextBoundary.WordEnd:
			case Atk.TextBoundary.WordStart:
			case Atk.TextBoundary.SentenceEnd:
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

		char[] anyNewLine = new char[] { '\n', '\r' };

		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			switch (boundaryType){
			case Atk.TextBoundary.WordEnd:
				//TODO: take in account other blanks, such as \r,\n,\t
				endOffset = offset + Name.Substring (offset).IndexOf(" ");
				startOffset = Name.Substring (0, endOffset - 1).LastIndexOf(" ");
				return Name.Substring (startOffset, endOffset - startOffset);
			case Atk.TextBoundary.WordStart:
				//TODO: take in account other blanks, such as \r,\n,\t
				endOffset = offset + Name.Substring (offset).IndexOf(" ") + 1;
				startOffset = Name.Substring (0, endOffset - 1).LastIndexOf(" ") + 1;
				return Name.Substring (startOffset, endOffset - startOffset);
			case Atk.TextBoundary.LineEnd:
				//TODO: check if a different NewLine nexus than the Environment one also applies
				endOffset = offset + Name.Substring (offset).IndexOf (Environment.NewLine) - 1;
				startOffset = Name.Substring (0, offset).LastIndexOf (Environment.NewLine);
				if (startOffset == -1)
					startOffset = 0;
				return Name.Substring (startOffset, endOffset - startOffset);
			case Atk.TextBoundary.LineStart:
				startOffset = 0;
				int i = offset;
				while ((i - 1) > 0)
				{
					if ((Name [i - 1] == '\r') || (Name [i - 1] == '\n')) {
						startOffset = i - 1;
						break;
					}
					i--;
				}
				endOffset = Name.Length;
				i = offset;
				while ((i + 1) < Name.Length)
				{
					if ((Name [i + 1] == '\r') || (Name [i + 1] == '\n'))
						endOffset = i + 1;
					else if (endOffset != Name.Length) {
						endOffset++;
						break;
					}
					i++;
				}
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
