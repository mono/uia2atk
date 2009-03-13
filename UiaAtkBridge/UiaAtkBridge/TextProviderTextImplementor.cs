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
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Drawing;
using Mono.UIAutomation.Bridge;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Text;
using CG = System.Collections.Generic;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	// TODO: Implement methods using more rich ITextProvider interface
	internal class TextProviderTextImplementor : BaseTextImplementor
	{
#region Public Properties
		public override string Text {
			get { return textProvider.DocumentRange.GetText (-1); }
		}

		public override Atk.Attribute [] DefaultAttributes {
			get { return GetAttributesInRange (-1, -1); }
		}
#endregion

#region Public Methods
		public TextProviderTextImplementor (Adapter resource, 
		                                    ITextProvider textProvider)
			: base (resource)
		{
			this.textProvider = textProvider;
		}

		public override Atk.Attribute [] GetRunAttributes (int offset,
		                                                   out int startOffset,
		                                                   out int endOffset)
		{
			// Ensure offset is within bounds.
			// Gail does the same instead of erroring
			offset = Math.Min (Length, offset);
			offset = Math.Max (0, offset);

			// This implementation is a bit of a hack.
			// The thing is, no one within a cursory google search
			// actually uses this API to find the bounds of a given
			// text attribute.  That's really good, because it's
			// going to be really really inefficient to do so.
			// Like on the order of (N * M)^2 (M = # attrs)

			// Instead, we'll return the list of attributes for the
			// current offset.
			
			startOffset = offset;
			endOffset = offset + 1;

			return GetAttributesInRange (startOffset, endOffset);
		}

		public override int NSelections {
			get {
				try {
					ITextRangeProvider [] selection = textProvider.GetSelection ();
					return selection.Length;
				} catch (InvalidOperationException e) {
					Log.Debug (e);
					return 0;
				}
			}
		}

		public override string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			if (CaretProvider != null) 
				return CaretProvider.GetSelection (selectionNum, out startOffset, out endOffset);
			return base.GetSelection (selectionNum, out startOffset, out endOffset);
		}

		public override bool AddSelection (int startOffset, int endOffset)
		{
			ITextRangeProvider textRange = GetTextRange (startOffset, endOffset);
			if (NSelections == 0) {
				textRange.Select ();
				return true;
			}

			try {
				textRange.AddToSelection ();
			} catch (InvalidOperationException e) {
				Log.Debug (e);
				return false;
			}
			return true;
		}

		public override bool RemoveSelection (int selectionNum)
		{
			if (textProvider.SupportedTextSelection == SupportedTextSelection.Single || selectionNum == 0) {
				if (selectionNum < 0 || selectionNum >= NSelections)
					return false;
				int offset = (CaretProvider != null ? CaretProvider.CaretOffset : 0);
				ITextRangeProvider textRange = GetTextRange (offset, offset);
				textRange.Select ();
				return true;
			}
			int startOffset, endOffset;
			string selection = GetSelection (selectionNum, out startOffset, out endOffset);
			if (selection != null && selection != String.Empty) {
				ITextRangeProvider textRange = GetTextRange (startOffset, endOffset);
				try {
					textRange.RemoveFromSelection ();
				} catch (InvalidOperationException e) {
					Log.Debug (e);
					return false;
				}

				return true;
			}
			return false;
		}

		public override bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			if (textProvider.SupportedTextSelection == SupportedTextSelection.Single) {
				if (selectionNum != 0 || NSelections != 1)
					return false;
				ITextRangeProvider textRange = GetTextRange (startOffset, endOffset);
				textRange.Select ();
				return true;
			}
			if (!RemoveSelection (selectionNum))
				return false;
			return AddSelection (startOffset, endOffset);
		}

#endregion

#region Private Methods
		private Atk.Attribute [] GetAttributesInRange (int start, int end)
		{
			CG.List<Atk.Attribute> attrs = new CG.List<Atk.Attribute> ();

			IRawElementProviderSimple prov = resource.Provider;
			if (prov == null) {
				return attrs.ToArray ();
			}

			ITextProvider textProvider
				= prov.GetPatternProvider (TextPatternIdentifiers.Pattern.Id)
					as ITextProvider;
			if (textProvider == null) {
				return attrs.ToArray ();
			}

			ITextRangeProvider textRange = GetTextRange (start, end);
			
			foreach (Atk.TextAttribute attr in SUPPORTED_ATTRS)
				AddTextAttribute (attrs, attr, textRange);

			return attrs.ToArray ();
		}

		private void AddTextAttribute (CG.List<Atk.Attribute> attrs,
		                               Atk.TextAttribute atkAttr,
		                               ITextRangeProvider textRange)
		{
			if (textRange == null) {
				return;
			}

			string name = Atk.TextAdapter.AttributeGetName (atkAttr);
			string val = null;
			object tmp;

			switch (atkAttr) {
			case Atk.TextAttribute.Style:
				if (IsAttrNotNullOrMultiValued (TextPattern.IsItalicAttribute.Id,
				                                textRange, out tmp))
					val = ((bool) tmp) ? "italic" : "normal";
				break;
			case Atk.TextAttribute.Justification:
				if (!IsAttrNotNullOrMultiValued (TextPattern.HorizontalTextAlignmentAttribute.Id,
				                                 textRange, out tmp))
					break;

				HorizontalTextAlignment align = (HorizontalTextAlignment) tmp;
				if (align == HorizontalTextAlignment.Left)
					val = "left";
				else if (align == HorizontalTextAlignment.Right)
					val = "right";
				else if (align == HorizontalTextAlignment.Centered)
					val = "center";
				else if (align == HorizontalTextAlignment.Justified)
					val = "fill";
				break;
			case Atk.TextAttribute.FgColor:
				if (IsAttrNotNullOrMultiValued (TextPattern.ForegroundColorAttribute.Id,
				                                textRange, out tmp)) {
					Color fgColor = Color.FromArgb ((int) tmp);
					val = String.Format ("{0},{1},{2}", fgColor.R, fgColor.G, fgColor.B);
				}
				break;
			case Atk.TextAttribute.BgColor:
				if (IsAttrNotNullOrMultiValued (TextPattern.BackgroundColorAttribute.Id,
				                                textRange, out tmp)) {
					Color fgColor = Color.FromArgb ((int) tmp);
					val = String.Format ("{0},{1},{2}", fgColor.R, fgColor.G, fgColor.B);
				}
				break;
			case Atk.TextAttribute.FamilyName:
				if (IsAttrNotNullOrMultiValued (TextPattern.FontNameAttribute.Id,
				                                textRange, out tmp)) 
					val = (string) tmp;
				break;
			case Atk.TextAttribute.Weight:
				if (IsAttrNotNullOrMultiValued (TextPattern.FontWeightAttribute.Id,
				                                textRange, out tmp)) 
					val = ((int) tmp).ToString ();
				break;
			case Atk.TextAttribute.Strikethrough:
				if (IsAttrNotNullOrMultiValued (TextPattern.StrikethroughStyleAttribute.Id,
				                                textRange, out tmp)) {
					TextDecorationLineStyle strikeStyle = (TextDecorationLineStyle) tmp;
					val = strikeStyle == TextDecorationLineStyle.None ? "false" : "true";
				}
				break;
			case Atk.TextAttribute.Underline:
				if (!IsAttrNotNullOrMultiValued (TextPattern.UnderlineStyleAttribute.Id,
				                                 textRange, out tmp))
					break;

				TextDecorationLineStyle underlineStyle = (TextDecorationLineStyle) tmp;
				if (underlineStyle == TextDecorationLineStyle.None)
					val = "none";
				else if (underlineStyle == TextDecorationLineStyle.Double
				         || underlineStyle == TextDecorationLineStyle.DoubleWavy)
					val = "double";
				else
					val = "single";
				break;
			case Atk.TextAttribute.PixelsBelowLines:
				if (IsAttrNotNullOrMultiValued (TextPattern.IndentationTrailingAttribute.Id,
				                                textRange, out tmp))
					val = ((int) tmp).ToString ();
				break;
			case Atk.TextAttribute.PixelsAboveLines:
				if (IsAttrNotNullOrMultiValued (TextPattern.IndentationLeadingAttribute.Id,
				                                textRange, out tmp))
					val = ((int) tmp).ToString ();
				break;
			case Atk.TextAttribute.Editable:
				if (IsAttrNotNullOrMultiValued (TextPattern.IsReadOnlyAttribute.Id,
				                                textRange, out tmp))
					val = !((bool) tmp) ? "true" : "false";
				break;
			case Atk.TextAttribute.Invisible:
				if (IsAttrNotNullOrMultiValued (TextPattern.IsHiddenAttribute.Id,
				                                textRange, out tmp))
					val = ((bool) tmp) ? "true" : "false";
				break;
			case Atk.TextAttribute.Indent:
				if (IsAttrNotNullOrMultiValued (TextPattern.IndentationFirstLineAttribute.Id,
				                                textRange, out tmp))
					val = ((int) tmp).ToString ();
				break;
			}
			
			if (val != null) {
				attrs.Add (new Atk.Attribute {Name = name, Value = val});
			}
		}

		private bool IsAttrNotNullOrMultiValued (int providerAttrId,
		                                         ITextRangeProvider prov,
		                                         out object val)
		{
			val = prov.GetAttributeValue (providerAttrId);
			return !((val == null) || (val == TextPattern.MixedAttributeValue));
		}

		private ITextRangeProvider GetTextRange (int start, int end)
		{
			ITextRangeProvider textRange = textProvider.DocumentRange;
			if (start >= 0 && end > 0) {
				textRange.MoveEndpointByUnit (TextPatternRangeEndpoint.Start,
				                              TextUnit.Character, start);
				textRange.MoveEndpointByRange (TextPatternRangeEndpoint.End,
				                               textRange, TextPatternRangeEndpoint.Start);
				textRange.MoveEndpointByUnit (TextPatternRangeEndpoint.End,
				                              TextUnit.Character, end - start);
			}
			return textRange;
		}
#endregion

#region Private Fields
		private ITextProvider textProvider;

		// TODO: put these somewhere shared with provider
		// Font weights
		private const int LOGFONT_NORMAL = 400;
		private const int LOGFONT_BOLD = 700;

		private static readonly Atk.TextAttribute [] SUPPORTED_ATTRS = new Atk.TextAttribute [] {
			Atk.TextAttribute.Style,
			Atk.TextAttribute.Justification,
			Atk.TextAttribute.FgColor,
			Atk.TextAttribute.BgColor,
			Atk.TextAttribute.FamilyName,
			Atk.TextAttribute.Weight,
			Atk.TextAttribute.Strikethrough,
			Atk.TextAttribute.Underline,
			Atk.TextAttribute.PixelsBelowLines,
			Atk.TextAttribute.PixelsAboveLines,
			Atk.TextAttribute.Editable,
			Atk.TextAttribute.Invisible,
			Atk.TextAttribute.Indent
		};
#endregion
	}
}
