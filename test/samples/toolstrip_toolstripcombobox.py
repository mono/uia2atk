#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        05/11/2008
# Description: This is a test application sample for winforms control:
#              ToolStrip
#              ToolStripComboBox
##############################################################################

import clr
import System
import System.IO

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System.Drawing.Text


class RunApp(Form):

    def __init__(self):
        self.Text = "Simple ToolStrip Example"
        self.Width = 800
        self.Height = 500
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.count = 0        
    
        self.rtb = TextBox()
        self.rtb.Multiline = True
        self.rtb.Dock = DockStyle.Fill
        self.rtb.BorderStyle = BorderStyle.FixedSingle
        self.Controls.Add(self.rtb)

##ToolStrip:
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)

##ToolStripButton:
        self.count_n = 0
        self.tb1 = ToolStripButton()
        #self.tb1.Image = Bitmap.FromFile("images/document-new.png")
        #self.tb1.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        self.tb1.DisplayStyle = ToolStripItemDisplayStyle.Text
        self.tb1.Text = "&New"
        self.tb1.Click += self.New_Document_Clicked
        self.ts.Items.Add(self.tb1)

        self.count_o = 0
        self.tb2 = ToolStripButton()
        #self.tb2.Image = Bitmap.FromFile("images/document-open.png")
        #self.tb2.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        self.tb2.DisplayStyle = ToolStripItemDisplayStyle.Text
        self.tb2.Text = "&Open"
        self.tb2.Click += self.Open_Document_Clicked
        self.ts.Items.Add(self.tb2)

##ToolStripSeparator
        self.toolstripseparator1 = ToolStripSeparator()
        self.ts.Items.Add(self.toolstripseparator1)

##ToolStripLabel
        self.tsl = ToolStripLabel("Font:")
        self.ts.Items.Add(self.tsl)

##ToolStripComboBox
        self.font_combo = ToolStripComboBox()
        self.font_combo.DropDownStyle = ComboBoxStyle.DropDownList
        self.font_combo.AutoSize = False
        self.font_combo.Width = 150
        self.ts.Items.Add(self.font_combo)

        self.count_b = 0

        self.tscb = ToolStripComboBox()
        self.tscb.DropDownStyle = ComboBoxStyle.DropDownList
        self.tscb.Items.Add("6")
        self.tscb.Items.Add("8")
        self.tscb.Items.Add("10")
        self.tscb.Items.Add("12")
        self.tscb.Items.Add("14")
        self.tscb.SelectedIndex = 1
        self.tscb.SelectedIndexChanged += self.tscb_SelectedIndexChanged
        self.tscb.AutoSize = False
        self.tscb.Width = 45
        self.ts.Items.Add(self.tscb)

        self.toolstripseparator2 = ToolStripSeparator()
        self.ts.Items.Add(self.toolstripseparator2)

##ToolStripDropDownButton
        self.db = ToolStripDropDownButton()
        self.dd = ToolStripDropDown()
        self.db.Text = "ToolStripDropDownButton"
        self.db.DropDown = self.dd
        self.db.DropDownDirection = ToolStripDropDownDirection.Left
        self.db.ShowDropDownArrow = True
        
        self.br = ToolStripButton()
        self.br.ForeColor = Color.Red
        self.br.Text = "Red"
        self.br.Name = "Red"
        self.bu = ToolStripButton()
        self.bu.ForeColor = Color.Blue
        self.bu.Text = "Blue"
        self.bu.Name = "Blue"
        self.br.Click += self.cc
        self.bu.Click += self.cc
        
        self.dd.Items.Add(self.br)
        self.dd.Items.Add(self.bu)
        self.ts.Items.Add(self.db)

##MenuStrip
        self.ms = MenuStrip()
        self.ms.Dock = DockStyle.Top
        self.Controls.Add(self.ms)

##ToolStripMenuItem
        self.mi = ToolStripMenuItem("File")
        self.mi2 = ToolStripMenuItem("Edit")
        self.mi3 = ToolStripMenuItem("View")
        self.mi4 = ToolStripMenuItem("Help")
        self.ms.Items.Add(self.mi)
        self.ms.Items.Add(self.mi2)
        self.ms.Items.Add(self.mi3)
        self.ms.Items.Add(self.mi4)

        ##File menu items
        self.mi.DropDownItems.Add("New",None,self.New_Document_Clicked)
        self.mi.DropDownItems.Add("Open",None,self.Open_Document_Clicked)

##ToolStripSplitButton
        self.ts.Items.Add(ToolStripSeparator())

        self.tssb = ToolStripSplitButton()
        self.tssb.Text = "ToolStripSplitButton"
        #self.tssb.ButtonClick += self.splitbutton_click
        self.tsmi = ToolStripMenuItem("Blue")
        self.tsmi.ForeColor = Color.Blue
        self.tsmi2 = ToolStripMenuItem("Red")
        self.tsmi2.ForeColor = Color.Red
        self.tsmi3 = ToolStripMenuItem("Green")
        self.tsmi3.ForeColor = Color.Green
        self.tsmi4 = ToolStripMenuItem("Black")
        self.tsmi4.ForeColor = Color.Black
        self.tsmi.ToolTipText = "Blue"
        self.tsmi2.ToolTipText = "Red"
        self.tsmi3.ToolTipText = "Green"
        self.tsmi4.ToolTipText = "Black"
        self.tsmi.Name = "Blue"
        self.tsmi2.Name = "Red"
        self.tsmi3.Name = "Green"
        self.tsmi4.Name = "Black"
        self.tsmi.Click += self.tsmi_c
        self.tsmi2.Click += self.tsmi2_c
        self.tsmi3.Click += self.tsmi3_c
        self.tsmi4.Click += self.tsmi4_c
        self.tssb.DropDownItems.Add(self.tsmi)
        self.tssb.DropDownItems.Add(self.tsmi2)
        self.tssb.DropDownItems.Add(self.tsmi3)
        self.tssb.DropDownItems.Add(self.tsmi4)

        self.ts.Items.Add(self.tssb)

##ToolStripTextBox
        self.ts.Items.Add(ToolStripSeparator())

        self.tstb = ToolStripTextBox()
        self.tstb.ToolTipText = "ToolStripTextBox"
        self.tstb.Name = "ToolStripTextBox"
        self.tstb.AcceptsReturn = True
        self.tstb.AcceptsTab = True
        self.tstb.MaxLength = 10
        
        self.ts.Items.Add(self.tstb)

##ToolStripLabel.IsLink
        self.ts.Items.Add(ToolStripSeparator())        
        self.count_l = 0
        self.linklabel = ToolStripLabel()
        self.linklabel.Text = "http://www.opensuse.org"
        self.linklabel.IsLink = True
        self.linklabel.AutoToolTip = True
        self.linklabel.Click += self.linklabel_c
        
        self.ts.Items.Add(self.linklabel)


##########################################################################################
##StatusStrip:
        self.statusstrip1 = StatusStrip()
        self.statusstrip1.GripStyle = ToolStripGripStyle.Visible
        self.statusstrip1.Name = "toolstrip1"
        self.Controls.Add(self.statusstrip1)
##ToolStripStatusLabel:
        self.toolstripstatuslabel1 = ToolStripStatusLabel()
        self.toolstripstatuslabel1.Text = "ToolStripStatusLabel..."
        self.toolstripstatuslabel1.BorderStyle = Border3DStyle.Raised
        self.toolstripstatuslabel1.BorderSides = ToolStripStatusLabelBorderSides.Bottom
        self.toolstripstatuslabel1.Spring = True

##ToolStripButton:
        self.toolstripbutton1 = ToolStripButton("Click Me")
        self.toolstripbutton1.Click += self.bc

        self.statusstrip1.Items.Add(self.toolstripbutton1)
        self.statusstrip1.Items.Add(self.toolstripstatuslabel1)

##ToolStripProgressBar
        self.toolstripprogressbar1 = ToolStripProgressBar()
        self.toolstripprogressbar1.Enabled = True
        self.toolstripprogressbar1.Name = "ToolStripProgressBar"
        self.toolstripprogressbar1.ToolTipText = "ToolStripProgressBar"
        self.statusstrip1.Items.Add(self.toolstripprogressbar1)


    def New_Document_Clicked(self, sender, event):
        self.rtb.Clear()
        self.count_n += 1
        print "Clicked \"New\" ToolStripButton %s times" % self.count_n

    def Open_Document_Clicked(self, sender, event):
        self.ofd = OpenFileDialog()
        if(self.ofd.ShowDialog() == DialogResult.OK):
            self.sr = System.IO.StreamReader(self.ofd.OpenFile())
            self.rtb.Text = self.sr.ReadToEnd()
            self.sr.Close()
        self.count_o += 1
        print "Clicked \"Open\" ToolStripButton %s times" % self.count_o

    def tscb_SelectedIndexChanged(self, sender, event):
        if(self.tscb.SelectedIndex >= 0):
            self.rtb.Font = Font(self.rtb.Font.Name, float.Parse(self.tscb.SelectedItem.ToString()))
        print "Selected ToolStripComboBox set the Font to Size=%s" % self.rtb.Font.Size

    def cc(self, sender, event):
        print "Selected ToolStripDropDownButton"
        
    def tsmi_c(self, sender, event):
        self.tssb.Text = "Blue"
        if(self.tssb.Text == "Blue"):
            self.rtb.ForeColor = Color.Blue
        print "Selected ToolStripSplitButton set ForeColor to \"Blue\""

    def tsmi2_c(self, sender, event):
        self.tssb.Text = "Red"
        if(self.tssb.Text == "Red"):
            self.rtb.ForeColor = Color.Red
        print "Selected ToolStripSplitButton set ForeColor to \"Red\""

    def tsmi3_c(self, sender, event):
        self.tssb.Text = "Green"    
        if(self.tssb.Text == "Green"):
            self.rtb.ForeColor = Color.Green
        print "Selected ToolStripSplitButton set ForeColor to \"Green\""

    def tsmi4_c(self, sender, event):
        self.tssb.Text = "Black"
        if(self.tssb.Text == "Black"):
            self.rtb.ForeColor = Color.Black
        print "Selected ToolStripSplitButton set ForeColor to \"Black\""

    def linklabel_c(self, sender, event):
        target = self.linklabel.Text
        System.Diagnostics.Process.Start(target)
        self.count_l += 1
        print "Click ToolStripLabel.IsLink %s times" % self.count_l

    def bc(object,sender,event):
        MessageBox.Show("the first plugin")
        print "Clicked ToolStripButton at StatusStrip"
    
   

form = RunApp()
Application.Run(form)


