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
using System.Reflection;
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
			
			normalizer = new TextNormalizer (textboxbase);
		}
		
		#endregion
		
		#region Public Properties 
		
		public int EndPoint {
			get { return normalizer.EndPoint; }
		}
		
		public int StartPoint {
			get { return normalizer.StartPoint; }
		}

		public ITextProvider TextProvider {
			get { return provider; }
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
			TextRangeProvider range = new TextRangeProvider (provider, textboxbase);
			FieldInfo field = range.GetType ().GetField ("normalizer", 
			                                             BindingFlags.Instance | BindingFlags.NonPublic);
			TextNormalizer normalizer = new TextNormalizer (textboxbase, StartPoint, EndPoint);
			field.SetValue (range, normalizer);

			return (ITextRangeProvider) range;
		}

		//TODO: Evaluate rangeProvider == null
		public bool Compare (ITextRangeProvider range)
		{
			TextRangeProvider rangeProvider = range as TextRangeProvider;
			if (rangeProvider.TextProvider != provider)
				throw new ArgumentException ();
		
			return (rangeProvider.StartPoint == StartPoint) 
				&& (rangeProvider.EndPoint == EndPoint);
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
			//TextUnit.Character does nothing.
			if (unit == TextUnit.Word)
				normalizer.WordNormalize ();
			else //TODO: Add missing units
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
			if (maxLength < -1)
				throw new ArgumentOutOfRangeException ();
				
			int startPoint = StartPoint;
			int endPoint = EndPoint;
			if (StartPoint > EndPoint) {
				startPoint = EndPoint;
				endPoint = StartPoint;
			}

			int length = endPoint - startPoint;
			if (length > maxLength && maxLength != -1)
				length = maxLength;

			return textboxbase.Text.Substring (startPoint, length);
		}

		//Character, Format, Word, Line, Paragraph, Page, Document
		public int Move (TextUnit unit, int count)
		{	
			TextNormalizerPoints points = normalizer.Move (unit, count);
			
			return points.Moved;
		}

		public void MoveEndpointByRange (TextPatternRangeEndpoint endpoint, 
		                                 ITextRangeProvider targetRange, 
		                                 TextPatternRangeEndpoint targetEndpoint)
		{
			throw new NotImplementedException();
		}

		public int MoveEndpointByUnit (TextPatternRangeEndpoint endpoint, 
		                               TextUnit unit, int count)
		{
			/* XXX: Needs investigation  --Brad
			//Same MS behavior
			if (this == TextProvider.DocumentRange)
				return 0;
			*/
			
			switch (unit) {
			case TextUnit.Character:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.CharacterMoveStartPoint (count);
				else
					return normalizer.CharacterMoveEndPoint (count);
			case TextUnit.Format:
				throw new NotImplementedException ();
			case TextUnit.Word:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.WordMoveStartPoint (count);
				else
					return normalizer.WordMoveEndPoint (count);
			case TextUnit.Line:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.LineMoveStartPoint (count);
				else
					return normalizer.LineMoveEndPoint (count);
			case TextUnit.Paragraph:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.ParagraphMoveStartPoint (count);
				else
					return normalizer.ParagraphMoveEndPoint (count);
			case TextUnit.Page:
			case TextUnit.Document:
				throw new NotImplementedException ();
			}

			return 0;
		}

		public void RemoveFromSelection ()
		{
			if (provider.SupportedTextSelection != SupportedTextSelection.Multiple)
				throw new InvalidOperationException ();
			
			//TODO: Verify if RichTextBox supports: SupportedTextSelection.Multiple
		}

		public void ScrollIntoView (bool alignToTop)
		{
			throw new NotImplementedException();
		}

		public void Select ()
		{
			textboxbase.SelectionStart = normalizer.StartPoint;
			textboxbase.SelectionLength = System.Math.Abs (normalizer.EndPoint - normalizer.StartPoint);
		}
		
		#endregion

		#region Private Members

		private TextNormalizer normalizer;
		private ITextProvider provider;
		private TextBoxBase textboxbase;

		#endregion

	}
}
