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
	public class MenuItem : ComponentAdapter, Atk.ActionImplementor, Atk.TextImplementor
	{

		public MenuItem (string name)
		{
			this.Name = name;
		}
		
		public override IRawElementProviderSimple Provider {
			get { return null; }
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			throw new NotImplementedException ();
		}

		public int NActions {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool DoAction (int i)
		{
			throw new NotImplementedException();
		}

		public string GetDescription (int i)
		{
			throw new NotImplementedException();
		}

		public string GetName (int i)
		{
			throw new NotImplementedException();
		}

		public string GetKeybinding (int i)
		{
			throw new NotImplementedException();
		}

		public bool SetDescription (int i, string desc)
		{
			throw new NotImplementedException();
		}

		public string GetLocalizedName (int i)
		{
			throw new NotImplementedException();
		}

		public int CaretOffset {
			get {
				throw new NotImplementedException();
			}
		}

		public GLib.SList DefaultAttributes {
			get {
				throw new NotImplementedException();
			}
		}

		public int CharacterCount {
			get {
				throw new NotImplementedException();
			}
		}

		public int NSelections {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string GetText (int start_offset, int end_offset)
		{
			throw new NotImplementedException();
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException();
		}

		public string GetTextAtOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException();
		}

		public char GetCharacterAtOffset (int offset)
		{
			throw new NotImplementedException();
		}

		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException();
		}

		public GLib.SList GetRunAttributes (int offset, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException();
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			throw new NotImplementedException();
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException();
		}

		public string GetSelection (int selection_num, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException();
		}
		
		public bool AddSelection (int start_offset, int end_offset)
		{
			throw new NotImplementedException();
		}

		public bool RemoveSelection (int selection_num)
		{
			throw new NotImplementedException();
		}

		public bool SetSelection (int selection_num, int start_offset, int end_offset)
		{
			throw new NotImplementedException();
		}

		public bool SetCaretOffset (int offset)
		{
			throw new NotImplementedException();
		}

		public void GetRangeExtents (int start_offset, int end_offset, Atk.CoordType coord_type, out Atk.TextRectangle rect)
		{
			throw new NotImplementedException();
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coord_type, Atk.TextClipType x_clip_type, Atk.TextClipType y_clip_type)
		{
			throw new NotImplementedException();
		}
	}
}
