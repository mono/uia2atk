#!/usr/bin/env ipy
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/08/2008
# Description: The sample for winforms control:
#              ToolStrip
#              ToolStripSplitButton
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolStripSplitButton" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System
import sys


class ToolStripSplitButtonSample(Form):

    def __init__(self):

        # setup form
        self.Text = "ToolStripSplitButton Control"

        self.label = Label()
        self.label.Text = "The current font size is 10"
        self.label.Dock = DockStyle.Top

        self.labelcount = Label()
        self.labelcount.Text = "0 clicks"
        self.labelcount.Dock = DockStyle.Bottom

        self.clickscount = 0

        # ToolStrip:
        self.toolstrip = ToolStrip()
        self.toolstrip.Text = "ToolStrip"

        # ToolStripLabel
        self.toolstriplabel = ToolStripLabel("Select font size:")

        # ToolStripMenuItems
        self.toolstrip_menuitem_10 = ToolStripMenuItem("10")
        self.toolstrip_menuitem_10.Click += self.menuitem_click
        self.toolstrip_menuitem_12 = ToolStripMenuItem("12")
        self.toolstrip_menuitem_12.Click += self.menuitem_click
        self.toolstrip_menuitem_14 = ToolStripMenuItem("14")
        self.toolstrip_menuitem_14.Click += self.menuitem_click

        # ToolStripSplitButton
        self.toolstrip_splitbutton = ToolStripSplitButton()
        self.toolstrip_splitbutton.Click += self.button_click
        self.toolstrip_splitbutton.Text = self.toolstrip_menuitem_10.Text
        sample_dir = sys.path[0]
        image_path = "%s/%s" % (sample_dir, "apple-red.png")
        self.toolstrip_splitbutton.Image = Bitmap.FromFile(image_path)
        self.toolstrip_splitbutton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight


        # add Controls
        self.toolstrip_splitbutton.DropDownItems.Add(self.toolstrip_menuitem_10)
        self.toolstrip_splitbutton.DropDownItems.Add(self.toolstrip_menuitem_12)
        self.toolstrip_splitbutton.DropDownItems.Add(self.toolstrip_menuitem_14)
        self.toolstrip.Items.Add(self.toolstriplabel)
        self.toolstrip.Items.Add(self.toolstrip_splitbutton)
        self.Controls.Add(self.label)
        self.Controls.Add(self.labelcount)
        self.Controls.Add(self.toolstrip)

    def menuitem_click(self, sender, event):
        self.label.Text = "The current font size is %s" % sender.Text
        self.toolstrip_splitbutton.Text = sender.Text
        self.label.Font = Font(self.label.Font.Name, int(sender.Text))

    def button_click(self, sender, event):
        self.clickscount = self.clickscount + 1
        self.labelcount.Text = "%s clicks" % self.clickscount

form = ToolStripSplitButtonSample()
Application.Run(form)
