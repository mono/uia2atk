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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;

namespace System.Windows.Automation
{
	public static class TextPatternIdentifiers
	{
#region Constructor
		private const int PatternId = 10014;
		private const int CaretMovedEventId = 60004;
		private const int TextChangedEventId = 20015;
		private const int TextSelectionChangedEventId = 20014;

		private const int AnimationStyleAttributeId = 40000;
		private const int BackgroundColorAttributeId = 40001;
		private const int BulletStyleAttributeId = 40002;
		private const int CapStyleAttributeId = 40003;
		private const int CultureAttributeId = 40004;
		private const int FontNameAttributeId = 40005;
		private const int FontSizeAttributeId = 40006;
		private const int FontWeightAttributeId = 40007;
		private const int ForegroundColorAttributeId = 40008;
		private const int HorizontalTextAlignmentAttributeId = 40009;
		private const int IndentationFirstLineAttributeId = 40010;
		private const int IndentationLeadingAttributeId = 40011;
		private const int IndentationTrailingAttributeId = 40012;
		private const int IsHiddenAttributeId = 40013;
		private const int IsItalicAttributeId = 40014;
		private const int IsReadOnlyAttributeId = 40015;
		private const int IsSubscriptAttributeId = 40016;
		private const int IsSuperscriptAttributeId = 40017;
		private const int MarginBottomAttributeId = 40018;
		private const int MarginLeadingAttributeId = 40019;
		private const int MarginTopAttributeId = 40020;
		private const int MarginTrailingAttributeId = 40021;
		private const int OutlineStylesAttributeId = 40022;
		private const int OverlineColorAttributeId = 40023;
		private const int OverlineStyleAttributeId = 40024;
		private const int StrikethroughColorAttributeId = 40025;
		private const int StrikethroughStyleAttributeId = 40026;
		private const int TabsAttributeId = 40027;
		private const int TextFlowDirectionsAttributeId = 40028;
		private const int UnderlineColorAttributeId = 40029;
		private const int UnderlineStyleAttributeId = 40030;
		
		static TextPatternIdentifiers ()
		{
			MixedAttributeValue = new object ();

			Pattern =
				new AutomationPattern (PatternId,
				                       "TextPatternIdentifiers.Pattern");
			CaretMovedEvent = 
				new AutomationEvent (CaretMovedEventId,
				                     "TextPatternIdentifiers.CaretMovedEvent");
			TextChangedEvent = 
				new AutomationEvent (TextChangedEventId,
				                     "TextPatternIdentifiers.TextChangedEvent");
			TextSelectionChangedEvent = 
				new AutomationEvent (TextSelectionChangedEventId,
				                     "TextPatternIdentifiers.TextSelectionChangedEvent");

			AnimationStyleAttribute =
				new AutomationTextAttribute (AnimationStyleAttributeId,
			                                     "TextPatternIdentifiers.AnimationStyleAttribute");
			BackgroundColorAttribute =
				new AutomationTextAttribute (BackgroundColorAttributeId,
			                                     "TextPatternIdentifiers.BackgroundColorAttribute");
			BulletStyleAttribute =
				new AutomationTextAttribute (BulletStyleAttributeId,
			                                     "TextPatternIdentifiers.BulletStyleAttribute");
			CapStyleAttribute =
				new AutomationTextAttribute (CapStyleAttributeId,
			                                     "TextPatternIdentifiers.CapStyleAttribute");
			CultureAttribute =
				new AutomationTextAttribute (CultureAttributeId,
			                                     "TextPatternIdentifiers.CultureAttribute");
			FontNameAttribute =
				new AutomationTextAttribute (FontNameAttributeId,
			                                     "TextPatternIdentifiers.FontNameAttribute");
			FontSizeAttribute =
				new AutomationTextAttribute (FontSizeAttributeId,
			                                     "TextPatternIdentifiers.FontSizeAttribute");
			FontWeightAttribute =
				new AutomationTextAttribute (FontWeightAttributeId,
			                                     "TextPatternIdentifiers.FontWeightAttribute");
			ForegroundColorAttribute =
				new AutomationTextAttribute (ForegroundColorAttributeId,
			                                     "TextPatternIdentifiers.ForegroundColorAttribute");
			HorizontalTextAlignmentAttribute =
				new AutomationTextAttribute (HorizontalTextAlignmentAttributeId,
			                                     "TextPatternIdentifiers.HorizontalTextAlignmentAttribute");
			IndentationFirstLineAttribute =
				new AutomationTextAttribute (IndentationFirstLineAttributeId,
			                                     "TextPatternIdentifiers.IndentationFirstLineAttribute");
			IndentationLeadingAttribute =
				new AutomationTextAttribute (IndentationLeadingAttributeId,
			                                     "TextPatternIdentifiers.IndentationLeadingAttribute");
			IndentationTrailingAttribute =
				new AutomationTextAttribute (IndentationTrailingAttributeId,
			                                     "TextPatternIdentifiers.IndentationTrailingAttribute");
			IsHiddenAttribute =
				new AutomationTextAttribute (IsHiddenAttributeId,
			                                     "TextPatternIdentifiers.IsHiddenAttribute");
			IsItalicAttribute =
				new AutomationTextAttribute (IsItalicAttributeId,
			                                     "TextPatternIdentifiers.IsItalicAttribute");
			IsReadOnlyAttribute =
				new AutomationTextAttribute (IsReadOnlyAttributeId,
			                                     "TextPatternIdentifiers.IsReadOnlyAttribute");
			IsSubscriptAttribute =
				new AutomationTextAttribute (IsSubscriptAttributeId,
			                                     "TextPatternIdentifiers.IsSubscriptAttribute");
			IsSuperscriptAttribute =
				new AutomationTextAttribute (IsSuperscriptAttributeId,
			                                     "TextPatternIdentifiers.IsSuperscriptAttribute");
			MarginBottomAttribute =
				new AutomationTextAttribute (MarginBottomAttributeId,
			                                     "TextPatternIdentifiers.MarginBottomAttribute");
			MarginLeadingAttribute =
				new AutomationTextAttribute (MarginLeadingAttributeId,
			                                     "TextPatternIdentifiers.MarginLeadingAttribute");
			MarginTopAttribute =
				new AutomationTextAttribute (MarginTopAttributeId,
			                                     "TextPatternIdentifiers.MarginTopAttribute");
			MarginTrailingAttribute =
				new AutomationTextAttribute (MarginTrailingAttributeId,
			                                     "TextPatternIdentifiers.MarginTrailingAttribute");
			OutlineStylesAttribute =
				new AutomationTextAttribute (OutlineStylesAttributeId,
			                                     "TextPatternIdentifiers.OutlineStylesAttribute");
			OverlineColorAttribute =
				new AutomationTextAttribute (OverlineColorAttributeId,
			                                     "TextPatternIdentifiers.OverlineColorAttribute");
			OverlineStyleAttribute =
				new AutomationTextAttribute (OverlineStyleAttributeId,
			                                     "TextPatternIdentifiers.OverlineStyleAttribute");
			StrikethroughColorAttribute =
				new AutomationTextAttribute (StrikethroughColorAttributeId,
			                                     "TextPatternIdentifiers.StrikethroughColorAttribute");
			StrikethroughStyleAttribute =
				new AutomationTextAttribute (StrikethroughStyleAttributeId,
			                                     "TextPatternIdentifiers.StrikethroughStyleAttribute");
			TabsAttribute =
				new AutomationTextAttribute (TabsAttributeId,
			                                     "TextPatternIdentifiers.TabsAttribute");
			TextFlowDirectionsAttribute =
				new AutomationTextAttribute (TextFlowDirectionsAttributeId,
			                                     "TextPatternIdentifiers.TextFlowDirectionsAttribute");
			UnderlineColorAttribute =
				new AutomationTextAttribute (UnderlineColorAttributeId,
			                                     "TextPatternIdentifiers.UnderlineColorAttribute");
			UnderlineStyleAttribute =
				new AutomationTextAttribute (UnderlineStyleAttributeId,
			                                     "TextPatternIdentifiers.UnderlineStyleAttribute");
		}
		
#endregion
		
#region Public Fields

		public static readonly AutomationTextAttribute AnimationStyleAttribute;

		public static readonly AutomationTextAttribute BackgroundColorAttribute;

		public static readonly AutomationTextAttribute BulletStyleAttribute;

		public static readonly AutomationTextAttribute CapStyleAttribute;

		public static readonly AutomationTextAttribute CultureAttribute;

		public static readonly AutomationTextAttribute FontNameAttribute;

		public static readonly AutomationTextAttribute FontSizeAttribute;

		public static readonly AutomationTextAttribute FontWeightAttribute;

		public static readonly AutomationTextAttribute ForegroundColorAttribute;

		public static readonly AutomationTextAttribute HorizontalTextAlignmentAttribute;

		public static readonly AutomationTextAttribute IndentationFirstLineAttribute;

		public static readonly AutomationTextAttribute IndentationLeadingAttribute;

		public static readonly AutomationTextAttribute IndentationTrailingAttribute;

		public static readonly AutomationTextAttribute IsHiddenAttribute;

		public static readonly AutomationTextAttribute IsItalicAttribute;

		public static readonly AutomationTextAttribute IsReadOnlyAttribute;

		public static readonly AutomationTextAttribute IsSubscriptAttribute;

		public static readonly AutomationTextAttribute IsSuperscriptAttribute;

		public static readonly AutomationTextAttribute MarginBottomAttribute;

		public static readonly AutomationTextAttribute MarginLeadingAttribute;

		public static readonly AutomationTextAttribute MarginTopAttribute;

		public static readonly AutomationTextAttribute MarginTrailingAttribute;

		public static readonly Object MixedAttributeValue;

		public static readonly AutomationTextAttribute OutlineStylesAttribute;

		public static readonly AutomationTextAttribute OverlineColorAttribute;

		public static readonly AutomationTextAttribute OverlineStyleAttribute;

		public static readonly AutomationPattern Pattern;
		
		public static readonly AutomationTextAttribute StrikethroughColorAttribute;

		public static readonly AutomationTextAttribute StrikethroughStyleAttribute;

		public static readonly AutomationTextAttribute TabsAttribute;

		internal static readonly AutomationEvent CaretMovedEvent;

		public static readonly AutomationEvent TextChangedEvent;

		public static readonly AutomationTextAttribute TextFlowDirectionsAttribute;
		
		public static readonly AutomationEvent TextSelectionChangedEvent;

		public static readonly AutomationTextAttribute UnderlineColorAttribute;

		public static readonly AutomationTextAttribute UnderlineStyleAttribute;
		
#endregion
	}
}
