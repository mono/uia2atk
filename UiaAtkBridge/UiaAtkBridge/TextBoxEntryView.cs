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

//using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	
	public class TextBoxEntryView : ComponentAdapter, Atk.TextImplementor, 
	  Atk.ActionImplementor, Atk.EditableTextImplementor, Atk.StreamableContentImplementor
	{
		private TextImplementorHelper textExpert = null;
		private bool multiLine = false;
		
		public TextBoxEntryView (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.Text;

			ITextProvider textProvider = (ITextProvider) provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id);
			IValueProvider valueProvider = (IValueProvider) provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			if ((textProvider == null) && (valueProvider == null))
				throw new ArgumentException ("Provider for TextBox should either implement IValue or IText");
			
			string text = (textProvider != null) ? textProvider.DocumentRange.GetText (-1) : 
				valueProvider.Value.ToString ();

			textExpert = new TextImplementorHelper (text, this);
			if ((int)provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id) ==
			    ControlType.Document.Id)
				multiLine = true;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			states.AddState (Atk.StateType.Editable);
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
		
		public GLib.SList GetRunAttributes (int offset, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException ();
		}
		
		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			throw new NotImplementedException ();
		}
		
		public string GetSelection (int selection_num, out int start_offset, out int end_offset)
		{
			throw new NotImplementedException ();
		}
		
		public bool AddSelection (int start_offset, int end_offset)
		{
			throw new NotImplementedException ();
		}
		
		public bool RemoveSelection (int selection_num)
		{
			throw new NotImplementedException ();
		}
		
		public bool SetSelection (int selection_num, int start_offset, int end_offset)
		{
			throw new NotImplementedException ();
		}
		
		public bool SetCaretOffset (int offset)
		{
			throw new NotImplementedException ();
		}
		
		public void GetRangeExtents (int start_offset, int end_offset, Atk.CoordType coord_type, out Atk.TextRectangle rect)
		{
			throw new NotImplementedException ();
		}
		
		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coord_type, Atk.TextClipType x_clip_type, Atk.TextClipType y_clip_type)
		{
			throw new NotImplementedException ();
		}
		
		public int CaretOffset {
			get {
				return multiLine ? textExpert.Length : 0;
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
			get {
				throw new NotImplementedException ();
			}
		}
		
		#endregion 
		
		#region ActionImplementor implementation 
		
		bool Atk.ActionImplementor.DoAction (int i)
		{
			throw new NotImplementedException ();
		}
		
		string Atk.ActionImplementor.GetDescription (int i)
		{
			throw new NotImplementedException ();
		}
		
		string Atk.ActionImplementor.GetName (int i)
		{
			throw new NotImplementedException ();
		}
		
		string Atk.ActionImplementor.GetKeybinding (int i)
		{
			throw new NotImplementedException ();
		}
		
		bool Atk.ActionImplementor.SetDescription (int i, string desc)
		{
			throw new NotImplementedException ();
		}
		
		string Atk.ActionImplementor.GetLocalizedName (int i)
		{
			throw new NotImplementedException ();
		}

		int Atk.ActionImplementor.NActions {
			get {
				throw new NotImplementedException ();
			}
		}
		
		#endregion 
		
		#region EditableTextImplementor implementation 
		
		bool Atk.EditableTextImplementor.SetRunAttributes (GLib.SList attrib_set, int start_offset, int end_offset)
		{
			throw new NotImplementedException ();
		}
		
		void Atk.EditableTextImplementor.InsertText (string str1ng, ref int position)
		{
			throw new NotImplementedException ();
		}
		
		void Atk.EditableTextImplementor.CopyText (int start_pos, int end_pos)
		{
			throw new NotImplementedException ();
		}
		
		void Atk.EditableTextImplementor.CutText (int start_pos, int end_pos)
		{
			throw new NotImplementedException ();
		}
		
		void Atk.EditableTextImplementor.DeleteText (int start_pos, int end_pos)
		{
			throw new NotImplementedException ();
		}
		
		void Atk.EditableTextImplementor.PasteText (int position)
		{
			throw new NotImplementedException ();
		}
		
		string Atk.EditableTextImplementor.TextContents {
			set {
				throw new NotImplementedException ();
			}
		}
		
		#endregion 
		
		
		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property.Id == ValuePatternIdentifiers.ValueProperty.Id) {
				string newText = (string)e.NewValue;
				
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.Text == newText)
					return;

				Atk.TextAdapter adapter = new Atk.TextAdapter (this);

				// First delete all text, then insert the new text
				// FIXME: this may not be feasible if the text is being changed by the user (so we delete&insert in every keystroke)
				adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

				textExpert = new TextImplementorHelper (newText, this);
				adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                         newText == null ? 0 : newText.Length);

			}
			else
				base.RaiseAutomationPropertyChangedEvent (e);
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			// TODO
		}

		#region StreamableContentImplementor implementation 
		
		public string GetMimeType (int i)
		{
			throw new NotImplementedException ();
		}
		
		public IntPtr GetStream (string mime_type)
		{
			throw new NotImplementedException ();
		}
		
		public string GetUri (string mime_type)
		{
			throw new NotImplementedException ();
		}
		
		public int NMimeTypes {
			get {
				throw new NotImplementedException ();
			}
		}
		
		#endregion 

	}
}
