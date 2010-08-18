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
        // According to http://msdn.microsoft.com/en-us/library/ms745809.aspx
        [TestFixture]
        [Description("Tests SWF.StatusStrip as ControlType.StatusBar")]
        public class StatusStripTest : BaseTest
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
                [Description("Value: See notes. | Notes: The bounding rectangle of a status bar must "
                        + "encompass all of the controls contained within it.")]
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

                // ERROR!!!!
                [Test]
                public override void MsdnIsKeyboardFocusablePropertyTest()
                {
                        base.MsdnIsKeyboardFocusablePropertyTest();
                }

                [Test]
                [Description("Value: See notes. | Notes: The status bar control does not need a name "
                        + "unless more than one is used within an application. In this case, distinguish "
                        + "each bar with names such as \"Internet Status\" or \"Application Status.\"")]
                public override void MsdnNamePropertyTest()
                {
                        StatusStrip statusStrip = new StatusStrip();
                        AutomationElement child = GetAutomationElementFromControl(statusStrip);
                        Assert.AreEqual(statusStrip.Text,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");

                        statusStrip.Text = "Unhappy statusStrip";
                        Assert.AreEqual(statusStrip.Text,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty, true),
                                "NameProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: Null | Notes: The status bar control usually does not have a label.")]
                public override void MsdnLabeledByPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LabeledByProperty, true),
                                "LabeledByProperty");
                }

                [Test]
                [Description("Value: StatusBar | Notes: This value is the same for all UI frameworks.")]
                public override void MsdnControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(ControlType.StatusBar,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty, true),
                                "ControlTypeProperty");
                }

                [Test]
                [Description("Value: \"status bar\" | Notes: Localized string corresponding to the "
                        + "StatusBar control type.")]
                public override void MsdnLocalizedControlTypePropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual("status bar",
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.LocalizedControlTypeProperty, true),
                                "LocalizedControlType");
                }

                [Test]
                [Description("Value: True | Notes: The status bar control always contains content.")]
                public override void MsdnIsContentElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsContentElementProperty, true),
                                "IsContentElementProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: True | Notes: The status bar control is always a control.")]
                public override void MsdnIsControlElementPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(true,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsControlElementProperty, true),
                                "IsControlElementProperty");
                }

                [Test]
                [Description("Value: Depends | Notes: A status bar control will return True for this "
                        + "property if it is not currently visible on the screen.")]
                public override void MsdnIsOffscreenPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(false,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.IsOffscreenProperty, true),
                                "IsOffscreenProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: Depends | Notes: The value of the control's orientation: horizontal "
                        + "or vertical.")]
                public override void MsdnOrientationPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.OrientationProperty, true),
                                "OrientationProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: \"\" | Notes: Status bars do not have Help text.")]
                public override void MsdnHelpTextPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(string.Empty,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.HelpTextProperty, true),
                                "HelpTextProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: Null | Notes: Status bars do not have accelerator keys.")]
                public override void MsdnAcceleratorKeyPropertyTest()
                {
                        AutomationElement child = GetAutomationElement();
                        Assert.AreEqual(null,
                                child.GetCurrentPropertyValue(AutomationElementIdentifiers.AcceleratorKeyProperty, true),
                                "AcceleratorKeyProperty");
                }

                [Test]
                [LameSpec]
                [Description("Value: \"\" | Notes: Status bars do not receive keyboard focus. Assistive "
                        + "technologies are able to programmatically read the contents.")]
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
                [Description("Support/Value: Optional | Notes: Status bar controls should support the "
                        + "Grid control pattern so that individual pieces can be monitored and easily "
                        + "referenced for information.")]
                public override void  MsdnGridPatternTest()
                {
                        AutomationElement element = GetAutomationElement();
                        Assert.IsTrue(SupportsPattern(element, GridPattern.Pattern),
                                "GridPattern SHOULD BE supported");
                        
                }

                #endregion

                #region Protected Methods

                protected override Control  GetControl()
                {
                        StatusStrip statusStrip = new StatusStrip();
                        statusStrip.Text = "I'm a happy SWF statusStrip :)";

                        return statusStrip;
                }

                #endregion
        }
}
