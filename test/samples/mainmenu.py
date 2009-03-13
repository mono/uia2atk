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
        self.label.Text = "Click a sub-menu of Main menu"
        self.label.Dock = DockStyle.Bottom

        # setup mainmenu
        self.mainmenu = MainMenu()

        # setup main menus
        self.menu_file = MenuItem()
        self.menu_file.Text = "&File"

        self.menu_edit = MenuItem()
        self.menu_edit.Text = "&Edit"

        self.menu_help = MenuItem()
        self.menu_help.Text = "&Help"

        # setup submenus of "File"
        self.menu_file_new = MenuItem()
        self.menu_file_new.Text = "&New"
        self.menu_file_new.Click += self.on_click

        self.menu_file_new_doc = MenuItem()
        self.menu_file_new_doc.Text = "&Document"
        self.menu_file_new_doc.Click += self.on_click

        self.menu_file_open = MenuItem()
        self.menu_file_open.Text = "&Open"
        self.menu_file_open.Click += self.on_click

        self.menu_file_exit = MenuItem()
        self.menu_file_exit.Text = "E&xit"
        self.menu_file_exit.Click += self.on_click

        # setup submenus of "Edit"
        self.menu_edit_undo = MenuItem()
        self.menu_edit_undo.Text = "&Undo"
        self.menu_edit_undo.Click += self.on_click

        self.menu_edit_redo = MenuItem()
        self.menu_edit_redo.Text = "&Redo"
        self.menu_edit_redo.Click += self.on_click

        # setup psubmenus of "Help"
        self.menu_help_about = MenuItem()
        self.menu_help_about.Text = "&About"
        self.menu_help_about.Click += self.on_click

        # add menu item to mainmenu
        self.mainmenu.MenuItems.Add(self.menu_file)
        self.mainmenu.MenuItems.Add(self.menu_edit)
        self.mainmenu.MenuItems.Add(self.menu_help)

        # add submenus
        self.menu_file.MenuItems.Add(self.menu_file_new)
        self.menu_file_new.MenuItems.Add(self.menu_file_new_doc)
        self.menu_file.MenuItems.Add(self.menu_file_open)
        self.menu_file.MenuItems.Add(self.menu_file_exit)
        self.menu_edit.MenuItems.Add(self.menu_edit_undo)
        self.menu_edit.MenuItems.Add(self.menu_edit_redo)
        self.menu_help.MenuItems.Add(self.menu_help_about)

        # bind the MainMenu to Form
        self.Menu = self.mainmenu
        self.Controls.Add(self.label)

    def on_click(self, sender, evernt):
        if sender == self.menu_file_exit:
            Application.Exit()
        elif sender == self.menu_help_about:
            MessageBox.Show("Mono:Accessibility winform controls test sample\n"
                            "Developer: Novell a11y hackers",
                            "About")
        else:
            self.label.Text = "You have clicked %s" % sender.Text

# run application
form = MainMenuSample()
Application.EnableVisualStyles()
Application.Run(form)
