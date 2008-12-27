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
using System.Xml;
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
	public abstract class BridgeTester : AtkTests
	{
		protected SWF.GroupBox gb1 = new SWF.GroupBox ();
		protected SWF.GroupBox gb2 = new SWF.GroupBox ();
		protected SWF.RadioButton rad1 = new SWF.RadioButton ();
		protected SWF.RadioButton rad2 = new SWF.RadioButton ();
		protected SWF.RadioButton rad3 = new SWF.RadioButton ();
		protected SWF.RadioButton rad4 = new SWF.RadioButton ();
		protected SWF.RadioButton radWithImage = new SWF.RadioButton ();
		protected List<SWF.RadioButton> radios = new List<SWF.RadioButton> ();
		protected int currentRadio = -1;
		protected SWF.ListBox lb1 = new SWF.ListBox ();
		protected SWF.CheckedListBox clb1 = new SWF.CheckedListBox ();
		protected SWF.ComboBox cbDD = new SWF.ComboBox ();
		protected SWF.ComboBox cbSim = new SWF.ComboBox ();
		protected SWF.ComboBox cbDDL = new SWF.ComboBox ();
		protected SWF.Label lab1 = new SWF.Label ();
		protected SWF.LinkLabel linklab1 = new SWF.LinkLabel ();
		protected SWF.Button butWithoutImage = new SWF.Button ();
		protected SWF.Button butWithImage = new SWF.Button ();
		protected SWF.CheckBox chkWithoutImage = new SWF.CheckBox ();
		protected SWF.CheckBox chkWithImage = new SWF.CheckBox ();
		protected SWF.StatusBar sb1 = new SWF.StatusBar ();
		protected SWF.StatusStrip ss1 = new SWF.StatusStrip ();
		protected SWF.ProgressBar pb1 = new SWF.ProgressBar ();
		protected SWF.NumericUpDown nud1 = new SWF.NumericUpDown ();
		protected SWF.DomainUpDown dud1 = new SWF.DomainUpDown ();
		protected SWF.Form form = new SWF.Form ();
		protected SWF.MenuStrip menuStrip1 = new SWF.MenuStrip ();
		protected SWF.Panel panel1 = new SWF.Panel ();
		protected SWF.PictureBox pboxWithoutImage = new SWF.PictureBox ();
		protected SWF.PictureBox pboxWithImage = new SWF.PictureBox ();
		protected SWF.TextBox tbx1 = new SWF.TextBox ();
		protected SWF.TextBox tbx2 = new SWF.TextBox ();
		protected SWF.ToolStrip toolStrip = new SWF.ToolStrip ();
		protected SWF.ToolStripComboBox toolStripComboBoxSim = new SWF.ToolStripComboBox ();
		protected SWF.ToolStripComboBox toolStripComboBoxDDL = new SWF.ToolStripComboBox ();
		protected SWF.ToolStripComboBox toolStripComboBoxDD = new SWF.ToolStripComboBox ();
		protected SWF.ToolStripTextBox toolStripTextBox1 = new SWF.ToolStripTextBox ();
		protected SWF.ToolStripTextBox toolStripTextBox2 = new SWF.ToolStripTextBox ();
		protected SWF.ToolStripLabel tsl1 = new SWF.ToolStripLabel ();
		protected SWF.ToolStripProgressBar tspb1 = new SWF.ToolStripProgressBar ();
		protected SWF.ListView lv1 = new SWF.ListView ();
		protected SWF.ToolStripDropDownButton tsddb = new SWF.ToolStripDropDownButton ();
		protected SWF.ToolStripSplitButton tssb = new SWF.ToolStripSplitButton ();
		protected SWF.TabControl tabControl = new SWF.TabControl ();

		protected int lastClickedLink = -1;
		
		public BridgeTester () 
		{
			//same effect as Application.Run() (the important bit is this causes a call to ApplicationStarts() ):
			AutomationInteropProvider.RaiseAutomationEvent (null, null, null);

			form.Show ();

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
			toolStripTextBox1.Multiline = false;
			tbx2.Multiline = true;
			toolStripTextBox2.Multiline = true;

			toolStripComboBoxSim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			toolStripComboBoxDDL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			toolStripComboBoxDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			
			toolStrip.Items.Add (toolStripComboBoxSim);
			toolStrip.Items.Add (toolStripComboBoxDDL);
			toolStrip.Items.Add (toolStripComboBoxDD);
			toolStrip.Items.Add (tsl1);
			toolStrip.Items.Add (tspb1);
			toolStrip.Items.Add (tsddb);
			toolStrip.Items.Add (tssb);
			toolStrip.Items.Add (toolStripTextBox1);
			toolStrip.Items.Add (toolStripTextBox2);
			form.Controls.Add (toolStrip);

			linklab1.Links [0].Visited = true;
			linklab1.Text = "openSUSE:www.opensuse.org \n\n webmail:gmail.novell.com";
			linklab1.Links.Add (9, 16, "www.opensuse.org");
			linklab1.Links.Add (35, 16, "gmail.novell.com");
			linklab1.LinkClicked += LinkLabelClicked;
			linklab1.Links [0].Enabled = false;
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
			form.Controls.Add (ss1);
			form.Controls.Add(menuStrip1);
			form.MainMenuStrip = menuStrip1;
			form.Controls.Add (pb1);
			form.Controls.Add (nud1);
			form.Controls.Add (dud1);
			form.Controls.Add (panel1);
			form.Controls.Add (pboxWithoutImage);
			form.Controls.Add (pboxWithImage);
			form.Controls.Add (tbx1);
			form.Controls.Add (tbx2);
				// TODO: Move following lines to the end of ListView test to test view switching
			lv1.View = SWF.View.Details;
			lv1.ShowGroups = true;
			form.Controls.Add (lv1);
			form.Controls.Add (radWithImage);
			rad1.Text = "rad1";
			rad2.Text = "rad2";
			rad3.Text = "rad3";
			rad4.Text = "rad4";
			radios.Add (rad1);
			radios.Add (rad2);
			radios.Add (rad3);
			radios.Add (rad4);
			form.Controls.Add (tabControl);
			form.Text = "UiaAtkBridge test";
			SWF.Application.EnableVisualStyles ();
			
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

		public override Atk.Object GetTopLevelRootItem () {
			return UiaAtkBridge.TopLevelRootItem.Instance;
		}

		public override bool IsBGO561414Addressed ()
		{
			return true;
		}

		protected override bool AllowsEmptyingSelectionOnComboBoxes { 
			get { return false; }
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
		
		public override void SetReadOnly (Atk.Object accessible, bool readOnly)
		{
			System.ComponentModel.Component comp = mappings [accessible];

			if (comp is SWF.UpDownBase)
				((SWF.UpDownBase)comp).ReadOnly = readOnly;
			else
				throw new NotSupportedException ();
		}

		public override I CastToAtkInterface <I> (Atk.Object accessible)
		{
			try {
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
				else if (typeof (I) == typeof (Atk.EditableText)) {
					return new Atk.EditableTextAdapter ((Atk.EditableTextImplementor)accessible) as I;
				}
				throw new NotImplementedException ("Couldn't cast to interface " +
				typeof (I).Name);
			} catch (InvalidCastException) {
				return null;
			}
		}
		
		public override Atk.Object GetAccessibleThatEmbedsAnImage (BasicWidgetType type, string name, bool real)
		{
			return GetAccessible (type, name, null, real, true);
		}

		protected Atk.Object GetAdapterForWidget (System.ComponentModel.Component widget)
		{
			if (widget == null)
				throw new ArgumentNullException ("widget");
			
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
			return GetAccessible (type, names, null, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names, object widget)
		{
			return GetAccessible (type, names, widget, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string name, object widget)
		{
			return GetAccessible (type, name, widget, true, false);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names, bool real)
		{
			return GetAccessible (type, names, null, real);
		}
		
		public Atk.Object GetAccessible (BasicWidgetType type, string [] names, object widget, bool real)
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

			case BasicWidgetType.ListView:
				lv1.Items.Clear ();
			lv1.Scrollable = false;
				foreach (string item in names)
					lv1.Items.Add (item);
				accessible = GetAdapterForWidget (lv1);
				break;

			case BasicWidgetType.DomainUpDown:
				dud1.Items.Clear ();
				foreach (string item in names)
					dud1.Items.Add (item);
			
				accessible = GetAdapterForWidget (dud1);
				break;

			case BasicWidgetType.TabControl:
				tabControl.TabPages.Clear ();
				foreach (string item in names)
					tabControl.TabPages.Add (item);
			
				accessible = GetAdapterForWidget (tabControl);
				break;

			case BasicWidgetType.ComboBoxSimple:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxDropDownEntry:
				if (!real)
					throw new NotSupportedException ("You, clown, we're gonna deprecate un-real support");
				
				System.ComponentModel.Component comp = null;
				if (widget != null) {
					comp = (System.ComponentModel.Component)widget;
				}
				else {
					if (type == BasicWidgetType.ComboBoxDropDownEntry)
						comp = cbDD;
					else if (type == BasicWidgetType.ComboBoxDropDownList)
						comp = cbDDL;
					else if (type == BasicWidgetType.ComboBoxSimple)
						comp = cbSim;
				}
				
				if (comp is SWF.ComboBox) {
					SWF.ComboBox normalCombo = (SWF.ComboBox)comp;
					normalCombo.Items.Clear();
					foreach (string item in names)
						normalCombo.Items.Add (item);
				}
				else if (comp is SWF.ToolStripComboBox) {
					SWF.ToolStripComboBox stripCombo = (SWF.ToolStripComboBox)comp;
					stripCombo.Items.Clear();
					foreach (string item in names)
						stripCombo.Items.Add (item);
				}
				else
					throw new NotSupportedException ("This kind of ComboBox is not supported: " + comp.GetType ().Name);

				accessible = GetAdapterForWidget (comp);
				break;

			case BasicWidgetType.ToolStripSplitButton:
				tssb.Text = names [0];
				for (int i = 1; i < names.Length; i++) {
					SWF.ToolStripMenuItem item
						= new SWF.ToolStripMenuItem ();
					item.Text = names [i];
					tssb.DropDownItems.Add (item);
				}
				accessible = GetAdapterForWidget (tssb);
				break;
			case BasicWidgetType.ToolStripDropDownButton:
				foreach (string name in names) {
					SWF.ToolStripMenuItem item
						= new SWF.ToolStripMenuItem ();
					item.Text = name;
					tsddb.DropDownItems.Add (item);
				}
				accessible = GetAdapterForWidget (tsddb);
				break;
			default:
				throw new NotImplementedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}

			return accessible;
		}
		

		public override Atk.Object GetAccessible (BasicWidgetType type, string name)
		{
			return GetAccessible (type, name, null);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string name, bool real)
		{
			return GetAccessible (type, name, null, real, false);
		}
		
		private Atk.Object GetAccessible (BasicWidgetType type, string name, object widget, bool real, bool embeddedImage)
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
				but.Text = name.Replace ("_", "&"); // Gtk uses '_' to underline, SWF uses '&'
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
				chk.Text = name.Replace ("_", "&"); // Gtk uses '_' to underline, SWF uses '&'
				if (real)
					accessible = GetAdapterForWidget (chk);
				else
					accessible = new UiaAtkBridge.CheckBoxButton (ProviderFactory.GetProvider (chk, true, true));
				break;
				
			case BasicWidgetType.RadioButton:
				// the way to group radioButtons is dependent on their parent control
				SWF.RadioButton radio = 
					(embeddedImage ? radWithImage : GiveMeARadio (name));
				radio.Text = name.Replace ("_", "&"); // Gtk uses '_' to underline, SWF uses '&'
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
					accessible = new UiaAtkBridge.TextContainer (ProviderFactory.GetProvider (sb, true, true));
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
					accessible = new UiaAtkBridge.SpinnerWithValue (ProviderFactory.GetProvider (nud, true, true));
				break;

			case BasicWidgetType.TextBoxEntry:
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");
				
				SWF.TextBox tbxEntry = tbx1;
				SWF.ToolStripTextBox tstbxEntry = null;

				if ((widget != null) && (widget is SWF.ToolStripTextBox)) {
					tstbxEntry = (SWF.ToolStripTextBox)widget;
					accessible = GetAdapterForWidget (tstbxEntry);
					tstbxEntry.Text = name;
				} else {
					accessible = GetAdapterForWidget (tbxEntry);
					tbx1.Text = name;
				}
				break;

			case BasicWidgetType.TextBoxView:
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");
				
				SWF.TextBox tbxView = tbx2;
				SWF.ToolStripTextBox tstbxView = null;
				if ((widget != null) && (widget is SWF.ToolStripTextBox)) {
					tstbxView = (SWF.ToolStripTextBox)widget;					
					accessible = GetAdapterForWidget (tstbxView);
					tstbxView.Text = name;
				} else {
					accessible = GetAdapterForWidget (tbxView);
					tbx1.Text = name;
				}
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

			case BasicWidgetType.ToolStripLabel:
				tsl1.Text = name;
				accessible = GetAdapterForWidget (tsl1);
				break;

			case BasicWidgetType.ListView:
				XmlDocument xml = new XmlDocument ();
				xml.LoadXml (name);
				lv1.Groups.Clear ();
				lv1.Items.Clear ();
				foreach (XmlElement th in xml.GetElementsByTagName ("th"))
					foreach (XmlElement td in th.GetElementsByTagName ("td"))
						lv1.Columns.Add (new SWF.ColumnHeader (td.InnerText));
				XmlElement root = xml.DocumentElement;
				for (XmlNode node = root.FirstChild; node != null; node = node.NextSibling)
					if (node.Name == "tr") {
						bool group = false;
						for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
							if (child.Name == "tr")
								group = true;
						if (group)
							GetListViewGroup (node);
						else
							lv1.Items.Add (GetListViewItem (node));
					}
				accessible = GetAdapterForWidget (lv1);
				break;
				
			case BasicWidgetType.ToolStripProgressBar:
				accessible = GetAdapterForWidget (tspb1);
				break;
			case BasicWidgetType.ContainerPanel://In the future we may return something different in Pane
				accessible = GetAdapterForWidget (panel1);
				break;
			case BasicWidgetType.ErrorProvider:
				if (!real)
					throw new NotSupportedException ("We don't support unreal anymore in tests");
					
				// the way to group radioButtons is dependent on their parent control
				SWF.ErrorProvider errorProvider = new SWF.ErrorProvider ();
				errorProvider.SetError (butWithImage, "Error message");
				accessible = GetAdapterForWidget (errorProvider);
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

		public override Atk.Object GetAccessible (
		  BasicWidgetType type, List <MenuLayout> menu)
		{
			System.ComponentModel.Component widget;
			
			//cleanup
			menuStrip1.Items.Clear ();
			
			widget = AddRecursively (menuStrip1.Items, menu, type);
			
			if (type == BasicWidgetType.MainMenuBar)
				widget = menuStrip1;
			
			return GetAdapterForWidget (widget);
		}

		private System.ComponentModel.Component AddRecursively (SWF.ToolStripItemCollection subcol, List <MenuLayout> menus, BasicWidgetType type)
		{
			System.ComponentModel.Component ret = null, ret_aux;
			if (menus.Count <= 0)
				return ret;
			
			List <SWF.ToolStripMenuItem> list = new List <SWF.ToolStripMenuItem> ();
			foreach (MenuLayout menu in menus) {
				SWF.ToolStripMenuItem tsmi = new SWF.ToolStripMenuItem ();
				tsmi.Text = menu.Label;
				ret_aux = AddRecursively (tsmi.DropDownItems, menu.SubMenus, type);
				if (ret == null) {
					if ((tsmi.DropDownItems.Count > 0) && (type == BasicWidgetType.ParentMenu))
						ret = tsmi;
					else if ((tsmi.DropDownItems.Count == 0) && (type == BasicWidgetType.ChildMenu))
						ret = tsmi;
					else
						ret = ret_aux;
				}
				
				list.Add (tsmi);
			}
			subcol.AddRange (list.ToArray ());
			return ret;
		}
		
		private void GetListViewGroup (XmlNode node)
		{
			XmlElement tr = node as XmlElement;
			if (tr == null)
				return;
			SWF.ListViewGroup group = new SWF.ListViewGroup (tr.FirstChild.InnerText);
			lv1.Groups.Add (group);
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
				if (child.Name == "tr") {
					SWF.ListViewItem item = GetListViewItem (child);
					lv1.Items.Add (item);
					item.Group = group;
				}
		}

		private SWF.ListViewItem GetListViewItem (XmlNode node)
		{
			SWF.ListViewItem item = null;
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
				if (child.Name == "td")
					if (item == null)
						item = new SWF.ListViewItem (child.InnerText);
					else
						item.SubItems.Add (child.InnerText);
			return item;
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

		protected override bool ContainerPanelIsResizable { get { return true; } }
		protected override int ValidNumberOfActionsForAButton { get { return 1; } }
		protected override int ValidNChildrenForAListView { get { return 22; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 0; } }
		protected override int ValidNChildrenForAScrollBar { get { return 0; } }

		[TestFixtureTearDown]
		public void TearDown ()
		{
			form.Close ();
			Atk.Util.GetRootHandler = null;
		}
	}
}
