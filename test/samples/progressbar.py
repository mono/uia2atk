#!/usr/bin/env ipy
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        Jun 16, 2008
# Description: the sample for winforms control:
#              ProgressBar
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ProgressBar" control
"""

# imports
import clr
import time
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import Application, Form,  ProgressBar
from System.Threading import ThreadStart, Thread

class ProgressBarSample(Form):
    """ProgressBar control class"""

    def __init__(self):
        """ProgressBarSample class init function."""

        # setup title
        self.Text = "ProgressBar control test sample"
        self.Width = 260
        self.Height = 100

        # setup progressbar
        self.progressbar = ProgressBar()
        self.progressbar.Width = 250
        self.progressbar.Visible = True
        self.progressbar.Minimum = 1
        self.progressbar.Maximum = 100
        self.progressbar.Value = 1
        self.progressbar.Step = 1
        # show progress event
        self.Shown += self.start_progress

        # add controls
        self.Controls.Add(self.progressbar)

    # progress handling method
    def start_progress(self, sender, event):
        def update():
            for i in range(100):
                print i 
                self.progressbar.Value = i + 1
                time.sleep(0.1)
            print 'Done'
        t = Thread(ThreadStart(update))
        t.Start()

# run application
form = ProgressBarSample()
Application.EnableVisualStyles()
Application.Run(form)
