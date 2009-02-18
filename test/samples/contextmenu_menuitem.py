#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/25/2008
# Description: the sample for winforms control:
#              ContextMenu
#              MenuItem
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ContextMenu" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, ContextMenu, Form, MenuItem, Timer, Label, BorderStyle
) 
from System.Drawing import Icon, Color

class ContextMenuSample(Form):
    """ContextMenu control class"""

    def __init__(self):
        """ContextMenuSample class init function."""

        # setup title
        self.Text = "ContextMenu_MenuItem control"

        # create menuitems
        self.menuitem1 = MenuItem("Item 1")
        self.menuitem2 = MenuItem("Item 2")
        self.menuitem2.RadioCheck = True
        self.menuitem2.Checked = True
        self.menuitem3 = MenuItem("Item 3")
        self.menuitem3.Checked = True
        self.menuitem4 = MenuItem("Exit")

        self.menuitem1.Click += self.on_click
        self.menuitem2.Click += self.on_click
        self.menuitem3.Click += self.on_click
        self.menuitem4.Click += self.on_click

        # setup contextmenu
        self.contextmenu = ContextMenu()
        self.contextmenu.MenuItems.Add(self.menuitem1)
        self.contextmenu.MenuItems.Add(self.menuitem2)
        self.contextmenu.MenuItems.Add(self.menuitem3)
        self.contextmenu.MenuItems.Add(self.menuitem4)

        # setup label
        self.label = Label()
        self.label.Text = "Right Click on me to see ContextMenu and MenuItem"
        self.label.Width = 200
        self.label.Height = 50
        self.label.ContextMenu = self.contextmenu
        self.label.BackColor = Color.Cyan
        self.label.BorderStyle = BorderStyle.FixedSingle

        # add control
        self.Controls.Add(self.label)

    def on_click(self, sender, event):
        if sender == self.menuitem2 or sender == self.menuitem3:
            sender.Checked = not sender.Checked
            status = sender.Checked and ' ' or ' not '
            self.label.Text = "%s is%schecked" % (sender.Text, status)
        elif sender == self.menuitem4:
            Application.Exit()
        else:
            self.label.Text = "you have clicked %s" % sender.Text

# run application
form = ContextMenuSample()
Application.EnableVisualStyles()
Application.Run(form)
