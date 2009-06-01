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
//      Brad Taylor <brad@getcoded.net>
// 

using Atk;

using System;
using System.Windows;
using System.Windows.Automation.Peers;

using Moonlight.AtkBridge;

namespace Moonlight.AtkBridge.ProviderImplementors
{
	[ImplementsPattern (PatternInterface.Value)]
	public class Value : Atk.TextImplementor
	{
#region Public Properties
		public int CaretOffset {
			get { return -1; }
		}

		public int CharacterCount {
			get { return -1; }
		}

		public Atk.Attribute[] DefaultAttributes {
			get { return new Atk.Attribute [0]; }
		}

		public int NSelections {
			get { return -1; }
		}

		IntPtr GLib.IWrapper.Handle {
			get { return IntPtr.Zero; }
		}
#endregion

#region Public Methods
		public bool AddSelection (int startOffset, int endOffset)
		{
			return false;
		}

		public TextRange GetBoundedRanges (TextRectangle rect,
		                                   CoordType coordType,
		                                   TextClipType xClipType,
		                                   TextClipType yClipType)
		{
			return new TextRange ();
		}

		public char GetCharacterAtOffset (int offset)
		{
			return ' ';
		}

		public void GetCharacterExtents (int offset,
		                                 out int x, out int y,
		                                 out int width, out int height,
		                                 CoordType coords)
		{
			x = y = width = height = 1;
		}

		public int GetOffsetAtPoint (int x, int y, CoordType coords)
		{
			return -1;
		}

		public void GetRangeExtents (int startOffset, int endOffset,
		                             CoordType coordType,
		                             out TextRectangle rect)
		{
			rect = new TextRectangle ();
		}

		public Atk.Attribute[] GetRunAttributes (int offset,
		                                         out int startOffset,
		                                         out int endOffset)
		{
			startOffset = endOffset = -1;
			return new Atk.Attribute [0];
		}

		public string GetSelection (int selectionNum,
		                            out int startOffset,
		                            out int endOffset)
		{
			startOffset = endOffset = -1;
			return String.Empty;
		}

		public string GetText (int startOffset, int endOffset)
		{
			return String.Empty;
		}

		public string GetTextAfterOffset (int offset,
		                                  TextBoundary boundaryType,
		                                  out int startOffset,
		                                  out int endOffset)
		{
			startOffset = endOffset = -1;
			return String.Empty;
		}

		public string GetTextAtOffset (int offset,
		                               TextBoundary boundaryType,
		                               out int startOffset,
		                               out int endOffset)
		{
			startOffset = endOffset = -1;
			return String.Empty;
		}

		public string GetTextBeforeOffset (int offset,
		                                   TextBoundary boundaryType,
		                                   out int startOffset,
		                                   out int endOffset)
		{
			startOffset = endOffset = -1;
			return String.Empty;
		}

		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public bool SetSelection (int selectionNum,
		                          int startOffset, int endOffset)
		{
			return false;
		}
#endregion
	}
}
public interface TextImplementor : GLib.IWrapper {
	
}
