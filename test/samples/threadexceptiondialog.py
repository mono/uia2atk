#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        09/04/2008
# Description: the sample for winforms control:
#              ThreadExceptionDialog
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ThreadExceptionDialog" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

import System
from System.Windows.Forms import (
    Application, Button, Form, ThreadExceptionDialog
)
from System.Drawing import Point, Size

class ThreadExceptionDialogSample(Form):
    """ThreadExceptionDialog control class"""

    def __init__(self):
        """ThreadExceptionDialogSample class init function."""

        # setup title
        self.Text = "ThreadExceptionDialog control"
        self.Size = Size(200, 100)

        # setup button
        self.button = Button()
        self.button.Text = "Raise an Exception"
        self.button.Location = Point(10, 20)
        self.button.Width = 150
        self.button.Click += self.button_on_click

        # add controls
        self.Controls.Add(self.button)

    def button_on_click(self, sender, event):
        try:
            tmp = 1 / 0
        except System.Exception, e:
            self.thread_exception_dialog = ThreadExceptionDialog(e)
            self.thread_exception_dialog.ShowDialog()


# run application
form = ThreadExceptionDialogSample()
Application.EnableVisualStyles()
Application.Run(form)
