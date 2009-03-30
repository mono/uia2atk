
using System;
using System.Windows.Automation.Peers;

namespace MoonUiaAtkBridge
{
	
	public class AtkTextImplementor : Atk.Text {

		public AtkTextImplementor (Atk.Object plain, AutomationPeer val)
		{
			
		}
	
		
		#region Text implementation
		public event Atk.TextCaretMovedHandler TextCaretMoved {
			add { /* TODO */ }
			remove { /* TODO */ }
		}
		
		public event EventHandler TextSelectionChanged {
			add { /* TODO */ }
			remove { /* TODO */ }
		}
		
		public event EventHandler TextAttributesChanged {
			add { /* TODO */ }
			remove { /* TODO */ }
		}
		
		public event Atk.TextChangedHandler TextChanged {
			add { /* TODO */ }
			remove { /* TODO */ }
		}
		
		public bool AddSelection (int start_offset, int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new System.NotImplementedException();
		}
		
		public string GetSelection (int selection_num, out int start_offset, out int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public string GetText (int start_offset, int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coord_type, Atk.TextClipType x_clip_type, Atk.TextClipType y_clip_type)
		{
			throw new System.NotImplementedException();
		}
		
		public Atk.TextRectangle GetRangeExtents (int start_offset, int end_offset, Atk.CoordType coord_type)
		{
			throw new System.NotImplementedException();
		}
		
		public bool RemoveSelection (int selection_num)
		{
			throw new System.NotImplementedException();
		}
		
		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			throw new System.NotImplementedException();
		}
		
		public Atk.Attribute[] GetRunAttributes (int offset, out int start_offset, out int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public bool SetCaretOffset (int offset)
		{
			throw new System.NotImplementedException();
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			throw new System.NotImplementedException();
		}
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundary_type, out int start_offset, out int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public bool SetSelection (int selection_num, int start_offset, int end_offset)
		{
			throw new System.NotImplementedException();
		}
		
		public int CaretOffset {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public int CharacterCount {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public Atk.Attribute[] DefaultAttributes {
			get {
				throw new System.NotImplementedException();
			}
		}
		
		public int NSelections {
			get {
				throw new System.NotImplementedException();
			}
		}
		public IntPtr Handle {
			get {
				throw new System.NotImplementedException();
			}
		}
		#endregion

	}

}
