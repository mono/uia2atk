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

from System.Windows.Forms import *
from System.Drawing import *

class NotifyIconSample(Form):
    """NotifyIcon control class"""

    def __init__(self):
        """NotifyIconSample class init function."""

        # setup label
        self.label = Label()
        self.label.Text = "Please click button to rise notifyicon"
        self.label.Location = Point(10,50)
        self.label.AutoSize = True
        self.Controls.Add(self.label)
        
        # setup title
        self.Text = "NotifyIcon control"

        # setup timer
        #self.timer = Timer()
        #self.timer.Interval = 15000
        #self.timer.Tick += self.on_tick
        #self.timer.Start()

        #click button to rise notifyicon
        self.button = Button()
        self.button.Text = "button"
        self.button.Location = Point(10,150)
        self.button.Click += self.on_tick
        self.Controls.Add(self.button)

        # setup notifyicon
        self.notifyicon = NotifyIcon()
        self.notifyicon.Icon = SystemIcons.Exclamation
        self.notifyicon.BalloonTipTitle = "Hello"
        self.notifyicon.BalloonTipText = "I'm NotifyIcon"
        self.notifyicon.BalloonTipIcon = ToolTipIcon.Error

    def on_tick(self, sender, event):
        self.notifyicon.Visible = True
        self.notifyicon.ShowBalloonTip(30) 


# run application
form = NotifyIconSample()
Application.EnableVisualStyles()
Application.Run(form)
