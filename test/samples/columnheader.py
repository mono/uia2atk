#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        12/24/2008
# Description: the sample for winforms control:
#              ColumnHeader
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ColumnHeader" control.It can be used for 
Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System

class ColumnHeaderSample(Form):
    """ColumnHeader control class"""

    def __init__(self):
        """ColumnHeaderSample class init function."""

        # setup title
        self.Text = "ColumnHeader control"
        self.Width = 450
        self.Height = 330
        self.toggle = True

        # setup label
        self.label = Label()
        self.label.Text = "Click column header to sort the order for items"
        self.label.Dock = DockStyle.Top

        # setup listview
        self.listview = ListView()
        # set the view to show details.
        self.listview.View = View.Details
        # display grid lines.
        self.listview.GridLines = True
        # sort the items in the list in ascending order.
        self.listview.Sorting = SortOrder.Ascending
        # place widget besides left.
        self.listview.Dock = DockStyle.Top
        self.listview.Width = 350
        self.listview.Height = 260
        self.listview.ColumnClick += self.on_click

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

        # add controls
        self.Controls.Add(self.listview)
        self.Controls.Add(self.label)


    def on_click(self, sender, event):
        if self.toggle == True:
            self.listview.Sorting = SortOrder.Descending
            self.toggle = False
        else:
            self.listview.Sorting = SortOrder.Ascending
            self.toggle = True

# run application
form = ColumnHeaderSample()
Application.EnableVisualStyles()
Application.Run(form)
