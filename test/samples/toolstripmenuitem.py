#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        11/14/2008
# Description: This is a test application sample for winforms control:
#              ToolStripMenuItem
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
        self.Text = "ToolStripMenuItem control"
        self.Width = 300
        self.Height = 300
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.count = 0        
        self.count_n = 0
    
        self.rtb = TextBox()
        self.rtb.Multiline = True
        self.rtb.Dock = DockStyle.Fill
        self.rtb.BorderStyle = BorderStyle.FixedSingle
        self.Controls.Add(self.rtb)

##ToolStrip:
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)

##MenuStrip
        self.ms = MenuStrip()
        self.ms.Dock = DockStyle.Top
        self.Controls.Add(self.ms)

##add ToolStripMenuItem to MenuStrip
        self.mi = ToolStripMenuItem("File")
        self.mi2 = ToolStripMenuItem("Edit")

        self.ms.Items.Add(self.mi)
        self.ms.Items.Add(self.mi2)

        ##File menu items
        self.mi.DropDownItems.Add("New",None,self.New_Document_Clicked)
        self.mi.DropDownItems.Add("Open",None,self.Open_Document_Clicked)

##add ToolStripMenuItem to ToolStrip
        self.mi3 = ToolStripMenuItem("View")
        self.mi4 = ToolStripMenuItem("Help")

        self.ts.Items.Add(self.mi3)
        self.ts.Items.Add(self.mi4)

        ##File menu items
        self.mi3.DropDownItems.Add("Creat",None,self.New_Document_Clicked)
        self.mi3.DropDownItems.Add("Write",None,self.Open_Document_Clicked)

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

form = RunApp()
Application.Run(form)


