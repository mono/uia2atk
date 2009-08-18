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
import os
from sys import path
from os.path import exists

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

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

        # image list
        self.imagelist = ImageList()
        self.imagelist.ColorDepth = ColorDepth.Depth32Bit
        self.imagelist.ImageSize = Size(16, 16)

        # small images
        names = [
                "abiword_48.png",
                "bmp.png",
                "desktop.png",
            ]

        for i in names:
            self.imagelist.Images.Add (Image.FromFile("%s/winforms/listview-items-icons/32x32/" % uiaqa_path + i))

        self.listview.SmallImageList = self.imagelist

        self.column1 = ColumnHeader()
        self.column1.Text = "Column A"
        self.column1.Width = 100
        self.column1.ImageIndex = 0
        self.listview.Columns.Add(self.column1)

        self.column2 = ColumnHeader()
        self.column2.Text = "Num"
        self.column2.Width = 80
        self.column2.ImageIndex = 1
        self.listview.Columns.Add(self.column2)

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
