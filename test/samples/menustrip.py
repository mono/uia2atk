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
from System.Drawing import *
import sys

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
        self.menuitem_file = ToolStripMenuItem("&File")
        self.menuitem_file.Click += self.click

        self.menuitem_edit = ToolStripMenuItem("&Edit")
        self.menuitem_edit.Click += self.click

        # menu items
        self.menuitem_file_new = ToolStripMenuItem("&New")
        self.menuitem_file_new.Click += self.click

        sample_dir = sys.path[0]
        image_path = "%s/%s" % (sample_dir, "apple-red.png")
        self.menuitem_file_new_doc = ToolStripMenuItem("&Document")
        self.menuitem_file_new_doc.Image = Bitmap.FromFile(image_path)
        self.menuitem_file_new_doc.Click += self.click

        self.menuitem_file_open = ToolStripMenuItem("&Open")
        self.menuitem_file_open.Click += self.click

        self.menuitem_edit_copy = ToolStripMenuItem("&Copy")
        self.menuitem_edit_copy.Click += self.click

        self.menuitem_edit_paste = ToolStripMenuItem("&Paste")
        self.menuitem_edit_paste.Click += self.click

        # Add controls
        self.menuitem_file.DropDownItems.Add(self.menuitem_file_new)
        self.menuitem_file.DropDownItems.Add(self.menuitem_file_open)
        self.menuitem_file_new.DropDownItems.Add(self.menuitem_file_new_doc)
        self.menuitem_edit.DropDownItems.Add(self.menuitem_edit_copy)
        self.menuitem_edit.DropDownItems.Add(self.menuitem_edit_paste)
        self.menustrip.Items.Add(self.menuitem_file)
        self.menustrip.Items.Add(self.menuitem_edit)
        self.Controls.Add(self.label)
        self.Controls.Add(self.menustrip)

    def click(self, sender, event):
        self.label.Text = "You are clicking %s" % sender.Text

form = MenuStripSample()
Application.Run(form)
