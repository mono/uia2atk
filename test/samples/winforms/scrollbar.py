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
from System.Drawing import Size, Point

class ListBoxSample(Form):
    """ListBox control class"""

    def __init__(self):
        """ListBoxSample class init function."""

        # setup title
        self.Text = "ScrollBar control"

        # setup label
        self.label1 = Label()
        self.label1.Text = "listbox with vertical scrollbar" 
        self.label1.AutoSize = True
        self.label1.Location = Point(10, 10)

        # setup listbox1
        self.listbox1 = ListBox()
        self.listbox1.Width = 260
        self.listbox1.Location = Point(10, 40)

        self.label2 = Label()
        self.label2.Text = "listbox with horizontal scrollbar" 
        self.label2.AutoSize = True
        self.label2.Location = Point(10, 150)

        # setup listbox2
        self.listbox2 = ListBox()
        self.listbox2.Width = 260
        self.listbox2.MultiColumn = True
        self.listbox2.Location = Point(10, 180)

        # add items in ListBox
        for i in range(30):
            self.listbox1.Items.Add(str(i))
            self.listbox2.Items.Add(str(i))

        # add controls
        self.Controls.Add(self.label1)
        self.Controls.Add(self.listbox1)
        self.Controls.Add(self.label2)
        self.Controls.Add(self.listbox2)


# run application
form = ListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
