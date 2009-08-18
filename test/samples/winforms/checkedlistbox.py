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
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *

class CheckedListBoxSample(Form):
    """CheckedListBox control class"""

    def __init__(self):
        """CheckedListBoxSample class init function."""

        # setup title
        self.Text = "CheckedListBox control"
        self.Width = 300
        self.Height = 350

        # setup label1
        self.label1 = Label()
        self.label1.AutoSize = True
        self.label1.Location = Point(10, 20)
        self.label1.Text = "CheckOnClick True"

        # setup checkedlistboxs1
        self.checkedlistbox1 = CheckedListBox()
        self.checkedlistbox1.Name = "list1 name"
        self.checkedlistbox1.Text = "list1 text"
        self.checkedlistbox1.Location = Point(10, 50)
        self.checkedlistbox1.Width = 260
        self.checkedlistbox1.SelectedIndexChanged += self.select_change
        self.checkedlistbox1.CheckOnClick = True
        
        # setup label2
        self.label2 = Label()
        self.label2.AutoSize = True
        self.label2.Location = Point(10, 160)
        self.label2.Text = "CheckOnClick False"
        
        # setup checkedlistboxs2
        self.checkedlistbox2 = CheckedListBox()
        self.checkedlistbox2.Name = "list2 name"
        self.checkedlistbox2.Text = "list2 text"
        self.checkedlistbox2.Location = Point(10, 190)
        self.checkedlistbox2.Width = 260
        self.checkedlistbox2.ItemCheck += self.toggle_change
        self.checkedlistbox2.CheckOnClick = False

        # add items in CheckedListBox
        for i in range(20):
            self.checkedlistbox1.Items.Add(str(i))
        for i in range(20, 50):
            self.checkedlistbox2.Items.Add(str(i))

        # add controls
        self.Controls.Add(self.label1)
        self.Controls.Add(self.checkedlistbox1)
        self.Controls.Add(self.label2)
        self.Controls.Add(self.checkedlistbox2)

    # SelectedIndexChanged change label's texts
    def select_change(self, sender, event):
        item = ""
        for i in range(self.checkedlistbox1.CheckedItems.Count):
            item += "" + self.checkedlistbox1.CheckedItems[i]
            item += " "
        self.label1.Text =  "Item " + item + "Checked"

    #ItemCheck change label's texts
    def toggle_change(self, sender, event):
        if event.CurrentValue == CheckState.Unchecked:
            item = self.checkedlistbox2.Items[event.Index]
            self.label2.Text =  "Item " + item + " Checked"
        elif event.CurrentValue == CheckState.Checked:
            item = self.checkedlistbox2.Items[event.Index]
            self.label2.Text =  "Item " + item + " Unchecked"

# run application
form = CheckedListBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
