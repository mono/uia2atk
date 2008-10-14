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
from System.Windows.Forms import Application, Form, ListBox, Label, DockStyle
from System.Drawing import Size

class ListBoxSample(Form):
    """ListBox control class"""

    def __init__(self):
        """ListBoxSample class init function."""

        # setup title
        self.Text = "ListBox control"

        # setup label
        self.label1 = Label()
        self.label1.Text = "listbox with vertical scrollbar" 
        self.label1.AutoSize = True
        self.label1.Dock = DockStyle.Top

        self.label2 = Label()
        self.label2.Text = "listbox with horizontal scrollbar" 
        self.label2.AutoSize = True
        self.label2.Dock = DockStyle.Top

        # setup listbox
        self.listbox1 = ListBox()
        self.listbox1.Dock = DockStyle.Top

        self.listbox2 = ListBox()
        self.listbox2.Dock = DockStyle.Top
        self.listbox2.MultiColumn = True

        # add items in ListBox
        for i in range(30):
            self.listbox1.Items.Add(str(i))
            self.listbox2.Items.Add(str(i))

        # add controls
        self.Controls.Add(self.listbox2)
        self.Controls.Add(self.label2)
        self.Controls.Add(self.listbox1)
        self.Controls.Add(self.label1)

# run application
form = ListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
