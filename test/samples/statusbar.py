#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              StatusBar
#              StatusBarPanel
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "StatusBar" and "StatusBarPanel" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr
import System

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *
from sys import path

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

class StatusBarStatusBarPanelApp(Form):
    """StatusBar and StatusBarPanel controls class"""

    def __init__(self):
        """StatusBarStatusBarPanelApp class init function."""

        # a counter
        self.count1 = 0
        self.count1 = 0

        # setup form
        self.Text = "StatusBar controls"
        self.Height = 100
        self.Width = 400

        # setup button1
        self.button1 = Button()
        self.button1.Text = "button1"
        self.button1.AutoSize = True
        self.button1.Location = Point(10, 10)
        self.button1.Click += self.button1_click

        # set StatusBar and StatusBarPanel. in statusbar add statusbarpanel1 
        # displays status text for an application, statusbarpanel2 displays 
        # the current date.
        self.statusbar = StatusBar()
        self.statusbar.Text = "Ye Olde Status Bar Text"
        self.statusbar.ShowPanels = False

        self.Controls.Add(self.statusbar)
        self.Controls.Add(self.button1)

    def button1_click(self, sender, event):
        self.count1 += 1
        self.statusbar.Text = "Changed text %d times" % self.count1

# run application
form = StatusBarStatusBarPanelApp()
Application.Run(form)
