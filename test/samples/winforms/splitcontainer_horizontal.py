#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              SplitContainer
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "SplitContainer" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import Application, DockStyle, Form, SplitContainer, Orientation, Label, BorderStyle
from System.Drawing import Color

class SplitContainerSample(Form):
    """SplitContainer control class"""

    def __init__(self):
        """SplitContainerSample class init function."""

        # setup title
        self.Text = "SplitContainer control"

        # setup labels
        self.label1 = Label()
        self.label1.AutoSize = True
        self.label1.Text = "label1 in splitcontainer.panel1"
        self.label2 = Label()
        self.label2.AutoSize = True
        self.label2.Text = "label2 in splitcontainer.panel2"


        # setup splitcontainer
        self.splitcontainer = SplitContainer()
        self.splitcontainer.Dock = DockStyle.Fill
        self.splitcontainer.BorderStyle = BorderStyle.Fixed3D
        self.splitcontainer.Orientation = Orientation.Horizontal
        self.splitcontainer.Panel1.BackColor = Color.DarkCyan
        self.splitcontainer.Panel2.BackColor = Color.Coral

        # add controls
        self.splitcontainer.Panel1.Controls.Add(self.label1)
        self.splitcontainer.Panel2.Controls.Add(self.label2)
        self.Controls.Add(self.splitcontainer)

# run application
form = SplitContainerSample()
Application.EnableVisualStyles()
Application.Run(form)
