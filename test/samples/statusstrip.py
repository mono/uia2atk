#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              StatusStrip
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "StatusStrip" control in the form. It can be used for 
Autotest tools(e.g. Strongwind) to test the behaviors of controls. StatusStrip 
can add items those are ToolStripStatusLabel, ToolStripDropDownButton, 
ToolStripSplitButton and ToolStripProgressBar
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

        self.Text = "StatusStrip control"
        self.Width = 450
        self.Height = 200

        #set up label
        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: StatusStrip."
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        #set StatusStrip:
        self.statusstrip1 = StatusStrip()
        self.statusstrip1.GripStyle = ToolStripGripStyle.Visible
        self.statusstrip1.Name = "statusstrip1"
        self.statusstrip1.ShowItemToolTips = True
        

        #set ToolStripStatusLabel:
        self.toolstripstatuslabel1 = ToolStripStatusLabel()
        self.toolstripstatuslabel1.Text = "ToolStripLabel Text..."
        self.toolstripstatuslabel1.BorderStyle = Border3DStyle.Raised
        self.toolstripstatuslabel1.BorderSides = ToolStripStatusLabelBorderSides.Bottom
        self.toolstripstatuslabel1.Spring = True

        #set ToolStripButton:
        self.toolstripbutton1 = ToolStripButton("Click Me")
        self.toolstripbutton1.Click += self.toolstripbutton1Click

        #add toolstripDropDownButton
        self.db = ToolStripDropDownButton()
        self.dd = ToolStripDropDown()
        self.db.Text = "ToolStripDropDownButton"
        self.db.DropDown = self.dd
        
        self.br = ToolStripButton()
        self.br.ForeColor = Color.Red
        self.br.Text = "Red"
        self.br.Name = "Red"
        self.bu = ToolStripButton()
        self.bu.ForeColor = Color.Blue
        self.bu.Text = "Blue"
        self.bu.Name = "Blue"
        self.br.Click += self.cc
        self.bu.Click += self.cc
        
        self.dd.Items.Add(self.br)
        self.dd.Items.Add(self.bu)

        #add toolstripsplitbutton
        self.tssb = ToolStripSplitButton()
        self.tssb.Text = "ToolStripSplitButton"
        self.tsmi1 = ToolStripMenuItem("Blue Color")
        self.tsmi1.ForeColor = Color.Blue
        self.tsmi2 = ToolStripMenuItem("Red Color")
        self.tsmi2.ForeColor = Color.Red
        self.tsmi1.Click += self.tsmi_c
        self.tsmi2.Click += self.tsmi_c

        self.tssb.DropDownItems.Add(self.tsmi1)
        self.tssb.DropDownItems.Add(self.tsmi2)

        #add toolstripprogressbar
        self.toolstripprogressbar1 = ToolStripProgressBar()
        self.toolstripprogressbar1.Enabled = True
        self.toolstripprogressbar1.Name = "ToolStripProgressBar"
        self.toolstripprogressbar1.ToolTipText = "ToolStripProgressBar"
        self.toolstripprogressbar1.Minimum = 0
        self.toolstripprogressbar1.Maximum = 100
        self.toolstripprogressbar1.Value = 0
        self.toolstripprogressbar1.Step = 10

        # add items into statusstrip
        self.statusstrip1.Items.Add(self.toolstripbutton1)
        self.statusstrip1.Items.Add(self.toolstripstatuslabel1)
        self.statusstrip1.Items.Add(self.db)
        self.statusstrip1.Items.Add(self.tssb)
        self.statusstrip1.Items.Add(self.toolstripprogressbar1)

        self.Controls.Add(self.statusstrip1)

    def toolstripbutton1Click(self, sender, event):
        #MessageBox.Show("message box")
        if self.toolstripprogressbar1.Value < self.toolstripprogressbar1.Maximum:
            self.toolstripprogressbar1.Value = self.toolstripprogressbar1.Value + self.toolstripprogressbar1.Step
            self.toolstripstatuslabel1.Text = "It is %d%% of 100%%" % self.toolstripprogressbar1.Value
        else:
            self.toolstripstatuslabel1.Text = "Done"

    def cc(self, sender, event):
        if sender == self.br:
            self.mainLabel1.ForeColor = Color.Red
        else:
            self.mainLabel1.ForeColor = Color.Blue

    def tsmi_c(self, sender, event):
        if sender == self.tsmi1:
            self.toolstripstatuslabel1.ForeColor = Color.Blue
        else:
            self.toolstripstatuslabel1.ForeColor = Color.Red
    

form = RunApp()
Application.Run(form)
