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
using System.IO;
using System.Text;
using Mono.Unix.Native;
using Mono.UIAutomation.Bridge;
using Mono.UIAutomation.Services;

//using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class TextBoxEntryView : ComponentParentAdapter, Atk.TextImplementor, 
	  Atk.EditableTextImplementor, Atk.StreamableContentImplementor
	{
		private ITextImplementor textExpert = null;
		private bool multiLine = false;
		private EditableTextImplementorHelper editableTextExpert;
		
		public TextBoxEntryView (IRawElementProviderSimple provider) : base (provider)
		{
			if (IsTableCell)
				Role = Atk.Role.TableCell;
			else
				Role = Atk.Role.Text;

			editableTextExpert = new EditableTextImplementorHelper (this, this);

			if (provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id) == null
			    && provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id) == null)
				throw new ArgumentException ("Provider for TextBox should either implement IValue or IText");
			
			textExpert = TextImplementorFactory.GetImplementor (this, provider);
			if ((int) provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) 
			    == ControlType.Document.Id)
				multiLine = true;
		}

		protected bool IsTableCell {
			get {
				IRawElementProviderFragment fragment = Provider as IRawElementProviderFragment;
				if (fragment == null)
					return false;
				fragment = fragment.Navigate (NavigateDirection.Parent);
				int controlTypeId = (int)fragment.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
				return (controlTypeId == ControlType.DataItem.Id);
			}
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
			if (IsTableCell)
				base.UpdateNameProperty (newName, fromCtor);
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			editableTextExpert.UpdateStates (states);
			
			states.AddState (multiLine ? Atk.StateType.MultiLine : Atk.StateType.SingleLine);
			states.RemoveState (multiLine ? Atk.StateType.SingleLine : Atk.StateType.MultiLine);

			return states;
		}
		
		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
		}

		#region TextImplementor implementation 
		
		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}
		
		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			return textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
		}
		
		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
		}

		public Atk.Attribute [] DefaultAttributes {
			get { return textExpert.DefaultAttributes; }
		}
		
		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
		}
		
		public bool AddSelection (int startOffset, int endOffset)
		{
			return textExpert.AddSelection (startOffset, endOffset);
		}
		
		public bool RemoveSelection (int selectionNum)
		{
			return textExpert.RemoveSelection (selectionNum);
		}
		
		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return textExpert.SetSelection (selectionNum, startOffset, endOffset);
		}
		
		public bool SetCaretOffset (int offset)
		{
			return textExpert.SetCaretOffSet (offset);
		}
		
		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}
		
		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException ();
		}
		
		public int CaretOffset {
			get {
				return textExpert.CaretOffset;
			}
		}
		
		public int CharacterCount {
			get { return textExpert.Length; }
		}
		
		public int NSelections {
			get { return textExpert.NSelections; }
		}
		
		#endregion 
		
		
		#region EditableTextImplementor implementation 
		
		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return editableTextExpert.SetRunAttributes (attrib_set, 
			                                            start_offset,
			                                            end_offset);
		}
		
		public void InsertText (string str, ref int position)
		{
			editableTextExpert.InsertText (str, ref position);
		}
		
		public void CopyText (int start_pos, int end_pos)
		{
			editableTextExpert.CopyText (start_pos, end_pos);
		}
		
		public void CutText (int start_pos, int end_pos)
		{
			editableTextExpert.CutText (start_pos, end_pos);
		}
		
		public void DeleteText (int start_pos, int end_pos)
		{
			editableTextExpert.DeleteText (start_pos, end_pos);
		}
		
		public void PasteText (int position)
		{
			editableTextExpert.PasteText (position);
		}
		
		public string TextContents {
			get { return editableTextExpert.TextContents; }
			set { editableTextExpert.TextContents = value; }
		}
		
		#endregion 
		
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationPropertyChangedEvent (e)
			    || textExpert.RaiseAutomationPropertyChangedEvent (e))
				return;

			base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (editableTextExpert.RaiseAutomationEvent (eventId, e)
			    || textExpert.RaiseAutomationEvent (eventId, e))
				return;

			base.RaiseAutomationEvent (eventId, e);
		}

		#region StreamableContentImplementor implementation 
		
		public int NMimeTypes {
			get { return 1; }
		}
		
		public string GetMimeType (int i)
		{
			if (i != 0) {
				return String.Empty;
			}

			return "text/plain";
		}
		
		public IntPtr GetStream (string mime_type)
		{
			if (mime_type != "text/plain") {
				return IntPtr.Zero;
			}

			GLib.IOChannel gio = null;

			StringBuilder filename = new StringBuilder ("streamXXXXXX");
			int fd = Syscall.mkstemp (filename);
			if (fd < 0) {
				Log.Error ("TextBoxEntryView: Unable to create temporary file.  Are you out of disk space?");
				return IntPtr.Zero;
			}

			gio = new GLib.IOChannel (fd);
			gio.Encoding = null;
			
			string written;
			gio.WriteChars (TextContents, out written);

			gio.SeekPosition (0, GLib.SeekType.Set);
			gio.Flush ();

			Syscall.unlink (filename.ToString ());

			return gio.Handle;
		}
		
		public string GetUri (string mime_type)
		{
			// This *must* return NULL so that clients will fall
			// back to using GetStream.
			return null;
		}
		
		#endregion 
	}
}
