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
using Mono.UIAutomation.Bridge;

//using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class TextBoxEntryView : ComponentAdapter, Atk.TextImplementor, 
	  Atk.EditableTextImplementor, Atk.StreamableContentImplementor
	{
		private TextImplementorHelper textExpert = null;
		private IText iText = null;
		private bool multiLine = false;
		private bool editable = true;
		private ITextProvider textProvider;
		private IValueProvider valueProvider;
		
		public TextBoxEntryView (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.Text;

			textProvider = (ITextProvider) provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id);
			valueProvider = (IValueProvider) provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			if ((textProvider == null) && (valueProvider == null))
				throw new ArgumentException ("Provider for TextBox should either implement IValue or IText");
			
			string text = (textProvider != null) ? textProvider.DocumentRange.GetText (-1) : 
				valueProvider.Value.ToString ();

			textExpert = new TextImplementorHelper (text, this);
			if ((int)provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) ==
			    ControlType.Document.Id)
				multiLine = true;

			if (valueProvider != null && valueProvider.IsReadOnly)
				editable = false;

			iText = textProvider as IText;
			if (iText == null)
				iText = valueProvider as IText;
			caretOffset = (iText != null? iText.CaretOffset: textExpert.Length);
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (editable)
				states.AddState (Atk.StateType.Editable);
			else
				states.RemoveState (Atk.StateType.Editable);
			
			states.AddState (multiLine ? Atk.StateType.MultiLine : Atk.StateType.SingleLine);
			states.RemoveState (multiLine ? Atk.StateType.SingleLine : Atk.StateType.MultiLine);
			return states;
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
		
		public GLib.SList GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
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
			startOffset = caretOffset;
			endOffset = caretOffset;
			return null;
		}
		
		public bool AddSelection (int startOffset, int endOffset)
		{
			throw new NotImplementedException ();
		}
		
		public bool RemoveSelection (int selectionNum)
		{
			return false;
		}
		
		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return false;
		}

		int caretOffset = -1;
		
		public bool SetCaretOffset (int offset)
		{
			if (iText != null) {
				if (iText.SetCaretOffset (offset))
					caretOffset = offset;
				// gail always returns true; tracking it
				// because of tests
			} else
				caretOffset = offset;
			return true;
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
				return caretOffset;
			}
		}
		
		public GLib.SList DefaultAttributes {
			get {
				throw new NotImplementedException ();
			}
		}
		
		public int CharacterCount {
			get { return textExpert.Length; }
		}
		
		public int NSelections {
			get { return 0; }
		}
		
		#endregion 
		
		
		#region EditableTextImplementor implementation 
		
		public bool SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			return false;
		}
		
		public void InsertText (string str, ref int position)
		{
			if (position < 0 || position > textExpert.Length)
				position = textExpert.Length;	// gail
			TextContents = textExpert.Text.Substring (0, position)
				+ str
				+ textExpert.Text.Substring (position);
			position += str.Length;
		}
		
		public void CopyText (int start_pos, int end_pos)
		{
			Console.WriteLine ("UiaAtkBridge (TextBoxEntryView): CopyText unimplemented");
		}
		
		public void CutText (int start_pos, int end_pos)
		{
			Console.WriteLine ("UiaAtkBridge (TextBoxEntryView): CutText unimplemented");
		}
		
		public void DeleteText (int start_pos, int end_pos)
		{
			if (start_pos < 0)
				start_pos = 0;
			if (end_pos < 0 || end_pos > textExpert.Length)
				end_pos = textExpert.Length;
			if (start_pos > end_pos)
				start_pos = end_pos;

			TextContents = TextContents.Remove (start_pos, end_pos - start_pos);
		}
		
		public void PasteText (int position)
		{
			Console.WriteLine ("UiaAtkBridge (TextBoxEntryView): PasteText unimplemented");
		}
		
		public string TextContents {
			get { return valueProvider.Value.ToString (); }
			set {
				if (valueProvider == null) {
					Console.Error.WriteLine ("WARNING: Cannot set text on a TextBox that does not implement IValueProvider.");
					return;
				}

				if (!valueProvider.IsReadOnly)
					valueProvider.SetValue (value);
			}
		}
		
		#endregion 
		
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id == ValuePatternIdentifiers.ValueProperty.Id) {
				string newText = (string)e.NewValue;
				
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.HandleSimpleChange (newText, ref caretOffset, iText == null))
					return;

				Atk.TextAdapter adapter = new Atk.TextAdapter (this);

				// First delete all text, then insert the new text
				adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

				textExpert = new TextImplementorHelper (newText, this);
				adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                         newText == null ? 0 : newText.Length);

				if (iText == null)
					caretOffset = textExpert.Length;
			} else if (e.Property.Id == ValuePatternIdentifiers.IsReadOnlyProperty.Id) {
				bool? isReadOnlyVal = e.NewValue as bool?;
				if (isReadOnlyVal == null && valueProvider != null)
					isReadOnlyVal = valueProvider.IsReadOnly;
				editable = isReadOnlyVal ?? false;
				
				NotifyStateChange (Atk.StateType.Editable, editable);
			} else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == TextPatternIdentifiers.CaretMovedEvent) {
				int newCaretOffset = iText.CaretOffset;
				if (newCaretOffset != caretOffset) {
					caretOffset = newCaretOffset;
					GLib.Signal.Emit (this, "text_caret_moved", caretOffset);
				}
			}
			else
				base.RaiseAutomationEvent (eventId, e);
		}

		#region StreamableContentImplementor implementation 
		
		public string GetMimeType (int i)
		{
			return string.Empty;
		}
		
		public IntPtr GetStream (string mime_type)
		{
			return IntPtr.Zero;
		}
		
		public string GetUri (string mime_type)
		{
			return string.Empty;
		}
		
		public int NMimeTypes {
			get {
				// TODO:
				return 0;
			}
		}
		
		#endregion 

	}
}
