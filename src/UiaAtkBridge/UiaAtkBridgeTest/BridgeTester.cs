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
			Atk.ComponentImplementor component = null;
			Atk.TextImplementor text = null;
			Atk.ActionImplementor action = null;
			accessible = null;

			switch (type) {
			case BasicWidgetType.Label:
				MWF.Label lab = new MWF.Label ();
				lab.Text = name;
				UiaAtkBridge.TextLabel uiaLab = new UiaAtkBridge.TextLabel (new LabelProvider (lab));
				accessible = uiaLab;
				text = uiaLab;
				component = uiaLab;
				break;
			case BasicWidgetType.Button:
				MWF.Button but = new MWF.Button ();
				but.Text = name;
				UiaAtkBridge.Button uiaBut = new UiaAtkBridge.Button (new ButtonProvider (but));
				accessible = uiaBut;
				text = uiaBut;
				component = uiaBut;
				action = uiaBut;
				break;
			case BasicWidgetType.Window:
				MWF.Form frm = new MWF.Form ();
				frm.Name = name;
				UiaAtkBridge.Window uiaWin = new UiaAtkBridge.Window (new WindowProvider (frm));
				accessible = uiaWin;
				component = uiaWin;
				break;
			default:
				throw new NotImplementedException ("The widget finder backend still hasn't got support for " +
					type.ToString ());
			}

			if (typeof (I) == typeof (Atk.Text)) {
				return new Atk.TextAdapter (text);
			}
			else if (typeof (I) == typeof (Atk.Component)) {
				return new Atk.ComponentAdapter (component);
			}
			else if (typeof (I) == typeof (Atk.Action)) {
				return new Atk.ActionAdapter (action);
			}
			// we are not using this yet
//			else if (typeof(I) == typeof (Atk.Action)) {
//				TestButtonControlType button = new TestButtonControlType ("Push Button", false);
//				return new Atk.ActionAdapter (new UiaAtkBridge.Button(button));
//			}
			throw new NotImplementedException ("The interface finder backend still hasn't got support for " +
				typeof(I).Name);
		}
		
		
		[Test]
		public void UIAButtonControlType ()
		{
			TestButtonControlType pushButton = 
				new TestButtonControlType ("Push Button", false);

			UiaAtkBridge.Adapter bridgeAdapter = new UiaAtkBridge.Button(pushButton);

			Atk.Action atkAction =
				new Atk.ActionAdapter (bridgeAdapter as Atk.ActionImplementor);
//				new Atk.ActionAdapter (new UiaAtkBridge.Button (pushButton));

			pushButton.SetPropertyValue(AutomationElementIdentifiers.AcceleratorKeyProperty.Id, "Magic Key");
			// Should only work on GetKeybinding for 0, the rest should be null
			Assert.AreEqual ("Magic Key", atkAction.GetKeybinding (0));
			Assert.IsNull (atkAction.GetKeybinding (1));
			Assert.IsNull (atkAction.GetKeybinding (-1));
			
			atkAction.SetDescription(0, "Some big ugly description");
			Assert.AreEqual ("Some big ugly description", atkAction.GetDescription (0));
	
			//disabling this test because in other LANG it may not work
			//Assert.AreEqual ("click", atkAction.GetLocalizedName(0));


			// TogglePatternIdentifiers.ToggleStateProperty
			AutomationPropertyChangedEventArgs args =
					new AutomationPropertyChangedEventArgs (TogglePatternIdentifiers.ToggleStateProperty,
					                                        null,
					                                        ToggleState.On);

			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);

			// AutomationElementIdentifiers.BoundingRectangleProperty
			System.Windows.Rect rect = new System.Windows.Rect(47, 47, 47, 47);
			args =
					new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.BoundingRectangleProperty,
					                                        null,
					                                        rect);

			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);


/*
			if (e.Property == TogglePatternIdentifiers.ToggleStateProperty) {
			} else if (e.Property == AutomationElementIdentifiers.BoundingRectangleProperty) {
			} else if (e.Property == AutomationElementIdentifiers.IsOffscreenProperty) { 
			} else if (e.Property == AutomationElementIdentifiers.IsEnabledProperty) {
			} else if (e.Property == AutomationElementIdentifiers.NameProperty) {
*/

			// lot's more tests
		}

	}
}
