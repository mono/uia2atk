#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
#              Brian G. Merrell <bgmerrell@novell.com>
# Date:        05/11/2008
# Description: This is a test application sample for winforms control:
#              ToolStripProgressBar
##############################################################################

# The docstring below is used in the generated log file
"""
This sample contains a StatusStrip with a ToolStripLabel, two
ToolStripDropDownButton controls, and a ToolStripProgresBar.  The sample also
contains a button that can be used to update the progress bar and a label
that changes when the progress bar is changed.
"""


import clr
import System

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *


class RunApp(Form):
    """StatusStrip controls class"""

    def __init__(self):
        """RunApp class init function."""

        self.Text = "ToolStripProgressBar Sample"
        self.Width = 650
        self.Height = 200

        #click button to change progressbar's process
        self.button = Button()
        self.button.Text = "button1"
        self.button.Location = Point(10, 20)
        self.button.AutoSize = True
        self.button.Click += self.bc
        self.Controls.Add(self.button)

        #set up label
        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Sample for ProgressBar"
        self.mainLabel1.AutoSize = True
        self.mainLabel1.Location = Point(10, 60)
        self.Controls.Add(self.mainLabel1)

        #set StatusStrip:
        self.statusstrip1 = StatusStrip()
        self.statusstrip1.GripStyle = ToolStripGripStyle.Visible
        self.statusstrip1.Name = "statusstrip1"
        self.statusstrip1.ShowItemToolTips = True
        

        #set ToolStripStatusLabel:
        self.toolstripstatuslabel1 = ToolStripStatusLabel()
        self.toolstripstatuslabel1.Text = "ToolStripLabel Text"
        self.toolstripstatuslabel1.BorderStyle = Border3DStyle.Raised
        self.toolstripstatuslabel1.BorderSides = ToolStripStatusLabelBorderSides.Bottom
        self.toolstripstatuslabel1.Spring = True

        #add toolstripDropDownButton
        self.db = ToolStripDropDownButton()
        self.db.Text = "ToolStripDropDownButton"
        self.db.DropDownDirection = ToolStripDropDownDirection.Left
        self.db.ShowDropDownArrow = True
        
        self.db.DropDownItems.Add("Red")
        self.db.DropDownItems.Add("Blue")
        self.db.DropDownItemClicked += self.db_click

        #add toolstripsplitbutton
        self.tssb = ToolStripSplitButton()
        self.tssb.Text = "ToolStripSplitButton"

        self.tssb.DropDownItems.Add("Blue Color")
        self.tssb.DropDownItems.Add("Red Color")
        self.tssb.DropDownItemClicked += self.tssb_click

        #add toolstripprogressbar
        self.toolstripprogressbar1 = ToolStripProgressBar()
        self.toolstripprogressbar1.Enabled = True
        self.toolstripprogressbar1.Text = "ToolStripProgressBar"
        self.toolstripprogressbar1.ToolTipText = "ToolStripProgressBar"
        self.toolstripprogressbar1.Minimum = 0
        self.toolstripprogressbar1.Maximum = 100
        self.toolstripprogressbar1.Value = 0
        self.toolstripprogressbar1.Step = 20

        # add items into statusstrip
        self.statusstrip1.Items.Add(self.toolstripstatuslabel1)
        self.statusstrip1.Items.Add(self.db)
        self.statusstrip1.Items.Add(self.tssb)
        self.statusstrip1.Items.Add(self.toolstripprogressbar1)

        self.Controls.Add(self.statusstrip1)

    def bc(self, sender, event):
        if self.toolstripprogressbar1.Value < self.toolstripprogressbar1.Maximum:
            self.toolstripprogressbar1.Value = self.toolstripprogressbar1.Value + self.toolstripprogressbar1.Step
            self.mainLabel1.Text = "It is %d%% of 100%%" % self.toolstripprogressbar1.Value
        else:
            self.mainLabel1.Text = "Done"

    def db_click(self, sender, event):
        if event.ClickedItem.Text is "Red":
            self.mainLabel1.Text = "You selected %s" % event.ClickedItem
            self.mainLabel1.BackColor = Color.Red
        elif event.ClickedItem.Text is "Blue":
            self.mainLabel1.Text = "You selected %s" % event.ClickedItem
            self.mainLabel1.BackColor = Color.Blue

    def tssb_click(self, sender, event):
        if event.ClickedItem.Text is "Red Color":
            self.toolstripstatuslabel1.Text = "You selected %s" % event.ClickedItem
            self.toolstripstatuslabel1.ForeColor = Color.Red
        elif event.ClickedItem.Text is "Blue Color":
            self.toolstripstatuslabel1.Text = "You selected %s" % event.ClickedItem
            self.toolstripstatuslabel1.ForeColor = Color.Blue
    

form = RunApp()
Application.Run(form)
