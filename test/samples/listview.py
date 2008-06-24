#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/24/2008
# Description: the sample for winforms control:
#              ListView
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ListView" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, DockStyle, Form, ListView, View, SortOrder, HorizontalAlignment)

class ListViewSample(Form):
    """ListView control class"""

    def __init__(self):
        """ListViewSample class init function."""

        # setup title
        self.Text = "ListView control"
        self.Width = 300
        self.Height = 300

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
        self.listview.Dock = DockStyle.Left
        self.listview.Width = 300

        # add conlumns
        self.listview.Columns.Add("Column A", 90, HorizontalAlignment.Left)
        self.listview.Columns.Add("Column B", 90, HorizontalAlignment.Left)
        self.listview.Columns.Add("Column C", 90, HorizontalAlignment.Left)

        # add items
        for i in range(5):
            self.listview.Items.Add("Item " + str(i), i)

        # add controls
        self.Controls.Add(self.listview)


# run application
form = ListViewSample()
Application.EnableVisualStyles()
Application.Run(form)
