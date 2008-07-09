#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              StatusStrip
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "StatusStrip" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
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

        self.Text = "Simple StatusStrip Example"
        self.Width = 350
        self.Height = 100

        # set up label
        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: StatusStrip."
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        # set StatusStrip:
        self.statusstrip1 = StatusStrip()
        self.statusstrip1.GripStyle = ToolStripGripStyle.Visible
        self.statusstrip1.Name = "toolstrip1"

        # set ToolStripStatusLabel:
        self.toolstripstatuslabel1 = ToolStripStatusLabel()
        self.toolstripstatuslabel1.Text = "ToolStripLabel Text..."
        self.toolstripstatuslabel1.BorderStyle = Border3DStyle.Raised
        self.toolstripstatuslabel1.BorderSides = ToolStripStatusLabelBorderSides.Bottom
        self.toolstripstatuslabel1.Spring = True

        # set ToolStripButton:
        self.toolstripbutton1 = ToolStripButton("Click Me")
        self.toolstripbutton1.Click += self.toolstripbutton1Click

        self.statusstrip1.Items.Add(self.toolstripbutton1)
        self.statusstrip1.Items.Add(self.toolstripstatuslabel1)
        self.statusstrip1.Items.Add("One")
        self.statusstrip1.Items.Add("Two")
        self.statusstrip1.Items.Add("Three")
        self.Controls.Add(self.statusstrip1)

    def toolstripbutton1Click(object,sender,event):
        MessageBox.Show("the first plugin")
    

form = RunApp()
Application.Run(form)
