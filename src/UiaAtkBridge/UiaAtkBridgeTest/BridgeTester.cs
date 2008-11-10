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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using SWF = System.Windows.Forms;
using System.Drawing;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	[TestFixture]
	public class BridgeTester : BridgeTests
	{
		
		public BridgeTester () 
		{
			//same effect as Application.Run() (the important bit is this causes a call to ApplicationStarts() ):
			AutomationInteropProvider.RaiseAutomationEvent (null, null, null);

			string uiaQaPath = Misc.LookForParentDir ("*.gif");
			string imgPath = System.IO.Path.Combine (uiaQaPath, "opensuse60x38.gif");

			butWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			butWithImage.AutoSize = true;

			chkWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			butWithImage.AutoSize = true;

			radWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			radWithImage.AutoSize = true;
			
			pboxWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			pboxWithImage.AutoSize = true;

			cbDDL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			cbSim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;

			tbx1.Multiline = false;
			tbx2.Multiline = true;

			linklab1.Links[0].Visited = true;
			linklab1.Text = "openSUSE:www.opensuse.org \n\n webmail:gmail.novell.com";
			linklab1.Links.Add(9, 16, "www.opensuse.org");
			linklab1.Links.Add(35, 16, "gmail.novell.com");
			linklab1.LinkClicked += LinkLabelClicked;
			linklab1.Links[0].Enabled = false;
			gb1.Controls.Add (rad1);
			gb1.Controls.Add (rad2);
			gb2.Controls.Add (rad3);
			gb2.Controls.Add (rad4);
			form.Controls.Add (gb1);
			form.Controls.Add (gb2);
			form.Controls.Add (lb1);
			form.Controls.Add (clb1);
			form.Controls.Add (cbDDL);
			form.Controls.Add (cbDD);
			form.Controls.Add (cbSim);
			form.Controls.Add (lab1);
			form.Controls.Add (linklab1);
			form.Controls.Add (butWithoutImage);
			form.Controls.Add (butWithImage);
			form.Controls.Add (chkWithoutImage);
			form.Controls.Add (chkWithImage);
			form.Controls.Add (sb1);
			form.Controls.Add(menuStrip1);
			form.MainMenuStrip = menuStrip1;
			form.Controls.Add (pb1);
			form.Controls.Add (nud1);
			form.Controls.Add (pboxWithoutImage);
			form.Controls.Add (pboxWithImage);
			form.Controls.Add (tbx1);
			form.Controls.Add (tbx2);
			form.Controls.Add (radWithImage);
			rad1.Text = "rad1";
			rad2.Text = "rad2";
			rad3.Text = "rad3";
			rad4.Text = "rad4";
			radios.Add (rad1);
			radios.Add (rad2);
			radios.Add (rad3);
			radios.Add (rad4);
			form.Text = "UiaAtkBridge test";
			form.Show ();
		}
		
		private SWF.RadioButton GiveMeARadio (string name) {
			if (currentRadio == 3) {
				currentRadio = -1;
			}
			
			currentRadio++;
			radios [currentRadio].Name = name;
			return radios [currentRadio];
		}
		
		public override void RunInGuiThread (System.Action d)
		{
			d ();
		}

		private Dictionary <Atk.Object, System.ComponentModel.Component> mappings = new Dictionary<Atk.Object, System.ComponentModel.Component> ();
		
		public override void DisableWidget (Atk.Object accessible)
		{
			System.ComponentModel.Component comp = mappings [accessible];

			if (comp is SWF.Control)
				((SWF.Control)comp).Enabled = false;
			else if (comp is SWF.ToolStripItem)
				((SWF.ToolStripItem)comp).Enabled = false;
			else
				throw new NotSupportedException ();
		}

		public override void EnableWidget (Atk.Object accessible)
		{
			System.ComponentModel.Component comp = mappings [accessible];

			if (comp is SWF.Control)
				((SWF.Control)comp).Enabled = true;
			else if (comp is SWF.ToolStripItem)
				((SWF.ToolStripItem)comp).Enabled = true;
			else
				throw new NotSupportedException ();
		}
		
		public override I CastToAtkInterface <I> (Atk.Object accessible)
		{
			if (typeof (I) == typeof (Atk.Component)) {
				return new Atk.ComponentAdapter ((Atk.ComponentImplementor)accessible) as I;
			}
			else if (typeof (I) == typeof (Atk.Text)) {
				return new Atk.TextAdapter ((Atk.TextImplementor)accessible) as I;
			}
			else if (typeof (I) == typeof (Atk.Action)) {
				return new Atk.ActionAdapter ((Atk.ActionImplementor)accessible) as I;
			}
			else if (typeof (I) == typeof (Atk.Table)) {
				return new Atk.TableAdapter ((Atk.TableImplementor)accessible) as I;
			}
			else if (typeof (I) == typeof (Atk.Value)) {
				return new Atk.ValueAdapter ((Atk.ValueImplementor)accessible) as I;
			}
			else if (typeof (I) == typeof (Atk.Image)) {
				return new Atk.ImageAdapter ((Atk.ImageImplementor)accessible) as I;
			}
			else if (typeof (I) == typeof (Atk.Selection)) {
				return new Atk.SelectionAdapter ((Atk.SelectionImplementor)accessible) as I;
			}
			throw new NotImplementedException ("Couldn't cast to interface " +
			typeof (I).Name);
		}
		
		public override Atk.Object GetAccessibleThatEmbedsAnImage (BasicWidgetType type, string name, bool real)
		{
			return GetAccessible (type, name, real, true);
		}

		private Atk.Object GetAdapterForWidget (System.ComponentModel.Component widget)
		{
//			return GetAdapterForWidget (widget, true);
//		}
//		private Atk.Object GetAdapterForWidget (System.ComponentModel.Component widget, bool recursive)
//		{
			Atk.Object acc = GetAdapterForProvider (ProviderFactory.GetProvider (widget, true, true));
			mappings [acc] = widget;
			
//NOTE: this code fragment below may be useful for discovering child items which don't have a test for themselves alone
//			System.Reflection.PropertyInfo itemsProp = 
//			  new List<System.Reflection.PropertyInfo> (widget.GetType ().GetProperties ()).
//			  Find (delegate (System.Reflection.PropertyInfo p) {
//				return p.Name == "Items";
//			  });
//			if (itemsProp != null) {
//				object value = itemsProp.GetValue (widget, null);
//				if (value is ICollection)
//					foreach (object o in (ICollection)value) 
//						if (o is System.ComponentModel.Component)
//							GetAdapterForWidget ((System.ComponentModel.Component)o, false);
//			}
			return acc;
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names)
		{
			return GetAccessible (type, names, true);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names, bool real)
		{
			Atk.Object accessible = null;
			
			switch (type) {
			case BasicWidgetType.ListBox:
			case BasicWidgetType.VScrollBar:
			case BasicWidgetType.HScrollBar:
				SWF.ListBox listBox = new SWF.ListBox ();
				if (real)
					listBox = lb1;
				listBox.Items.Clear ();
				listBox.ScrollAlwaysVisible = (type == BasicWidgetType.VScrollBar);
				listBox.HorizontalScrollbar = (type == BasicWidgetType.HScrollBar);
				foreach (string item in names)
					listBox.Items.Add (item);
			
				if (real)
					accessible = GetAdapterForWidget (listBox);
				else
					accessible = new UiaAtkBridge.List ((IRawElementProviderFragmentRoot) 
					                                    ProviderFactory.GetProvider (listBox, true, true));
				break;
				
			case BasicWidgetType.CheckedListBox:
				SWF.CheckedListBox clistBox = new SWF.CheckedListBox ();
				if (real)
					clistBox = clb1;
				clistBox.Items.Clear ();
				foreach (string item in names)
					clistBox.Items.Add (item);
			
				if (real)
					accessible = GetAdapterForWidget (clistBox);
				else
					accessible = new UiaAtkBridge.List ((IRawElementProviderFragmentRoot) 
					                                    ProviderFactory.GetProvider (clistBox, true, true));
				break;
				
			case BasicWidgetType.ComboBoxDropDownEntry:
				if (!real)
					throw new NotSupportedException ("ComboBox has no un-real support");

				cbDD.Items.Clear();
				foreach (string item in names)
					cbDD.Items.Add (item);

				if (real)
					accessible = GetAdapterForWidget (cbDD);
				else
					accessible = new UiaAtkBridge.ComboBox ((IRawElementProviderFragmentRoot) 
					                                        ProviderFactory.GetProvider (cbDD, true, true));
				break;
				
			case BasicWidgetType.ComboBoxDropDownList:
				if (!real)
					throw new NotSupportedException ("ComboBox has no un-real support");
				
				cbDDL.Items.Clear();
				foreach (string item in names)
					cbDDL.Items.Add (item);
			
				if (real)
					accessible = GetAdapterForWidget (cbDDL);
				else
					accessible = new UiaAtkBridge.ComboBox ((IRawElementProviderFragmentRoot) 
					                                        ProviderFactory.GetProvider (cbDDL, true, true));
				break;

			case BasicWidgetType.MainMenuBar:
			case BasicWidgetType.ParentMenu:

				if (!real)
					throw new NotSupportedException ("You, clown, we're gonna deprecate un-real support");
				
				SWF.ToolStripMenuItem parentMenu = new SWF.ToolStripMenuItem ();
				parentMenu.Text = names [0];
				
				SWF.ToolStripMenuItem[] subMenus = new SWF.ToolStripMenuItem [names.Length - 1];
				for (int i = 1; i < names.Length; i++) {
					SWF.ToolStripMenuItem subMenu = new SWF.ToolStripMenuItem ();
					subMenu.Text = names [i];
					subMenus [i - 1] = subMenu;
				}
				
				parentMenu.DropDownItems.AddRange (subMenus);
				menuStrip1.Items.AddRange (new SWF.ToolStripItem [] { parentMenu });

				if (type == BasicWidgetType.ParentMenu)
					accessible = GetAdapterForWidget (parentMenu);
				else
					accessible = GetAdapterForWidget (menuStrip1);
				
				break;
			default:
				throw new NotImplementedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}

			return accessible;
		}
		

		public override Atk.Object GetAccessible (BasicWidgetType type, string name)
		{
			return GetAccessible (type, name);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string name, bool real)
		{
			return GetAccessible (type, name, real, false);
		}
		
		private Atk.Object GetAccessible (BasicWidgetType type, string name, bool real, bool embeddedImage)
		{
			Atk.Object accessible = null;
			string[] names = null;

			switch (type) {
			case BasicWidgetType.Label:
				SWF.Label lab = new SWF.Label ();
				if (real)
					lab = lab1;
				lab.Text = name;
				if (real)
					accessible = GetAdapterForWidget (lab);
				else
					accessible = new UiaAtkBridge.TextLabel (ProviderFactory.GetProvider (lab, true, true));
				break;
				
			case BasicWidgetType.NormalButton:
				SWF.Button but = new SWF.Button ();
				if (real)
					but = (embeddedImage ? butWithImage : butWithoutImage);
				but.Text = name;
				if (!real)
					throw new NotSupportedException ("We don't support unreal anymore in tests");
				accessible = GetAdapterForWidget (but);
				break;
				
			case BasicWidgetType.Window:
				SWF.Form frm = new SWF.Form ();
				if (real)
					frm = form;
				frm.Name = name;
				if (real)
					accessible = GetAdapterForWidget (frm);
				else
					accessible = new UiaAtkBridge.Window (ProviderFactory.GetProvider (frm, true, true));
				break;
				
			case BasicWidgetType.CheckBox:
				SWF.CheckBox chk = new SWF.CheckBox ();
				if (real)
					chk = (embeddedImage ? chkWithImage : chkWithoutImage);
				chk.Text = name;
				if (real)
					accessible = GetAdapterForWidget (chk);
				else
					accessible = new UiaAtkBridge.CheckBoxButton (ProviderFactory.GetProvider (chk, true, true));
				break;
				
			case BasicWidgetType.RadioButton:
				// the way to group radioButtons is dependent on their parent control
				SWF.RadioButton radio = 
					(embeddedImage ? radWithImage : GiveMeARadio (name));
				radio.Text = name;
				if (real)
					accessible = GetAdapterForWidget (radio);
				else
					throw new NotSupportedException ("No un-real support for this");
				break;
				
			case BasicWidgetType.StatusBar:
				SWF.StatusBar sb = new SWF.StatusBar ();
				if (real)
					sb = sb1;
				sb.Text = name;
				if (real)
					accessible = GetAdapterForWidget (sb);
				else
					accessible = new UiaAtkBridge.StatusBar (ProviderFactory.GetProvider (sb, true, true));
				break;

			case BasicWidgetType.HScrollBar:
				names = new string [] { "First item", "Second Item", "Last Item", "A really, really long item that's here to try to ensure that we have a scrollbar, assuming that it's even possible to have a scrollbar just by having a relaly, really long item and we don't also have to perform some other function which I'm not aware of, like display the form on the screen" };
				accessible = GetAccessible (type, names, real);
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
				break;

			case BasicWidgetType.VScrollBar:
				names = new string [100];
				for (int i = 0; i < 100; i++)
					names [i] = i.ToString();
				accessible = GetAccessible (type, names, real);
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
				break;

			case BasicWidgetType.ProgressBar:
				SWF.ProgressBar pb = new SWF.ProgressBar ();
				if (real) {
					pb = pb1;
					accessible = GetAdapterForWidget (pb);
				} else {
					accessible = new UiaAtkBridge.ProgressBar (ProviderFactory.GetProvider (pb, true, true));
				}
				break;

			case BasicWidgetType.Spinner:
				SWF.NumericUpDown nud = new SWF.NumericUpDown();
				if (real)
					nud = nud1;
				nud.Minimum = 0;
				nud.Maximum = 100;
				nud.Value = 50;
				if (real)
					accessible = GetAdapterForWidget (nud);
				else
					accessible = new UiaAtkBridge.Spinner (ProviderFactory.GetProvider (nud, true, true));
				break;

			case BasicWidgetType.TextBoxEntry:
				SWF.TextBox tbxEntry = tbx1;
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");

				tbx1.Text = name;
				accessible = GetAdapterForWidget (tbxEntry);
				break;

			case BasicWidgetType.TextBoxView:
				SWF.TextBox tbxView = tbx2;
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");

				tbx2.Text = name;
				accessible = GetAdapterForWidget (tbxView);
				break;
				
			case BasicWidgetType.PictureBox:
				SWF.PictureBox pbox = new SWF.PictureBox ();
				if (real)
					pbox = (embeddedImage ? pboxWithImage: pboxWithoutImage);
				if (real)
					accessible = GetAdapterForWidget (pbox);
				else
					accessible = new UiaAtkBridge.Image (ProviderFactory.GetProvider (pbox, true, true));
				break;

			case BasicWidgetType.ListBox:
			case BasicWidgetType.CheckedListBox:
			case BasicWidgetType.ParentMenu:
			case BasicWidgetType.ComboBoxDropDownEntry:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxSimple:
				throw new NotSupportedException ("You have to use the GetObject overload that receives a name array");
			default:
				throw new NotImplementedException ("The widget finder backend still hasn't got support for " +
					type.ToString ());
			}

			return accessible;
		}


		private static Atk.Object GetAdapterForProvider (IRawElementProviderSimple provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");
#pragma warning disable 618
			object adapter = UiaAtkBridge.AutomationBridge.GetAdapterForProvider (provider);
#pragma warning restore 618
			Assert.IsNotNull (adapter, "Object retreived from AutomationBridge.GetAdapterForProvider should not be null");
			Atk.Object atkObj = adapter as Atk.Object;
			Assert.IsNotNull (atkObj, "Object retreived from AutomationBridge.GetAdapterForProvider is not Atk.Object");
			return atkObj;
		}

		public void LinkLabelClicked (object source, SWF.LinkLabelLinkClickedEventArgs e)
		{
			lastClickedLink = linklab1.Links.IndexOf (e.Link);
		}

		protected override int ValidNumberOfActionsForAButton { get { return 1; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 0; } }
		protected override int ValidNChildrenForAScrollBar { get { return 0; } }
		

		
		/*[Test]
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
		}*/

		[TestFixtureTearDown]
		public void TearDown ()
		{
			form.Close ();
		}
	}
}
