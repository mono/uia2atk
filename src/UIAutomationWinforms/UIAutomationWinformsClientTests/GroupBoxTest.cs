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
        // According to http://msdn.microsoft.com/en-us/library/ms742689.aspx
        [TestFixture]
        [Description("Tests SWF.GroupBox as ControlType.Group")]
        public class GroupBoxTest : BaseTest
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
                [Description("Value: See notes. | Notes: The outermost rectangle that contains the "
                        + "whole control.")]
                public override void MsdnBoundingRectanglePropertyTest()
                {
                        base.MsdnBoundingRectanglePropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: Supported if there is a bounding rectangle. "
                        + "If not every point within the bounding rectangle is clickable, and you "
                        + "perform specialized hit testing, then override and provide a clickable point.")]
                public override void MsdnClickablePointPropertyTest()
                {
                        base.MsdnClickablePointPropertyTest();
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: If the control can receive keyboard focus, it "
                        + "must support this property.")]
                public override void MsdnIsKeyboardFocusablePropertyTest()
                {
                        base.MsdnIsKeyboardFocusablePropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: The group control typically gets its name from "
                        + "the text that labels the control.")]
                public override void MsdnNamePropertyTest()
                {
                        GroupBox groupBox = new GroupBox();
                        AutomationElement child = GetAutomationElementFromControl(groupBox);
                        Assert.AreEqual(groupBox.Text,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");

                        groupBox.Text = "Unhappy groupBox";
                        Assert.AreEqual(groupBox.Text,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: Group controls are typically self-labeling. In "
                        + "these cases return null here. If there is a static text label for the group "
                        + "then that must be returned as the value of the LabeledBy property.")]
                public override void MsdnLabeledByPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LabeledByProperty, true),
                                "LabeledByProperty");
                }

                [Test]
                [Description("Value: Group | Notes: This value is the same for all UI frameworks.")]
                public override void MsdnControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(ControlType.Group,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty, true),
                                "ControlTypeProperty");
                }

                [Test]
                [Description("Value: \"group\" | Notes: Localized string corresponding to the Group "
                        + "control type.")]
                public override void MsdnLocalizedControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual("group",
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
                                "LocalizedControlType");
                }

                [Test]
                [Description("Value: True | Notes: The group control is always included in the content "
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
                [Description("Value: True | Notes: The calendar group is always included in the control "
                        + "view of the UI Automation tree.")]
                public override void MsdnIsControlElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsControlElementProperty, true),
                                "IsControlElementProperty");
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
                [Description("Is not listed. We are using string.Empty as default value.")]
                public override void MsdnAccessKeyPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(string.Empty,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.AccessKeyProperty, true),
                                "AccessKeyProperty");
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
                [Description("Is not listed. We are using false as default value.")]
                public override void MsdnIsPasswordPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(false,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsPasswordProperty, true),
                                "IsPasswordProperty");
                }

                #endregion

                #region Pattern Tests

                [Test]
                [Description("Support/Value: Depends | Notes: Group controls that can be used to show "
                        + "or hide information must support the Expand Collapse pattern.")]
                public override void MsdnExpandCollapsePatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsFalse(SupportsPattern(element, ExpandCollapsePattern.Pattern),
                                "ExpandCollapsePattern SHOULD NOT be supported");
                }

                #endregion

                #region Protected Methods

                protected override Control GetControl()
                {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Text = "I'm a happy SWF groupBox :)";

                        return groupBox;
                }

                #endregion
        }
}
