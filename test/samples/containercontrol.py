#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/27/2008
# Description: the sample for winforms control:
#              ScrollableControl
#              ContainerControl
##############################################################################

# Since we do not typically use the ScrollableControl class directly. 
# The ContainerControl and Panel classes inherit from this class.
# So we implement ContainerControl to indicate the features of ScrollableControl

# The docstring below is used in the generated log file
"""
Test accessibility of "ContainerControl" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, Form, Label, ContainerControl, DockStyle, BorderStyle
)

class ContainerControlSample(Form):
    """ContainerControl control class"""

    def __init__(self):
        """ContainerControlSample class init function."""

        self.count = 1

        # setup title
        self.Text = "ContainerControl control"
        self.Height = 230

        # setup labels
        self.label1 = Label()
        self.label1.Text = "Press Tab, please"
        self.label1.AutoSize = True
        self.label1.Dock = DockStyle.Fill
        self.label1.BorderStyle = BorderStyle.Fixed3D
        
        self.label2 = Label()
        self.label2.Text = "Press Tab, again please"
        self.label2.AutoSize = True
        self.label2.Dock = DockStyle.Fill
        self.label2.BorderStyle = BorderStyle.Fixed3D

        # setup containercontrols
        self.containercontrol1 = ContainerControl()
        self.containercontrol1.Height = 100
        self.containercontrol1.Dock = DockStyle.Bottom
        self.containercontrol1.LostFocus += self.focus

        self.containercontrol2 = ContainerControl()
        self.containercontrol2.Height = 100
        self.containercontrol2.Dock = DockStyle.Bottom
        self.containercontrol2.LostFocus += self.focus

        # add controls
        self.containercontrol1.Controls.Add(self.label1)
        self.containercontrol2.Controls.Add(self.label2)
        self.Controls.Add(self.containercontrol1)
        self.Controls.Add(self.containercontrol2)

    def focus(self, sender, event):
        if self.count == 1:
            self.label1.Text = "I lose focus"
            self.label2.Text = "I got it"
            self.count += 1
        else:
            self.label1.Text = "I got it"
            self.label2.Text = "I lose focus"
            self.count -= 1

# run application
form = ContainerControlSample()
Application.EnableVisualStyles()
Application.Run(form)
