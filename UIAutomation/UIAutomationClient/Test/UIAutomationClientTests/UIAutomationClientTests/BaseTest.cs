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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

using NUnit.Framework;

namespace MonoTests.System.Windows.Automation
{
	public class BaseTest
	{
		#region Setup/Teardown

		protected Process p;
		protected AutomationElement testFormElement;
		protected AutomationElement groupBox1Element;
		protected AutomationElement button1Element;
		protected AutomationElement button2Element;
		protected AutomationElement button3Element;
		protected AutomationElement button4Element;
		protected AutomationElement button5Element;
		protected AutomationElement button6Element;
		protected AutomationElement button7Element;
		protected AutomationElement label1Element;
		protected AutomationElement textbox1Element;
		protected AutomationElement textbox2Element;
		protected AutomationElement textbox3Element;
		protected AutomationElement tb3horizontalScrollBarElement;
		protected AutomationElement tb3verticalScrollBarElement;
		//protected AutomationElement horizontalMenuStripElement;
		//protected AutomationElement verticalMenuStripElement;

		public static Process StartApplication (string name, string arguments)
		{
			Process p = new Process ();
			p.StartInfo.FileName = name;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.Start ();
			return p;
		}

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			p = StartApplication (@"SampleForm.exe",
				String.Empty);

			Thread.Sleep (1000);

			testFormElement = AutomationElement.RootElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ProcessIdProperty,
					p.Id));
			groupBox1Element = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Group));
			button1Element = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Button));
			textbox1Element = testFormElement.FindFirst (TreeScope.Children,
				new AndCondition (new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Edit),
					new PropertyCondition (AEIds.IsPasswordProperty, false)));
			textbox2Element = testFormElement.FindFirst (TreeScope.Children,
				new AndCondition (new PropertyCondition (AEIds.ControlTypeProperty, ControlType.Edit),
					new PropertyCondition (AEIds.IsPasswordProperty, true)));
			label1Element = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Text));
			button2Element = groupBox1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty,
					"button2"));
			button3Element = groupBox1Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.NameProperty,
					"button3"));
			button4Element = groupBox1Element.FindFirst (TreeScope.Descendants,
				new PropertyCondition (AEIds.NameProperty,
					"button4"));
			button5Element = groupBox1Element.FindFirst (TreeScope.Descendants,
				new PropertyCondition (AEIds.NameProperty,
					"button5"));
			button6Element = groupBox1Element.FindFirst (TreeScope.Descendants,
				new PropertyCondition (AEIds.NameProperty,
					"button6"));
			button7Element = groupBox1Element.FindFirst (TreeScope.Descendants,
				new PropertyCondition (AEIds.NameProperty,
					"button7"));
			textbox3Element = testFormElement.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.ControlTypeProperty,
					ControlType.Document));
			tb3horizontalScrollBarElement = textbox3Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.OrientationProperty,
					OrientationType.Horizontal));
			tb3verticalScrollBarElement = textbox3Element.FindFirst (TreeScope.Children,
				new PropertyCondition (AEIds.OrientationProperty,
					OrientationType.Vertical));
			//horizontalMenuStripElement = testFormElement.FindFirst (TreeScope.Descendants,
			//        new PropertyCondition (AEIds.NameProperty,
			//                "menuStrip1"));
			//verticalMenuStripElement = testFormElement.FindFirst (TreeScope.Descendants,
			//        new PropertyCondition (AEIds.NameProperty,
			//                "menuStrip2"));

			Assert.IsNotNull (testFormElement);
			Assert.IsNotNull (groupBox1Element);
			Assert.IsNotNull (button1Element);
			Assert.IsNotNull (button2Element);
			Assert.IsNotNull (button3Element);
			Assert.IsNotNull (button4Element);
			Assert.IsNotNull (button5Element);
			Assert.IsNotNull (button6Element);
			Assert.IsNotNull (button7Element);
			Assert.IsNotNull (label1Element);
			Assert.IsNotNull (textbox1Element);
			Assert.IsNotNull (textbox2Element);
			Assert.IsNotNull (textbox3Element);
			Assert.IsNotNull (tb3horizontalScrollBarElement);
			Assert.IsNotNull (tb3verticalScrollBarElement);
			//Assert.IsNotNull (horizontalMenuStripElement);
			//Assert.IsNotNull (verticalMenuStripElement);
		}

		[TestFixtureTearDown]
		public void FixtureTearDown ()
		{
			p.Kill ();
			p = null;
		}

		#endregion
	}
}
