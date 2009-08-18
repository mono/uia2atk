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
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *

class ListBoxSample(Form):
    """ListBox control class"""

    def __init__(self):
        """ListBoxSample class init function."""

        # setup title
        self.Text = "ListBox control"
        self.Height = 230
        self.Width = 370

        # setup checkbox to change event
        self.checkbox = CheckBox()
        self.checkbox.Text = "SelectedIndexChanged Event"
        self.checkbox.Dock = DockStyle.Top
        self.checkbox.AutoSize = True
        self.checkbox.Click += self.checkbox_click

        # setup label
        self.label = Label()
        self.label.Text = "You select " 
        self.label.AutoSize = True
        self.label.Dock = DockStyle.Top

        # setup listbox
        self.listbox = ListBox()
        self.listbox.Name = "listbox name"
        self.listbox.Text = "listbox text"
        self.listbox.Height = 150
        self.listbox.Dock = DockStyle.Top
        self.listbox.Click += self.select
        self.listbox.MultiColumn = True
        self.listbox.ItemHeight = 20

        # add items in ListBox
        for i in range(20):
            self.listbox.Items.Add(str(i))

        self.listbox.SetSelected(0, True)

        # add controls
        self.Controls.Add(self.listbox)
        self.Controls.Add(self.label)
        self.Controls.Add(self.checkbox)

    def checkbox_click(self, sender, event):
        if sender.Checked:
            self.listbox.SelectedIndexChanged += self.select
        else:
            self.listbox.SelectedIndexChanged -= self.select

    # ListBox click event
    def select(self, sender, event):
        """select a item"""
        self.label.Text = "You select " + self.listbox.SelectedItem

# run application
form = ListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
