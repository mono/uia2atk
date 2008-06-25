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
from System.Windows.Forms import Application, ContextMenu, Form, MenuItem, Timer, Label
from System.Drawing import Icon

class ContextMenuSample(Form):
    """ContextMenu control class"""

    def __init__(self):
        """ContextMenuSample class init function."""

        # setup title
        self.Text = "ContextMenu control"

        # create menuitems
        self.menuitem1 = MenuItem("Item 1")
        self.menuitem2 = MenuItem("Item 2")
        self.menuitem3 = MenuItem("Item 3")
        self.menuitem4 = MenuItem("Exit")
        self.menuitem4.Click += self.on_exit

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
        self.label.Height = 100
        self.label.ContextMenu = self.contextmenu

        # add control
        self.Controls.Add(self.label)

    def on_exit(self, sender, event):
        Application.Exit() 

# run application
form = ContextMenuSample()
Application.EnableVisualStyles()
Application.Run(form)
