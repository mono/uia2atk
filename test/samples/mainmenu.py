#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/02/2008
# Description: the sample for winforms control:
#              MainMenu
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "MainMenu" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, Form, Label, MainMenu, MenuItem, DockStyle
)

class MainMenuSample(Form):
    """MainMenu control class"""

    def __init__(self):
        """MainMenuSample class init function."""

        # setup title
        self.Text = "MainMenu control"

        # setup mainmenu
        self.mainmenu = MainMenu()

        # setup menuitems
        self.menuitem1 = MenuItem()
        self.menuitem1.Text = "&File"

        self.menuitem2 = MenuItem()
        self.menuitem2.Text = "&Edit"

        # add two MenuItem objects to the MainMenu
        self.mainmenu.MenuItems.Add(self.menuitem1)
        self.mainmenu.MenuItems.Add(self.menuitem2)

        # bind the MainMenu to Form
        self.Menu = self.mainmenu

# run application
form = MainMenuSample()
Application.EnableVisualStyles()
Application.Run(form)
