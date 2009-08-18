#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        11/28/2008
# Description: This is a test application sample for winforms control:
#              ToolStripDropDownButton
##############################################################################

import clr
import System
import System.IO

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System.Drawing.Text


class ToolStripDropDownButtonApp(Form):

    def __init__(self):
        self.Text = "ToolStripDropDownButton control"
        self.Width = 420
        self.Height = 300
        self.FormBorderStyle = FormBorderStyle.Fixed3D
    
        # setup label
        self.label = Label()
        self.label.AutoSize = True
        self.label.Location = Point(10, 60)
        self.label.Text = "Please Select one Color from ToolStripDropDownButton"
        self.Controls.Add(self.label)

        #ToolStrip:
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)

        #ToolStripDropDownButton1
        self.db1 = ToolStripDropDownButton()
        self.db1.Text = "ToolStripDropDownButton1"
        self.db1.DropDownDirection = ToolStripDropDownDirection.Left
        self.db1.ShowDropDownArrow = True
        
        self.db1.DropDownItems.Add("Red")
        self.db1.DropDownItems.Add("Blue")
        self.db1.DropDownItems.Add("Green")
        self.db1.DropDownItemClicked += self.cc
        self.ts.Items.Add(self.db1)

        #ToolStripDropDownButton2
        self.db2 = ToolStripDropDownButton()
        self.db2.Text = "ToolStripDropDownButton2"
        self.db2.DropDownDirection = ToolStripDropDownDirection.Left
        self.db2.ShowDropDownArrow = True
        
        self.db2.DropDownItems.Add("Item1")
        self.db2.DropDownItems.Add("Item2")
        self.db2.DropDownItems.Add("Item3")
        self.db2.DropDownItemClicked += self.cc
        self.ts.Items.Add(self.db2)

    def cc(self, sender, event):
        if event.ClickedItem.Text is "Red":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Color.Red
        elif event.ClickedItem.Text is "Blue":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Color.Blue
        elif event.ClickedItem.Text is "Green":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Color.Green
        elif event.ClickedItem.Text is "Item1":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Button.DefaultBackColor
        elif event.ClickedItem.Text is "Item2":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Button.DefaultBackColor
        elif event.ClickedItem.Text is "Item3":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Button.DefaultBackColor

form = ToolStripDropDownButtonApp()
Application.Run(form)


