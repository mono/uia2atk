#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        12/01/2008
# Description: This is a test application sample for winforms control:
#              ToolStripProgressBar
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolStripProgressBar"  controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr
import System
import System.IO

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System.Drawing.Text


class RunApp(Form):

    def __init__(self):

        # setup form
        self.Text = "ToolStripProgressBar control"
        self.Width = 300
        self.Height = 230
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.count = 0        
    
        #click button to change progressbar's process
        self.button = Button()
        self.button.Text = "button1"
        self.button.Location = Point(10, 20)
        self.button.AutoSize = True
        self.button.Click += self.bc
        self.Controls.Add(self.button)

        #label shows the change of process
        self.label = Label()
        self.label.Text = "It is 0% of 100%"
        self.label.Location = Point(10, 80)
        self.label.AutoSize = True
        self.Controls.Add(self.label)

        #StatusStrip:
        self.statusstrip1 = StatusStrip()
        self.statusstrip1.GripStyle = ToolStripGripStyle.Visible
        self.statusstrip1.Name = "toolstrip1"
        self.Controls.Add(self.statusstrip1)

##ToolStripProgressBar
        self.toolstripprogressbar1 = ToolStripProgressBar()
        self.toolstripprogressbar1.Enabled = True
        self.toolstripprogressbar1.Text = "ToolStripProgressBar"
        self.toolstripprogressbar1.ToolTipText = "ToolStripProgressBar"
        self.toolstripprogressbar1.Minimum = 0
        self.toolstripprogressbar1.Maximum = 100
        self.toolstripprogressbar1.Value = 0
        self.toolstripprogressbar1.Step = 10

        self.statusstrip1.Items.Add(self.toolstripprogressbar1)


    def bc(self, sender, event):
        #MessageBox.Show("the first plugin")
        #print "Clicked ToolStripButton at StatusStrip"
        if self.toolstripprogressbar1.Value < self.toolstripprogressbar1.Maximum:
            self.toolstripprogressbar1.Value = self.toolstripprogressbar1.Value + self.toolstripprogressbar1.Step
            self.label.Text = "It is %d%% of 100%%" % self.toolstripprogressbar1.Value
        else:
            self.label.Text = "Done"

form = RunApp()
Application.Run(form)


