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
        self.mi.DropDownItems.Add("New", None, self.new_clicked)
        self.mi.DropDownItems.Add("Open", None, self.open_clicked)
        openrecent = ToolStripMenuItem ()
        openrecent.Text = "Open Recent"
        foo = ToolStripMenuItem()
        foo.Text = "foo"
        bar = ToolStripMenuItem()
        bar.Text = "bar"
        openrecent.DropDownItems.Add (foo)
        openrecent.DropDownItems.Add (bar)
        self.mi.DropDownItems.Add(openrecent)
        #self.mi.DropDownItems.Add("Open Recent", None, self.open_clicked)
        #self.mi.DropDownItems(2).DropDownItems.Add("Foo", None, self.open_clicked)
        #elf.mi.DropDownItems(2).DropDownItems.Add("Bar", None, self.open_clicked)

        ##Edit menu items
        self.mi2.DropDownItems.Add("Copy This", None, self.copy_clicked)
        self.mi2.DropDownItems.Add("Paste That", None, self.paste_clicked)

        ##add ToolStripMenuItem to ToolStrip
        self.mi3 = ToolStripMenuItem("View")
        self.mi4 = ToolStripMenuItem("Help")

        self.ts.Items.Add(self.mi3)
        self.ts.Items.Add(self.mi4)

        ##View menu items
        self.mi3.DropDownItems.Add("Create",None,self.create_clicked)
        self.mi3.DropDownItems.Add("Write",None,self.write_clicked)

        ##Help menu items
        self.mi4.DropDownItems.Add("Financial", None, self.help_financial_clicked)
        self.mi4.DropDownItems.Add("Medical", None, self.help_medical_clicked)

    def new_clicked(self, sender, event):
        self.rtb.Clear()

    def open_clicked(self, sender, event):
        MessageBox.Show("Open Clicked", "Open Clicked")

    def help_financial_clicked(self, sender, event):
        self.rtb.AppendText('Have some money\n')
       
    def help_medical_clicked(self, sender, event):
        self.rtb.AppendText('Here is a bandage\n')
    
    def copy_clicked(self, sender, event):
        self.rtb.AppendText('Copy Clicked\n')

    def paste_clicked(self, sender, event):
        self.rtb.AppendText('Paste Clicked\n')
    
    def create_clicked(self, sender, event):
        MessageBox.Show("Create Clicked", "Create Clicked")

    def write_clicked(self, sender, event):
        self.rtb.AppendText('Write Clicked\n')

form = RunApp()
Application.Run(form)


