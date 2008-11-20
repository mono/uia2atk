#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/18/2008
# Description: the sample for winforms control:
#              ProgressBar
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ProgressBar" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import Application, Button, Form, Label, ProgressBar
from System.Drawing import Point

class ProgressBarSample(Form):
    """ProgressBar control class"""

    def __init__(self):
        """ProgressBarSample class init function."""

        # setup title
        self.Text = "ProgressBar control"
        self.Width = 260
        self.Height = 100

        # setup label
        self.label = Label()
        self.label.Text = "It is %d percent of 100%%" % 0 
        self.label.Width = 150 
        self.label.Location = Point(0, 0)

        # setup button
        self.button = Button()
        self.button.Text = "Click"
        self.button.Location = Point(170, 0)
        self.button.Click += self.start_progress

        # setup progressbar
        self.progressbar = ProgressBar()
        self.progressbar.Width = 250
        self.progressbar.Visible = True
        self.progressbar.Minimum = 0
        self.progressbar.Maximum = 100
        self.progressbar.Value = 0
        self.progressbar.Step = 20 
        self.progressbar.Location = Point(0, 50)

        # add controls
        self.Controls.Add(self.label)
        self.Controls.Add(self.button)
        self.Controls.Add(self.progressbar)

    # progress handling method
    def start_progress(self, sender, event):
        if self.progressbar.Value < self.progressbar.Maximum:
            self.progressbar.Value = self.progressbar.Value + \
                                     self.progressbar.Step
            self.label.Text = "It is %d%% percent of 100%%" % \
                              self.progressbar.Value
        else:
            self.progressbar.Value = 0
            self.progressbar.Value = self.progressbar.Value + \
                                     self.progressbar.Step
            self.label.Text = "It is %d%% percent of 100%%" % \
                              self.progressbar.Value

# run application
form = ProgressBarSample()
Application.EnableVisualStyles()
Application.Run(form)
