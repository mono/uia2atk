#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/02/2008
# Description: the sample for winforms control:
#              MainMenu
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "MainMenu" and "MenuItem" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, Form, Label, MainMenu, MenuItem, DockStyle, MessageBox
)

class MainMenuSample(Form):
    """MainMenu control class"""

    def __init__(self):
        """MainMenuSample class init function."""

        # setup title
        self.Text = "MainMenu control"

        # setup label
        self.label = Label()
        self.label.Dock = DockStyle.Top

        # setup mainmenu
        self.mainmenu = MainMenu()

        # setup main menus
        self.menu_file = MenuItem("&File")
        self.menu_file.Click += self.click

        self.menu_edit = MenuItem("&Edit")
        self.menu_edit.Click += self.click

        # add menu item to mainmenu
        self.mainmenu.MenuItems.Add(self.menu_file)
        self.mainmenu.MenuItems.Add(self.menu_edit)

        # bind the MainMenu to Form
        self.Menu = self.mainmenu
        self.Controls.Add(self.label)
        
    def click(self, sender, event):
        self.label.Text = "You are clicking %s" % sender.Text

# run application
form = MainMenuSample()
Application.EnableVisualStyles()
Application.Run(form)
