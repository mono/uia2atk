#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/24/2008
# Description: the sample for winforms control:
#              ListView
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ListView" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, DockStyle, Form, ListView, View, SortOrder, HorizontalAlignment, Label
)

class ListViewSample(Form):
    """ListView control class"""

    def __init__(self):
        """ListViewSample class init function."""

        # setup title
        self.Text = "ListView control"
        self.Width = 300
        self.Height = 300
        self.toggle = True

        # setup label
        self.label = Label()
        self.label.Text = "Click Column header to switch items Ascending and Descending sorting."
        self.label.Dock = DockStyle.Top

        # setup listview
        self.listview = ListView()
        # set the view to show details.
        self.listview.View = View.Details
        # allow the user to edit item text.
        self.listview.LabelEdit = True
        # display grid lines.
        self.listview.GridLines = True
        # sort the items in the list in ascending order.
        self.listview.Sorting = SortOrder.Ascending
        # place widget besides left.
        self.listview.Dock = DockStyle.Top
        self.listview.Width = 300
        self.listview.ColumnClick += self.on_click

        # add conlumns
        self.listview.Columns.Add("Column A", 200, HorizontalAlignment.Left)

        # add items
        for i in range(5):
            self.listview.Items.Add("Item " + str(i), i)

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
form = ListViewSample()
Application.EnableVisualStyles()
Application.Run(form)
