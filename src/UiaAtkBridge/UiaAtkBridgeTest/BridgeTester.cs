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
//      Andres G. Aragoneses <aaragoneses@novell.com>
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using MWF = System.Windows.Forms;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	[TestFixture]
	public class BridgeTester : AtkTester {
		
		public override object GetAtkObjectThatImplementsInterface <I> (BasicWidgetType type, string name, out Atk.Object accessible, bool real)
		{
			accessible = null;

			switch (type) {
			case BasicWidgetType.Label:
				MWF.Label lab = new MWF.Label ();
				lab.Text = name;
				accessible = new UiaAtkBridge.TextLabel (new LabelProvider (lab));
				break;
			case BasicWidgetType.Button:
				MWF.Button but = new MWF.Button ();
				but.Text = name;
				accessible = new UiaAtkBridge.Button (new ButtonProvider (but));
				break;
			}

			if (typeof (I) == typeof (Atk.Text)) {
				return new Atk.TextAdapter ((Atk.TextImplementor) accessible);
			}
			else if (typeof (I) == typeof (Atk.Component)) {
				return new Atk.ComponentAdapter ((Atk.ComponentImplementor) accessible);
			}
			// we are not using this yet
//			else if (typeof(I) == typeof (Atk.Action)) {
//				TestButtonControlType button = new TestButtonControlType ("Push Button", false);
//				return new Atk.ActionAdapter (new UiaAtkBridge.Button(button));
//			}

			return null;
		}
		
		
		//[Test]
		public void UIAButtonControlType ()
		{
			TestButtonControlType pushButton = 
				new TestButtonControlType ("Push Button", false);

			Atk.Action atkAction =
				new Atk.ActionAdapter (new UiaAtkBridge.Button(pushButton));

			Assert.AreEqual (1, atkAction.NActions);				
			Assert.AreEqual ("click", atkAction.GetName (0));

			pushButton.SetPropertyValue (AutomationElementIdentifiers.AcceleratorKeyProperty.Id, "Magic Key");
			Assert.AreEqual ("Magic Key", atkAction.GetKeybinding (0));
			
			atkAction.SetDescription(0, "Some big ugly description");
			Assert.AreEqual ("Some big ugly description", atkAction.GetDescription (0));
			
			
			// lot's more tests
		}
		
		//[Test]
		public void AtkActionButton ()
		{
			TestButtonControlType pushButton = 
				new TestButtonControlType("Push Button", false);

			Atk.Action atkAction =
				new Atk.ActionAdapter(new UiaAtkBridge.Button(pushButton));

			Assert.AreEqual (atkAction.NActions, 1);

			// only action 0 should work
			Assert.AreEqual (true, atkAction.DoAction (0));
			Assert.AreEqual (false, atkAction.DoAction (-1));
			Assert.AreEqual (false, atkAction.DoAction (1));
			Assert.AreEqual (false, atkAction.DoAction (2));

			// Should only work on SetDecription for 0
			Assert.AreEqual (true, atkAction.SetDescription (0, "Some great description"));
			Assert.AreEqual (false, atkAction.SetDescription (-1, "Some false great description"));
			Assert.AreEqual (false, atkAction.SetDescription (1, "Some false great description"));
			Assert.AreEqual (false, atkAction.SetDescription (2, "Some false great description"));

			// Should only work on GetDecription for 0, the rest should be empty string, not null
			Assert.AreEqual ("Some great description", atkAction.GetDescription (0));
			Assert.AreEqual (String.Empty, atkAction.GetDescription (-1));
			Assert.AreEqual (String.Empty, atkAction.GetDescription (1));
			Assert.AreEqual (String.Empty, atkAction.GetDescription (2));

			// With no keybinding set, everything should return ""
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (0));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (-1));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (1));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (2));

			pushButton.SetPropertyValue(AutomationElementIdentifiers.AcceleratorKeyProperty.Id, "Magic Key");

			// Should only work on GetKeybinding for 0, the rest should be empty string, not null
			Assert.AreEqual ("Magic Key", atkAction.GetKeybinding (0));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (-1));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (1));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (2));

			// Should only work on GetKeybinding for 0, the rest should be empty string, not null
			Assert.AreEqual ("click", atkAction.GetLocalizedName(0));
			Assert.AreEqual (String.Empty, atkAction.GetLocalizedName (-1));
			Assert.AreEqual (String.Empty, atkAction.GetLocalizedName (1));
			Assert.AreEqual (String.Empty, atkAction.GetLocalizedName (2));

			// Should only work on GetKeybinding for 0, the rest should be empty string, not null
			Assert.AreEqual ("click", atkAction.GetName(0));
			Assert.AreEqual (String.Empty, atkAction.GetName (-1));
			Assert.AreEqual (String.Empty, atkAction.GetName (1));
			Assert.AreEqual (String.Empty, atkAction.GetName (2));
		}

	}
}
