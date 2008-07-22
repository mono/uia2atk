#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              ScrollBar
#              VScrollBar
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ScrollBar" and "VScrollBar" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, DockStyle, Form, VScrollBar, Label

class VScrollBarSample(Form):
    """VScrollBar control class"""

    def __init__(self):
        """VScrollBarSample class init function."""

        # setup title
        self.Text = "VScrollBar control"

        # setup label
        self.label = Label()
        self.label.Text = "Value:"

        # setup vscrollbar
        self.vscrollbar = VScrollBar()
        self.vscrollbar.LargeChange = 20
        self.vscrollbar.SmallChange = 10
        self.vscrollbar.Minimum = 0
        self.vscrollbar.Maximum = 119 
        self.vscrollbar.Value = 0
        self.vscrollbar.Dock = DockStyle.Right
        self.vscrollbar.Scroll += self.on_scroll

        # add controls
        self.Controls.Add(self.label)
        self.Controls.Add(self.vscrollbar)

    def on_scroll(self, sender, event):
        self.label.Text = "Value: " + str(self.vscrollbar.Value)

# run application
form = VScrollBarSample()
Application.EnableVisualStyles()
Application.Run(form)
