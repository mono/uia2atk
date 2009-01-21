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
//      Brad Taylor <brad@getcoded.net>
// 

using Atk;
using System;

namespace UiaAtkBridge
{
	internal interface ITextImplementor
	{
		int Length {
			get;
		}
		
		string GetSelection (int selectionNum, out int startOffset, out int endOffset);

		string GetTextAfterOffset (int offset, TextBoundary boundaryType,
		                           out int startOffset, out int endOffset); 
		string GetTextAtOffset (int offset, TextBoundary boundaryType,
		                        out int startOffset, out int endOffset);
		string GetTextBeforeOffset (int offset, TextBoundary boundaryType,
		                            out int startOffset, out int endOffset);

		string Text {
			get;
		}

		string GetText (int startOffset, int endOffset);
		void GetRangeExtents (int startOffset, int endOffset, CoordType coordType,
		                      out TextRectangle rect);

		char GetCharacterAtOffset (int offset);
		void GetCharacterExtents (int offset, out int x, out int y,
		                          out int width, out int height, CoordType coords);

		Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset);
		Atk.Attribute [] DefaultAttributes {
			get;
		}

		bool HandleSimpleChange (string newText, ref int caretOffset);
		bool HandleSimpleChange (string newText, ref int caretOffset, bool updateCaret);
	}
}
