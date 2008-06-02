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
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{

	//A TextPatternRange can represent an 
	//- insertion point, 
	//- a subset, or 
	//- all of the text in a TextPattern container.
	internal class TextRangeProvider : ITextRangeProvider
	{

#region Constructors

		public TextRangeProvider (ITextProvider provider, TextBoxBase textboxbase)
		{
			this.provider = provider;
			this.textboxbase = textboxbase;

			start_point = textboxbase.SelectionStart;				
			end_point = start_point + textboxbase.SelectionLength;
		}
		
#endregion
		
#region Public Properties 

		public ITextProvider TextProvider {
			get { return provider; }
		}
		
		public int StartPoint {
			get { return start_point; }
		}
		
		public int EndPoint {
			get { return end_point; }
		}

#endregion

#region ITextRangeProvider Members

		public void AddToSelection ()
		{
			if (provider.SupportedTextSelection != SupportedTextSelection.Multiple)
				throw new InvalidOperationException ();
			
			//TODO: Verify if RichTextBox supports: SupportedTextSelection.Multiple
		}

		public ITextRangeProvider Clone ()
		{
			throw new NotImplementedException();
		}

		//TODO: Evaluate rangeProvider == null
		public bool Compare (ITextRangeProvider range)
		{
			TextRangeProvider rangeProvider = range as TextRangeProvider;
			if (rangeProvider.TextProvider != provider)
				throw new ArgumentException ();
		
			return (rangeProvider.StartPoint != StartPoint) 
				&& (rangeProvider.EndPoint != EndPoint);
		}

		//TODO: Evaluate rangeProvider == null
		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, 
		                             ITextRangeProvider targetRange,
		                             TextPatternRangeEndpoint targetEndpoint)
		{
			TextRangeProvider targetRangeProvider = targetRange as TextRangeProvider;
			if (targetRangeProvider.TextProvider != provider)
				throw new ArgumentException ();
			
			int point = endpoint == TextPatternRangeEndpoint.End ? EndPoint : StartPoint;
			int targePoint = targetEndpoint == TextPatternRangeEndpoint.End 
				? targetRangeProvider.EndPoint : targetRangeProvider.StartPoint;

			return point - targePoint;
		}

		//Character, Format, Word, Line, Paragraph, Page, Document
		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider FindAttribute (int attribute, object value, 
		                                         bool backward)
		{
			throw new NotImplementedException();
		}

		public ITextRangeProvider FindText (string text, bool backward, 
		                                    bool ignoreCase)
		{
			throw new NotImplementedException();
		}

		public object GetAttributeValue (int attribute)
		{
			throw new NotImplementedException();
		}

		public double[] GetBoundingRectangles ()
		{
			throw new NotImplementedException();
		}

		public IRawElementProviderSimple[] GetChildren ()
		{
			throw new NotImplementedException();
		}

		public IRawElementProviderSimple GetEnclosingElement ()
		{
			throw new NotImplementedException();
		}

		public string GetText (int maxLength)
		{
			throw new NotImplementedException();
		}

		//Character, Format, Word, Line, Paragraph, Page, Document
		public int Move (TextUnit unit, int count)
		{
			if (count == 0)
				return 0;
				
			TextNormalizer normalizer = new TextNormalizer (textboxbase.Text, 
			                                                textboxbase.SelectionStart, 
			                                                textboxbase.SelectionLength);
			TextNormalizerPoints points = normalizer.Normalize (unit, count);
			
			textboxbase.SelectionStart = points.Start;
			textboxbase.SelectionLength = points.Length;
			
			//TODO: Update provider.DocumentRange Start and End.

			return points.Moved;

//			if (unit == TextUnit.Character) {
//			} else if (unit == TextUnit.Format) {
//			} else if (unit == TextUnit.Word) {
//			} else if (unit == TextUnit.Line) {
//			} else if (unit == TextUnit.Paragraph) {
//			} else if (unit == TextUnit.Page) {
//			} else if (unit == TextUnit.Document) {
//			}
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, 
		                                 ITextRangeProvider targetRange, 
		                                 TextPatternRangeEndpoint targetEndpoint)
		{
			throw new NotImplementedException();
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, TextUnit unit, int count)
		{
			throw new NotImplementedException();
		}

		public void RemoveFromSelection ()
		{
			throw new NotImplementedException();
		}

		public void ScrollIntoView (bool alignToTop)
		{
			throw new NotImplementedException();
		}

		public void Select ()
		{
			throw new NotImplementedException();
		}
		
#endregion

#region Private Members

		private ITextProvider provider;
		private TextBoxBase textboxbase;
		private int start_point;
		private int end_point;

#endregion

	}
}
