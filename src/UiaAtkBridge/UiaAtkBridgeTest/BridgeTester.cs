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
			
			switch (type) {
			case BasicWidgetType.ListBox:
				MWF.ListBox listBox = new MWF.ListBox ();
				if (real)
					listBox = lb1;
				listBox.Items.Clear ();
				foreach (string item in names)
					listBox.Items.Add (item);
			
				UiaAtkBridge.List uiaList;
				if (real)
					uiaList = (UiaAtkBridge.List) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (listBox, true, true));
				else
					uiaList = new UiaAtkBridge.List ((IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (listBox, true, true));
				accessible = uiaList;
				component = uiaList;
				selection = uiaList;
				break;
			case BasicWidgetType.ComboBox:
				MWF.ComboBox comboBox = new MWF.ComboBox ();
				if (real)
					comboBox = cb1;
				comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
				comboBox.Items.Clear();
				foreach (string item in names)
					comboBox.Items.Add (item);
			
				UiaAtkBridge.ComboBox uiaComb;
				if (real)
					uiaComb = (UiaAtkBridge.ComboBox) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (comboBox, true, true));
				else
					uiaComb = new UiaAtkBridge.ComboBox ((IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (comboBox, true, true));
				accessible = uiaComb;
				component = uiaComb;
				selection = uiaComb;
				action = uiaComb;
				break;
				
			case BasicWidgetType.ParentMenu:
				
				MWF.ToolStripMenuItem parentMenu = new MWF.ToolStripMenuItem();
				
				MWF.ToolStripMenuItem[] subMenus = new MWF.ToolStripMenuItem [names.Length - 1];
				for (int i = 1; i < names.Length; i++) {
					MWF.ToolStripMenuItem subMenu = new MWF.ToolStripMenuItem ();
					subMenu.Text = names [i];
					subMenus [i - 1] = subMenu;
				}
				
				parentMenu.DropDownItems.AddRange (subMenus);
				
				menuStrip1.Items.AddRange (new MWF.ToolStripItem[] {
					parentMenu
				});
				
				string [] submenus  = new string [names.Length - 1];
				Array.Copy (names, 1, submenus, 0, names.Length - 1);
				
				//FIXME: change call to ctor to send the provider
				UiaAtkBridge.ParentMenu uiaPMenu = new UiaAtkBridge.ParentMenu (submenus);
				accessible = uiaPMenu;
				component = uiaPMenu;
				selection = uiaPMenu;
				//FIXME: uncomment this after committing full ParentMenu class
				//action = uiaPMenu;

				break;
			default:
				throw new NotImplementedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}
			
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
		MWF.ListBox lb1 = new MWF.ListBox ();
		MWF.ComboBox cb1 = new MWF.ComboBox ();
		MWF.Label lab1 = new MWF.Label ();
		MWF.Button but1 = new MWF.Button ();
		MWF.CheckBox chk1 = new MWF.CheckBox ();
		MWF.StatusBar sb1 = new MWF.StatusBar ();
		MWF.ProgressBar pb1 = new MWF.ProgressBar ();
		MWF.NumericUpDown nud1 = new MWF.NumericUpDown();
		MWF.Form form = new MWF.Form ();
		MWF.MenuStrip menuStrip1 = new MWF.MenuStrip();
		
		public BridgeTester () 
		{
			cb1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			gb1.Controls.Add (rad1);
			gb1.Controls.Add (rad2);
			gb2.Controls.Add (rad3);
			gb2.Controls.Add (rad4);
			form.Controls.Add (gb1);
			form.Controls.Add (gb2);
			form.Controls.Add (lb1);
			form.Controls.Add (cb1);
			form.Controls.Add (lab1);
			form.Controls.Add (but1);
			form.Controls.Add (chk1);
			form.Controls.Add (sb1);
			form.Controls.Add(menuStrip1);
			form.MainMenuStrip = menuStrip1;
			form.Controls.Add (pb1);
			form.Controls.Add (nud1);
			radios.Add (rad1);
			radios.Add (rad2);
			radios.Add (rad3);
			radios.Add (rad4);
			form.Show ();
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
			Atk.TableImplementor table = null;
			Atk.ValueImplementor value = null;
			accessible = null;

			string[] names = null;

			switch (type) {
			case BasicWidgetType.Label:
				MWF.Label lab = new MWF.Label ();
				if (real)
					lab = lab1;
				lab.Text = name;
				UiaAtkBridge.TextLabel uiaLab;
				if (real)
					uiaLab = (UiaAtkBridge.TextLabel) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (lab, true, true));
				else
					uiaLab = new UiaAtkBridge.TextLabel (ProviderFactory.GetProvider (lab, true, true));
				accessible = uiaLab;
				text = uiaLab;
				component = uiaLab;
				break;
			case BasicWidgetType.NormalButton:
				MWF.Button but = new MWF.Button ();
				if (real)
					but = but1;
				but.Text = name;
				UiaAtkBridge.Button uiaBut;
				if (real)
					uiaBut = (UiaAtkBridge.Button) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (but, true, true));
				else
					uiaBut = new UiaAtkBridge.Button (ProviderFactory.GetProvider (but, true, true));
				accessible = uiaBut;
				text = uiaBut;
				component = uiaBut;
				action = uiaBut;
				break;
			case BasicWidgetType.Window:
				MWF.Form frm = new MWF.Form ();
				if (real)
					frm = form;
				frm.Name = name;
				UiaAtkBridge.Window uiaWin;
				if (real)
					uiaWin = (UiaAtkBridge.Window) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (frm, true, true));
				else
					uiaWin = new UiaAtkBridge.Window (ProviderFactory.GetProvider (frm, true, true));
				accessible = uiaWin;
				component = uiaWin;
				break;
			case BasicWidgetType.CheckBox:
				MWF.CheckBox chk = new MWF.CheckBox ();
				if (real)
					chk = chk1;
				chk.Text = name;
				UiaAtkBridge.CheckBox uiaChk;
				if (real)
					uiaChk = (UiaAtkBridge.CheckBox) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (chk, true, true));
				else
					uiaChk = new UiaAtkBridge.CheckBox (ProviderFactory.GetProvider (chk, true, true));
				accessible = uiaChk;
				component = uiaChk;
				action = uiaChk;
				text = uiaChk;
				break;
			case BasicWidgetType.RadioButton:
				// the way to group radioButtons is dependent on their parent control
				IRawElementProviderFragment prov = ProviderFactory.GetProvider (GiveMeARadio (name), true, true);
				UiaAtkBridge.RadioButton uiaRad;
				if (real)
					uiaRad = (UiaAtkBridge.RadioButton) UiaAtkBridge.AutomationBridge.GetAdapterForProvider (prov);
				else
					uiaRad = new UiaAtkBridge.RadioButton (prov);
				accessible = uiaRad;
				component = uiaRad;
				action = uiaRad;
				break;
			case BasicWidgetType.StatusBar:
				MWF.StatusBar sb = new MWF.StatusBar ();
				if (real)
					sb = sb1;
				sb.Text = name;
				UiaAtkBridge.StatusBar uiaSb;
				if (real)
					uiaSb = (UiaAtkBridge.StatusBar) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (sb, true, true));
				else
					uiaSb = new UiaAtkBridge.StatusBar (ProviderFactory.GetProvider (sb, true, true));
				accessible = uiaSb;
				component = uiaSb;
				text = uiaSb;
				break;

			case BasicWidgetType.HScrollBar:
				names = new string[] { "First item", "Second Item", "Last Item", "A really, really long item that's here to try to ensure that we have a scrollbar, assuming that it's even possible to have a scrollbar just by having a relaly, really long item and we don't also have to perform some other function which I'm not aware of, like display the form on the screen" };
				GetAtkObjectThatImplementsInterface <Atk.Component> (BasicWidgetType.ListBox, names, out accessible, real);
				for (int i = accessible.NAccessibleChildren - 1; i >= 0; i--)
				{
					Atk.Object child = accessible.RefAccessibleChild (i);
					if (child.Role == Atk.Role.ScrollBar && child.RefStateSet().ContainsState(Atk.StateType.Horizontal))
					{
						accessible = child;
						break;
					}
				}
				if (accessible.Role != Atk.Role.ScrollBar)
					return null;
				component = (Atk.ComponentImplementor) accessible;
				value = (Atk.ValueImplementor) accessible;
				break;

			case BasicWidgetType.VScrollBar:
				names = new string[100];
				for (int i = 0; i < 100; i++)
					names[i] = i.ToString();
				GetAtkObjectThatImplementsInterface <Atk.Component> (BasicWidgetType.ListBox, names, out accessible, real);
				for (int i = accessible.NAccessibleChildren - 1; i >= 0; i--)
				{
					Atk.Object child = accessible.RefAccessibleChild (i);
					if (child.Role == Atk.Role.ScrollBar && child.RefStateSet().ContainsState(Atk.StateType.Vertical))
					{
						accessible = child;
						break;
					}
				}
				if (accessible.Role != Atk.Role.ScrollBar)
					return null;
				component = (Atk.ComponentImplementor) accessible;
				value = (Atk.ValueImplementor) accessible;
				break;

			case BasicWidgetType.ProgressBar:
				MWF.ProgressBar pb = new MWF.ProgressBar ();
				if (real)
					pb = pb1;
				UiaAtkBridge.ProgressBar uiaPb;
				if (real)
					uiaPb = (UiaAtkBridge.ProgressBar) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (pb, true, true));
				else
					uiaPb = new UiaAtkBridge.ProgressBar (ProviderFactory.GetProvider (pb, true, true));
				accessible = uiaPb;
				component = uiaPb;
				text = uiaPb;
				value = uiaPb;
				break;

			case BasicWidgetType.Spinner:
				MWF.NumericUpDown nud = new MWF.NumericUpDown();
				if (real)
					nud = nud1;
				nud.Minimum = 0;
				nud.Maximum = 100;
				nud.Value = 50;
				UiaAtkBridge.Spinner uiaSp;
				if (real)
					uiaSp = (UiaAtkBridge.Spinner) UiaAtkBridge.AutomationBridge.GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (nud, true, true));
				else
					uiaSp = new UiaAtkBridge.Spinner (ProviderFactory.GetProvider (nud, true, true));
				accessible = uiaSp;
				component = uiaSp;
				value = uiaSp;
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
			else if (typeof (I) == typeof (Atk.Table)) {
				return new Atk.TableAdapter (table);
			}
			else if (typeof (I) == typeof (Atk.Value)) {
				return new Atk.ValueAdapter (value);
			}
			throw new NotImplementedException ("The interface finder backend still hasn't got support for " +
				typeof(I).Name);
		}
		
		protected override int ValidNumberOfActionsForAButton { get { return 1; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 0; } }
		protected override int ValidNChildrenForAScrollBar { get { return 3; } }
		
		[Test]
		public void UIACheckBox ()
		{
		}
		
		[Test]
		public void ListBox ()
		{
			BasicWidgetType type = BasicWidgetType.ListBox;
			Atk.Object accessible;
			
			string[] names = new string[] { "First item", "Second Item", "Last Item" };
			Atk.Component atkComponent = (Atk.Component)
				GetAtkObjectThatImplementsInterface <Atk.Component> (type, names, out accessible, true);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (3, accessible.NAccessibleChildren, "ListBox#RO numChildren");
			
			Atk.Object listItemChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (listItemChild, "ListBox child#0 should not be null");
			Assert.AreEqual (listItemChild.Role, Atk.Role.ListItem, "ListBox child#0 should be a list item");
			
			InterfaceAction (BasicWidgetType.ListItem, listItemChild);

			Assert.IsTrue (listItemChild.RefStateSet ().ContainsState (Atk.StateType.Selectable), "RefStateSet().Contains(Selectable)");

			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Parent (type, accessible);
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
