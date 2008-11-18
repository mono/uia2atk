#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/24/2008
# Description: the sample for winforms control:
#              ListView
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ListView" control with Details mode, CheckBoxes property and CheckItem event in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System

class ListViewSample(Form):
    """ListView control class"""

    def __init__(self):
        """ListViewSample class init function."""

        # setup title
        self.Text = "ListView control"
        self.Width = 380
        self.Height = 330
        self.toggle = True

        # setup label
        self.label = Label()
        self.label.Text = "View.Details mode with CheckBoxes property and ColumnClick&ItemCheck events."
        self.label.Dock = DockStyle.Top

        # setup listview
        self.listview = ListView()
        # set the view to show details.
        self.listview.View = View.Details
        # allow the user to edit item text.
        self.listview.CheckBoxes = True
        self.listview.FullRowSelect = True
        # display grid lines.
        self.listview.GridLines = True
        # sort the items in the list in ascending order.
        self.listview.Sorting = SortOrder.Ascending
        # place widget besides left.
        self.listview.Dock = DockStyle.Top
        self.listview.Width = 350
        self.listview.Height = 260
        self.listview.ColumnClick += self.on_click
        self.listview.ItemCheck += self.item_click

        # add conlumns
        self.listview.Columns.Add("Column A", 200, HorizontalAlignment.Left)
        self.listview.Columns.Add("Num", 200, HorizontalAlignment.Left)

        # add items
        listItem = ["Item0", "Item1", "Item2", "Item3", "Item4", "Item5"]
        num = ["0", "1", "2", "3", "4", "5"]

        self.listview.BeginUpdate()

        for count in range(6):
            self.listItem = ListViewItem(listItem[count])
            self.listItem.SubItems.Add(num[count])
            self.listview.Items.Add(self.listItem)
 
        self.listview.EndUpdate()

        # setup label
        self.label1 = Label()
        self.label1.Text = "click the CheckBox to sum the num"
        self.label1.Dock = DockStyle.Top

        # add controls
        self.Controls.Add(self.listview)
        self.Controls.Add(self.label1)
        self.Controls.Add(self.label)


    def on_click(self, sender, event):
        if self.toggle == True:
            self.listview.Sorting = SortOrder.Descending
            self.toggle = False
        else:
            self.listview.Sorting = SortOrder.Ascending
            self.toggle = True

    def item_click(self, sender, event):
        num = 0
        checkeditems = self.listview.CheckedItems
        for item in checkeditems:
            num += System.Double.Parse(item.SubItems[1].Text)
        if event.CurrentValue == CheckState.Unchecked:
            num += System.Double.Parse(self.listview.Items[event.Index].SubItems[1].Text)
        elif event.CurrentValue == CheckState.Checked:
            num -= System.Double.Parse(self.listview.Items[event.Index].SubItems[1].Text)
        self.label1.Text = "sum = " + num.ToString()

# run application
form = ListViewSample()
Application.EnableVisualStyles()
Application.Run(form)
