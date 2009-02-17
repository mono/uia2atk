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

        # MenuStrip
        self.menustrip = MenuStrip()
        self.menustrip.Dock = DockStyle.Top

        # Add ToolStripMenuItem to MenuStrip
        self.menuitem_file = ToolStripMenuItem("File")
        self.menuitem_edit = ToolStripMenuItem("Edit")

        # Add controls
        self.menustrip.Items.Add(self.menuitem_file)
        self.menustrip.Items.Add(self.menuitem_edit)
        self.Controls.Add(self.menustrip)

form = MenuStripSample()
Application.Run(form)
