#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: This is a test application sample for winforms control:
#              MenuStrip
##############################################################################

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *

class MenuStripSample(Form):

    def __init__(self):

        # Form
        self.Text = "MenuStrip Control"

        # Label
        self.label = Label()
        self.label.Dock = DockStyle.Top

        # MenuStrip
        self.menustrip = MenuStrip()
        self.menustrip.Dock = DockStyle.Top
        self.menustrip.Click += self.click

        # Add ToolStripMenuItem to MenuStrip
        self.menuitem_file = ToolStripMenuItem("File")
        self.menuitem_file.Click += self.click

        self.menuitem_edit = ToolStripMenuItem("Edit")
        self.menuitem_edit.Click += self.click

        # Add controls
        self.menustrip.Items.Add(self.menuitem_file)
        self.menustrip.Items.Add(self.menuitem_edit)
        self.Controls.Add(self.label)
        self.Controls.Add(self.menustrip)

    def click(self, sender, event):
        self.label.Text = "You are clicking %s" % sender.Text

form = MenuStripSample()
Application.Run(form)
