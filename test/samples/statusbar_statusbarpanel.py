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


class StatusBarStatusBarPanelApp(Form):
    """StatusBar and StatusBarPanel controls class"""

    def __init__(self):
        """StatusBarStatusBarPanelApp class init function."""

        # a counter
        self.count = 0

        # setup form
        self.Text = "StatusBar & StatusBarPanel Example"
        self.Height = 100

        # setup button
        self.button = Button()
        self.button.Text = "Click me"
        self.button.AutoSize = True
        self.button.Click += self.on_click
        
        # set StatusBar and StatusBarPanel. in statusbar add statusbarpanel1 
        # displays status text for an application, statusbarpanel2 displays 
        # the current date.
        self.statusbar = StatusBar()
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

        # add controls
        self.statusbar.Panels.Add(self.statusbarpanel1)
        self.statusbar.Panels.Add(self.statusbarpanel2)
        self.Controls.Add(self.statusbar)
        self.Controls.Add(self.button)

    def on_click(self, sender, event):
        self.count += 1
        self.statusbarpanel1.Text = "You have click %d times" % self.count

# run application
form = StatusBarStatusBarPanelApp()
Application.Run(form)
