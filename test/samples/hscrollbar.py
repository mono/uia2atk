#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              ScrollBar
#              HScrollBar
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "HScrollBar" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, DockStyle, Form, HScrollBar

class HScrollBarSample(Form):
    """HScrollBar control class"""

    def __init__(self):
        """HScrollBarSample class init function."""

        # setup title
        self.Text = "HScrollBar control"

        # setup hscrollbar
        self.hscrollbar = HScrollBar()
        self.hscrollbar.Dock = DockStyle.Bottom

        # add controls
        self.Controls.Add(self.hscrollbar)

# run application
form = HScrollBarSample()
Application.EnableVisualStyles()
Application.Run(form)
