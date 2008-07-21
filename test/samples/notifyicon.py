#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/25/2008
# Description: the sample for winforms control:
#              NotifyIcon
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "NotifyIcon" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""
# NOTICE:
# Please wait Balloon Tip for around 15 secs, when it comes up, 
# keep moving your mouse. Then it will be vanished after about 10 secs.
# Or, click on the Balloon Tip directly.

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, Form, Label, MenuItem, NotifyIcon, Timer , ToolTipIcon
)
from System.Drawing import SystemIcons

class NotifyIconSample(Form):
    """NotifyIcon control class"""

    def __init__(self):
        """NotifyIconSample class init function."""

        # setup label
        self.label = Label()
        self.label.Text = "Please wait Balloon Tip for around 15 secs,\n\n" + \
                          "When it comes up, keep moving your mouse.\n\n" + \
                         "Then it will be vanished after about 10 secs.\n\n" + \
                          "Or, click on the Balloon Tip directly."
        self.label.Width = self.Width
        self.label.Height = self.Height
        self.Controls.Add(self.label)
        
        # setup title
        self.Text = "NotifyIcon control"

        # setup timer
        self.timer = Timer()
        self.timer.Interval = 15000
        self.timer.Tick += self.on_tick
        self.timer.Start()

        # setup notifyicon
        self.notifyicon = NotifyIcon()
        self.notifyicon.Icon = SystemIcons.Exclamation
        self.notifyicon.BalloonTipTitle = "Hello"
        self.notifyicon.BalloonTipText = "I'm IronPython, who are you?"
        self.notifyicon.BalloonTipIcon = ToolTipIcon.Error

    def on_tick(self, sender, event):
        self.notifyicon.Visible = True
        self.notifyicon.ShowBalloonTip(30) 


# run application
form = NotifyIconSample()
Application.EnableVisualStyles()
Application.Run(form)
