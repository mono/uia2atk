#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/10/2008
# Description: the sample for winforms control:
#              CheckedListBox
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "CheckedListBox" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, Form, CheckedListBox, Label, DockStyle

class CheckedListBoxSample(Form):
    """CheckedListBox control class"""

    def __init__(self):
        """CheckedListBoxSample class init function."""

        # setup title
        self.Text = "CheckedListBox control"

        # setup label
        self.label = Label()
        self.label.Height = 100
        self.label.Dock = DockStyle.Top
        
        # setup checkedlistbox
        self.checkedlistbox = CheckedListBox()
        self.checkedlistbox.Dock = DockStyle.Top
        self.checkedlistbox.SelectedIndexChanged += self.change
        self.checkedlistbox.CheckOnClick = True

        # add items in CheckedListBox
        for i in range(5):
            self.label.Text += "Item %d is: \n" % i 
            self.checkedlistbox.Items.Add(str(i))

        # add controls
        self.Controls.Add(self.checkedlistbox)
        self.Controls.Add(self.label)

    # CheckedListBox check change event
    def change(self, sender, event):
        """select a item"""

        self.label.Text = "" 
        for i in range(5):
            self.label.Text += "Item %d is: " % i + \
                            str(self.checkedlistbox.GetItemCheckState(i)) + "\n"

# run application
form = CheckedListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
