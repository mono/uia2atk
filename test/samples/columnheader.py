#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/01/2008
# Description: the sample for winforms control:
#              ListView
#              ColumnHeader
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ColumnHeader" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, DockStyle, Form, ListView, ListViewItem, View, ColumnHeader, SortOrder, HorizontalAlignment
)
from System.IO import DirectoryInfo, FileInfo

class ListViewSample(Form):
    """ListView control class"""

    def __init__(self):
        """ListViewSample class init function."""

        # setup title
        self.Text = "ListView control"

        # setup size
        self.Width = 700
        self.Height = 250

        # setup listview
        self.listview = ListView()
        self.listview.Height = 250
        self.listview.Dock = DockStyle.Top
        self.listview.View = View.Details
        self.listview.GridLines = True


        # setup columnheader
        self.columnheader1 = ColumnHeader()
        self.columnheader1.Text = "File name"
        self.columnheader1.TextAlign = HorizontalAlignment.Left
        self.columnheader1.Width = 200

        self.columnheader2 = ColumnHeader()
        self.columnheader2.Text = "Location"
        self.columnheader2.Width = 300

        # add conlumns
        self.listview.Columns.Add(self.columnheader1)
        self.listview.Columns.Add(self.columnheader2)

        # get files from the location
        dir_info = DirectoryInfo(".")
        files = dir_info.GetFiles("*.jpg")

        # add items in listview
        if files != None:
            for file in files:
                item = ListViewItem(file.Name)
                item.SubItems.Add(file.FullName)
                self.listview.Items.Add(item)

        # add controls
        self.Controls.Add(self.listview)


# run application
form = ListViewSample()
Application.EnableVisualStyles()
Application.Run(form)
