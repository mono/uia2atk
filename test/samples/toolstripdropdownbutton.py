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
        self.Text = "Simple ToolStrip Example"
        self.Width = 300
        self.Height = 300
        self.FormBorderStyle = FormBorderStyle.Fixed3D
    
        # setup label
        self.label = Label()
        self.label.AutoSize = True
        self.label.Location = Point(10, 60)
        self.label.Text = "Please Select one Color from the ComboxBox"
        self.Controls.Add(self.label)

        #ToolStrip:
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)

        #ToolStripDropDownButton
        self.db = ToolStripDropDownButton()
        self.db.Text = "ToolStripDropDownButton"
        self.db.DropDownDirection = ToolStripDropDownDirection.Left
        self.db.ShowDropDownArrow = True
        
        self.db.DropDownItems.Add("Red")
        self.db.DropDownItems.Add("Blue")
        self.db.DropDownItemClicked += self.cc
        self.ts.Items.Add(self.db)

    def cc(self, sender, event):
        if event.ClickedItem.Text is "Red":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Color.Red
        elif event.ClickedItem.Text is "Blue":
            self.label.Text = "You selected %s" % event.ClickedItem
            self.label.BackColor = Color.Blue
     

form = ToolStripDropDownButtonApp()
Application.Run(form)


