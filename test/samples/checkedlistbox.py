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
        self.label1 = Label()
        self.label1.AutoSize = True
        self.label1.Dock = DockStyle.Top
        self.label1.Text = "CheckOnClick True"
        
        self.label2 = Label()
        self.label2.AutoSize = True
        self.label2.Dock = DockStyle.Top
        self.label2.Text = "CheckOnClick False"
        
        # setup checkedlistboxs
        self.checkedlistbox1 = CheckedListBox()
        self.checkedlistbox1.Dock = DockStyle.Top
        self.checkedlistbox1.SelectedIndexChanged += self.change
        self.checkedlistbox1.CheckOnClick = True

        self.checkedlistbox2 = CheckedListBox()
        self.checkedlistbox2.Dock = DockStyle.Top
        self.checkedlistbox2.SelectedIndexChanged += self.change
        self.checkedlistbox2.CheckOnClick = False

        # add items in CheckedListBox
        for i in range(20):
            self.checkedlistbox1.Items.Add(str(i))
            self.checkedlistbox2.Items.Add(str(i))

        # add controls
        self.Controls.Add(self.checkedlistbox2)
        self.Controls.Add(self.label2)
        self.Controls.Add(self.checkedlistbox1)
        self.Controls.Add(self.label1)

    # CheckedListBox check change event
    def change(self, sender, event):
        """select a item"""

#        items = "" 
#        for i in range(20):
#            status = str(self.checkedlistbox1.GetItemCheckState(i))
#            if status == "Checked":
#                items += "%d " % i

        if sender is self.checkedlistbox1:
            items = self.check_items(self.checkedlistbox1)
            self.label1.Text = "Item " + items + ": " + "Checked"

        if sender is self.checkedlistbox2:
            items = self.check_items(self.checkedlistbox2)
            self.label2.Text = "Item " + items + ": " + "Checked"

    def check_items(self, control):
       items = "" 
       for i in range(20):
           status = str(control.GetItemCheckState(i))
           if status == "Checked":
               items += "%d " % i
       return items

# run application
form = CheckedListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
