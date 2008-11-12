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
//	Neville Gao <nevillegao@gmail.com>
// 
using System;
using System.Windows.Automation;
using System.Windows.Forms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms.Client
{
        // According to http://msdn.microsoft.com/en-us/library/ms748367.aspx
        [TestFixture]
        [Description("Tests SWF.StatusBarPanel as ControlType.Edit")]
        public class StatusBarPanelTest : BaseTest
        {
                #region Properties Test

                [Test]
                [Description("Value: See notes. | Notes: The value of this property needs to be unique "
                        + "across all controls in an application.")]
                public override void MsdnAutomationIdPropertyTest()
                {
                        base.MsdnAutomationIdPropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: The outermost rectangle that contains the whole "
                        + "control.")]
                public override void MsdnBoundingRectanglePropertyTest()
                {
                        base.MsdnBoundingRectanglePropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: The edit control must have a clickable point "
                        + "that gives input focus to the edit portion of the control when a user clicks "
                        + "the mouse there.")]
                public override void MsdnClickablePointPropertyTest()
                {
                        base.MsdnClickablePointPropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: If the control can receive keyboard focus, it "
                        + "must support this property.")]
                public override void MsdnIsKeyboardFocusablePropertyTest()
                {
                        base.MsdnIsKeyboardFocusablePropertyTest();
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: The name of the edit control is typically "
                        + "generated from a static text label. If there is not a static text label, a "
                        + "property value for Name must be assigned by the application developer. The "
                        + "Name property should never contain the textual contents of the edit control.")]
                public override void MsdnNamePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual("Panel: 0",
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: If there is a static text label associated "
                        + "with the control, then this property must expose a reference to that control. "
                        + "If the text control is a subcomponent of another control, it will not have "
                        + "a LabeledBy property set.")]
                public override void MsdnLabeledByPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LabeledByProperty, true),
                                "LabeledByProperty");
                }

                [Test]
                [Description("Value: Edit | Notes: This value is the same for all UI frameworks.")]
                public override void MsdnControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(ControlType.Edit,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty, true),
                                "ControlTypeProperty");
                }

                [Test]
                [Description("Value: \"edit\" | Notes: Localized string corresponding to the Edit control "
                        + "type.")]
                public override void MsdnLocalizedControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual("edit",
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
                                "LocalizedControlType");
                }

                [Test]
                [Description("Value: True | Notes: The edit control is always included in the content "
                        + "view of the UI Automation tree.")]
                public override void MsdnIsContentElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsContentElementProperty, true),
                                "IsContentElementProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: True | Notes: The edit control is always included in the control "
                        + "view of the UI Automation tree.")]
                public override void MsdnIsControlElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsControlElementProperty, true),
                                "IsControlElementProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: Must be set to true on edit controls that contain "
                        + "passwords. If an edit control does contain Password contents then this property "
                        + "can be used by a screen reader to determine whether keystrokes should be read "
                        + "out as the user types them.")]
                public override void MsdnIsPasswordPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(false,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsPasswordProperty, true),
                                "IsPasswordProperty");
                }

                [Test]
                [NotListed]
                [Description("Is not listed. We are using false as default value.")]
                public override void MsdnIsOffscreenPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(false,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsOffscreenProperty, true),
                                "IsOffscreenProperty");
                }

                [Test]
                [NotListed]
                [Description("Is not listed. We are using null as default value.")]
                public override void MsdnOrientationPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.OrientationProperty, true),
                                "OrientationProperty");
                }

                [Test]
                [NotListed]
                [Description("Is not listed. We are using string.Empty as default value.")]
                public override void MsdnHelpTextPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(string.Empty,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.HelpTextProperty, true),
                                "HelpTextProperty");
                }

                [Test]
                [NotListed]
                [Description("Is not listed. We are using null as default value.")]
                public override void MsdnAcceleratorKeyPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.AcceleratorKeyProperty, true),
                                "AcceleratorKeyProperty");
                }

                [Test]
                [NotListed]
                [Description("Is not listed. We are using string.Empty as default value.")]
                public override void MsdnAccessKeyPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(string.Empty,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.AccessKeyProperty, true),
                                "AccessKeyProperty");
                }

                #endregion

                #region Pattern Tests

                [Test]
                [LameSpec]
                [Description("Support/Value: Required | Notes: All edit controls must support the Text "
                        + "control pattern because detailed information must always be available for "
                        + "assistive technology clients.")]
                public override void MsdnTextPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsTrue(SupportsPattern(element, TextPattern.Pattern),
                                "TextPattern SHOULD BE supported.");
                }

                [Test]
                [Description("Value: Depends | Notes: All edit controls that take a string must expose "
                        + "the Value pattern.")]
                public override void MsdnValuePatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsTrue(SupportsPattern(element, ValuePattern.Pattern),
                                "ValuePattern SHOULD BE supported.");
                }

                [Test]
                [Description("Support/Value: Depends | Notes: All edit controls that take a numeric range "
                        + "must expose Range Value control pattern.")]
                public override void MsdnRangeValuePatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsFalse(SupportsPattern(element, RangeValuePattern.Pattern),
                                "RangeValuePattern SHOULD NOT be supported.");
                }

                [Test]
                public override void MsdnGridItemPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsTrue(SupportsPattern(element, GridItemPattern.Pattern),
                                "GridItemPattern SHOULD be supported.");
                }

                #endregion

                #region Protected Methods

                protected override AutomationElement GetAutomationElement()
                {
                        StatusBar statusBar = new StatusBar();
                        for (int i = 0; i < 10; ++i)
                                statusBar.Panels.Add(string.Format("Panel: {0}", i));

                        AutomationElement statusBarElement = GetAutomationElementFromControl(statusBar);

                        AutomationElement child = TreeWalker.ContentViewWalker.GetFirstChild(statusBarElement);
                        while (child != null)
                        {
                                if (child.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty)
                                        == ControlType.Edit)
                                        return child;
                                else
                                        child = TreeWalker.ContentViewWalker.GetNextSibling(child);
                        }

                        return null;
                }

                protected override Control GetControl()
                {
                        return null;
                }

                #endregion
        }
}
