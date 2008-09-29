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

namespace atkSharpHelloWorld
{
	
	
	public class HelloButton : Atk.Object , Atk.TextImplementor
	{

		public int CaretOffset {
			get {
				Console.WriteLine ("ATKTEXT: CaretOffset");
				return this.Name.Length;
			}
		}

		public GLib.SList DefaultAttributes {
			get {
				Console.WriteLine ("ATKTEXT: DefaultAttributes");
				throw new NotImplementedException();
			}
		}

		public int CharacterCount {
			get {
				Console.WriteLine ("ATKTEXT: CharacterCount");
				throw new NotImplementedException();
			}
		}

		public int NSelections {
			get {
				Console.WriteLine ("ATKTEXT: NSelections");
				throw new NotImplementedException();
			}
		}

		
		public HelloButton(string caption)
		{
			this.Name = caption;
			this.Role = Atk.Role.PushButton;
		}
		
		public void FirePushButton ()
		{
			NotifyStateChange (Atk.StateType.Armed, true);
		}

		public string GetText (int start_offset, int end_offset)
		{
			Console.WriteLine ("ATKTEXT: GetText");
			throw new NotImplementedException();
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			Console.WriteLine ("ATKTEXT: GetTextAfterOffset");
			throw new NotImplementedException();
		}

		public string GetTextAtOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			Console.WriteLine ("ATKTEXT: GetTextAtOffset " + offset);
			switch (boundary_type){
			case Atk.TextBoundary.Char:
				Console.WriteLine("char");
				break;
			case Atk.TextBoundary.LineEnd:
				Console.WriteLine("le");
				break;
			case Atk.TextBoundary.LineStart:
				Console.WriteLine("ls");
				break;
			case Atk.TextBoundary.SentenceEnd:
				Console.WriteLine("se");
				break;
			case Atk.TextBoundary.SentenceStart:
				Console.WriteLine("ss");
				break;
			case Atk.TextBoundary.WordEnd:
				Console.WriteLine("we");
				break;
			case Atk.TextBoundary.WordStart:
				Console.WriteLine("ws");
				break;
			default: 
				Console.WriteLine ("noooo:" + boundary_type.value__);
				start_offset = offset;
				end_offset = Name.Length;
				
				//expected NRE, still to determine why:
//Exception in Gtk# callback delegate
//  Note: Applications can use GLib.ExceptionManager.UnhandledException to handle the exception.
//System.NullReferenceException: Object reference not set to an instance of an object
//  at atkSharpHelloWorld.HelloButton.GetTextAtOffset (Int32 offset, TextBoundary boundary_type, System.Int32& start_offset, System.Int32& end_offset) [0x00000] 
//  at Atk.TextAdapter.GetTextAtOffsetCallback (IntPtr text, Int32 offset, Int32 boundary_type, System.Int32& start_offset, System.Int32& end_offset) [0x00000] 
//   at GLib.ExceptionManager.RaiseUnhandledException(System.Exception e, Boolean is_terminal)
//   at Atk.TextAdapter.GetTextAtOffsetCallback(IntPtr text, Int32 offset, Int32 boundary_type, Int32 ByRef start_offset, Int32 ByRef end_offset)
//   at Atk.TextAdapter.GetTextAtOffsetCallback(IntPtr , Int32 , Int32 , Int32 ByRef , Int32 ByRef )
//   at GLib.MainLoop.g_main_loop_run(IntPtr )
//   at GLib.MainLoop.g_main_loop_run(IntPtr )
//   at GLib.MainLoop.Run()
//   at atkSharpHelloWorld.MainClass.Main(System.String[] args)
				return Name;
			}
			
			start_offset = 0;
			end_offset = 0;
			Console.WriteLine ("NRE?");
			return null;
			//throw new NotImplementedException();
		}

		public char GetCharacterAtOffset (int offset)
		{
			Console.WriteLine ("ATKTEXT: GetCharacterAtOffset");
			throw new NotImplementedException();
		}

		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			Console.WriteLine ("ATKTEXT: GetTextBeforeOffset");
			throw new NotImplementedException();
		}

		public GLib.SList GetRunAttributes (int offset, out int start_offset, out int end_offset)
		{
			Console.WriteLine ("ATKTEXT: GetRunAttributes");
			throw new NotImplementedException();
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			Console.WriteLine ("ATKTEXT: GetCharacterExtents");
			throw new NotImplementedException();
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			Console.WriteLine ("ATKTEXT: GetOffsetAtPoint");
			throw new NotImplementedException();
		}

		public string GetSelection (int selection_num, out int start_offset, out int end_offset)
		{
			Console.WriteLine ("ATKTEXT: GetSelection");
			throw new NotImplementedException();
		}

		public bool AddSelection (int start_offset, int end_offset)
		{
			Console.WriteLine ("ATKTEXT: AddSelection");
			throw new NotImplementedException();
		}

		public bool RemoveSelection (int selection_num)
		{
			Console.WriteLine ("ATKTEXT: RemoveSelection");
			throw new NotImplementedException();
		}

		public bool SetSelection (int selection_num, int start_offset, int end_offset)
		{
			Console.WriteLine ("ATKTEXT: SetSelection");
			throw new NotImplementedException();
		}

		public bool SetCaretOffset (int offset)
		{
			Console.WriteLine ("ATKTEXT: SetCaretOffset");
			throw new NotImplementedException();
		}

		public void GetRangeExtents (int start_offset, int end_offset, Atk.CoordType coord_type, out Atk.TextRectangle rect)
		{
			Console.WriteLine ("ATKTEXT: GetRangeExtents");
			throw new NotImplementedException();
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coord_type, Atk.TextClipType x_clip_type, Atk.TextClipType y_clip_type)
		{
			Console.WriteLine ("ATKTEXT: GetBoundedRanges");
			throw new NotImplementedException();
		}
	}
}
