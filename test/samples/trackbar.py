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
from System.Windows.Forms import *

class TrackBarSample(Form):
    """TrackBar control class"""

    def __init__(self):
        """TrackBarSample class init function."""

        # setup title
        self.Text = "TrackBar control"

        # setup label
        self.label_hor = Label()
        self.label_hor.Dock = DockStyle.Top
        self.label_hor.Text = "The value of TrackBar(Horizontal) is: "
        self.label_hor.Width = 300

        self.label_ver = Label()
        self.label_ver.Dock = DockStyle.Top
        self.label_ver.Text = "The value of TrackBar(Vertical) is: "
        self.label_ver.Width = 300

        # setup trackbar(Horizontal)
        self.trackbar_hor = TrackBar()
        self.trackbar_hor.Maximum = 100
        self.trackbar_hor.Minimum = 1
        self.trackbar_hor.LargeChange = 10
        self.trackbar_hor.SmallChange = 1 
        self.trackbar_hor.Dock = DockStyle.Top
        # set how many positions are between each tick-mark.
        self.trackbar_hor.TickFrequency = 5
        self.trackbar_hor.Orientation = Orientation.Horizontal
        self.trackbar_hor.ValueChanged += self.scroll

        # setup trackbar(Vertical)
        self.trackbar_ver = TrackBar()
        self.trackbar_ver.Maximum = 100
        self.trackbar_ver.Minimum = 1
        self.trackbar_ver.LargeChange = 10
        self.trackbar_ver.SmallChange = 1 
        self.trackbar_ver.Dock = DockStyle.Top
        # set how many positions are between each tick-mark.
        self.trackbar_ver.TickFrequency = 5
        self.trackbar_ver.Orientation = Orientation.Vertical
        self.trackbar_ver.Height = 150
        self.trackbar_ver.ValueChanged += self.scroll

        # add controls
        self.Controls.Add(self.trackbar_ver)
        self.Controls.Add(self.label_ver)
        self.Controls.Add(self.trackbar_hor)
        self.Controls.Add(self.label_hor)

    def scroll(self, sender, event):
        if sender.Orientation == Orientation.Horizontal:
            self.label_hor.Text = "The value of TrackBar(Horizontal) is: %d" % \
                        self.trackbar_hor.Value
        elif sender.Orientation == Orientation.Vertical:
            self.label_ver.Text = "The value of TrackBar(Vertical) is: %d" % \
                        self.trackbar_ver.Value



# run application
form = TrackBarSample()
Application.EnableVisualStyles()
Application.Run(form)
