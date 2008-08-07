#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        08/07/2008
# Description: the sample for winforms control:
#              ContextMenuStrip
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ContextMenuStrip" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, Form, Label, BorderStyle, ContextMenuStrip, ToolStripMenuItem
)
from System.Drawing import Color

class ContextMenuStripSample(Form):
    """ContextMenuStrip control class"""

    def __init__(self):
        """ContextMenuStripSample class init function."""

        # set up form
        self.Text = "ContextMenuStrip control"

        # set up menu items
        self.toolstrip_menuitem1 = ToolStripMenuItem("Apple")
        self.toolstrip_menuitem1.Click += self.cms_click
        self.toolstrip_menuitem2 = ToolStripMenuItem("Banana")
        self.toolstrip_menuitem2.Click += self.cms_click
        self.toolstrip_menuitem3 = ToolStripMenuItem("Watermelon")
        self.toolstrip_menuitem3.Click += self.cms_click
        self.toolstrip_menuitem4 = ToolStripMenuItem("Orange")
        self.toolstrip_menuitem4.Click += self.cms_click
        self.toolstrip_menuitem5 = ToolStripMenuItem("Peach")
        self.toolstrip_menuitem5.Click += self.cms_click

        # set up context_menu_strip
        self.context_menu_strip = ContextMenuStrip()
        self.context_menu_strip.Items.Add(self.toolstrip_menuitem1)
        self.context_menu_strip.Items.Add(self.toolstrip_menuitem2)
        self.context_menu_strip.Items.Add(self.toolstrip_menuitem3)
        self.context_menu_strip.Items.Add(self.toolstrip_menuitem4)
        self.context_menu_strip.Items.Add(self.toolstrip_menuitem5)

        # set up label
        self.label = Label()
        self.label.Text = "Right Click on me to see ContextMenuStrip"
        self.label.Width = 200
        self.label.Height = 50
        self.label.ContextMenuStrip = self.context_menu_strip
        self.label.BackColor = Color.Cyan
        self.label.BorderStyle = BorderStyle.FixedSingle

        # add controls
        self.Controls.Add(self.label)
        
    def cms_click(self, sender, event):
         self.label.Text = "You have clicked %s" % sender.Text

# run application
form = ContextMenuStripSample()
Application.EnableVisualStyles()
Application.Run(form)
