#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/25/2008
# Description: the sample for winforms control:
#              TrackBar
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "TrackBar" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, Form, Label, TrackBar, DockStyle

class TrackBarSample(Form):
    """TrackBar control class"""

    def __init__(self):
        """TrackBarSample class init function."""

        # setup title
        self.Text = "TrackBar control"

        # setup label
        self.label = Label()
        self.label.Dock = DockStyle.Top
        self.label.Text = "The value of TrackBar is: "
        self.label.Width = 300

        # setup trackbar
        self.trackbar = TrackBar()
        self.trackbar.Maximum = 100
        self.trackbar.Minimum = 1
        self.trackbar.LargeChange = 10
        self.trackbar.SmallChange = 1 
        self.trackbar.Dock = DockStyle.Top
        # set how many positions are between each tick-mark.
        self.trackbar.TickFrequency = 5
        self.trackbar.Scroll += self.scroll

        # add controls
        self.Controls.Add(self.trackbar)
        self.Controls.Add(self.label)

    def scroll(self, sender, event):
        self.label.Text = "The value of TrackBar is: %d"  % self.trackbar.Value


# run application
form = TrackBarSample()
Application.EnableVisualStyles()
Application.Run(form)
