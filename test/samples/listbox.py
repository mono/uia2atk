#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/19/2008
# Description: the sample for winforms control:
#              ListBox
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ListBox" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, Form, ListBox, Label, DockStyle

class ListBoxSample(Form):
    """ListBox control class"""

    def __init__(self):
        """ListBoxSample class init function."""

        # setup title
        self.Text = "ListBox control"

        # setup label
        self.label = Label()
        self.label.Text = "You select " 
        self.label.AutoSize = True
        self.label.Dock = DockStyle.Top

        # setup listbox
        self.listbox = ListBox()
        self.listbox.Dock = DockStyle.Top
        self.listbox.Click += self.select

        # add items in ListBox
        for i in range(10):
            self.listbox.Items.Add(str(i))

        # add controls
        self.Controls.Add(self.listbox)
        self.Controls.Add(self.label)

    # ListBox click event
    def select(self, sender, event):
        """select a item"""
        self.label.Text = "You select " + self.listbox.SelectedItem

# run application
form = ListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
