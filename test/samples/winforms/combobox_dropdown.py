#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/19/2008
# Description: the sample for winforms control:
#              ComboBox
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ComboBox" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import *
from System.Drawing import *

class ComboBoxSample(Form):
    """ComboBox control class"""

    def __init__(self):
        """ComboBoxSample class init function."""

        # setup title
        self.Text = "ComboBox control"

        # setup label
        self.label = Label()
        self.label.Text = "You select " 
        self.label.AutoSize = True
        self.label.Dock = DockStyle.Top

        # setup combobox
        self.combobox = ComboBox()
        self.combobox.MaxDropDownItems = 3
        self.combobox.Dock = DockStyle.Top
        self.combobox.DropDownStyle = ComboBoxStyle.DropDown
        self.combobox.SelectionChangeCommitted += self.select
        self.combobox.TextChanged += self.select
        self.combobox.ItemHeight = 20

        # add items in ComboBox
        for i in range(10):
            self.combobox.Items.Add(str(i))
        self.combobox.SelectedIndex = 1

        # add controls
        self.Controls.Add(self.combobox)
        self.Controls.Add(self.label)

    # ComboBox click event
    def select(self, sender, event):
        """select a item"""
        self.label.Text = "You select " + self.combobox.Text

# run application
form = ComboBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
