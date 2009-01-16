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

	public class TextLabel : ComponentAdapter , Atk.TextImplementor
	{
		private TextImplementorHelper textExpert = null;
		
		public TextLabel (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.Label;
			
			string text = (string) provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id);
			textExpert = new TextImplementorHelper (text, this);
			Name = text;
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();
			
			int controlTypeId = (int) Provider.GetPropertyValue (AutomationElementIdentifiers.ControlTypeProperty.Id);
			if (controlTypeId != ControlType.HeaderItem.Id)
				states.AddState (Atk.StateType.MultiLine);
			
			return states;
		}

		public int CaretOffset {
			get {
				return 0;
			}
		}

		public int CharacterCount {
			get {
				return textExpert.Length;
			}
		}

		public int NSelections {
			get {
				return 0;
			}
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == TextPatternIdentifiers.TextChangedEvent) {
				string newText = Provider.GetPropertyValue (AutomationElementIdentifiers.NameProperty.Id) as string;
				
				// Don't fire spurious events if the text hasn't changed
				if (textExpert.Text == newText)
					return;

				Atk.TextAdapter adapter = new Atk.TextAdapter (this);

				// First delete all text, then insert the new text
				adapter.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

				textExpert = new TextImplementorHelper (newText, this);
				adapter.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
				                         newText == null ? 0 : newText.Length);

				// Accessible name and label text are one and
				// the same, so update accessible name
				Name = newText;

				EmitVisibleDataChanged ();
			}
		}

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
			throw new NotImplementedException();
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
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
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			throw new NotImplementedException ();
		}
	}
}
