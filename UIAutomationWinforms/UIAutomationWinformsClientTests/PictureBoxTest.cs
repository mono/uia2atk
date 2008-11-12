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
        // According to http://msdn.microsoft.com/en-us/library/ms749129.aspx
        [TestFixture]
        [Description("Tests SWF.PictureBox as ControlType.Pane")]
        public class PictureBoxTest : BaseTest
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
                [LameSpec]
                [Description("Value: See notes. | Notes: If the control can receive keyboard focus, it "
                        + "must support this property.")]
                public override void MsdnIsKeyboardFocusablePropertyTest()
                {
                        base.MsdnIsKeyboardFocusablePropertyTest();
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: The value for this property must always be a "
                        + "clear, concise and meaningful title.")]
                public override void MsdnNamePropertyTest()
                {
                        PictureBox pictureBox = new PictureBox();
                        AutomationElement child = GetAutomationElementFromControl(pictureBox);
                        Assert.AreEqual(pictureBox.Name,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");

                        pictureBox.Name = "Unhappy pictureBox";
                        Assert.AreEqual(pictureBox.Name,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");
                }

                [Test]
                [Description("Value: See notes. | Notes: This property exposes a clickable point of the "
                        + "pane control that causes the pane to become focused when it is clicked.")]
                public override void MsdnClickablePointPropertyTest()
                {
                        base.MsdnClickablePointPropertyTest();
                }

                
                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: Pane controls typically do not have a static "
                        + "label. If there is a static text label, it should be exposed through this property.")]
                public override void MsdnLabeledByPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LabeledByProperty, true),
                                "LabeledByProperty");
                }

                [Test]
                [Description("Value: Pane | Notes: This value is the same for all UI frameworks.")]
                public override void MsdnControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(ControlType.Pane,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty, true),
                                "ControlTypeProperty");
                }

                [Test]
                [Description("Value: \"pane\" | Notes: Localized string corresponding to the Pane "
                        + "control type.")]
                public override void MsdnLocalizedControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual("pane",
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
                                "LocalizedControlType");
                }

                [Test]
                [Description("Value: True | Notes: Pane controls are always included in the content "
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
                [Description("Value: True | Notes: Pane controls are always included in the control "
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
                [Description("Value: \"\" | Notes: The help text for pane controls should explain why "
                        + "the purpose of the frame and how it relates to other frames. A description "
                        + "is necessary if the purpose and relationship of frames is not clear from the "
                        + "value of the NameProperty.")]
                public override void MsdnHelpTextPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(string.Empty,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.HelpTextProperty, true),
                                "HelpTextProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: See notes. | Notes: If a specific key combination gives focus to "
                        + "the pane then that information should be exposed through this property.")]
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
                [LameSpec]
                [Description("Support/Value: Depends | Notes: Implement this control pattern if the pane "
                        + "control can be moved, resized, or rotated on the screen.")]
                public override void MsdnTransformPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsTrue(SupportsPattern(element, TransformPattern.Pattern),
                                "TransformPattern SHOULD BE supported");
                }

                [Test]
                [Description("Support/Value: Never | Notes: If you need to implement this control "
                        + "pattern, your control should be based on the Window control type. ")]
                public override void MsdnWindowPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsFalse(SupportsPattern(element, WindowPattern.Pattern),
                                "WindowPattern SHOULD NOT be supported");
                }

                [Test]
                [LameSpec]
                [Description("Support/Value: Depends | Notes: Implement this control pattern if the "
                        + "pane control can be docked.")]
                public override void MsdnDockPatternTest()
                {
                        AutomationElement elment = GetAutomationElement();
                        Assert.IsTrue(SupportsPattern(elment, DockPattern.Pattern),
                                "DockPattern SHOULD BE supported");
                }

                #endregion

                #region Protected Methods

                protected override Control GetControl()
                {
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Name = "I'm a happy SWF pictureBox :)";

                        return pictureBox;
                }

                #endregion
        }
}
