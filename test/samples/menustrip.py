#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <cachen@novell.com>
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

        # form
        self.Text = "MenuStrip Control"

        # label
        self.label = Label()
        self.label.Text = "You click:"
        self.label.Dock = DockStyle.Top

        # MenuStrip
        self.menustrip = MenuStrip()
        self.menustrip.Dock = DockStyle.Top
        self.menustrip.Select += self.item_click

        # Add ToolStripMenuItem to MenuStrip
        self.menuitem_file = ToolStripMenuItem("File")
        self.menuitem_edit = ToolStripMenuItem("Edit")

        # Add controls
        self.Controls.Add(self.label)
        self.menustrip.Items.Add(self.menuitem_file)
        self.menustrip.Items.Add(self.menuitem_edit)
        self.Controls.Add(self.menustrip)

    def item_click(self, sender, event):
        self.label.Text = "You click: %s" % sender.Text

form = MenuStripSample()
Application.Run(form)
