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
using SWF = System.Windows.Forms;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	[TestFixture]
	public class BridgeTester : AtkTests {

		SWF.GroupBox gb1 = new SWF.GroupBox ();
		SWF.GroupBox gb2 = new SWF.GroupBox ();
		SWF.RadioButton rad1 = new SWF.RadioButton ();
		SWF.RadioButton rad2 = new SWF.RadioButton ();
		SWF.RadioButton rad3 = new SWF.RadioButton ();
		SWF.RadioButton rad4 = new SWF.RadioButton ();
		SWF.RadioButton radWithImage = new SWF.RadioButton ();
		List<SWF.RadioButton> radios = new List<SWF.RadioButton> ();
		int currentRadio = -1;
		SWF.ListBox lb1 = new SWF.ListBox ();
		SWF.CheckedListBox clb1 = new SWF.CheckedListBox ();
		SWF.ComboBox cbDD = new SWF.ComboBox ();
		SWF.ComboBox cbSim = new SWF.ComboBox ();
		SWF.ComboBox cbDDL = new SWF.ComboBox ();
		SWF.Label lab1 = new SWF.Label ();
		SWF.LinkLabel linklab1 = new SWF.LinkLabel ();
		SWF.Button butWithoutImage = new SWF.Button ();
		SWF.Button butWithImage = new SWF.Button ();
		SWF.CheckBox chkWithoutImage = new SWF.CheckBox ();
		SWF.CheckBox chkWithImage = new SWF.CheckBox ();
		SWF.StatusBar sb1 = new SWF.StatusBar ();
		SWF.ProgressBar pb1 = new SWF.ProgressBar ();
		SWF.NumericUpDown nud1 = new SWF.NumericUpDown ();
		SWF.Form form = new SWF.Form ();
		SWF.MenuStrip menuStrip1 = new SWF.MenuStrip ();
		SWF.PictureBox pboxWithoutImage = new SWF.PictureBox ();
		SWF.PictureBox pboxWithImage = new SWF.PictureBox ();
		SWF.TextBox tbx1 = new SWF.TextBox ();
		SWF.TextBox tbx2 = new SWF.TextBox ();
		
		public BridgeTester () 
		{
			//same effect as Application.Run() (the important bit is this causes a call to ApplicationStarts() ):
			AutomationInteropProvider.RaiseAutomationEvent (null, null, null);

			string uiaQaPath = System.IO.Directory.GetCurrentDirectory ();
			string imgPath = uiaQaPath + "/../../../../../test/samples/opensuse60x38.gif";

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

		public override Atk.Object GetAccessible (BasicWidgetType type, string[] names, bool real)
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) 
					                                    ProviderFactory.GetProvider (listBox, true, true));
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) 
					                                    ProviderFactory.GetProvider (clistBox, true, true));
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) 
					                                    ProviderFactory.GetProvider (cbDD, true, true));
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) 
					                                    ProviderFactory.GetProvider (cbDD, true, true));
				else
					accessible = new UiaAtkBridge.ComboBox ((IRawElementProviderFragmentRoot) 
					                                        ProviderFactory.GetProvider (cbDD, true, true));
				break;
				
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

				accessible = GetAdapterForProvider ((IRawElementProviderSimple) 
					                                 ProviderFactory.GetProvider (parentMenu, true, true));

				break;
			default:
				throw new NotImplementedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}

			return accessible;
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (lab, true, true));
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
				accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (but, true, true));
				break;
			case BasicWidgetType.Window:
				SWF.Form frm = new SWF.Form ();
				if (real)
					frm = form;
				frm.Name = name;
				if (real)
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (frm, true, true));
				else
					accessible = new UiaAtkBridge.Window (ProviderFactory.GetProvider (frm, true, true));
				break;
			case BasicWidgetType.CheckBox:
				SWF.CheckBox chk = new SWF.CheckBox ();
				if (real)
					chk = (embeddedImage ? chkWithImage : chkWithoutImage);
				chk.Text = name;
				if (real)
					accessible = GetAdapterForProvider (
					    (IRawElementProviderSimple) ProviderFactory.GetProvider (chk, true, true));
				else
					accessible = new UiaAtkBridge.CheckBoxButton (ProviderFactory.GetProvider (chk, true, true));
				break;
			case BasicWidgetType.RadioButton:
				// the way to group radioButtons is dependent on their parent control
				SWF.RadioButton radio = 
					(embeddedImage ? radWithImage : GiveMeARadio (name));
				radio.Text = name;
				if (real)
					accessible = GetAdapterForProvider (ProviderFactory.GetProvider (radio, true, true));
				else
					throw new NotSupportedException ("No un-real support for this");
				break;
			case BasicWidgetType.StatusBar:
				SWF.StatusBar sb = new SWF.StatusBar ();
				if (real)
					sb = sb1;
				sb.Text = name;
				if (real)
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (sb, true, true));
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (pb, true, true));
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
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (nud, true, true));
				else
					accessible = new UiaAtkBridge.Spinner (ProviderFactory.GetProvider (nud, true, true));
				break;

			case BasicWidgetType.TextBoxEntry:
				SWF.TextBox tbxEntry = tbx1;
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");

				accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (tbxEntry, true, true));
				break;

			case BasicWidgetType.TextBoxView:
				SWF.TextBox tbxView = tbx2;
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");

				accessible = GetAdapterForProvider ((IRawElementProviderSimple) ProviderFactory.GetProvider (tbxView, true, true));
				break;
				
			case BasicWidgetType.PictureBox:
				SWF.PictureBox pbox = new SWF.PictureBox ();
				if (real)
					pbox = (embeddedImage ? pboxWithImage: pboxWithoutImage);
				if (real)
					accessible = GetAdapterForProvider ((IRawElementProviderSimple) 
					                                    ProviderFactory.GetProvider (pbox, true, true));
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
		
		private int lastClickedLink = -1;

		public void LinkLabelClicked (object source, SWF.LinkLabelLinkClickedEventArgs e)
		{
			lastClickedLink = linklab1.Links.IndexOf (e.Link);
		}

		protected override int ValidNumberOfActionsForAButton { get { return 1; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 0; } }
		protected override int ValidNChildrenForAScrollBar { get { return 0; } }
		
		[Test]
		public void ListBox ()
		{
			BasicWidgetType type = BasicWidgetType.ListBox;
			Atk.Object accessible;
			
			string[] names = new string [] { simpleTestText, "Second Item", "Last Item" };
			accessible = GetAccessible (type, names, true);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (3, accessible.NAccessibleChildren, "ListBox#RO numChildren");
			
			Atk.Object listItemChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (listItemChild, "ListBox child#0 should not be null");
			Assert.AreEqual (listItemChild.Role, Atk.Role.ListItem, "ListBox child#0 should be a list item");
			
			Atk.StateSet stateSet = listItemChild.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "RefStateSet().Contains(Focusable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Selectable), "RefStateSet().Contains(Selectable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "RefStateSet().Contains(Sensitive)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "RefStateSet().Contains(Showing)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "RefStateSet().Contains(Visible)");

			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Atk.SelectionImplementor selection = accessible as Atk.SelectionImplementor;
			Assert.IsNotNull (selection, "ListBox Atk.Selection should not be null");
			InterfaceSelection (new Atk.SelectionAdapter(selection), names, accessible, type);

			// Below line needed because InterfaceAction tests that first item is not selected, so that it can test the action
			selection.AddSelection(1);
			InterfaceAction (BasicWidgetType.ListItem, CastToAtkInterface <Atk.Action> (listItemChild), listItemChild);

			InterfaceText (BasicWidgetType.ListItem, true, listItemChild);

			Parent (type, accessible);
		}

		[Test]
		public void CheckedListBox ()
		{
			BasicWidgetType type = BasicWidgetType.CheckedListBox;
			Atk.Object accessible;
			
			string[] names = new string[] { simpleTestText, "Second Item", "Last Item" };
			accessible = GetAccessible (type, names, true);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (3, accessible.NAccessibleChildren, "ListBox#RO numChildren");
			
			Atk.Object listItemChild = accessible.RefAccessibleChild (0);
			Assert.IsNotNull (listItemChild, "ListBox child#0 should not be null");
			Assert.AreEqual (listItemChild.Role, Atk.Role.CheckBox, "ListBox child#0 should be a check box");
			
			Atk.StateSet stateSet = listItemChild.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "RefStateSet().Contains(Focusable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Selectable), "RefStateSet().Contains(Selectable)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "RefStateSet().Contains(Sensitive)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "RefStateSet().Contains(Showing)");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "RefStateSet().Contains(Visible)");
			Assert.AreEqual (0, listItemChild.NAccessibleChildren, "ListBox ListItem numChildren");

			Atk.SelectionImplementor selection = accessible as Atk.SelectionImplementor;
			Assert.IsNotNull (selection, "ListBox Atk.Selection should not be null");
			InterfaceSelection (new Atk.SelectionAdapter(selection), names, accessible, type);

			// Below line needed because InterfaceAction tests that first item is not selected, so that it can test the action
			selection.AddSelection(1);
			InterfaceAction (BasicWidgetType.CheckedListItem, CastToAtkInterface <Atk.Action> (listItemChild), listItemChild);

			InterfaceText (BasicWidgetType.CheckedListItem, true, listItemChild);

			Parent (type, accessible);
		}

		[Test]
		public void LinkLabel ()
		{
			UiaAtkBridge.Hyperlink hyperlink;
			hyperlink = (UiaAtkBridge.Hyperlink) 
			  UiaAtkBridge.AutomationBridge.GetAdapterForProviderLazy (
			    (IRawElementProviderSimple) ProviderFactory.GetProvider (linklab1, true, true));
			Atk.Text atkText = CastToAtkInterface<Atk.Text> (hyperlink);

			Assert.AreEqual (53, atkText.CharacterCount, "LinkLabel character count");
			Assert.AreEqual ("openSUSE", atkText.GetText (0, 8), "LinkLabel GetText");
			Atk.HypertextAdapter hypertext = new Atk.HypertextAdapter (hyperlink);
			Assert.AreEqual (2, hypertext.NLinks, "LinkLabel NLinks");
			Assert.AreEqual (-1, hypertext.GetLinkIndex (8), "GetLinkIndex #1");
			Assert.AreEqual (0, hypertext.GetLinkIndex (9), "GetLinkIndex #2");
			Assert.AreEqual (-1, hypertext.GetLinkIndex (25), "GetLinkIndex #3");
			Assert.AreEqual (1, hypertext.GetLinkIndex (35), "GetLinkIndex #4");
			Assert.AreEqual (1, hypertext.GetLinkIndex (42), "GetLinkIndex #5");
			Assert.AreEqual (-1, hypertext.GetLinkIndex (51), "GetLinkIndex #6");
			Atk.Hyperlink link1 = hypertext.GetLink (0);
			Assert.AreEqual ("www.opensuse.org", link1.GetUri (0), "Link 1 uri");
			Assert.IsNull (link1.GetUri (1), "LinkLabel GetUri OOR #1");
			Assert.IsNull (link1.GetUri (-1), "LinkLabel GetUri OOR #2");
			Assert.AreEqual (9, link1.StartIndex, "Link 1 StartIndex");
			Assert.AreEqual (25, link1.EndIndex, "Link 1 EndIndex");
			Atk.Object obj1 = link1.GetObject (0);
			Assert.IsNotNull (obj1, "LinkLabel GetObject #1");
			Assert.IsFalse (obj1.RefStateSet().ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			Assert.IsNull (link1.GetObject (1), "GetObject OOR #1");
			Assert.IsNull (link1.GetObject (-1), "GetObject OOR #2");
			Atk.Hyperlink link2 = hypertext.GetLink (1);
			Assert.AreEqual ("gmail.novell.com", link2.GetUri (0), "Link 1 uri");
			Assert.AreEqual (35, link2.StartIndex, "Link 1 StartIndex");
			Assert.AreEqual (51, link2.EndIndex, "Link 1 EndIndex");
			Atk.Object obj2 = link2.GetObject (0);
			Assert.IsNotNull (obj2, "LinkLabel GetObject #2");
			Assert.IsTrue (obj2.RefStateSet ().ContainsState (Atk.StateType.Enabled), "RefStateSet().Contains(Enabled)");
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (obj2);
			Assert.AreEqual (1, atkAction.NActions, "LinkLabel link NActions");
			Assert.IsTrue (atkAction.DoAction (0), "LinkLabel DoAction #1");
			Assert.IsFalse (atkAction.DoAction (1), "LinkLabel DoAction OOR #1");
			Assert.IsFalse (atkAction.DoAction (-1), "LinkLabel DoAction OOR #2");
			Assert.AreEqual (1, lastClickedLink, "LastClickedLink");
		}

		[Test]
		public void GroupBox ()
		{
			BasicWidgetType type = BasicWidgetType.GroupBox;
			Atk.Object accessible = 
			  UiaAtkBridge.AutomationBridge.GetAdapterForProviderLazy (
			    (IRawElementProviderSimple) ProviderFactory.GetProvider (gb1, true, true));

			PropertyRole (type, accessible);

			States (accessible,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
		}

		[Test]
		public void ComboBoxSimple ()
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxSimple;

			Atk.Object accessible = 
			  UiaAtkBridge.AutomationBridge.GetAdapterForProviderLazy (
			    (IRawElementProviderSimple) ProviderFactory.GetProvider (cbSim, true, true));
			
			PropertyRole (type, accessible);

			StatesComboBox (accessible);

			Assert.AreEqual (2, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
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
