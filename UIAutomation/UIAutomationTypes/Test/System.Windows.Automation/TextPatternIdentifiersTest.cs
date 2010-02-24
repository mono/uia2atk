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
using System.Windows.Automation;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation {

	[TestFixture]
	public class TextPatternIdentifiersTest {

		[Test]
		public void PatternTest ()
		{
			AutomationPattern pattern = TextPatternIdentifiers.Pattern;
			Assert.IsNotNull (pattern, "Pattern field must not be null");
			Assert.AreEqual (10014, pattern.Id, "Id");
			Assert.AreEqual ("TextPatternIdentifiers.Pattern", pattern.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (pattern, AutomationPattern.LookupById (pattern.Id), "LookupById");
		}

		[Test]
		public void TextChangedEventTest ()
		{
			AutomationEvent automationEvent = TextPatternIdentifiers.TextChangedEvent;
			Assert.IsNotNull (automationEvent, "Property field must not be null");
			Assert.AreEqual (20015, automationEvent.Id, "Id");
			Assert.AreEqual ("TextPatternIdentifiers.TextChangedEvent", automationEvent.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (automationEvent, AutomationEvent.LookupById (automationEvent.Id), "LookupById");
		}

		[Test]
		public void TextSelectionChangedEventTest ()
		{
			AutomationEvent automationEvent = TextPatternIdentifiers.TextSelectionChangedEvent;
			Assert.IsNotNull (automationEvent, "Property field must not be null");
			Assert.AreEqual (20014, automationEvent.Id, "Id");
			Assert.AreEqual ("TextPatternIdentifiers.TextSelectionChangedEvent", automationEvent.ProgrammaticName, "ProgrammaticName");
			Assert.AreEqual (automationEvent, AutomationEvent.LookupById (automationEvent.Id), "LookupById");
		}

		[Test]
		public void AnimationStyleAttributeTest ()
		{
			AutomationTextAttribute myAnimationStyleAttribute =
				TextPatternIdentifiers.AnimationStyleAttribute;
			Assert.IsNotNull (
				myAnimationStyleAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40000,
				myAnimationStyleAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.AnimationStyleAttribute",
				myAnimationStyleAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myAnimationStyleAttribute,
				AutomationTextAttribute.LookupById (myAnimationStyleAttribute.Id),
				"LookupById");
		}

		[Test]
		public void BackgroundColorAttributeTest ()
		{
			AutomationTextAttribute myBackgroundColorAttribute =
				TextPatternIdentifiers.BackgroundColorAttribute;
			Assert.IsNotNull (
				myBackgroundColorAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40001,
				myBackgroundColorAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.BackgroundColorAttribute",
				myBackgroundColorAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myBackgroundColorAttribute,
				AutomationTextAttribute.LookupById (myBackgroundColorAttribute.Id),
				"LookupById");
		}

		[Test]
		public void BulletStyleAttributeTest ()
		{
			AutomationTextAttribute myBulletStyleAttribute =
				TextPatternIdentifiers.BulletStyleAttribute;
			Assert.IsNotNull (
				myBulletStyleAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40002,
				myBulletStyleAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.BulletStyleAttribute",
				myBulletStyleAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myBulletStyleAttribute,
				AutomationTextAttribute.LookupById (myBulletStyleAttribute.Id),
				"LookupById");
		}

		[Test]
		public void CapStyleAttributeTest ()
		{
			AutomationTextAttribute myCapStyleAttribute =
				TextPatternIdentifiers.CapStyleAttribute;
			Assert.IsNotNull (
				myCapStyleAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40003,
				myCapStyleAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.CapStyleAttribute",
				myCapStyleAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myCapStyleAttribute,
				AutomationTextAttribute.LookupById (myCapStyleAttribute.Id),
				"LookupById");
		}

		[Test]
		public void CultureAttributeTest ()
		{
			AutomationTextAttribute myCultureAttribute =
				TextPatternIdentifiers.CultureAttribute;
			Assert.IsNotNull (
				myCultureAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40004,
				myCultureAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.CultureAttribute",
				myCultureAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myCultureAttribute,
				AutomationTextAttribute.LookupById (myCultureAttribute.Id),
				"LookupById");
		}

		[Test]
		public void FontNameAttributeTest ()
		{
			AutomationTextAttribute myFontNameAttribute =
				TextPatternIdentifiers.FontNameAttribute;
			Assert.IsNotNull (
				myFontNameAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40005,
				myFontNameAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.FontNameAttribute",
				myFontNameAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myFontNameAttribute,
				AutomationTextAttribute.LookupById (myFontNameAttribute.Id),
				"LookupById");
		}

		[Test]
		public void FontSizeAttributeTest ()
		{
			AutomationTextAttribute myFontSizeAttribute =
				TextPatternIdentifiers.FontSizeAttribute;
			Assert.IsNotNull (
				myFontSizeAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40006,
				myFontSizeAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.FontSizeAttribute",
				myFontSizeAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myFontSizeAttribute,
				AutomationTextAttribute.LookupById (myFontSizeAttribute.Id),
				"LookupById");
		}

		[Test]
		public void FontWeightAttributeTest ()
		{
			AutomationTextAttribute myFontWeightAttribute =
				TextPatternIdentifiers.FontWeightAttribute;
			Assert.IsNotNull (
				myFontWeightAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40007,
				myFontWeightAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.FontWeightAttribute",
				myFontWeightAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myFontWeightAttribute,
				AutomationTextAttribute.LookupById (myFontWeightAttribute.Id),
				"LookupById");
		}

		[Test]
		public void ForegroundColorAttributeTest ()
		{
			AutomationTextAttribute myForegroundColorAttribute =
				TextPatternIdentifiers.ForegroundColorAttribute;
			Assert.IsNotNull (
				myForegroundColorAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40008,
				myForegroundColorAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.ForegroundColorAttribute",
				myForegroundColorAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myForegroundColorAttribute,
				AutomationTextAttribute.LookupById (myForegroundColorAttribute.Id),
				"LookupById");
		}

		[Test]
		public void HorizontalTextAlignmentAttributeTest ()
		{
			AutomationTextAttribute myHorizontalTextAlignmentAttribute =
				TextPatternIdentifiers.HorizontalTextAlignmentAttribute;
			Assert.IsNotNull (
				myHorizontalTextAlignmentAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40009,
				myHorizontalTextAlignmentAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.HorizontalTextAlignmentAttribute",
				myHorizontalTextAlignmentAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myHorizontalTextAlignmentAttribute,
				AutomationTextAttribute.LookupById (myHorizontalTextAlignmentAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IndentationFirstLineAttributeTest ()
		{
			AutomationTextAttribute myIndentationFirstLineAttribute =
				TextPatternIdentifiers.IndentationFirstLineAttribute;
			Assert.IsNotNull (
				myIndentationFirstLineAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40010,
				myIndentationFirstLineAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IndentationFirstLineAttribute",
				myIndentationFirstLineAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIndentationFirstLineAttribute,
				AutomationTextAttribute.LookupById (myIndentationFirstLineAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IndentationLeadingAttributeTest ()
		{
			AutomationTextAttribute myIndentationLeadingAttribute =
				TextPatternIdentifiers.IndentationLeadingAttribute;
			Assert.IsNotNull (
				myIndentationLeadingAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40011,
				myIndentationLeadingAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IndentationLeadingAttribute",
				myIndentationLeadingAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIndentationLeadingAttribute,
				AutomationTextAttribute.LookupById (myIndentationLeadingAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IndentationTrailingAttributeTest ()
		{
			AutomationTextAttribute myIndentationTrailingAttribute =
				TextPatternIdentifiers.IndentationTrailingAttribute;
			Assert.IsNotNull (
				myIndentationTrailingAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40012,
				myIndentationTrailingAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IndentationTrailingAttribute",
				myIndentationTrailingAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIndentationTrailingAttribute,
				AutomationTextAttribute.LookupById (myIndentationTrailingAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IsHiddenAttributeTest ()
		{
			AutomationTextAttribute myIsHiddenAttribute =
				TextPatternIdentifiers.IsHiddenAttribute;
			Assert.IsNotNull (
				myIsHiddenAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40013,
				myIsHiddenAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IsHiddenAttribute",
				myIsHiddenAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsHiddenAttribute,
				AutomationTextAttribute.LookupById (myIsHiddenAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IsItalicAttributeTest ()
		{
			AutomationTextAttribute myIsItalicAttribute =
				TextPatternIdentifiers.IsItalicAttribute;
			Assert.IsNotNull (
				myIsItalicAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40014,
				myIsItalicAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IsItalicAttribute",
				myIsItalicAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsItalicAttribute,
				AutomationTextAttribute.LookupById (myIsItalicAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IsReadOnlyAttributeTest ()
		{
			AutomationTextAttribute myIsReadOnlyAttribute =
				TextPatternIdentifiers.IsReadOnlyAttribute;
			Assert.IsNotNull (
				myIsReadOnlyAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40015,
				myIsReadOnlyAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IsReadOnlyAttribute",
				myIsReadOnlyAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsReadOnlyAttribute,
				AutomationTextAttribute.LookupById (myIsReadOnlyAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IsSubscriptAttributeTest ()
		{
			AutomationTextAttribute myIsSubscriptAttribute =
				TextPatternIdentifiers.IsSubscriptAttribute;
			Assert.IsNotNull (
				myIsSubscriptAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40016,
				myIsSubscriptAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IsSubscriptAttribute",
				myIsSubscriptAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsSubscriptAttribute,
				AutomationTextAttribute.LookupById (myIsSubscriptAttribute.Id),
				"LookupById");
		}

		[Test]
		public void IsSuperscriptAttributeTest ()
		{
			AutomationTextAttribute myIsSuperscriptAttribute =
				TextPatternIdentifiers.IsSuperscriptAttribute;
			Assert.IsNotNull (
				myIsSuperscriptAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40017,
				myIsSuperscriptAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.IsSuperscriptAttribute",
				myIsSuperscriptAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myIsSuperscriptAttribute,
				AutomationTextAttribute.LookupById (myIsSuperscriptAttribute.Id),
				"LookupById");
		}

		[Test]
		public void MarginBottomAttributeTest ()
		{
			AutomationTextAttribute myMarginBottomAttribute =
				TextPatternIdentifiers.MarginBottomAttribute;
			Assert.IsNotNull (
				myMarginBottomAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40018,
				myMarginBottomAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.MarginBottomAttribute",
				myMarginBottomAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMarginBottomAttribute,
				AutomationTextAttribute.LookupById (myMarginBottomAttribute.Id),
				"LookupById");
		}

		[Test]
		public void MarginLeadingAttributeTest ()
		{
			AutomationTextAttribute myMarginLeadingAttribute =
				TextPatternIdentifiers.MarginLeadingAttribute;
			Assert.IsNotNull (
				myMarginLeadingAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40019,
				myMarginLeadingAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.MarginLeadingAttribute",
				myMarginLeadingAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMarginLeadingAttribute,
				AutomationTextAttribute.LookupById (myMarginLeadingAttribute.Id),
				"LookupById");
		}

		[Test]
		public void MarginTopAttributeTest ()
		{
			AutomationTextAttribute myMarginTopAttribute =
				TextPatternIdentifiers.MarginTopAttribute;
			Assert.IsNotNull (
				myMarginTopAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40020,
				myMarginTopAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.MarginTopAttribute",
				myMarginTopAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMarginTopAttribute,
				AutomationTextAttribute.LookupById (myMarginTopAttribute.Id),
				"LookupById");
		}

		[Test]
		public void MarginTrailingAttributeTest ()
		{
			AutomationTextAttribute myMarginTrailingAttribute =
				TextPatternIdentifiers.MarginTrailingAttribute;
			Assert.IsNotNull (
				myMarginTrailingAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40021,
				myMarginTrailingAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.MarginTrailingAttribute",
				myMarginTrailingAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myMarginTrailingAttribute,
				AutomationTextAttribute.LookupById (myMarginTrailingAttribute.Id),
				"LookupById");
		}

		[Test]
		public void OutlineStylesAttributeTest ()
		{
			AutomationTextAttribute myOutlineStylesAttribute =
				TextPatternIdentifiers.OutlineStylesAttribute;
			Assert.IsNotNull (
				myOutlineStylesAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40022,
				myOutlineStylesAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.OutlineStylesAttribute",
				myOutlineStylesAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myOutlineStylesAttribute,
				AutomationTextAttribute.LookupById (myOutlineStylesAttribute.Id),
				"LookupById");
		}

		[Test]
		public void OverlineColorAttributeTest ()
		{
			AutomationTextAttribute myOverlineColorAttribute =
				TextPatternIdentifiers.OverlineColorAttribute;
			Assert.IsNotNull (
				myOverlineColorAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40023,
				myOverlineColorAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.OverlineColorAttribute",
				myOverlineColorAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myOverlineColorAttribute,
				AutomationTextAttribute.LookupById (myOverlineColorAttribute.Id),
				"LookupById");
		}

		[Test]
		public void OverlineStyleAttributeTest ()
		{
			AutomationTextAttribute myOverlineStyleAttribute =
				TextPatternIdentifiers.OverlineStyleAttribute;
			Assert.IsNotNull (
				myOverlineStyleAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40024,
				myOverlineStyleAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.OverlineStyleAttribute",
				myOverlineStyleAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myOverlineStyleAttribute,
				AutomationTextAttribute.LookupById (myOverlineStyleAttribute.Id),
				"LookupById");
		}

		[Test]
		public void StrikethroughColorAttributeTest ()
		{
			AutomationTextAttribute myStrikethroughColorAttribute =
				TextPatternIdentifiers.StrikethroughColorAttribute;
			Assert.IsNotNull (
				myStrikethroughColorAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40025,
				myStrikethroughColorAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.StrikethroughColorAttribute",
				myStrikethroughColorAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myStrikethroughColorAttribute,
				AutomationTextAttribute.LookupById (myStrikethroughColorAttribute.Id),
				"LookupById");
		}

		[Test]
		public void StrikethroughStyleAttributeTest ()
		{
			AutomationTextAttribute myStrikethroughStyleAttribute =
				TextPatternIdentifiers.StrikethroughStyleAttribute;
			Assert.IsNotNull (
				myStrikethroughStyleAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40026,
				myStrikethroughStyleAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.StrikethroughStyleAttribute",
				myStrikethroughStyleAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myStrikethroughStyleAttribute,
				AutomationTextAttribute.LookupById (myStrikethroughStyleAttribute.Id),
				"LookupById");
		}

		[Test]
		public void TabsAttributeTest ()
		{
			AutomationTextAttribute myTabsAttribute =
				TextPatternIdentifiers.TabsAttribute;
			Assert.IsNotNull (
				myTabsAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40027,
				myTabsAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.TabsAttribute",
				myTabsAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTabsAttribute,
				AutomationTextAttribute.LookupById (myTabsAttribute.Id),
				"LookupById");
		}

		[Test]
		public void TextFlowDirectionsAttributeTest ()
		{
			AutomationTextAttribute myTextFlowDirectionsAttribute =
				TextPatternIdentifiers.TextFlowDirectionsAttribute;
			Assert.IsNotNull (
				myTextFlowDirectionsAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40028,
				myTextFlowDirectionsAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.TextFlowDirectionsAttribute",
				myTextFlowDirectionsAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myTextFlowDirectionsAttribute,
				AutomationTextAttribute.LookupById (myTextFlowDirectionsAttribute.Id),
				"LookupById");
		}

		[Test]
		public void UnderlineColorAttributeTest ()
		{
			AutomationTextAttribute myUnderlineColorAttribute =
				TextPatternIdentifiers.UnderlineColorAttribute;
			Assert.IsNotNull (
				myUnderlineColorAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40029,
				myUnderlineColorAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.UnderlineColorAttribute",
				myUnderlineColorAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myUnderlineColorAttribute,
				AutomationTextAttribute.LookupById (myUnderlineColorAttribute.Id),
				"LookupById");
		}

		[Test]
		public void UnderlineStyleAttributeTest ()
		{
			AutomationTextAttribute myUnderlineStyleAttribute =
				TextPatternIdentifiers.UnderlineStyleAttribute;
			Assert.IsNotNull (
				myUnderlineStyleAttribute,
				"Field must not be null.");
			Assert.AreEqual (
				40030,
				myUnderlineStyleAttribute.Id,
				"Id");
			Assert.AreEqual (
				"TextPatternIdentifiers.UnderlineStyleAttribute",
				myUnderlineStyleAttribute.ProgrammaticName,
				"ProgrammaticName");
			Assert.AreEqual (
				myUnderlineStyleAttribute,
				AutomationTextAttribute.LookupById (myUnderlineStyleAttribute.Id),
				"LookupById");
		}
	}
}
