#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              ScrollBar
#              HScrollBar
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ScrollBar" and "HScrollBar" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, DockStyle, Form, HScrollBar, Label

class HScrollBarSample(Form):
    """HScrollBar control class"""

    def __init__(self):
        """HScrollBarSample class init function."""

        # setup title
        self.Text = "HScrollBar control"
        self.Height = 100

        # setup label
        self.label = Label()
        self.label.Text = "Value:"

        # setup hscrollbar
        self.hscrollbar = HScrollBar()
        self.hscrollbar.LargeChange = 20
        self.hscrollbar.SmallChange = 10
        self.hscrollbar.Minimum = 0
        self.hscrollbar.Maximum = 119 
        self.hscrollbar.Value = 0
        self.hscrollbar.Dock = DockStyle.Bottom
        self.hscrollbar.ValueChanged += self.on_scroll

        # add controls
        self.Controls.Add(self.label)
        self.Controls.Add(self.hscrollbar)

    def on_scroll(self, sender, event):
        self.label.Text = "Value: " + str(self.hscrollbar.Value)

# run application
form = HScrollBarSample()
Application.EnableVisualStyles()
Application.Run(form)
