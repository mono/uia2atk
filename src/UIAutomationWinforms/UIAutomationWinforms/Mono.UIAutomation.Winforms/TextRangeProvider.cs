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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Mono.UIAutomation.Winforms
{
	// A TextPatternRange can represent an 
	// - insertion point, 
	// - a subset, or 
	// - all of the text in a TextPattern container.
	internal class TextRangeProvider : ITextRangeProvider
	{
#region Constructors
		public TextRangeProvider (ITextProvider provider, TextBoxBase textboxbase)
		{
			this.provider = provider;
			this.textboxbase = textboxbase;
			
			normalizer = new TextNormalizer (textboxbase);
		}

		private TextRangeProvider (ITextProvider provider, TextBoxBase textboxbase,
		                           int start_point, int end_point)
		{
			this.provider = provider;
			this.textboxbase = textboxbase;
			this.normalizer = new TextNormalizer (textboxbase, start_point, end_point);
		}
#endregion
		
#region Public Properties 
		public ITextProvider TextProvider {
			get { return provider; }
		}		
#endregion

#region ITextRangeProvider Members
		public void AddToSelection ()
		{
			if (provider.SupportedTextSelection != SupportedTextSelection.Multiple) {
				throw new InvalidOperationException ();
			}
			
			//TODO: Verify if RichTextBox supports: SupportedTextSelection.Multiple
		}
	
		public ITextRangeProvider Clone ()
		{		
			return new TextRangeProvider (provider, textboxbase,
			                              StartPoint, EndPoint); 
		}

		public bool Compare (ITextRangeProvider range)
		{
			TextRangeProvider rangeProvider = range as TextRangeProvider;
			if (rangeProvider == null) {
				return false;
			}

			if (rangeProvider.TextProvider != provider) {
				throw new ArgumentException ();
			}
		
			return (rangeProvider.StartPoint == StartPoint) 
				&& (rangeProvider.EndPoint == EndPoint);
		}

		public int CompareEndpoints (TextPatternRangeEndpoint endpoint, 
		                             ITextRangeProvider targetRange,
		                             TextPatternRangeEndpoint targetEndpoint)
		{
			TextRangeProvider targetRangeProvider = targetRange as TextRangeProvider;
			if (targetRangeProvider == null) {
				throw new ArgumentNullException ();
			}

			if (targetRangeProvider.TextProvider != provider) {
				throw new ArgumentException ();
			}
			
			int point = endpoint == TextPatternRangeEndpoint.End ? EndPoint : StartPoint;
			int targePoint = (targetEndpoint == TextPatternRangeEndpoint.End)
				? targetRangeProvider.EndPoint : targetRangeProvider.StartPoint;

			return point.CompareTo (targePoint);
		}

		public void ExpandToEnclosingUnit (TextUnit unit)
		{
			switch (unit) {
			case TextUnit.Character:
				// This does nothing
				break;
			case TextUnit.Format:
				break;
			case TextUnit.Word:
				normalizer.WordNormalize ();
				break;
			case TextUnit.Line:
				normalizer.LineNormalize ();
				break;
			case TextUnit.Paragraph:
			case TextUnit.Page:
			case TextUnit.Document:
				// TODO:
				break;
			}
		}

		public ITextRangeProvider FindAttribute (int attribute, object value, 
		                                         bool backward)
		{
			if (textboxbase is TextBox) {
				return null;
			}

			throw new NotImplementedException();
		}

		public ITextRangeProvider FindText (string text, bool backward, 
		                                    bool ignoreCase)
		{
			string contents = textboxbase.Text;	
			StringComparison cmp = ignoreCase ? StringComparison.CurrentCultureIgnoreCase
			                                  : StringComparison.CurrentCulture;
			if (String.IsNullOrEmpty (text) || String.IsNullOrEmpty (contents)) {
				return null;
			}
			
			int index = -1;
			if (backward) {
				index = contents.LastIndexOf (text, EndPoint - 1,
				                              EndPoint - StartPoint, cmp);
			} else {
				index = contents.IndexOf (text, StartPoint,
				                          EndPoint - StartPoint, cmp);
			}

			return (index >= 0) ? new TextRangeProvider (provider, textboxbase,
			                                             index, index + text.Length)
			                    : null;
		}

		public object GetAttributeValue (int attribute)
		{
			throw new NotImplementedException();
		}

		public Rect[] GetBoundingRectangles ()
		{
			if (StartPoint == EndPoint
			    || String.IsNullOrEmpty (textboxbase.Text)) {
				return new Rect[0];
			}

			object document = GetInternalDocument (textboxbase);
			if (document == null) {
				return new Rect[0];
			}
			
			List<Rect> rects = new List<Rect> ();

			int num_lines = GetNumLines (document);
			for (int i = 0; i < num_lines; i++) {
				object line = GetLine (document, i);
				if (line == null) {
					continue;
				}
				
				rects.Add (GetLineRect (line));
			}

			return rects.ToArray ();
		}

#region GetBoundingRectangles support methods
		private object GetInternalDocument (TextBoxBase textbox)
		{
			Type textbox_type = textbox.GetType ();
			FieldInfo textbox_fi = textbox_type.GetField ("document", BindingFlags.NonPublic | BindingFlags.Instance);
			if (textbox_fi == null) {
				// XXX: Is it best to throw an exception here,
				// or to return silently?
				throw new Exception ("document field not found in TextBoxBase");
			}
			
			return textbox_fi.GetValue (textbox);
		}

		private object GetLine (object document, int line)
		{
			// TODO: optimize
			Assembly asm = Assembly.GetAssembly (typeof (TextBoxBase));
			Type document_type = asm.GetType ("System.Windows.Forms.Document", false);
			if (document_type == null) {
				throw new Exception ("Internal Document class not found in System.Windows.Forms");
			}

			MethodInfo mi = document_type.GetMethod ("GetLine", BindingFlags.NonPublic | BindingFlags.Instance);
			if (mi == null) {
				throw new Exception ("GetLine method not found in Document class");
			}

			return mi.Invoke (document, new object[] { line });
		}

		private int GetNumLines (object document)
		{
			Assembly asm = Assembly.GetAssembly (typeof (TextBoxBase));
			Type document_type = asm.GetType ("System.Windows.Forms.Document", false);
			if (document_type == null) {
				throw new Exception ("Internal Document class not found in System.Windows.Forms");
			}

			PropertyInfo pi = document_type.GetProperty ("Lines", BindingFlags.NonPublic | BindingFlags.Instance);
			if (pi == null) {
				throw new Exception ("Lines property not found in Document class");
			}

			return (int)pi.GetValue (document, null);
		}

		private Rect GetLineRect (object line)
		{
			Assembly asm = Assembly.GetAssembly (typeof (TextBoxBase));
			Type line_type = asm.GetType ("System.Windows.Forms.Line", false);
			if (line_type == null) {
				throw new Exception ("Internal Line class not found in System.Windows.Forms");
			}

			PropertyInfo x_pi
				= line_type.GetProperty ("X",
			                                 BindingFlags.NonPublic | BindingFlags.Instance);
			if (x_pi == null) {
				throw new Exception ("X property not found in Line class");
			}

			PropertyInfo y_pi
				= line_type.GetProperty ("Y",
			                                 BindingFlags.NonPublic | BindingFlags.Instance);
			if (y_pi == null) {
				throw new Exception ("Y property not found in Line class");
			}

			PropertyInfo width_pi
				= line_type.GetProperty ("Width",
			                                 BindingFlags.NonPublic | BindingFlags.Instance);
			if (width_pi == null) {
				throw new Exception ("Width property not found in Line class");
			}

			PropertyInfo height_pi
				= line_type.GetProperty ("Height",
			                                 BindingFlags.NonPublic | BindingFlags.Instance);
			if (height_pi == null) {
				throw new Exception ("Height property not found in Line class");
			}
			
			return new Rect ((int)x_pi.GetValue (line, null),
			                 (int)y_pi.GetValue (line, null),
			                 (int)width_pi.GetValue (line, null),
			                 (int)height_pi.GetValue (line, null));
		}
#endregion

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

		public int Move (TextUnit unit, int count)
		{	
			return normalizer.Move (unit, count).Moved;
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
			// NOTE: The order of these cases is crucial
			switch (unit) {
			case TextUnit.Character:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.CharacterMoveStartPoint (count);
				else
					return normalizer.CharacterMoveEndPoint (count);
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
			// LAMESPEC: this should fall back on TextUnit.Word
			// according to MSDN, but for TextBox, this resembles
			// TextUnit.Page.
			case TextUnit.Format:
			// Document and Page appear to behave similarly
			case TextUnit.Page:
			case TextUnit.Document:
				if (endpoint == TextPatternRangeEndpoint.Start)
					return normalizer.PageMoveStartPoint (count);
				else
					return normalizer.PageMoveEndPoint (count);
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
			throw new NotImplementedException ();
		}

		public void Select ()
		{
			textboxbase.SelectionStart = normalizer.StartPoint;
			textboxbase.SelectionLength = System.Math.Abs (normalizer.EndPoint - normalizer.StartPoint);
		}
#endregion

#region Private Properties
		private int EndPoint {
			get { return normalizer.EndPoint; }
		}
		
		private int StartPoint {
			get { return normalizer.StartPoint; }
		}
#endregion

#region Private Members
		private TextNormalizer normalizer;
		private ITextProvider provider;
		private TextBoxBase textboxbase;
#endregion
	}
}
