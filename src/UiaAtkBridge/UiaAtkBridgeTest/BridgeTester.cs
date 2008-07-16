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
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using MWF = System.Windows.Forms;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	[TestFixture]
	public class BridgeTester : AtkTester {
		
		public override object GetAtkObjectThatImplementsInterface <I> (
		  BasicWidgetType type, string[] names, out Atk.Object accessible, bool real)
		{
			Atk.ComponentImplementor component = null;
			Atk.ActionImplementor action = null;
			Atk.SelectionImplementor selection = null;
			accessible = null;
			
			if (type != BasicWidgetType.ComboBox) {
				throw new NotImplementedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}
			
			MWF.ComboBox comboBox = new MWF.ComboBox ();
			foreach (string item in names)
				comboBox.Items.Add (item);
			
			UiaAtkBridge.ComboBox uiaComb = new UiaAtkBridge.ComboBox ((IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (comboBox));
			accessible = uiaComb;
			component = uiaComb;
			selection = uiaComb;
			action = uiaComb;
			
			if (typeof (I) == typeof (Atk.Component)) {
				return new Atk.ComponentAdapter (component);
			}
			else if (typeof (I) == typeof (Atk.Action)) {
				return new Atk.ActionAdapter (action);
			}
			else if (typeof (I) == typeof (Atk.Selection)) {
				return new Atk.SelectionAdapter (selection);
			}
			throw new NotImplementedException ("The interface finder backend still hasn't got support for " +
				typeof(I).Name);
		}
		
		private MWF.GroupBox gb1 = new MWF.GroupBox ();
		private MWF.GroupBox gb2 = new MWF.GroupBox ();
		MWF.RadioButton rad1 = new MWF.RadioButton ();
		MWF.RadioButton rad2 = new MWF.RadioButton ();
		MWF.RadioButton rad3 = new MWF.RadioButton ();
		MWF.RadioButton rad4 = new MWF.RadioButton ();
		List<MWF.RadioButton> radios = new List<MWF.RadioButton> ();
		int currentRadio = -1;
		
		public BridgeTester () 
		{
			gb1.Controls.Add (rad1);
			gb1.Controls.Add (rad2);
			gb2.Controls.Add (rad3);
			gb2.Controls.Add (rad4);
			radios.Add (rad1);
			radios.Add (rad2);
			radios.Add (rad3);
			radios.Add (rad4);
		}
		
		private MWF.RadioButton GiveMeARadio (string name) {
			if (currentRadio == 3) {
				currentRadio = -1;
			}
			
			currentRadio++;
			radios [currentRadio].Name = name;
			return radios [currentRadio];
		}
		
		public override object GetAtkObjectThatImplementsInterface <I> (
		  BasicWidgetType type, string name, out Atk.Object accessible, bool real)
		{
			Atk.ComponentImplementor component = null;
			Atk.TextImplementor text = null;
			Atk.ActionImplementor action = null;
			accessible = null;

			switch (type) {
			case BasicWidgetType.Label:
				MWF.Label lab = new MWF.Label ();
				lab.Text = name;
				UiaAtkBridge.TextLabel uiaLab = new UiaAtkBridge.TextLabel (ProviderFactory.GetProvider (lab));
				accessible = uiaLab;
				text = uiaLab;
				component = uiaLab;
				break;
			case BasicWidgetType.NormalButton:
				MWF.Button but = new MWF.Button ();
				but.Text = name;
				UiaAtkBridge.Button uiaBut = new UiaAtkBridge.Button (ProviderFactory.GetProvider (but));
				accessible = uiaBut;
				text = uiaBut;
				component = uiaBut;
				action = uiaBut;
				break;
			case BasicWidgetType.Window:
				MWF.Form frm = new MWF.Form ();
				frm.Name = name;
				UiaAtkBridge.Window uiaWin = new UiaAtkBridge.Window (ProviderFactory.GetProvider (frm));
				accessible = uiaWin;
				component = uiaWin;
				break;
			case BasicWidgetType.CheckBox:
				MWF.CheckBox chk = new MWF.CheckBox ();
				chk.Name = name;
				UiaAtkBridge.CheckBox uiaChk = new UiaAtkBridge.CheckBox (ProviderFactory.GetProvider (chk));
				accessible = uiaChk;
				component = uiaChk;
				action = uiaChk;
				break;
			case BasicWidgetType.RadioButton:
				// the way to group radioButtons is dependent on their parent control
				IRawElementProviderFragment prov = ProviderFactory.GetProvider (GiveMeARadio (name));
				UiaAtkBridge.RadioButton uiaRad = new UiaAtkBridge.RadioButton (prov);
				accessible = uiaRad;
				component = uiaRad;
				action = uiaRad;
				break;
			case BasicWidgetType.ComboBox:
				throw new NotSupportedException ("You have to use the GetObject overload that receives a name array");
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
			throw new NotImplementedException ("The interface finder backend still hasn't got support for " +
				typeof(I).Name);
		}
		
		protected override int ValidNumberOfActionsForAButton { get { return 1; } }
		
		[Test]
		public void UIACheckBox ()
		{
		}
		
		//[Test]
		public void UIAButtonControlType ()
		{
			TestButtonControlType pushButton = 
				new TestButtonControlType ("Push Button", false);

			UiaAtkBridge.Adapter bridgeAdapter = new UiaAtkBridge.Button(pushButton);

			Console.WriteLine("About to create the Atk.Action");
			Atk.Action atkAction =
				new Atk.ActionAdapter (bridgeAdapter as Atk.ActionImplementor);
				
			pushButton.SetPropertyValue(AutomationElementIdentifiers.AcceleratorKeyProperty.Id, "Magic Key");
			// Should only work on GetKeybinding for 0, the rest should be null
			Assert.AreEqual ("Magic Key", atkAction.GetKeybinding (0));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (1));
			Assert.AreEqual (String.Empty, atkAction.GetKeybinding (-1));

			Console.WriteLine("Setting a description");
			
			atkAction.SetDescription(0, "Some big ugly description");
			Assert.AreEqual ("Some big ugly description", atkAction.GetDescription (0));
	
			Assert.IsNotNull (atkAction.GetLocalizedName (0));

			// TogglePatternIdentifiers.ToggleStateProperty
			AutomationPropertyChangedEventArgs args =
					new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.IsEnabledProperty,
					                                        null,
					                                        true);
			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);
			Assert.AreEqual(true, (bridgeAdapter as Atk.Object).RefStateSet().ContainsState(Atk.StateType.Sensitive));

			args = new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.IsEnabledProperty,
					                                        null,
					                                        false);
			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);
			Assert.AreEqual(false, (bridgeAdapter as Atk.Object).RefStateSet().ContainsState(Atk.StateType.Sensitive));


			// AutomationElementIdentifiers.IsOffscreenProperty
			args = new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.IsOffscreenProperty,
					                                        null,
					                                        true);
			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);
			Assert.AreEqual(false, (bridgeAdapter as Atk.Object).RefStateSet().ContainsState(Atk.StateType.Visible));

			args = new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.IsOffscreenProperty,
					                                        null,
					                                        false);
			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);
			Assert.AreEqual(true, (bridgeAdapter as Atk.Object).RefStateSet().ContainsState(Atk.StateType.Visible));


			// AutomationElementIdentifiers.NameProperty
			args = new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.NameProperty,
					                                        null,
					                                        "Actionizer");
			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);
			Assert.AreEqual("Actionizer", (bridgeAdapter as Atk.Object).Name);


			// AutomationElementIdentifiers.BoundingRectangleProperty
			System.Windows.Rect rect = new System.Windows.Rect(47, 47, 47, 47);
			args =
					new AutomationPropertyChangedEventArgs (AutomationElementIdentifiers.BoundingRectangleProperty,
					                                        null,
					                                        rect);
			bridgeAdapter.RaiseAutomationPropertyChangedEvent(args);

			// TODO: test the bounds that were set
		}

	}
}
