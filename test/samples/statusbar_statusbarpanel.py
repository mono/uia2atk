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
        self.count2 = 0

        # setup form
        self.Text = "StatusBar_StatusBarPanel controls"
        self.Height = 100
        self.Width = 400

        # setup button1
        self.button1 = Button()
        self.button1.Text = "button1"
        self.button1.AutoSize = True
        self.button1.Location = Point(10, 10)
        self.button1.Click += self.button1_click

        # setup button2
        self.button2 = Button()
        self.button2.Text = "button2"
        self.button2.AutoSize = True
        self.button2.Location = Point(120, 10)
        self.button2.Click += self.button2_click
        
        # set StatusBar and StatusBarPanel. in statusbar add statusbarpanel1 
        # displays status text for an application, statusbarpanel2 displays 
        # the current date.
        self.statusbar = StatusBar()
        self.statusbar.Text = "texts in statusbar"
        self.statusbar.ShowPanels = True

        self.statusbarpanel1 = StatusBarPanel()
        self.statusbarpanel1.BorderStyle = StatusBarPanelBorderStyle.Sunken
        self.statusbarpanel1.AutoSize = StatusBarPanelAutoSize.Spring
        self.statusbarpanel1.Text = "Started: " + \
                                    System.DateTime.Now.ToShortTimeString()

        self.statusbarpanel2 = StatusBarPanel()
        self.statusbarpanel2.BorderStyle = StatusBarPanelBorderStyle.Raised
        self.statusbarpanel2.AutoSize = StatusBarPanelAutoSize.Contents
        self.statusbarpanel2.Text = System.DateTime.Today.ToLongDateString()

        self.statusbarpanel3 = StatusBarPanel()
        self.statusbarpanel3.BorderStyle = StatusBarPanelBorderStyle.Raised
        self.statusbarpanel3.AutoSize = StatusBarPanelAutoSize.Contents
        self.statusbarpanel3.Text = "Icon"
        self.statusbarpanel3.Icon = Icon("%s/samples/statusbarpanel.ico" % uiaqa_path)

        # add controls
        self.statusbar.Panels.Add(self.statusbarpanel1)
        self.statusbar.Panels.Add(self.statusbarpanel2)
        self.statusbar.Panels.Add(self.statusbarpanel3)
        self.Controls.Add(self.statusbar)
        self.Controls.Add(self.button1)
        self.Controls.Add(self.button2)

    def button1_click(self, sender, event):
        self.count1 += 1
        self.statusbarpanel1.Text = "You have click %d times" % self.count1

    def button2_click(self, sender, event):
        self.count2 += 1
        self.statusbar.Text = "Change texts %d times" % self.count2

# run application
form = StatusBarStatusBarPanelApp()
Application.Run(form)
