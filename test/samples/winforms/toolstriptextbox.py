#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        12/10/2008
# Description: This is a test application sample for winforms control:
#              ToolStripTextBox
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
        self.Text = "ToolStripTextBox control"
        self.Width = 380
        self.Height = 200
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.label = Label()
        self.label.Text = "Your input:"
        self.label.Location = Point(10, 80)
        self.label.AutoSize = True
        self.Controls.Add(self.label)

        #ToolStrip:
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)

        #ToolStripTextBox1
        self.tstb1 = ToolStripTextBox()
        self.tstb1.ToolTipText = "SingleLine"
        self.tstb1.AccessibleName = "ToolStripTextBox1"
        self.tstb1.AccessibleDescription = "SingleLine"
        self.tstb1.AcceptsReturn = True
        self.tstb1.AcceptsTab = True
        self.tstb1.MaxLength = 10
        self.tstb1.TextChanged += self.textbox_click

        #ToolStripTextBox2
        self.tstb2 = ToolStripTextBox()
        self.tstb2.ToolTipText = "MultiLine"
        self.tstb2.AccessibleName = "ToolStripTextBox2"
        self.tstb2.AccessibleDescription = "MultiLine"
        self.tstb2.AcceptsReturn = True
        self.tstb2.AcceptsTab = True
        self.tstb2.Multiline = True
        self.tstb2.TextChanged += self.textbox_click

        #ToolStripTextBox3
        self.tstb3 = ToolStripTextBox()
        self.tstb3.ToolTipText = "ReadOnly"
        self.tstb3.AccessibleName = "ToolStripTextBox3"
        self.tstb3.AccessibleDescription = "ReadOnly"
        self.tstb3.AcceptsReturn = True
        self.tstb3.AcceptsTab = True
        self.tstb3.ReadOnly = True
        self.tstb3.TextChanged += self.textbox_click
        
        self.ts.Items.Add(self.tstb1)
        self.ts.Items.Add(self.tstb2)
        self.ts.Items.Add(self.tstb3)

    def textbox_click(self, sender, event):
        self.label.Text = "Your input:%s" % sender.Text

form = RunApp()
Application.Run(form)


