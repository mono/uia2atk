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
        // According to http://msdn.microsoft.com/en-us/library/ms750425.aspx
        [TestFixture]
        [Description("Tests SWF.ToolBar as ControlType.ToolBar")]
        public class ToolBarTest : BaseTest
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
                [Description("Value: See notes. | Notes: Supported if there is a bounding rectangle. If "
                        + "not every point within the bounding rectangle is clickable, and you perform "
                        + "specialized hit testing, then override and provide a clickable point.")]
                public override void MsdnClickablePointPropertyTest()
                {
                        base.MsdnClickablePointPropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: If the control can receive keyboard focus, it "
                        + "must support this property.")]
                public override void MsdnIsKeyboardFocusablePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(false,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsKeyboardFocusableProperty, true),
                                "IsKeyboardFocusableProperty");
                }

                [Test]
                [Description("Value: Depends | Notes: The tool bar control does not need a name unless "
                        + "more than one is used within an application. If more than one is present, "
                        + "each must have a distinguishing name (for example, Formatting or Outlining).")]
                public override void MsdnNamePropertyTest()
                {
                        ToolBar toolBar = GetControl() as ToolBar;
                        AutomationElement child = GetAutomationElementFromControl(toolBar);
                        Assert.AreEqual(toolBar.Text,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");

                        toolBar.Text = "Unhappy toolBar";
                        Assert.AreEqual(toolBar.Text,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: Null | Notes: Tool bar controls never have a label.")]
                public override void MsdnLabeledByPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LabeledByProperty, true),
                                "LabeledByProperty");
                }

                [Test]
                [Description("Value: ToolBar | Notes: This value is the same for all UI frameworks.")]
                public override void MsdnControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(ControlType.ToolBar,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty, true),
                                "ControlTypeProperty");
                }

                [Test]
                [Description("Value: \"tool bar\" | Notes: Localized string corresponding to the ToolBar "
                        + "control type.")]
                public override void MsdnLocalizedControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual("tool bar",
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
                                "LocalizedControlType");
                }

                [Test]
                [LameSpec]
                [Description("Value: True | Notes: The tool bar control is always content.")]
                public override void MsdnIsContentElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsContentElementProperty, true),
                                "IsContentElementProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: True | Notes: The tool bar control must always be a control.")]
                public override void MsdnIsControlElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsControlElementProperty, true),
                                "IsControlElementProperty");
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
                [Description("Support/Value: Depends | Notes: If the tool bar can be expanded and "
                        + "collapsed to show more items, then it must support this pattern.")]
                public override void  MsdnExpandCollapsePatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsFalse(SupportsPattern(element, ExpandCollapsePattern.Pattern),
                                "ExpandCollapsePattern SHOULD NOT be supported");
                }

                [Test]
                [Description("Support/Value: Depends | Notes: If the tool bar can be docked to different "
                        + "parts of the screen, then it must support this pattern.")]
                public override void MsdnDockPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsFalse(SupportsPattern(element, DockPattern.Pattern),
                                "DockPattern SHOULD NOT be supported");
                }

                [Test]
                [Description("Support/Value: Depends | Notes: If the tool bar can be resized, rotated, "
                        + "or moved, it must support this pattern.")]
                public override void MsdnTransformPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsFalse(SupportsPattern(element, TransformPattern.Pattern),
                                "TransformPattern SHOULD NOT be supported");
                }

                #endregion

                #region Protected Methods

                protected override Control  GetControl()
                {
                        ToolBar toolBar = new ToolBar();
                        toolBar.Text = "I'm a happy SWF toolBar :)";

                        return toolBar;
                }

                #endregion
        }
}
