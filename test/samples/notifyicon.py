#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/25/2008
# Description: the sample for winforms control:
#              NotifyIcon
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "NotifyIcon" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import Application, Form, MenuItem, NotifyIcon, Timer 
from System.Drawing import Icon

class NotifyIconSample(Form):
    """NotifyIcon control class"""

    def __init__(self):
        """NotifyIconSample class init function."""

        # setup title
        self.Text = "NotifyIcon control"

        # setup timer
        self.timer = Timer()
        self.timer.Interval = 3000
        self.timer.Tick += self.on_tick
        self.timer.Start()

        # setup notifyicon
        self.notifyicon = NotifyIcon()
        self.notifyicon.Icon = Icon("test.ico")
        self.notifyicon.Visible = True

        # add controls
       # self.Controls.Add(self.notifyicon)

    def on_tick(self, sender, event):
        self.notifyicon.BalloonTipTitle = "Hello"
        self.notifyicon.BalloonTipText = "I'm IronPython, who are you?"
        self.notifyicon.ShowBalloonTip(1000) 


# run application
form = NotifyIconSample()
Application.EnableVisualStyles()
Application.Run(form)
